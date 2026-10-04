using konrevise.Remote;
using konrevise.Remote.Services;
using konrevise.Remote.App;
using System.Security.Cryptography;

// var salt = CryptographyService.CreateRandomBytesToken();
// var hash = CryptographyService.StringToPBKDF2Hash("мойПароль", salt, 300300);
// var iterations = 300300;

// Console.WriteLine($"salt: {CryptographyService.BytesToHex(salt)}");
// Console.WriteLine($"hash: {hash}");
// Console.WriteLine($"iterations: {iterations}");

var app = new Application(args);
app.Initialize();

async Task<T> GetJson<T>(HttpContext ctx)
{
    if (!ctx.Request.HasJsonContentType())
        throw new Exception("expected type application/json");

    using var reader = new StreamReader(ctx.Request.Body);
    var body = await reader.ReadToEndAsync();

    return JsonService.Deserialize<T>(body);
}

async Task<T?> HandleRequestWithJson<T>(HttpContext ctx) =>
    await GetJson<T>(ctx);

// -------------------------------------------------------------- working on api

const string postMethod = "POST";

app.MapHttpMethod("/api/app/getGuardInstructions", postMethod,
    async (HttpContext ctx) =>
{
    GetGuardInstructions deserialized;
    try
    {
        deserialized = await HandleRequestWithJson<GetGuardInstructions>(ctx);
    }
    catch (Exception exception)
    {
        return ApiHelper.BadRequest(exception);
    }

    var guardInstructions = Guard.Call(deserialized);
    return ApiHelper.JsonContent(guardInstructions);
});

app.MapHttpMethod("/api/app/initiateLoginChallenge", postMethod,
    async (HttpContext ctx) =>
{
    InitiateLoginChallenge deserialized;
    try
    {
        deserialized = await HandleRequestWithJson<InitiateLoginChallenge>(ctx);
    }
    catch (Exception exception)
    {
        return ApiHelper.BadRequest(exception);
    }

    var name = deserialized.Name;

    try
    {
        var user = UsersService.Identify(name);
        Console.WriteLine("user has been found! challenge will be accepted.");

        var challengeID = Guid.NewGuid();
        var nonce = CryptographyService.CreateRandomToken();
        var salt = user.Password.HashSalt;
        var iterations = user.Password.HashIterations;

        var challenge = new LoginChallenge(user, nonce, salt, iterations);
        if (!LoginChallengesService.TryAdd(challengeID, challenge))
            throw new InvalidOperationException(
                "challenge id collision (should never happen with GUID).");

        var result = new LoginChallengeAccepted(challengeID, nonce, salt,
            iterations);
        return ApiHelper.JsonContent(result);
    }
    catch (Exception e)
    {
        Console.WriteLine($"couldn't initiate login challenge!\n{e}");
        return ApiHelper.BadRequest(e);
    }
});

app.MapHttpMethod("/api/app/finishLoginChallenge", postMethod,
    async (HttpContext ctx) =>
{
    FinishLoginChallenge deserialized;
    try
    {
        deserialized = await HandleRequestWithJson<FinishLoginChallenge>(ctx);
    }
    catch (Exception exception)
    {
        return ApiHelper.BadRequest(exception);
    }

    LoginChallengesService.Collect();

    if (!Guid.TryParse(deserialized.ChallengeID, out var challengeID))
        return ApiHelper.Unauthorized("invalid challenge id");

    // Атомарно забираем челлендж себе ДО любой проверки.
    // Это гарантирует, что повторный запрос с тем же id не пройдёт.
    if (!LoginChallengesService.TryTake(challengeID, out var challenge)
        || challenge is null)
        return ApiHelper.Unauthorized("challenge expired or already used");

    byte[] expectedBytes;
    byte[] clientBytes;
    try
    {
        var expectedOneTimeKey = CryptographyService.HMAC(
            keyHex: challenge.User.Password.Hash,
            messageHex: challenge.Nonce);

        expectedBytes = CryptographyService.HexToBytes(expectedOneTimeKey);
        clientBytes = CryptographyService.HexToBytes(deserialized.OneTimeKey);
    }
    catch (FormatException)
    {
        // Клиент прислал мусор вместо hex.
        return ApiHelper.Unauthorized("invalid credentials");
    }

    var isPasswordCorrect = CryptographicOperations
        .FixedTimeEquals(expectedBytes, clientBytes);

    if (!isPasswordCorrect)
        return ApiHelper.Unauthorized("invalid credentials");

    Console.WriteLine("the challenge is succeeded! creating a new session.");

    var authenticationToken = CryptographyService.CreateRandomToken();

    var registeredSession = SessionsService.TryRegisterSession(
        authenticationToken, challenge.User);
    if (!registeredSession)
        ApiHelper.BadRequest("failed to register a new session!");

    var result = new LoginChallengeSucceeded(authenticationToken);
    return ApiHelper.JsonContent(result);
});

app.MapHttpMethod("/api/app/logout", postMethod,
    async (HttpContext ctx) =>
{
    Logout deserialized;
    try
    {
        deserialized = await HandleRequestWithJson<Logout>(ctx);
    }
    catch (Exception exception)
    {
        return ApiHelper.BadRequest(exception);
    }

    var authToken = deserialized.AuthToken;

    Console.WriteLine($"deregistering session with auth token {authToken}...");
    try
    {
        var deregisteredSession = SessionsService.TryDeregisterSession(
            authToken);
        if (!deregisteredSession)
            ApiHelper.BadRequest("failed to deregister a new session!");
        return Results.Ok();
    }
    catch (Exception e)
    {
        Console.WriteLine($"couldn't deregister the session!\n{e}");
        return ApiHelper.BadRequest(e);
    }
});

app.WebApp.Use(async (ctx, next) =>
{
    // пропускаем всё, что не /api/main
    if (!ctx.Request.Path.StartsWithSegments("/api/main"))
    {
        await next();
        return;
    }

    Console.WriteLine($"\n\n########## verifying the session");
    Console.WriteLine($"\nchecking the \"Authorization\" header...");

    if (!ctx.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        Console.WriteLine($"there's no \"Authorization\" header!");
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("missing token");
        return;
    }

    var header = authHeader.ToString();
    if (!header.StartsWith("Bearer "))
    {
        Console.WriteLine($"invalid \"Authorization\" header scheme.");
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("invalid auth scheme");
        return;
    }

    var token = header["Bearer ".Length..];
    var user = SessionsService.IdentifyByToken(token);
    if (user is null)
    {
        Console.WriteLine($"invalid token in the \"Authorization\" header.");
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("invalid token");
        return;
    }

    ctx.Items["user"] = user;
    Console.WriteLine($"everything is ok!");
    await next();
});

IResult ExecuteCommand(string command)
{
    try
    {
        SystemService.ExecuteCommand(command);
    }
    catch (Exception exception)
    {
        return ApiHelper.BadRequest(exception);
    }

    return Results.Ok();
}

app.MapHttpMethod("/api/main/session", postMethod,
    async (HttpContext ctx) =>
{
    var user = ctx.Items["user"];
    if (user is null) return ApiHelper.Unauthorized("the user is null.");

    Session deserialized;
    try
    {
        deserialized = await HandleRequestWithJson<Session>(ctx);
    }
    catch (Exception exception)
    {
        return ApiHelper.BadRequest(exception);
    }

    string command = deserialized.Command switch
    {
        SessionCommand.PowerOff => "/usr/bin/systemctl poweroff",
        SessionCommand.Reboot => "/usr/bin/systemctl reboot",
        SessionCommand.Logout => "/usr/bin/xfce4-session-logout -l",
        SessionCommand.SwitchUser => "xflock4",
        SessionCommand.Sleep => "/usr/bin/systemctl suspend",
        _ => ""
    };

    return ExecuteCommand(command);
});

// -----------------------------------------------------------------------------

app.Run();

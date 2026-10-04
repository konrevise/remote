namespace konrevise.Remote.App;

public static class Guard
{
	public static GuardInstructions Call(GetGuardInstructions request)
	{
		var authenticationToken = request.AuthenticationToken;
		var url = request.Url;

	    Console.WriteLine(
	$"called guard for user with authentication token {authenticationToken}.");
	    Console.WriteLine($"user is in: {url}");

	    GuardInstructions result = new();

	    var isTokenValid = authenticationToken != null &&
	    	SessionsService.IdentifyByToken(authenticationToken) != null;
	    Console.WriteLine($"is the token valid? {isTokenValid}");

    	if (isTokenValid)
    	{
    		result.DestinationUrl = RedirectTo(
    			url switch
    			{
    				string s when s == AddressesStorage.Empty =>
    					AddressesStorage.MainMenuPage,
    				string s when s == AddressesStorage.LoginPage =>
    					AddressesStorage.MainMenuPage,

    				_ => GuardInstructions.DontRedirect
    			},
    			request
    		);
    	}
    	else
	    {
	    	result.DestinationUrl = RedirectTo(AddressesStorage.LoginPage,
	    		request);
	    }

	    Console.WriteLine(
	    	$"guard wants the user to be in {result.DestinationUrl}");

	    return result;
	}

	private static string RedirectTo(string target,
		GetGuardInstructions request) =>
		request.Url == target ? GuardInstructions.DontRedirect : target;
}

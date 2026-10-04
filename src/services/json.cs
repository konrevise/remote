using System.Text.Json;

// converters
using konrevise.Remote.App;

namespace konrevise.Remote.Services;

public static class JsonService
{
	private static JsonSerializerOptions jsonSerializerOptions = new()
	{
		Converters =
		{
			// api commands
			// - main
			new SessionConverter(),
			// - app
			new FinishLoginChallengeConverter(),
			new GetGuardInstructionsConverter(),
			new InitiateLoginChallengeConverter(),
			new LogoutConverter(),

			// json responses
			// - app
			new GuardInstructionsConverter(),
			new LoginChallengeAcceptedConverter(),
			new LoginChallengeSucceededConverter()
		}
	};

	public static string Serialize<T>(T value)
	{
		return JsonSerializer.Serialize<T>(value, jsonSerializerOptions);
	}

	public static T Deserialize<T>(string json)
	{
    	Console.WriteLine($"gotta deserialize this string to json!\n{json}");
		return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions)!;
	}
}

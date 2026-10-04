using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public record struct LoginChallengeSucceeded(string AuthenticationToken);

public class LoginChallengeSucceededConverter :
	JsonConverter<LoginChallengeSucceeded>
{
	public override void Write(Utf8JsonWriter writer,
		LoginChallengeSucceeded value,
		JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName("authenticationToken");
		writer.WriteStringValue(value.AuthenticationToken);

		writer.WriteEndObject();
	}

	public override LoginChallengeSucceeded Read(ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options)
	{
		return default;
	}
}

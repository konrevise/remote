using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public record struct LoginChallengeAccepted(
	Guid ChallengeID,
	string Nonce,
	string HashSalt,
	int HashIterations
);

public class LoginChallengeAcceptedConverter :
	JsonConverter<LoginChallengeAccepted>
{
	public override void Write(Utf8JsonWriter writer,
		LoginChallengeAccepted value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName("challengeID");
		writer.WriteStringValue(value.ChallengeID.ToString());

		writer.WritePropertyName("nonce");
		writer.WriteStringValue(value.Nonce);

		writer.WritePropertyName("hashSalt");
		writer.WriteStringValue(value.HashSalt);

		writer.WritePropertyName("hashIterations");
		writer.WriteNumberValue(value.HashIterations);

		writer.WriteEndObject();
	}

	public override LoginChallengeAccepted Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		return default;
	}
}

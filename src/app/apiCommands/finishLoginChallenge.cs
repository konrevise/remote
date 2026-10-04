using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public record struct FinishLoginChallenge(
	string ChallengeID,
	string OneTimeKey
);

public class FinishLoginChallengeConverter :
	JsonConverter<FinishLoginChallenge>
{
	public override void Write(Utf8JsonWriter writer,
		FinishLoginChallenge value, JsonSerializerOptions options)
	{

	}

	public override FinishLoginChallenge Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

        FinishLoginChallenge result = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString() ?? "";
                reader.Read();

                switch (propertyName)
                {
                    case "challengeID":
                        result.ChallengeID = reader.GetString() ?? "";
                        break;
                    case "oneTimeKey":
                        result.OneTimeKey = reader.GetString() ?? "";
                        break;
                }
            }
        }
        throw new JsonException();
	}
}

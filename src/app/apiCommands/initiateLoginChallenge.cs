using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public record struct InitiateLoginChallenge(string Name);

public class InitiateLoginChallengeConverter :
	JsonConverter<InitiateLoginChallenge>
{
	public override void Write(Utf8JsonWriter writer,
		InitiateLoginChallenge value, JsonSerializerOptions options)
	{

	}

	public override InitiateLoginChallenge Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

        InitiateLoginChallenge result = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString() ?? "";
                reader.Read();

                switch (propertyName)
                {
                    case "name":
                        result.Name = reader.GetString() ?? "";
                        break;
                }
            }
        }
        throw new JsonException();
	}
}

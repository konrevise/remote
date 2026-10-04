using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public record struct GetGuardInstructions(
    string AuthenticationToken,
    string Url
);

public class GetGuardInstructionsConverter : JsonConverter<GetGuardInstructions>
{
	public override void Write(Utf8JsonWriter writer,
        GetGuardInstructions value, JsonSerializerOptions options)
	{

	}

	public override GetGuardInstructions Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

		GetGuardInstructions result = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString() ?? "";
                reader.Read();

                switch (propertyName)
                {
                    case "authenticationToken":
                        result.AuthenticationToken = reader.TokenType switch
                        {
                            JsonTokenType.String => reader.GetString() ?? "",
                            _ => ""
                        };
                        break;
                    case "url":
                        result.Url = reader.GetString() ?? "";
                        break;
                }
            }
        }
        throw new JsonException();
	}
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public record struct Logout(string AuthToken);

public class LogoutConverter : JsonConverter<Logout>
{
	public override void Write(Utf8JsonWriter writer, Logout value,
		JsonSerializerOptions options)
	{

	}

	public override Logout Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

        Logout result = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString() ?? "";
                reader.Read();

                switch (propertyName)
                {
                    case "authToken":
                        result.AuthToken = reader.GetString() ?? "";
                        break;
                }
            }
        }
        throw new JsonException();
	}
}

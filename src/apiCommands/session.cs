using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote;

public enum SessionCommand
{
	Nothing = 0,
	PowerOff = 1,
	Reboot = 2,
	Logout = 3,
	SwitchUser = 4,
	Sleep = 5
}

public record struct Session
(
	SessionCommand Command
);

public class SessionConverter : JsonConverter<Session>
{
	public override void Write(Utf8JsonWriter writer, Session value,
		JsonSerializerOptions options)
	{
		
	}

	public override Session Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException();

		Session result = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString() ?? "";
                reader.Read();

                switch (propertyName)
                {
                    case "commandID":
                        result.Command = (SessionCommand)reader.GetInt32();
                        break;
                }
            }
        }
        throw new JsonException();
	}
}

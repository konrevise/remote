using System.Text.Json;
using System.Text.Json.Serialization;

namespace konrevise.Remote.App;

public struct GuardInstructions(string destinationUrl)
{
	public const string DontRedirect = "none";

	public string DestinationUrl { get; set; } = destinationUrl;

	public override string ToString()
	{
		List<string> result = new();
		
		result.Add(DestinationUrl != DontRedirect ?
			$"redirect to {DestinationUrl}" :
			$"dont redirect");

		return string.Join(", ", result);
	}
}

public class GuardInstructionsConverter : JsonConverter<GuardInstructions>
{
	public override void Write(Utf8JsonWriter writer, GuardInstructions value,
		JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName("destinationUrl");
		writer.WriteStringValue(value.DestinationUrl);

		writer.WriteEndObject();
	}

	public override GuardInstructions Read(ref Utf8JsonReader reader,
		Type typeToConvert, JsonSerializerOptions options)
	{
		return default;
	}
}

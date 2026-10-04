using konrevise.Remote.Services;

namespace konrevise.Remote;

public static class ApiHelper
{
	public static IResult BadRequest(string message)
	{
	    return Results.BadRequest(
	        $"something bad happened in the server side: {message}");
	}

	public static IResult BadRequest(Exception exception) =>
	    BadRequest(exception.Message);

	public static IResult JsonContent<T>(T obj)
	{
	    var responseBody = JsonService.Serialize(obj);
	    return Results.Content(responseBody, "application/json");
	}

	public static IResult Unauthorized(string message) =>
	    Results.Json(new { errorMessage = message }, statusCode: 401);
}
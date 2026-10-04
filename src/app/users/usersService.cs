namespace konrevise.Remote.App;

public static class UsersService
{
	// прикрутите какую-нибудь базу данных сюда
	private static IEnumerable<User> Users => new List<User>();

	public static User Identify(string name)
	{
		Console.WriteLine($"identifying {name}...");
		var user = Users.FirstOrDefault(x => x.Name == name);

		if (user is null)
			throw new InvalidOperationException($"could not identify {user}.");

		return user;
	}
}

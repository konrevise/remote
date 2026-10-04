namespace konrevise.Remote.App;

public static class UsersService
{
	private static IEnumerable<User> Users
	{
		get
		{
			if (field != null) return field;

			field = new List<User>()
			{
				new("konrevise")
				{
					Password = new(
			"1c60dbc07bc8c3fdb7201a158df033fd40080bf76c7f6b58aaeabb5d70e18746",
						"3a9b655cbc3666eb9db20752650c5c2e",
						300300
					)
				}
			};
			return field;
		}
	}

	public static User Identify(string name)
	{
		Console.WriteLine($"identifying {name}...");
		var user = Users.FirstOrDefault(x => x.Name == name);

		if (user is null)
			throw new InvalidOperationException($"could not identify {user}.");

		return user;
	}
}

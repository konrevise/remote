namespace konrevise.Remote.App;

public static class AddressesStorage
{
	public const string Domain = ""; // вставьте домен

	public const string Login = "login";
	public const string MainMenu = "mainMenu";

	public static string Empty => $"{Domain}/";
	public static string LoginPage => $"{Empty}{Login}/";
	public static string MainMenuPage => $"{Empty}{MainMenu}/";
}

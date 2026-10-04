namespace konrevise.Remote.App;

public static class AddressesStorage
{
	public const string Domain = "https://192.168.0.108:8080";

	public const string Login = "login";
	public const string MainMenu = "mainMenu";

	public static string Empty => $"{Domain}/";
	public static string LoginPage => $"{Empty}{Login}/";
	public static string MainMenuPage => $"{Empty}{MainMenu}/";
}

namespace konrevise.Remote.App;

public class User(string name)
{
    public string Name { get; init; } = name;
    public Password Password { get; init; }
}

public record struct Password(string Hash, string HashSalt, int HashIterations);

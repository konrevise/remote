using System.Collections.Concurrent;

namespace konrevise.Remote.App;

public record LoginChallenge(User User, string Nonce, string Salt,
	int IterationsCount)
{
	private readonly DateTime expiresAt = DateTime.UtcNow.AddMinutes(2);
	public bool IsExpired => DateTime.UtcNow > expiresAt;
}

public static class LoginChallengesService
{
	private static readonly ConcurrentDictionary<Guid, LoginChallenge>
		challenges = new();

	public static bool TryAdd(Guid id, LoginChallenge instance) =>
		challenges.TryAdd(id, instance);

	public static bool TryTake(Guid id, out LoginChallenge? challenge) =>
		challenges.TryRemove(id, out challenge);

	public static void Collect()
	{
		foreach (var kvp in challenges.ToArray())
			if (kvp.Value.IsExpired) challenges.TryRemove(kvp.Key, out _);
	}
}

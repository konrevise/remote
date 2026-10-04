using System.Collections.Concurrent;
using konrevise.Remote.Services;

namespace konrevise.Remote.App;

public static class SessionsService
{
	// ключ — SHA-256 хеш токена, значение — пользователь
	private static readonly ConcurrentDictionary<string, User> sessions = new();

	public static bool TryRegisterSession(string authenticationToken, User user)
	{
		var hash = StringToHash(authenticationToken);
		// TryAdd атомарно: если ключ уже есть — вернёт false, не перезапишет.
		return sessions.TryAdd(hash, user);
	}

	public static bool TryDeregisterSession(string authenticationToken)
	{
		var hash = StringToHash(authenticationToken);
		return sessions.TryRemove(hash, out _);
	}

	// Возвращает пользователя по токену, или null, если токен невалиден.
	public static User? IdentifyByToken(string authenticationToken)
	{
		var hash = StringToHash(authenticationToken);
		return sessions.TryGetValue(hash, out var user) ? user : null;
	}

	private static string StringToHash(string str) =>
		CryptographyService.StringToSHA256Hash(str);
}

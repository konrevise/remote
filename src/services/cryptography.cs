using System.Text;
using System.Security.Cryptography;

namespace konrevise.Remote.Services;

public static class CryptographyService
{
	public static string BytesToHex(byte[] bytes) =>
		Convert.ToHexString(bytes).ToLowerInvariant();

	public static byte[] HexToBytes(string hex) =>
		Convert.FromHexString(hex);

	public static string StringToSHA256Hash(string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		byte[] hashBytes = SHA256.HashData(bytes);
		return BytesToHex(hashBytes);
	}

	// str — это пароль в открытом виде (UTF-8), не hex.
	// Возвращает hex-строку производного ключа.
	public static string StringToPBKDF2Hash(string str, byte[] salt,
		int iterations)
	{
		const int hashSize = 32;

		byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
			str,
			salt,
			iterations,
			HashAlgorithmName.SHA256,
			hashSize
		);

		return BytesToHex(hash);
	}

	// keyHex — секретный ключ, messageHex — подписываемое сообщение.
	// Оба — hex-строки. Возвращает hex-строку HMAC-SHA256.
	public static string HMAC(string keyHex, string messageHex)
	{
		byte[] keyBytes = HexToBytes(keyHex);
		byte[] messageBytes = HexToBytes(messageHex);
		using var hmac = new HMACSHA256(keyBytes);
		return BytesToHex(hmac.ComputeHash(messageBytes));
	}

	private const int tokenSize = 16;

	public static string CreateRandomToken() =>
		RandomNumberGenerator.GetHexString(tokenSize, true);

	public static byte[] CreateRandomBytesToken() =>
		RandomNumberGenerator.GetBytes(tokenSize);
}

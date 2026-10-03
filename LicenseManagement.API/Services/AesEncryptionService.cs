using System.Security.Cryptography;
using System.Text;

namespace LicenseManagement.API.Services;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(IConfiguration configuration)
    {
        var key = configuration["LicenseEncryption:Key"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "License encryption key is not configured.");
        }

        try
        {
            _key = Convert.FromBase64String(key);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(
                "License encryption key is not a valid Base64 string.");
        }

        if (_key.Length != 32)
        {
            throw new InvalidOperationException(
                "License encryption key must be exactly 32 bytes.");
        }
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();

        aes.Key = _key;
        aes.GenerateIV();

        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        using var encryptor = aes.CreateEncryptor();

        var encryptedBytes = encryptor.TransformFinalBlock(
            plainBytes,
            0,
            plainBytes.Length);

        // Store IV + encrypted data together.
        var result = new byte[aes.IV.Length + encryptedBytes.Length];

        Buffer.BlockCopy(
            aes.IV,
            0,
            result,
            0,
            aes.IV.Length);

        Buffer.BlockCopy(
            encryptedBytes,
            0,
            result,
            aes.IV.Length,
            encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var combinedBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();

        aes.Key = _key;

        var ivLength = aes.BlockSize / 8;

        if (combinedBytes.Length <= ivLength)
        {
            throw new CryptographicException(
                "Invalid license key.");
        }

        var iv = new byte[ivLength];

        var encryptedBytes =
            new byte[combinedBytes.Length - ivLength];

        Buffer.BlockCopy(
            combinedBytes,
            0,
            iv,
            0,
            iv.Length);

        Buffer.BlockCopy(
            combinedBytes,
            ivLength,
            encryptedBytes,
            0,
            encryptedBytes.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();

        var decryptedBytes = decryptor.TransformFinalBlock(
            encryptedBytes,
            0,
            encryptedBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
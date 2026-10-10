using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Bidding_Common.Helpers
{
public static class OtpEncryptionHelper
{
    // Your encryption key must be at least 16 bytes (128-bit) for AES-128, 
    // or 32 bytes (256-bit) for AES-256. Below is a placeholder:
    private static readonly string EncryptionKey = "N0AFJNwU4qlBQDoWmBe4cTmRviW1z4UL05+/6Q+y3CY=";


    public static string EncryptOtp(string plainOtp)
    {
        using (Aes aes = Aes.Create())
        {
            // Convert the Base64 string to a 32-byte array
            aes.Key = Convert.FromBase64String(EncryptionKey);

            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream())
                {
                    // Write the IV first
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        var plainBytes = Encoding.UTF8.GetBytes(plainOtp);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }

                    // Return IV + Ciphertext in Base64
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }


    /// <summary>
    /// Decrypts a Base64-encoded ciphertext OTP.
    /// </summary>
    /// <param name="encryptedOtp">Base64-encoded string of IV + encrypted OTP.</param>
    /// <returns>The decrypted plain-text OTP.</returns>
    public static string DecryptOtp(string encryptedOtp)
    {
        // Convert IV + ciphertext (in Base64) back to bytes
        var cipherBytes = Convert.FromBase64String(encryptedOtp);

        using (Aes aes = Aes.Create())
        {
            // Base64-decode your key string to get a proper 32-byte array
            aes.Key = Convert.FromBase64String(EncryptionKey);

            // Extract the IV from the start of cipherBytes
            byte[] iv = new byte[aes.IV.Length];
            Array.Copy(cipherBytes, iv, iv.Length);

            // Assign the IV to aes
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                // The actual ciphertext is after the IV bytes
                using (var ms = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            // Read the decrypted plain text
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }


        /// <summary>
        /// Convenience method that decrypts a stored OTP and compares it to user input.
        /// </summary>
        /// <param name="userInputOtp">OTP submitted by the user.</param>
        /// <param name="storedEncryptedOtp">The encrypted OTP from your database or cache.</param>
        /// <returns>True if they match; otherwise, false.</returns>
        public static bool CompareOtp(string userInputOtp, string storedEncryptedOtp)
        {
            try
            {
                var decryptedOtp = DecryptOtp(storedEncryptedOtp);
                return userInputOtp == decryptedOtp;

            }
            catch
            {
                return false;
            }
        }
}
}
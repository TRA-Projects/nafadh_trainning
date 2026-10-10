using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Bidding_Common
{
    public static class Crypto
    {
        private static readonly byte[] salt;
        static Crypto()
        {
            var val = "the Password that generate the salt";
            if (string.IsNullOrWhiteSpace(val))
            {
                val = "the Password that generate the salt";
            }
            salt = Encoding.ASCII.GetBytes(val);
        }
       
        private const string Key = "by9NZe3#0i5w0s$Qufcf4kAI";
        private const string secretKey = "8BR9NASQ6LZNXJIQOS6HP1LJ7KDW52O1YXCE0QND4DGA7QNHFC";
        public static string Encrypt(this object plainText)
        {
            string text = plainText == null ? String.Empty : plainText.ToString();
            if (!String.IsNullOrEmpty(text))
            {
                // Get bytes of plaintext string
                byte[] plainBytes = Encoding.UTF8.GetBytes(text);
                byte[] _key = Encoding.UTF8.GetBytes(Key);

                // Get parameter sizes
                int nonceSize = AesGcm.NonceByteSizes.MaxSize;
                int tagSize = AesGcm.TagByteSizes.MaxSize;
                int cipherSize = plainBytes.Length;

                // We write everything into one big array for easier encoding
                int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
                Span<byte> encryptedData = encryptedDataLength < 1024 ? stackalloc byte[encryptedDataLength] : new byte[encryptedDataLength].AsSpan();

                // Copy parameters
                BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
                BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
                var nonce = encryptedData.Slice(4, nonceSize);
                var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
                var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

                // Generate secure nonce
                RandomNumberGenerator.Fill(nonce);

                // Encrypt
                using var aes = new AesGcm(_key);
                aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

                // Encode for transmission
                return Convert.ToBase64String(encryptedData);
            }
            return String.Empty;
         
        }
        public static string Decrypt(this string cipher)
        {
            // Decode
            Span<byte> encryptedData = Convert.FromBase64String(cipher).AsSpan();
            byte[] _key = Encoding.UTF8.GetBytes(Key);

            // Extract parameter sizes
            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            // Extract parameters
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Decrypt
            Span<byte> plainBytes = cipherSize < 1024
                                  ? stackalloc byte[cipherSize]
                                  : new byte[cipherSize];
            using var aes = new AesGcm(_key);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            // Convert plain bytes back into string
            return Encoding.UTF8.GetString(plainBytes);
        }
        public static string DecryptString(this string value)
        {

            if (string.IsNullOrEmpty(value))
                return value;
            var tempValue = value.Replace(' ', '+').Replace('_', '/').Replace('-', '+');
            //if it is not base64, return same value - it is not encrypted
            if (!tempValue.IsBase64String() || value.Length <= 8)
                return value;
            value = tempValue;
            // Declare the AesManaged object
            // used to decrypt the data.
            AesManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(secretKey, salt, 5);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(value);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndelManaged object
                    // with the specified key and IV.
                    aesAlg = new AesManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    aesAlg.Padding = PaddingMode.Zeros;
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the AesManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        public static string EncryptString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;


            string outStr = null;                       // Encrypted string to return
            AesManaged aesAlg = null;              // AesManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(secretKey, salt, 5);
                key.Reset();
                // Create a AesManaged object
                aesAlg = new AesManaged();
                //aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.KeySize / 16);
                aesAlg.Padding = PaddingMode.Zeros;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(value);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {

                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr.Replace('/', '_').Replace('+', '-');
        }

        #region Helpers

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
       

       
       
       
        #endregion
    }
    static class extentions
    {
       

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Bidding_Common
{
    public static class PasswordHelper
    {
        public static string GenerateRandomComplexPassword(int length = 12)
        {
            // Generates a random password with at least:
            // - 1 lowercase
            // - 1 uppercase
            // - 1 digit
            // - 1 symbol

            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()-_=+[]{};:,.?/";


            // Ensure complexity requirements are met by pre-selecting 1 char from each set.
            var chars = new List<char>(length)
            {
                GetRandomChar(lower),
                GetRandomChar(upper),
                GetRandomChar(digits),
                GetRandomChar(symbols)
            };

            var all = lower + upper + digits + symbols;
            while (chars.Count < length)
                chars.Add(GetRandomChar(all));

            // Shuffle to avoid predictable positions.
            Shuffle(chars);

            return new string(chars.ToArray()).Encrypt();
        }

        private static char GetRandomChar(string allowed)
        {
            if (string.IsNullOrEmpty(allowed))
                throw new ArgumentException("Allowed characters set must not be null/empty.", nameof(allowed));

            var index = RandomNumberGenerator.GetInt32(allowed.Length);
            return allowed[index];
        }

        private static void Shuffle(IList<char> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}

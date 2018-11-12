using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Cc.Common.ExtensionMethods
{
    public static class StringExtension
    {
        private const string Key = "IsolucionUpdater";
        private static readonly Random RandomSeed = new Random();

        public static string Encode(this string word)
        {
            var toEncryptArray = Encoding.UTF8.GetBytes(word);

            var hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
            hashmd5.Clear();

            var tripleDesc = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tripleDesc.CreateEncryptor();

            var result = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tripleDesc.Clear();

            return Convert.ToBase64String(result, 0, result.Length);
        }

        public static string Decode(this string words)
        {
            var toEncryptArray = Convert.FromBase64String(words);

            var hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
            hashmd5.Clear();

            var tripleDesc = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tripleDesc.CreateDecryptor();
            var result = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tripleDesc.Clear();
            return Encoding.UTF8.GetString(result);
        }

        public static bool ValidateName(this string input, string pattern)
        {
            var m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static string RandomString(int size, bool lowerCase = false)
        {
            var randStr = new StringBuilder(size);

            var start = lowerCase ? 97 : 65;

            for (var i = 0; i < size; i++)
                randStr.Append((char) (26 * RandomSeed.NextDouble() + start));

            return randStr.ToString();
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
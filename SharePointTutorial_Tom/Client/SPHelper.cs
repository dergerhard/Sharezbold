namespace Client
{
    using Microsoft.SharePoint;
    using System;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Contains our SPHelper class with our extension methods.
    /// </summary>
    public static class SPHelper
    {

        /// <summary>
        /// Encrypts a string using the specified password.
        /// </summary>
        /// <param name="text">The string to be encrypted.</param>
        /// <param name="password">The password which shall be used to encrypt the string.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(this string text, string password)
        {
            return EncDec.Encrypt(text, password);
        }

        /// <summary>
        /// Decrypts a string using the specified password.
        /// </summary>
        /// <param name="text">The string to be decrypted.</param>
        /// <param name="password">The password which shall be used to decrypt the string.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(this string text, string password)
        {
            return EncDec.Decrypt(text, password);
        }
    }

    /// <summary>
    /// Represents our encryption/decryption class.
    /// </summary>
    public static class EncDec
    {
        private static byte[] Encrypt(byte[] clearText, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearText, 0, clearText.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        public static string Encrypt(string clearText, string password)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

        public static string Decrypt(string cipherText, string password)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Key is not available '{0}'", password);
                return null;
            }
        }
    }
}

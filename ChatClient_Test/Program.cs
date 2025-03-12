﻿using System.Security.Cryptography;
using System.Text;
using WebSocketSharp;

namespace ChatClient_Test
{
    public class AesEncryption
    {
        public static byte[] Encrypt(string plaintext, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedBytes;
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
                return encryptedBytes;
            }
        }
        public static string Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] decryptedBytes;
                using (var msDecrypt = new System.IO.MemoryStream(ciphertext))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new System.IO.MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            decryptedBytes = msPlain.ToArray();
                        }
                    }
                }
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string plaintext = "Hello, World!";
            Console.WriteLine(plaintext);
            // Generate a random key and IV
            byte[] key = new byte[32]; // 256-bit key
            byte[] iv = new byte[16]; // 128-bit IV
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }
            // Encrypt
            byte[] ciphertext = AesEncryption.Encrypt(plaintext, key, iv);
            string encryptedText = Convert.ToBase64String(ciphertext);
            Console.WriteLine("Encrypted Text: " + encryptedText);
            // Decrypt
            byte[] bytes = Convert.FromBase64String(encryptedText);
            string decryptedText = AesEncryption.Decrypt(bytes, key, iv);
            Console.WriteLine("Decrypted Text: " + decryptedText);
            //point
            Console.ReadLine();
        }
    }
}

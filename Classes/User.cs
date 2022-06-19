using System;
using System.Collections.Generic;

namespace BlazorServerApp.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string SHA512 { get; set; }

        public string SqlInsertStatement()
        {
            return "INSERT INTO Users (UserName,SHA512,Role) VALUES(@UserName,@SHA512,@Role); ";
        }

        public dynamic SqlAnonymousType()
        {
            return new { UserName = UserName, SHA512 = SHA512, Role = Role };
        }

        public static string CreateSHAHash(string PasswordSHA512)
        {
            System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();

            Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PasswordSHA512));

            sha512.Clear();

            return Convert.ToBase64String(EncryptedSHA512);
        }

        public string SqlUpdateStatement()
        {
            return "UPDATE Users SET Role=@role, SHA512 = @SHA512 WHERE UserName = @UserName";
        }

        public static List<string> GetCurrentRoles()
        {
            return new List<string>() { "Administrator","User" };
        }
    }
}
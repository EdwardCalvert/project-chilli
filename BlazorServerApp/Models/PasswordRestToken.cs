using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class PasswordRestToken :ISqlInsertible,ISqlUpdatible,ISqlDeletible
    {
        Random _random = new Random();
        public const int MAXTOKENSIZE = 20;
        public const int MAXOTPSIZE = 6;
        public PasswordRestToken()
        {

        }
        public PasswordRestToken(string userName)
        {
            UserName = userName;
            ResetTokenID =  GenerateResetToken(MAXTOKENSIZE);
            ResetTokenIDViewed = false;
            OTP = GenerateResetToken(MAXOTPSIZE);
            OTPUsed = false;
        }

        public string GenerateResetToken(int tokenLength)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            string token = "";

            for (int i = 0; i < tokenLength; i++)
            {
                int randomIndex = _random.Next(26);
                token += alphabet[randomIndex];
            }
            return token;
        }

        public string GetAbsoluteURL(string baseUri)
        {
            return   baseUri+ PasswordResetBaseUri + $"/{ResetTokenID}/change"; 
        }

        public const string PasswordResetBaseUri = "admin/newpassword";

        public string ResetTokenID { get; set; }
        public bool ResetTokenIDViewed { get; set; }
        public bool OTPUsed { get; set; }
        public string OTP { get; set; }
        public string UserName { get; set; }

        public string SqlInsertStatement()
        {
            return "INSERT INTO PasswordResetToken(ResetTokenID,ResetTokenIDViewed,OTPUsed,OTP,UserName) VALUES(@ResetTokenID,@ResetTokenIDViewed,@OTPUsed,@OTP,@UserName);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { ResetTokenID = ResetTokenID, ResetTokenIDViewed = ResetTokenIDViewed, OTPUsed = OTPUsed, OTP = OTP, UserName = UserName };
        }

        public string SqlUpdateStatement()
        {
            return "UPDATE PasswordResetToken SET ResetTokenIDViewed = @ResetTokenIDViewed, OTPUsed = @OTPUsed, OTP = @OTP, UserName = @UserName WHERE ResetTokenID = @ResetTokenID";
        }

        public string SqlDeleteStatement()
        {
            return "DELETE FROM PasswordResetToken WHERE ResetTokenID = @ResetTokenID";
        }

        public dynamic SqlDeleteAnonymousType()
        {
            return new { ResetTokenID = ResetTokenID };
        }
    }
}

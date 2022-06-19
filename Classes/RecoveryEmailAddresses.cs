using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class RecoveryEmailAddresses 
    {
        [Required, EmailAddress]
        public string EmailAddress { get; set; }

        [Required, MinLength(2)]
        public string UserName { get; set; }

        public string SqlInsertStatement()
        {
            return "INSERT INTO RecoveryEmailAddress(EmailAddress,UserName) VALUES(@EmailAddress,@UserName);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { EmailAddress = EmailAddress, UserName = UserName };
        }

        public string SqlDeleteStatement()
        {
            return "DELETE FROM RecoveryEmailAddress WHERE EmailAddress = @EmailAddress ";
        }

        public dynamic SqlDeleteAnonymousType()
        {
            return new { EmailAddress = EmailAddress };
        }
    }
}
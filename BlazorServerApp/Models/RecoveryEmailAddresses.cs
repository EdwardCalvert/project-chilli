using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class RecoveryEmailAddresses : ISqlDeletible, ISqlInsertible
    {

        public string EmailAddress { get; set; }
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
            return new { EmailAddress = EmailAddress};
        }
    }
}

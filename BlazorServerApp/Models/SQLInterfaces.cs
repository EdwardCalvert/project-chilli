using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public interface ISqlInsertible
    {
        public string SqlInsertStatement();
        public dynamic SqlAnonymousType();
    }

    public interface ISqlUpdatible
    {
        public string SqlUpdateStatement();
        public dynamic SqlAnonymousType();
    }

    public interface ISqlDeletible
    {
        public string SqlDeleteStatement();
        public dynamic SqlDeleteAnonymousType();
    }
}

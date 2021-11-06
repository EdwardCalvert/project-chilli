using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataLibrary;
using BlazorServerApp.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorServerApp.Models
{
    public class DataComponent : ComponentBase
    {
        //[Inject]
        //public IDataAccess _data { get; set; }
        //[Inject]
        //public IConfiguration _config { get; set; }

        public DataComponent() : base()
        {
        
        }
    }
}

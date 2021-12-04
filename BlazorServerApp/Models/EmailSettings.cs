using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class EmailSettings
    {
        public string From { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

	}
	public class EmailAddress
	{
		public EmailAddress()
		{

		}

		public EmailAddress(string name, string address)
		{
			Name = name;
			Address = address;
		}

		public string Name { get; set; }
		public string Address { get; set; }
	}
	public class EmailMessage
	{
		public string Subject { get; set; }
		public string Content { get; set; }
	}

}

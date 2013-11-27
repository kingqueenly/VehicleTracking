using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace VT.WebService
{
    public class CredentialSoapHeader : SoapHeader
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool CheckUsernameAndPassword()
        {
            return true;
        }
    }
}
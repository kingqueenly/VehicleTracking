using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VT.Model
{
    public class UserInformation
    {
        public long UserID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Mobile { get; set; }

        public int RoleID { get; set; }
    }
}

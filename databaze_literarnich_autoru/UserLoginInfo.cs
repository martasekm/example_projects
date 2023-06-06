using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    internal class UserLoginInfo
    {
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public UserLoginInfo(string hash, UserRole role)
        {
            PasswordHash = hash;
            Role = role;
        }
    }
}

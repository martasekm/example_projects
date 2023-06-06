using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.UserRoles
{
    public class Editor : UserRole
    {
        public Editor()
        {
            HasEditPermissions = true;
            HasCreatePermissions = true; 
            HasDeletePermissions = true;
        }
    }
}

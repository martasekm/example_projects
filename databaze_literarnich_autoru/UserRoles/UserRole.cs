using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    public abstract class UserRole
    {
        public bool HasEditPermissions { get; protected set; } = false;
        public bool HasCreatePermissions { get; protected set; } = false;
        public bool HasDeletePermissions { get; protected set; } = false;
        public bool HasProgramConfigurationEditPermissions { get; protected set; } = false;
    }
}

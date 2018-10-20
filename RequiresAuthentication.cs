using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi_Online_Server_1._1
{
    [AttributeUsage(AttributeTargets.Method)]
    class RequiresAuthentication : Attribute
    {
        public RequiresAuthentication()
        {
            
        }
    }
}

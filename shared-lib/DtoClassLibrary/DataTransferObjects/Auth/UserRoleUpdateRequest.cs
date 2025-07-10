using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferLib.DataTransferObjects.Auth
{
    public class UserRoleUpdateRequest
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }
}

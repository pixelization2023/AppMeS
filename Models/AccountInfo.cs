using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLGMes;

namespace AppMeS.Models
{
    public class AccountInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public UserInfoDto UserInfo { get; set; }
    }
}

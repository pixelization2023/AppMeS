using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WLGMes
{
    public class WLGLogin : WLGMesBase
    {
        //登录对象
        public LoginPost LoginPost { get; set; } = new LoginPost
        {
            username = "hank",
            password = "123456"
        };

        public ApiResponse<LoginResultDto> LoginResult { get; set; } = new ApiResponse<LoginResultDto>();

        public WLGLogin()
        {
            Url = "https://wlgmes-uat.aacoptics.com/sky-boot/sys/login";
        }
        public ApiResponse<LoginResultDto> Login(LoginPost LoginPost, out string responseJson)
        {
            try
            {
                string LoginJson = JsonConvert.SerializeObject(LoginPost);

                var content = new StringContent(LoginJson, Encoding.UTF8, "application/json");

                var response = WebClient.PostAsync(Url, content).GetAwaiter().GetResult();

                responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                LoginResult = JsonConvert.DeserializeObject<ApiResponse<LoginResultDto>>(responseJson);

                return LoginResult;
            }
            catch (Exception ex)
            {
                responseJson = ex.Message;
                throw;
            }
        }
    }
    public class LoginPost
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginResultDto
    {
        public UserInfoDto UserInfo { get; set; }

        public string Token { get; set; }
    }
    public class UserInfoDto
    {
        public string Avatar { get; set; }

        public string Birthday { get; set; }

        public string CreateBy { get; set; }

        public string CreateTime { get; set; }

        public string DelFlag { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string RealName { get; set; }

        public string Salt { get; set; }

        public int? Sex { get; set; }

        public int Status { get; set; }

        public string UpdateBy { get; set; }

        public string UpdateTime { get; set; }

        public string UserName { get; set; }
    }
}

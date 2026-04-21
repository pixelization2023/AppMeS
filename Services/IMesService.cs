using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLGMes;

namespace AppMeS.Services
{
    public interface IMesService
    {

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<(bool Success, string Token, UserInfoDto UserInfo)> LoginAsync(string username, string password);
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="machineCode"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        Task<List<OrderResultDto>> GetOrdersAsync(string machineCode, string stationCode);

        /// <summary>
        /// 过站
        /// </summary>
        /// <param name="trackOutData"></param>
        /// <returns></returns>
        Task<(bool Success, TrackOutResultDto Result)> TrackOutAsync(TrackOutPut trackOutData);
    }
}

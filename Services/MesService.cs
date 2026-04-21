using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppMeS.Comm;
using AppMeS.Models;
using Serilog;
using WLGMes;

namespace AppMeS.Services
{
    public class MesService : IMesService
    {

        private readonly ILogger _logger;
        private string _authToken;
        private PlcMesConfig _config;

        public MesService(ILogger logger)
        {
            _logger = logger;
            _config = ConfigHelper.LoadConfig();
        }

        public async Task<(bool Success, string Token, UserInfoDto UserInfo)> LoginAsync(string username, string password)
        {
            try
            {
                var login = new WLGLogin();
                login.LoginPost.username = username;
                login.LoginPost.password = password;
                var result = await Task.Run(() => login.Login(login.LoginPost, out string json));
                if (result.Code == 200 && result.Success)
                {
                    _authToken = result.Result.Token;
                    _logger.Information("MES 登录成功: {User}", username);
                    return (true, _authToken, result.Result.UserInfo);
                }
                else
                {
                    _logger.Warning("MES 登录失败: {Msg}", result.Message ?? result.Msg);
                    return (false, null, null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MES 登录异常");
                return (false, null, null);
            }
        }

        public async Task<List<OrderResultDto>> GetOrdersAsync(string machineCode, string stationCode)
        {
            try
            {
                var order = new WLGOrder();
                order.OrderGet.machinecode = machineCode;
                order.OrderGet.stationCode = stationCode;
                var result = await Task.Run(() => order.Order(order.OrderGet, out string json));
                if (result.Code == 200 && result.Success)
                {
                    _logger.Information("获取工单成功，数量: {Count}", result.Result?.Count ?? 0);
                    return result.Result ?? new List<OrderResultDto>();
                }
                else
                {
                    _logger.Warning("获取工单失败: {Msg}", result.Message);
                    return new List<OrderResultDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "获取工单异常");
                return new List<OrderResultDto>();
            }
        }

        public async Task<(bool Success, TrackOutResultDto Result)> TrackOutAsync(TrackOutPut trackOutData)
        {
            try
            {
                var track = new WLGTrackOut();
                // 如果 MES 需要 Token，可以在这里设置 Header（需扩展 WLGTrackOut 添加 HttpClient 的默认 Headers）
                var result = await Task.Run(() => track.TrackOut(trackOutData, out string json));
                if (result.Code == 200 && result.Success)
                {
                    _logger.Information("过站上报成功: {Carrier}/{Sn}", trackOutData.carrier, trackOutData.sn);
                    return (true, result.Result);
                }
                else
                {
                    _logger.Warning("过站上报失败: {Msg}", result.Message);
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "过站上报异常");
                return (false, null);
            }
        }
    }
}

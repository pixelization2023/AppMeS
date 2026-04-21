using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppMeS.Comm;
using AppMeS.Models;
using HslCommunication.Profinet.Inovance;
using HslCommunication.Profinet.Siemens;
using Serilog;

namespace AppMeS.Services
{
    public class PlcService:IPlcService
    {
        private InovanceTcpNet _plc;
        private readonly ILogger _logger;
        private PlcMesConfig _config;
        private bool _isConnected;
        private CancellationTokenSource _cts;
        private Task _heartbeatTask;
        private Task _triggerScanTask;

        public event Action<string, object> DataChanged;

        public bool IsConnected => _isConnected;

        public PlcService(ILogger logger)
        {
            _logger = logger;
            _config = ConfigHelper.LoadConfig();
        }

        public async Task<bool> ConnectAsync(string ip, int port, short station = 1)
        {
            try
            {
                _plc?.ConnectClose();
                _plc = new InovanceTcpNet(InovanceSeries.AM, ip, port, (byte)station);
                _plc.ConnectTimeOut = 2000;
                var result = await Task.Run(() => _plc.ConnectServer());
                if (result.IsSuccess)
                {
                    _isConnected = true;
                    _logger.Information("PLC 连接成功: {Ip}:{Port}", ip, port);
                    StartBackgroundTasks();
                    return true;
                }
                else
                {
                    _logger.Error("PLC 连接失败: {Msg}", result.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PLC 连接异常");
                return false;
            }
        }

        public void Disconnect()
        {
            _cts?.Cancel();
            _heartbeatTask?.Wait(1000);
            _triggerScanTask?.Wait(1000);
            _plc?.ConnectClose();
            _isConnected = false;
            _logger.Information("PLC 已断开");
        }

        public async Task<bool> ReadBoolAsync(string address)
        {
            if (!_isConnected) return false;
            var result = await Task.Run(() => _plc.ReadBool(address));
            return result.IsSuccess && result.Content;
        }

        public async Task<int> ReadInt32Async(string address)
        {
            if (!_isConnected) return 0;
            var result = await Task.Run(() => _plc.ReadInt32(address));
            return result.IsSuccess ? result.Content : 0;
        }

        public async Task WriteBoolAsync(string address, bool value)
        {
            if (!_isConnected) return;
            await Task.Run(() => _plc.Write(address, value));
        }

        public async Task WriteInt32Async(string address, int value)
        {
            if (!_isConnected) return;
            await Task.Run(() => _plc.Write(address, value));
        }

        public async Task WriteStringAsync(string address, string value, int length = 20)
        {
            if (!_isConnected) return;
            await Task.Run(() => _plc.Write(address, value, length));
        }

        private void StartBackgroundTasks()
        {
            _cts = new CancellationTokenSource();
            _heartbeatTask = Task.Run(() => HeartbeatLoop(_cts.Token));
            _triggerScanTask = Task.Run(() => TriggerAndScannerLoop(_cts.Token));
        }

        private async Task HeartbeatLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_isConnected)
                {
                    var result = _plc.ReadBool("MW10096", 1);
                    if (!result.IsSuccess)
                    {
                        _isConnected = false;
                        _logger.Warning("PLC 心跳丢失，尝试重连...");
                        await ReconnectAsync();
                    }
                }
                await Task.Delay(1000, token);
            }
        }
        /// <summary>
        /// 重新连接
        /// </summary>
        /// <returns></returns>
        private async Task ReconnectAsync()
        {
            for (int i = 0; i < 3; i++)
            {
                if (await ConnectAsync(_config.PlcIp, _config.PlcPort))
                    return;
                await Task.Delay(2000);
            }
            _logger.Error("PLC 重连失败，请检查网络");
        }

        private async Task TriggerAndScannerLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_isConnected)
                {
                    // 处理视觉触发信号
                    foreach (var mapping in _config.TriggerMappings.Values)
                    {
                        var value = _plc.ReadInt32(mapping.Address).Content;
                        if (value.ToString() == mapping.ExpectedValue)
                        {
                            _logger.Information("PLC 触发信号 {Addr} = {Val}", mapping.Address, value);
                            DataChanged?.Invoke("VisionTrigger", mapping);
                            // 可选：复位触发信号（根据协议决定是否自动复位）
                            // await WriteInt32Async(mapping.Address, 0);
                        }
                    }

                    // 处理扫码枪触发信号
                    foreach (var scanner in _config.ScannerTriggerMappings.Values)
                    {
                        var triggerValue = _plc.ReadInt16(scanner.TriggerAddress).Content;
                        if (triggerValue.ToString() == scanner.ExpectedValue)
                        {
                            _logger.Information("扫码枪触发 {Addr}", scanner.TriggerAddress);
                            DataChanged?.Invoke("ScannerTrigger", scanner);
                            // 注意：不在此处复位，等待业务处理完成后写入结果
                        }
                    }
                }
                await Task.Delay(50, token); // 轮询间隔
            }
        }
    }
}



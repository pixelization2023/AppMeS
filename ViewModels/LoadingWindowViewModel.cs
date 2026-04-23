using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using AppMeS.Comm;
using AppMeS.Models;
using AppMeS.Services;
using Serilog;
using Serilog.Core;

namespace AppMeS.ViewModels
{
    public  class LoadingWindowViewModel : BindableBase, IDialogAware
    {

        private readonly IPlcService _plcService;
        private readonly ILogger _logger;
        private readonly Dispatcher _dispatcher;


        public LoadingWindowViewModel(ILogger logger, IPlcService plcService)
        {

            _dispatcher= Dispatcher.CurrentDispatcher;
           _logger = logger;
            _plcService = plcService;
        }
        public DialogCloseListener RequestClose { get; set; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            ProgressValue = 0;
            StatusMessage = "正在初始化系统组件...";
            _timer.Start();
        }

            try
        {
                await LoadAsync();
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }
            catch (Exception ex)
        {
                _logger.Error(ex, "加载过程发生异常");
                StatusMessage = $"加载失败: {ex.Message}";
                await Task.Delay(1500);
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        }

        private async Task LoadAsync()
        {
            // 步骤1: HSL 授权
            UpdateProgress(5, "激活 HSL 通讯授权...");
            await Task.Run(() =>
            {
                var success = HslCommunication.Authorization.SetAuthorizationCode("2b3b2d73-01ff-4f68-b39f-fcc1bfb82b54");
                _logger.Information(success ? "HSL 授权成功" : "HSL 授权失败");
            });
            UpdateProgress(20);

            // 步骤2: 连接 PLC（带重试）
            UpdateProgress(25, "连接 PLC...");
            var config = ConfigHelper.LoadConfig();
            bool plcConnected = false;
            for (int i = 0; i < 3; i++)
            {
                plcConnected = await _plcService.ConnectAsync(config.PlcIp, config.PlcPort);
                if (plcConnected) break;
                _logger.Warning($"PLC 连接失败，正在重试 ({i + 1}/3)...");
                StatusMessage = $"PLC 连接失败，重试 {i + 1}/3...";
                await Task.Delay(1000);
            }
            //if (!plcConnected)
            //{
            //    _logger.Warning("PLC 连接最终失败，将使用模拟模式");
            //    StatusMessage = "PLC 连接失败，使用模拟模式";
            //    await Task.Delay(800);
            //}
            UpdateProgress(50);

            // 步骤3: 加载配置参数（可扩展更多配置）
            UpdateProgress(55, "加载配置参数...");
            await Task.Run(() =>
            {
                // 加载其他配置（如视觉参数、MES 配置等）
                // 模拟耗时操作
                System.Threading.Thread.Sleep(200);
            });
            UpdateProgress(80);

            // 步骤4: 初始化其他组件（如视觉流程）
            UpdateProgress(85, "初始化视觉组件...");
            await Task.Run(() =>
            {
                // 例如：VmSolution.Load(...)
                System.Threading.Thread.Sleep(100);
            });
            UpdateProgress(95);

            // 步骤5: 完成
            UpdateProgress(100, "加载完成！");
            await Task.Delay(300); // 让用户看到 100%
            }

        private void UpdateProgress(double value, string message = null)
        {
            // 确保在 UI 线程更新
            if (_dispatcher.CheckAccess())
            {
                ProgressValue = value;
                if (message != null) StatusMessage = message;
            }
            else
            {
                _dispatcher.Invoke(() =>
            {
                    ProgressValue = value;
                    if (message != null) StatusMessage = message;
                });
            }
            }
        private double _progressValue;
        public double ProgressValue
            {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
            }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }



    }
}

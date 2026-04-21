using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using AppMeS.Services;
using Serilog;
using Serilog.Core;

namespace AppMeS.ViewModels
{
    public  class LoadingWindowViewModel : BindableBase, IDialogAware
    {
        private readonly DispatcherTimer _timer;
        private readonly IPlcService _plcService;
        private readonly ILogger logger;
        public LoadingWindowViewModel(ILogger logger, IPlcService plcService)
        {

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1); // 每50ms更新一次
            _timer.Tick +=OnTimerTickAsync;
            this.logger = logger;
            _plcService = plcService;
        }
        public DialogCloseListener RequestClose { get; set; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _timer.Stop();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ProgressValue = 0;
            StatusMessage = "正在初始化系统组件...";
            _timer.Start();
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


        // 定时器触发：模拟进度增加
        private async void OnTimerTickAsync(object sender, EventArgs e)
        {
            if (ProgressValue < 20)
            {
                ProgressValue += 2;
                StatusMessage = "激活 HSL 通讯授权...";
                HslCommunication.Authorization.SetAuthorizationCode("2b3b2d73-01ff-4f68-b39f-fcc1bfb82b54");
            }
            else if (ProgressValue < 50)
            {
                ProgressValue += 2;
                StatusMessage = "连接 PLC...";
                var connected = await _plcService.ConnectAsync("192.168.0.1", 102); // 修改为实际IP
                if (!connected) logger.Warning("PLC 连接失败，将使用模拟模式");
            }
            else if (ProgressValue < 80)
            {
                ProgressValue += 1.5;
                StatusMessage = "加载配置参数...";
                await Task.Delay(200);
            }
            else if (ProgressValue < 100)
            {
                ProgressValue += 1;
                StatusMessage = "准备就绪...";
            }
            else
            {
                _timer.Stop();
                StatusMessage = "加载完成！";
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AppMeS.Comm;
using AppMeS.Models;
using AppMeS.Services;
using Newtonsoft.Json.Linq;
using Serilog;
using WLGMes;

namespace AppMeS.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {
      

        private readonly IMesService _mesService;
        private readonly IPlcService _plcService;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;
        private PlcMesConfig _config;

        public MainWindowViewModel(IMesService mesService, IPlcService plcService, ILogger logger, IDialogService dialogService)
        {
            _mesService = mesService;
            _plcService = plcService;
            _logger = logger;
            _dialogService = dialogService;
            _config = ConfigHelper.LoadConfig();


            Orders = new ObservableCollection<OrderResultDto>();
            EventLogs = new ObservableCollection<string>();


            // 初始化界面绑定属性
            MachineCode = _config.MachineCode;
            StationCode = _config.StationCode;
            SectionCode = _config.SectionCode;
            PlcIp = _config.PlcIp;
            PlcPort = _config.PlcPort;
            DataRetentionDays = _config.DataRetentionDays;

            // 订阅 PLC 数据变化
            _plcService.DataChanged += OnPlcDataChanged;


            // 默认加载工单
          ExecuteLoadOrdersCommandAsync();
        }

      

        private void OnPlcDataChanged(string address, object value)
        {
            // 收到 PLC 触发信号
            if (address == "Trigger" && value is true)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddLog("PLC 触发信号到达，开始视觉检测...");
                    // 模拟检测结果 OK/NG
                    var random = new Random();
                    bool isOk = random.Next(0, 100) > 5; // 95% 良率
                    SnStatus = isOk ? "OK" : "NG";
                    ResultText = isOk ? "检测合格" : "检测不合格";

                    // 将结果写回 PLC（例如 DB1.DBX1.0 为 OK 标志）
                    _plcService.WriteBoolAsync("DB1.DBX1.0", isOk);
                    AddLog($"检测完成，结果: {ResultText}");
                });
            }
        }


        private void AddLog(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                EventLogs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
                if (EventLogs.Count > 100) EventLogs.RemoveAt(EventLogs.Count - 1);
                _logger.Information(message);
            });
        }


        #region 绑定属性


        // 配置绑定
        public string MachineCode { get; set; }
        public string StationCode { get; set; }
        public string SectionCode { get; set; }
        public string PlcIp { get; set; }
        public int PlcPort { get; set; }
        public int DataRetentionDays { get; set; }


        private OrderResultDto _selectedOrder;
        public OrderResultDto SelectedOrder
        {
            get { return _selectedOrder; }
            set { SetProperty(ref _selectedOrder, value); }
        }


        private ObservableCollection<OrderResultDto> _orders;
        public ObservableCollection<OrderResultDto> Orders
        {
            get { return _orders; }
            set { SetProperty(ref _orders, value); }
        }

        private ObservableCollection<string> _eventLogs;
        public ObservableCollection<string> EventLogs{       
            get { return _eventLogs; }
            set { SetProperty(ref _eventLogs, value); }
        }
      

        private string _palletCode;
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }


        private string _scanInfo;
        public string ScanInfo { get => _scanInfo; set => SetProperty(ref _scanInfo, value); }

        private bool _isInput = true;
        public bool IsInput { get => _isInput; set => SetProperty(ref _isInput, value); }

        private string _resultText = "等待扫码...";
        public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }

        private string _snStatus = "未连接";
        private ObservableCollection<string> eventLogs;

        public string SnStatus { get => _snStatus; set => SetProperty(ref _snStatus, value); }
        #endregion



        #region 命令

        private DelegateCommand _loadOrdersCommand;
        public DelegateCommand LoadOrdersCommand =>
            _loadOrdersCommand ?? (_loadOrdersCommand = new DelegateCommand(ExecuteLoadOrdersCommandAsync));

        async void ExecuteLoadOrdersCommandAsync()
        {
            AddLog("正在加载工单...");
            var orders = await _mesService.GetOrdersAsync("JYZ03", "hank001_2_M102");
            Orders.Clear();
            foreach (var order in orders)
                Orders.Add(order);
            AddLog($"加载完成，共 {orders.Count} 个工单");
        }


        private DelegateCommand _startDetectionCommand;
        public DelegateCommand StartDetectionCommand =>
            _startDetectionCommand ?? (_startDetectionCommand = new DelegateCommand(ExecuteStartDetectionCommand));

        void ExecuteStartDetectionCommand()
        {
            AddLog("启动自动检测模式，等待 PLC 触发...");
            // 实际业务中可启动循环监听
        }

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand =>
            _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand));

        void ExecuteStopCommand()
        {
            AddLog("停止检测");
        }


        private DelegateCommand _saveDataCommand;
        public DelegateCommand SaveDataCommand =>
            _saveDataCommand ?? (_saveDataCommand = new DelegateCommand(ExecuteSaveDataCommand));

        void ExecuteSaveDataCommand()
        {
            AddLog("数据保存（演示）");
        }


        private DelegateCommand _sendTrackOutCommand;
        public DelegateCommand SendTrackOutCommand =>
            _sendTrackOutCommand ?? (_sendTrackOutCommand = new DelegateCommand(ExecuteSendTrackOutCommandAsync));

        async void ExecuteSendTrackOutCommandAsync()
        {
            if (SelectedOrder == null)
            {
                ResultText = "请先选择工单";
                return;
            }
            if (string.IsNullOrEmpty(PalletCode) && string.IsNullOrEmpty(ScanInfo))
            {
                ResultText = "请输入托盘码或 SN";
                return;
            }

            var trackData = new TrackOutPut
            {
                carrier = PalletCode,
                sn = ScanInfo,
                machineCode = SelectedOrder.machineCode,
                materialCode = SelectedOrder.materialCode,
                orderItemMachineNo = SelectedOrder.orderItemMachineNo,
                projectCode = SelectedOrder.projectCode,
                quantity = 1,
                scanFlag = IsInput ? "input" : "output",
                scanInfo = string.IsNullOrEmpty(ScanInfo) ? "carrier" : "sn",
                stationCode = "hank001_2_M101",
                createBy = "Operator"
            };

            var (success, result) = await _mesService.TrackOutAsync(trackData);
            if (success)
            {
                ResultText = "过站成功！";
                AddLog($"过站上报成功，载具: {PalletCode}, SN: {ScanInfo}");
                // 可选清空输入
                PalletCode = "";
                ScanInfo = "";
            }
            else
            {
                ResultText = "过站失败，请检查网络或重试";
            }
        }


        #endregion
    }
}

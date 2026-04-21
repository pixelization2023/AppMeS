using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppMeS.Models;
using AppMeS.Services;
using Serilog;

namespace AppMeS.ViewModels
{
    public class LogWindowViewModel:BindableBase,IDialogAware
    {

        private readonly IMesService _mesService;
        private readonly ILogger _logger;
        public LogWindowViewModel(IMesService mesService, ILogger logger)
        {
            UserName = "hank";
            PassWord = "123456";
            _mesService = mesService;
            _logger = logger;
        }

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }


        private DelegateCommand _LoginCommand;
        public DelegateCommand LoginCommand =>
            _LoginCommand ?? (_LoginCommand = new DelegateCommand(async () => await ExecuteLoginCommandAsync()));

        async Task ExecuteLoginCommandAsync()
        {
            if (UserName != string.Empty || PassWord != string.Empty)
            {
                var parameters = new DialogParameters {
                    { "AccountInfo", new AccountInfo(){ UserName = this.UserName,
                        Password = this.PassWord } } };
                RequestClose.Invoke(parameters, ButtonResult.OK);
            }

            //if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PassWord))
            //    return;

            //IsLogging = true;
            //var (success, token, userInfo) = await _mesService.LoginAsync(UserName, PassWord);
            //IsLogging = false;

            //if (success)
            //{
            //    var parameters = new DialogParameters
            //    {
            //        { "AccountInfo", new AccountInfo { UserName = UserName, Password = PassWord, Token = token, UserInfo = userInfo } }
            //    };
            //    RequestClose.Invoke(parameters, ButtonResult.OK);
            //}
            //else
            //{
            //    // 提示登录失败
            //    //var dialogService = new DialogService(); // 或通过构造函数注入
            //    // 可显示错误弹窗
            //}
        }

        private DelegateCommand forgotPasswordCommand;
        public DelegateCommand ForgotPasswordCommand =>
            forgotPasswordCommand ?? (forgotPasswordCommand = new DelegateCommand(ExecuteForgotPasswordCommand));

        void ExecuteForgotPasswordCommand()
        {
            RequestClose.Invoke(ButtonResult.Cancel);
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }


        private string _passWord;
        public string PassWord
        {
            get { return _passWord; }
            set { SetProperty(ref _passWord, value); }
        }

        private bool _isLogging;
        public bool IsLogging
        {
            get { return _isLogging; }
            set { SetProperty(ref _isLogging, value); }
        }
    }
}

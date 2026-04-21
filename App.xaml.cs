using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;
using Serilog;
using System.Windows.Threading;
using AppMeS.Views;
using System.IO;
using AppMeS.Services;

namespace AppMeS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //注册
            containerRegistry.RegisterDialog<LoadingWindow>();
            containerRegistry.RegisterDialog<LogWindow>();
            containerRegistry.RegisterDialog<ForgetPass>();


            containerRegistry.RegisterSingleton<IMesService, MesService>();
            containerRegistry.RegisterSingleton<IPlcService, PlcService>();
            containerRegistry.RegisterSingleton<ILogger>(_ =>
        new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(path: @"Logs/log.txt",
        rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 50 * 1024 * 1024, flushToDiskInterval: TimeSpan.FromMilliseconds(5))
        .CreateLogger());

            // 注册自定义对话框窗口容器
            containerRegistry.RegisterDialogWindow<CustomDialogWindow>();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            //注册全局异常处理
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;


            base.OnStartup(e);

        }



        #region 异常处理

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 立即处理崩溃，不等待异步完成
            HandleCrash(e.ExceptionObject as Exception);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleCrash(e.Exception);
            e.Handled = true;  // 标记为已处理
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleCrash(e.Exception);
            e.SetObserved(); // 标记为已观察
        }

        private void HandleCrash(Exception ex)
        {
            try
            {
                var crashInfo = $"[崩溃时间] {DateTime.Now}\n" +
                               $"[异常类型] {ex?.GetType().Name}\n" +
                               $"[异常信息] {ex?.Message}\n" +
                               $"[堆栈跟踪] {ex?.StackTrace}\n\n";

          
                
                var logger = Container.Resolve<ILogger>();
                logger.Information("程序崩溃，正在执行资源清理...");
                logger.Error(crashInfo);

            }
            finally
            {
                try
                {
                    // 先执行资源清理

                }
                catch (Exception cleanupEx)
                {
                 
                    var logger = Container.Resolve<ILogger>();
                    logger.Error("清理资源失败: {Message}", cleanupEx.Message);
                }
                finally
                {
                    Application.Current?.Dispatcher.InvokeShutdown();
                    Environment.Exit(1);
                }
            }
        }
        #endregion

        protected override void Initialize()
        {
            base.Initialize();
            var dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog("LogWindow", callback: r =>

            {

                if (r.Result == ButtonResult.OK)
                {

                }
                //点击忘记密码
                if (r.Result == ButtonResult.Cancel)
                {
                    dialogService.ShowDialog("ForgetPass", r =>
                    {
                        Application.Current.Shutdown();
                    });
                }




            });
            dialogService.ShowDialog("LoadingWindow", callback: r =>
            {
                //在这里处理对话框关闭后的逻辑 
                if (r.Result == ButtonResult.OK)
                {
                    var parameters = r.Parameters; // 获取传递的参数
                }
                else
                {
                    //用户点击了取消按钮或关闭了对话框  Application.Current.Shutdown();
                }
            });
            var logger = Container.Resolve<ILogger>();

            logger.Information("程序启动");

        }
        override protected void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //在应用程序退出时执行的代码
            var logger = Container.Resolve<ILogger>();
            logger.Information("程序退出");
        }

    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppMeS.Comm
{
    /// <summary>
    /// 为 PasswordBox 提供可绑定的 Password 附加属性
    /// </summary>
    public static class PasswordBoxHelper
    {
        // 1. 可绑定的密码属性（用于 ViewModel 绑定）
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        // 当 ViewModel 中的密码变化时，更新 PasswordBox
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // 避免循环更新：仅在外部更新时修改 PasswordBox 的实际密码
                passwordBox.PasswordChanged -= HandlePasswordChanged;
                passwordBox.Password = (string)e.NewValue;
                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        // 2. 启用绑定的开关附加属性（用于挂载事件）
        public static readonly DependencyProperty EnablePasswordBindingProperty =
            DependencyProperty.RegisterAttached(
                "EnablePasswordBinding",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnEnablePasswordBindingChanged));

        public static bool GetEnablePasswordBinding(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnablePasswordBindingProperty);
        }

        public static void SetEnablePasswordBinding(DependencyObject obj, bool value)
        {
            obj.SetValue(EnablePasswordBindingProperty, value);
        }

        private static void OnEnablePasswordBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += HandlePasswordChanged;
                }
                else
                {
                    passwordBox.PasswordChanged -= HandlePasswordChanged;
                }
            }
        }

        // 当用户输入密码时，将新密码同步回 ViewModel 的绑定源
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // 更新附加属性 BoundPassword（会触发绑定源的更新）
                SetBoundPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}

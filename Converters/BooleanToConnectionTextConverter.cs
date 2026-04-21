using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AppMeS.Converters
{
    public class BooleanToConnectionTextConverter : IValueConverter
    {
        // 当绑定源数据 -> 绑定目标时调用
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected)
            {
                return isConnected ? "已连接" : "未连接";
            }
            return "未连接";
        }

        // 双向绑定需要，这里不需要实现回写
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

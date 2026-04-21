using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeS.Services
{
    public interface IPlcService
    {
        bool IsConnected { get; }
        Task<bool> ConnectAsync(string ip, int port, short station = 0);
        void Disconnect();
        Task<bool> ReadBoolAsync(string address);
        Task<int> ReadInt32Async(string address);
        Task WriteBoolAsync(string address, bool value);
        Task WriteInt32Async(string address, int value);
        event Action<string, object> DataChanged;
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WLGMes
{
    /// <summary>
    /// WebApi的通用返回接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Msg { get; set; }
        public T Result { get; set; }
        public bool Success { get; set; }
        public long Timestamp { get; set; }
        public string Tips { get; set; }
    }

    /// <summary>
    /// 接口调用的父类，主要用于抽取公共方法
    /// </summary>
    public class WLGMesBase
    {
        public string Url { get; set; }
        static public HttpClientHandler WebHandler { get; set; } = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        //登录http客户端，服务器地址相同因此使用静态对象便于提升性能  20260206-LWQ
        static public HttpClient WebClient { get; set; }
        public WLGMesBase()
        {
            if (WebClient == null)
            {
                WebClient = new HttpClient(WebHandler);
            }
        }

        /// <summary>
        /// 把类的属性保存成json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public void SaveConfig<T>(T obj, string filePath)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            File.WriteAllText(filePath, json, Encoding.UTF8);
        }
        /// <summary>
        /// 从json文件中加载后实例化为类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public void LoadConfig<T>(T obj, string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var json = File.ReadAllText(filePath, Encoding.UTF8);

            obj = JsonConvert.DeserializeObject<T>(json);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppMeS.Models;

namespace AppMeS.Comm
{
    /// <summary>
    /// 读写配置文件JSON储存
    /// </summary>
    public static class ConfigHelper
    {

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "plc_mes_config.json");

        static ConfigHelper()
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static PlcMesConfig LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                return GetDefaultConfig();
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<PlcMesConfig>(json) ?? GetDefaultConfig();
        }

        public static void SaveConfig(PlcMesConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }

        private static PlcMesConfig GetDefaultConfig()
        {
            var config = new PlcMesConfig
            {
                PlcIp = "192.168.0.1",
                PlcPort = 502,
                MachineCode = "HPM001",
                StationCode = "hank001_2_M101",
                SectionCode = "1230121167_1_M5",
                DataRetentionDays = 7,
                TriggerMappings = new Dictionary<string, TriggerMapping>
                {
                    ["拍照位置1"] = new TriggerMapping
                    {
                        Address = "MW10196",
                        ExpectedValue = "1",
                        CameraName = "相机1",
                        ProcedureOrder = "1",
                        FeedbackAddress = "MW10198"
                    }
                },
                ScannerTriggerMappings = new Dictionary<string, ScannerTriggerMapping>
                {
                    ["Input"] = new ScannerTriggerMapping
                    {
                        TriggerAddress = "MW10384",
                        ExpectedValue = "1",
                        ResultAddress = "MB10206",
                        ScanFlag = "input"
                    },
                    ["Output"] = new ScannerTriggerMapping
                    {
                        TriggerAddress = "MW10365",
                        ExpectedValue = "1",
                        ResultAddress = "MB10207",
                        ScanFlag = "output"
                    }
                }
            };
            SaveConfig(config);
            return config;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeS.Models
{
    public class PlcMesConfig
    {
        // PLC 配置
        public string PlcIp { get; set; } = "192.168.0.1";
        public int PlcPort { get; set; } = 502;

        // MES 配置
        public string MachineCode { get; set; } = "HPM001";
        public string StationCode { get; set; } = "hank001_2_M101";
        public string SectionCode { get; set; } = "1230121167_1_M5";
        public int DataRetentionDays { get; set; } = 7;

        // 触发地址映射（根据实际 PLC 点位配置）
        public Dictionary<string, TriggerMapping> TriggerMappings { get; set; } = new();
        public Dictionary<string, ScannerTriggerMapping> ScannerTriggerMappings { get; set; } = new();
    }
}

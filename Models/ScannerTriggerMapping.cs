namespace AppMeS.Models
{
    public class ScannerTriggerMapping
    {
        public string TriggerAddress { get; set; }   // 触发地址 "MW10384"
        public string ExpectedValue { get; set; }    // "1"
        public string ResultAddress { get; set; }    // 结果存放地址 "MB10206"
        public string ScanFlag { get; set; }         // "input" 或 "output"
    }
}
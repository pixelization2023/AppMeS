namespace AppMeS.Models
{
    public class TriggerMapping
    {
        public string Address { get; set; }      // 触发地址，如 "MW10196"
        public string ExpectedValue { get; set; } // 期望值 "1"
        public string CameraName { get; set; }    // 相机名称
        public string ProcedureOrder { get; set; }// 流程顺序
        public string FeedbackAddress { get; set; }// 反馈地址
    }
}
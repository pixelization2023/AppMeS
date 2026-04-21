
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WLGMes
{
    public class WLGTrackOut : WLGMesBase
    {
        public TrackOutPut TrackOutPut { get; set; } = new TrackOutPut
        {
            carrier = "20240906B102300000000000",
            machineCode = "HPM001",
            materialCode = "1230055567",
            orderItemMachineNo = "HPM0012026012001",
            projectCode = "hank-2",
            quantity = 42,
            badQuantity = 0,
            scanFlag = "input",
            scanInfo = "carrier",
            sectionCode = "1230121167_1_M5",
            stationCode = "hank001_2_M101",
            createBy = "v-dengshike"
        };

        public ApiResponse<TrackOutResultDto> TrackOutResult { get; set; } = new ApiResponse<TrackOutResultDto>();
        public WLGTrackOut()
        {
            Url = "https://wlgmes-uat.aacoptics.com/sky-boot/prism/processwip/scanposting";
        }
        public ApiResponse<TrackOutResultDto>TrackOut(TrackOutPut trackOutPut, out string responseJson)
        {
            try
            {
                string json = JsonConvert.SerializeObject(trackOutPut);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
              
                var response = WebClient.PutAsync(Url, content).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var result = JsonConvert.DeserializeObject<ApiResponse<TrackOutResultDto>>(responseJson);

                TrackOutResult = result;

                return result;
            }
            catch (Exception ex)
            {
                // 统一异常处理
                responseJson = ex.Message;
                throw;
            }
        }
    }
    /// <summary>
    /// 自动过站时提交给mes的信息
    /// </summary>
    public class TrackOutPut
    {
        /// <summary>
        /// 载具号
        /// </summary>
        public string carrier { get; set; }

        /// <summary>
        /// 流程卡 / 工艺卡
        /// </summary>
        public string processCard { get; set; }

        /// <summary>
        /// 单个 SN
        /// </summary>
        public string sn { get; set; }

        /// <summary>
        /// SN 列表
        /// </summary>
        public List<string> snList { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string machineCode { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string materialCode { get; set; }

        /// <summary>
        /// 工单-设备号
        /// </summary>
        public string orderItemMachineNo { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }

        /// <summary>
        /// 产出数量
        /// </summary>
        public int? quantity { get; set; }

        /// <summary>
        /// 不良数量
        /// </summary>
        public int? badQuantity { get; set; }

        /// <summary>
        /// 扫描标识（input / output）
        /// </summary>
        public string scanFlag { get; set; }

        /// <summary>
        /// 扫描信息类型（carrier / sn 等）
        /// </summary>
        public string scanInfo { get; set; }

        /// <summary>
        /// 工段编码
        /// </summary>
        public string sectionCode { get; set; }

        /// <summary>
        /// 工站编码
        /// </summary>
        public string stationCode { get; set; }

        /// <summary>
        /// 前模具编码
        /// </summary>
        public string preMoldCode { get; set; }

        /// <summary>
        /// 前制程节拍
        /// </summary>
        public int? preProductionCycle { get; set; }

        /// <summary>
        /// 前版本号
        /// </summary>
        public string preVersion { get; set; }

        /// <summary>
        /// 前模穴号列表
        /// </summary>
        public List<string> preMoldNumList { get; set; }

        /// <summary>
        /// 后模具编码
        /// </summary>
        public string finMoldCode { get; set; }

        /// <summary>
        /// 后制程节拍
        /// </summary>
        public int? finProductionCycle { get; set; }

        /// <summary>
        /// 后版本号
        /// </summary>
        public string finVersion { get; set; }

        /// <summary>
        /// 后模穴号列表
        /// </summary>
        public List<string> finMoldNumList { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string createBy { get; set; }
    }
    /// <summary>
    /// 自动过站结果信息
    /// </summary>
    public class TrackOutResultDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 机台工单号
        /// </summary>
        public string OrderItemMachineNo { get; set; }

        /// <summary>
        /// 工段编码
        /// </summary>
        public string SectionCode { get; set; }

        /// <summary>
        /// 来源工序
        /// </summary>
        public string SourceProcess { get; set; }

        /// <summary>
        /// 当前工序
        /// </summary>
        public string CurrentProcess { get; set; }

        /// <summary>
        /// 下一工序
        /// </summary>
        public string NextProcess { get; set; }

        /// <summary>
        /// 是否进站（0/1）
        /// </summary>
        public int? TrackIn { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// SN号
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 载具
        /// </summary>
        public string Carrier { get; set; }

        public string ProcessCard { get; set; }
        public string LocationCode { get; set; }

        /// <summary>
        /// 来源机台号
        /// </summary>
        public string SourceMachineNo { get; set; }

        /// <summary>
        /// 是否返工（0/1）
        /// </summary>
        public int? IsRework { get; set; }

        public string ReworkProcess { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }

        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public string StationCode { get; set; }
        public string Operator { get; set; }
        public string CarrierNo { get; set; }

        public int? ScanFlag { get; set; }
        public string ScanInfo { get; set; }

        public int? BadQuantity { get; set; }
        public string ProjectCode { get; set; }

        public string Label { get; set; }
        public int? LabelNum { get; set; }

        public string MachineCode { get; set; }
        public string InputBatchNo { get; set; }
        public string OutputBatchNo { get; set; }

        public string PreMoldCode { get; set; }
        public string PreProductionCycle { get; set; }
        public string PreVersion { get; set; }
        public string PreMoldNum { get; set; }

        public string FinMoldCode { get; set; }
        public string FinProductionCycle { get; set; }
        public string FinVersion { get; set; }
        public string FinMoldNum { get; set; }

        public List<string> SnList { get; set; }
        public List<string> PreMoldNumList { get; set; }
        public List<string> FinMoldNumList { get; set; }
    }
}

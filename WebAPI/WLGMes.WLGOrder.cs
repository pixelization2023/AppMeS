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
    public class WLGOrder : WLGMesBase
    {
        public OrderGet OrderGet { get; set; } = new OrderGet { machinecode = "JYZ03", stationCode = "hank001_2_M102" };

        public ApiResponse<List<OrderResultDto>> OrderResult { get; set; } = new ApiResponse<List<OrderResultDto>>();

        public OrderResultDto SelectOrder { get; set; } = new OrderResultDto();
        public WLGOrder()
        {
            Url = "https://wlgmes-uat.aacoptics.com/sky-boot/prism/processwip/machine/list";
        }
        public ApiResponse<List<OrderResultDto>> Order(OrderGet OrderGet, out string responseJson)
        {
            try
            {
                string url = Url + "?machineCode=" + OrderGet.machinecode + "&stationCode=" + OrderGet.stationCode;

                var response = WebClient.GetAsync(url).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var result = JsonConvert.DeserializeObject<ApiResponse<List<OrderResultDto>>>(responseJson);

                return result;
            }
            catch (Exception ex)
            {
                responseJson = ex.Message;
                throw;
            }
        }
    }

    public class OrderGet
    {
        public string machinecode { get; set; }
        public string stationCode { get; set; }
    }

    /// <summary>
    /// 机台工单信息返回对象
    /// </summary>
    public class OrderResultDto
    {
        /// <summary>
        /// 机台工单
        /// </summary>
        public string orderItemMachineNo { get; set; }

        /// <summary>
        /// 机台编号
        /// </summary>
        public string machineCode { get; set; }

        /// <summary>
        /// 机台名称
        /// </summary>
        public string machineName { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string materialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string materialName { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string projectCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 计划投入数量
        /// </summary>
        public int planInputQty { get; set; }

        /// <summary>
        /// 计划产出数量
        /// </summary>
        public int planOutputQty { get; set; }

        /// <summary>
        /// 实际投入数量
        /// </summary>
        public int actualInputQty { get; set; }

        /// <summary>
        /// 实际产出数量
        /// </summary>
        public int actualOutputQty { get; set; }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? planStartTime { get; set; }

        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? planEndTime { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime? actualStartTime { get; set; }

        /// <summary>
        /// 实际结束时间
        /// </summary>
        public DateTime? actualEndTime { get; set; }
    }

}

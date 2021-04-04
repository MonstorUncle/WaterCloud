using Chloe;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using WaterCloud.Code;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.AutoJob
{
	public class SaveServerStateJob : IJobTask
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ServerStateService _server;
        public SaveServerStateJob(IDbContext context)
        {
            _hostingEnvironment = GlobalContext.HostingEnvironment;
            _server = new ServerStateService(context);
         }
        public async Task<AlwaysResult> Start()
        {
            AlwaysResult obj = new AlwaysResult();
            try
            {
                ServerStateEntity entity = new ServerStateEntity();
                var computer = ComputerHelper.GetComputerInfo();
                entity.F_ARM = computer.RAMRate;
                entity.F_CPU = computer.CPURate;
                entity.F_IIS = "0";
                entity.F_WebSite = _hostingEnvironment.ContentRootPath;
                await _server.SubmitForm(entity);
                obj.state = ResultType.success.ToString();
                obj.message = "服务器状态更新成功！";
            }
            catch (Exception ex)
            {
                obj.state = ResultType.error.ToString();
                obj.message = "服务器状态更新失败！"+ex.Message;
            }
            return obj;
        }
    }
}

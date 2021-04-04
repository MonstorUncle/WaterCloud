using System.Threading.Tasks;
using WaterCloud.Code;

namespace WaterCloud.Service.AutoJob
{
    public interface IJobTask
    {
        //执行方法
        Task<AlwaysResult> Start();
    }
}

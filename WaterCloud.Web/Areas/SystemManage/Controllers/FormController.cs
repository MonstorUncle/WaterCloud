using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Code;
using WaterCloud.Domain.SystemManage;
using WaterCloud.Service;
using WaterCloud.Service.SystemManage;
using System.IO;
using WaterCloud.Code.Model;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-08 14:33
    /// 描 述：表单设计控制器类
    /// </summary>
    [Area("SystemManage")]
    public class FormController :  ControllerBase
    {

        public FormService _service {get;set;}

        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetGridJson(string ItemId, string keyword)
        {
            var data = await _service.GetLookList(ItemId, keyword);
            return Success(data.Count, data);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetListJson(string keyword)
        {
            var data = await _service.GetList(keyword);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetExtendForm(string keyword)
        {
            string filePath = GlobalContext.HostingEnvironment.WebRootPath + $@"/form/";
            DirectoryInfo root = new DirectoryInfo(filePath);
            List<AppLogEntity> list = new List<AppLogEntity>();
            foreach (FileInfo f in root.GetFiles())
            {
                AppLogEntity app = new AppLogEntity();
                app.FileName = f.Name.Remove(f.Name.Length - 5, 5); ;
                list.Add(app);
            }
            return Content(list.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetFormJson(string keyValue)
        {
            var data = await _service.GetLookForm(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> SubmitForm(FormEntity entity, string keyValue)
        {
            try
            {
                await _service.SubmitForm(entity, keyValue);
                return await Success("操作成功。", "", keyValue);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ServiceFilter(typeof(HandlerAuthorizeAttribute))]
        public async Task<ActionResult> DeleteForm(string keyValue)
        {
            try
            {
                await _service.DeleteForm(keyValue);
                return await Success("操作成功。", "", keyValue, DbLogType.Delete);
            }
            catch (Exception ex)
            {
                return await Error(ex.Message, "", keyValue, DbLogType.Delete);
            }
        }
        #endregion
    }
}

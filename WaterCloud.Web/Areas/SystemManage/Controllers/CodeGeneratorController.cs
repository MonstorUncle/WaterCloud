using System.Collections.Generic;
using System.Web;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using WaterCloud.Domain;
using WaterCloud.Code;
using WaterCloud.CodeGenerator;
using WaterCloud.Service.CommonService;
using System.Linq;
using WaterCloud.Service;
using System.Threading.Tasks;
using WaterCloud.Code.Extend;
using Chloe;

namespace WaterCloud.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class CodeGeneratorController : ControllerBase
    {

        private readonly IDatabaseTableService _service;
        private readonly IDbContext _context;
        public CodeGeneratorController(IDbContext context)
        {
            string dbType = GlobalContext.SystemConfig.DBProvider;
            _context = context;
            switch (dbType)
            {
                case Define.DBTYPE_SQLSERVER:
                    _service = new DatabaseTableSqlServerService(context);
                    break;
                case Define.DBTYPE_MYSQL:
                    _service = new DatabaseTableMySqlService(context);
                    break;
                case Define.DBTYPE_ORACLE:
                    _service = new DatabaseTableOracleService(context);
                    break;
                default:
                    _service = new DatabaseTableMySqlService(context);
                    break;
            }
        }
        #region 视图功能

        #endregion

        #region 获取数据
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTablePageListJson(Pagination pagination, string keyword)
        {
            //导出全部页使用
            if (pagination.rows == 0 && pagination.page == 0)
            {
                pagination.rows = 99999999;
                pagination.page = 1;
            }
            List<TableInfo> data = await _service.GetTablePageList(keyword, pagination);
            return Success(pagination.records, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTableJson(string keyword)
        {
            List<TableInfo> data = await _service.GetTableList(keyword);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTableSelectJson(string keyword)
        {
            List<TableInfo> data = await _service.GetTableList(keyword);
            List<object> list = new List<object>();
            foreach (var item in data)
            {
                list.Add(new { name = item.TableName, value = item.TableName });
            }
            return Content(list.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTableFieldJson(string keyValue, string keyword)
        {
            List<TableFieldInfo> data = await _service.GetTableFieldList(keyValue);
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(a => a.TableColumn.Contains(keyword)).ToList();
            }
            return Success(data.Count, data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetTableFieldSelectJson(string keyValue)
        {
            List<TableFieldInfo> data = await _service.GetTableFieldList(keyValue);
            List<object> list = new List<object>();
            foreach (var item in data)
            {
                list.Add(new { text = item.TableColumn, id = item.TableColumn });
            }
            return Content(list.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public async Task<ActionResult> GetBaseConfigJson(string keyValue)
        {
            BaseConfigModel data = new BaseConfigModel();

            string tableDescription = string.Empty;
            List<TableFieldInfo> tDataTableField = await _service.GetTableFieldList(keyValue);

            var columnList = tDataTableField.Where(p => !BaseField.BaseFieldList.Contains(p.TableColumn) && p.TableIdentity != "Y").Select(p => new { p.TableColumn, p.Remark }).ToList();
            List<ColumnField> dic = new List<ColumnField>();
            foreach (var item in columnList)
            {
                ColumnField field = new ColumnField();
                field.field = item.TableColumn;
                field.title = string.IsNullOrEmpty(item.Remark) ? item.TableColumn : item.Remark;
                field.isFilter = true;
                field.isAotuWidth = false;
                field.isSorted = true;
                field.isShow = true;
                field.width = 100;
                dic.Add(field);
            }
            string serverPath = GlobalContext.HostingEnvironment.ContentRootPath;
            data = new SingleTableTemplate(_context).GetBaseConfig(serverPath, _logService.currentuser.UserName, keyValue, tableDescription, dic);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [HandlerAjaxOnly]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CodePreviewJson(BaseConfigModel baseConfig)
        {
            try
            {
                List<TableFieldInfo> list = await _service.GetTableFieldList(baseConfig.TableName);
                SingleTableTemplate template = new SingleTableTemplate(_context);
                DataTable dt = DataTableHelper.ListToDataTable(list);  // 用DataTable类型，避免依赖
                string idcolumn = string.Empty;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                baseConfig.PageIndex.ButtonList = ExtList.removeNull(baseConfig.PageIndex.ButtonList);
                baseConfig.PageIndex.ColumnList = baseConfig.PageIndex.ColumnList.Where(a => a.field != "").ToList();
                baseConfig.PageForm.FieldList.Remove("");
                string idType = "string";
                foreach (DataRow dr in dt.Rows)
                {

                    if (dr["TableIdentity"].ToString() == "Y")
                    {
                        idcolumn = dr["TableColumn"].ToString();
                        string datatype = dr["Datatype"].ToString();
                        datatype = TableMappingHelper.GetPropertyDatatype(datatype);
                        if (datatype == "int?")
                        {
                            idType = "int";
                        }
                        else if (datatype == "long?")
                        {
                            idType = "long";
                        }
                        else
                        {
                            idType = "string";
                        }
                    }
                    string columnName = dr["TableColumn"].ToString();
                }


                string codeEntity = template.BuildEntity(baseConfig, dt, idcolumn);
                string codeService = template.BuildService(baseConfig, dt, idcolumn, idType);
                string codeController = template.BuildController(baseConfig, idcolumn, idType);
                string codeIndex = template.BuildIndex(baseConfig, idcolumn);
                string codeForm = template.BuildForm(baseConfig);
                string codeDetails = template.BuildDetails(baseConfig);
                string codeMenu = template.BuildMenu(baseConfig, idcolumn);
                var json = new
                {
                    CodeEntity = HttpUtility.HtmlEncode(codeEntity),
                    CodeService = HttpUtility.HtmlEncode(codeService),
                    CodeController = HttpUtility.HtmlEncode(codeController),
                    CodeIndex = HttpUtility.HtmlEncode(codeIndex),
                    CodeForm = HttpUtility.HtmlEncode(codeForm),
                    CodeDetails = HttpUtility.HtmlEncode(codeDetails),
                    CodeMenu = HttpUtility.HtmlEncode(codeMenu)
                };
                return Success("操作成功", json);
            }
            catch (System.Exception ex)
            {
                return Error(ex.Message);
            }

        }
        [HttpGet]
        public virtual ActionResult AddForm()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult RuleForm()
        {
            return View();
        }
        [HttpGet]
        public virtual ActionResult EntityCode()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> CodeGenerateJson(BaseConfigModel baseConfig, string Code)
        {
            try
            {
                if (!GlobalContext.SystemConfig.Debug)
                {
                    throw new System.Exception("请在本地开发模式时使用代码生成");
                }
                else
                {
                    SingleTableTemplate template = new SingleTableTemplate(_context);
                    await template.CreateCode(baseConfig, HttpUtility.UrlDecode(Code));
                }
                return await Success("操作成功。", "", "");
            }
            catch (System.Exception ex)
            {
                return await Error(ex.Message, "", "");
            }
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public async Task<ActionResult> EntityCodeGenerateJson(BaseConfigModel baseConfig, string keyValue)
        {
            try
            {
                if (!GlobalContext.SystemConfig.Debug)
                {
                    throw new System.Exception("请在本地开发模式时使用代码生成");
                }
                else
                {
                    List<TableFieldInfo> list = await _service.GetTableFieldList(baseConfig.TableName);
                    SingleTableTemplate template = new SingleTableTemplate(_context);
                    DataTable dt = DataTableHelper.ListToDataTable(list);  // 用DataTable类型，避免依赖
                    string idcolumn = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["TableIdentity"].ToString() == "Y")
                        {
                            idcolumn = dr["TableColumn"].ToString();
                        }
                    }
                    string codeEntity = template.BuildEntity(baseConfig, dt, idcolumn);
                    await template.EntityCreateCode(baseConfig, codeEntity);
                }
                return await Success("操作成功。", "", "");
            }
            catch (System.Exception ex)
            {
                return await Error(ex.Message, "", "");
            }
        }
        #endregion
    }
}
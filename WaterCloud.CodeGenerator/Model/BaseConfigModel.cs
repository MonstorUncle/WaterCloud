using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCloud.CodeGenerator
{
    public class BaseConfigModel
    {
        /// <summary>
        /// 数据库表名sys_menu
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表名首字母大写Sys_Menu
        /// </summary>
        public string TableNameUpper { get; set; }
        public FileConfigModel FileConfig { get; set; }
        public OutputConfigModel OutputConfig { get; set; }
        public PageIndexModel PageIndex { get; set; }
        public PageFormModel PageForm { get; set; }
    }

    public class FileConfigModel
    {
        public string ClassPrefix { get; set; }
        public string ClassDescription { get; set; }
        public string CreateUserName { get; set; }
        public string CreateDate { get; set; }
        public string EntityName { get; set; }
        public string ServiceName { get; set; }
        public string ControllerName { get; set; }
        public string PageIndexName { get; set; }
        public string PageFormName { get; set; }
        public string PageDetailsName { get; set; }
    }
    public class OutputConfigModel
    {
        public string OutputModule { get; set; }
        public string OutputEntity { get; set; }
        public string OutputService { get; set; }
        public string OutputWeb { get; set; }
    }
    public class PageIndexModel
    {
        /// <summary>
        /// 是否菜单显示
        /// </summary>
        public bool IsMunu { get; set; }
        /// <summary>
        /// 是否需要搜索框
        /// </summary>
        public bool IsSearch { get; set; }
        /// <summary>
        /// 是否树形表格
        /// </summary>
        public bool IsTree { get; set; }
        /// <summary>
        /// 是否字段控制
        /// </summary>
        public bool IsFields { get; set; }
        /// <summary>
        /// 是否公共
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 工具栏按钮（新增 修改 删除 查看）
        /// </summary>
        public List<string> ButtonList { get; set; }

        /// <summary>
        /// 是否有分页
        /// </summary>
        public bool IsPagination { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 父级字段
        /// </summary>
        public string ParentColum { get; set; }
        /// <summary>
        /// 树形显示字段
        /// </summary>
        public string TreeColum { get; set; }
        /// <summary>
        /// 模糊查询字段
        /// </summary>
        public List<string> KeywordColum { get; set; }
        /// <summary>
        /// 删除字段字段
        /// </summary>
        public string DeleteColum { get; set; }
        /// <summary>
        /// 创建时间字段
        /// </summary>
        public string CreateColum { get; set; }
        public bool? IsAsc { get; set; }
        public List<ColumnField> ColumnList { get; set; }

    }
    public class PageFormModel
    {
        /// <summary>
        /// 1表示显示成1列，2表示显示成2列
        /// </summary>
        public int ShowMode { get; set; }
        public Dictionary<string, string> FieldList { get; set; }
    }
    public class ColumnField
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int? width { get; set; }
        /// <summary>
        /// 是否minWidth
        /// </summary>
        public bool? isAotuWidth { get; set; }
        /// <summary>
        /// 是否排序
        /// </summary>
        public bool? isSorted { get; set; }
        /// <summary>
        /// 是否过滤
        /// </summary>
        public bool? isFilter { get; set; }
        /// <summary>
        /// 过滤类型
        /// </summary>
        public string filterType { get; set; }
        /// <summary>
        /// 格式化显示
        /// </summary>
        public string templet { get; set; }
        public bool? isShow { get; set; }
        /// <summary>
        /// 初始值
        /// </summary>
        public string value { get; set; }
    }

}

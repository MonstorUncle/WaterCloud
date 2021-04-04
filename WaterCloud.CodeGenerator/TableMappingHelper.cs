using System.Text;
using WaterCloud.Code;

namespace WaterCloud.CodeGenerator
{
    public class TableMappingHelper
    {
        /// <summary>
        /// sys_role转成SysRole
        /// </summary>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ConvertToUppercase(string lowercase)
        {
            lowercase = lowercase.ParseToString();
            StringBuilder sb = new StringBuilder();
            string[] arr = lowercase.Split('_');
            for (int i = 0; i < arr.Length; i++)
            {
                sb.Append(arr[i][0].ToString().ToUpper() + arr[i].Substring(1));
            }
            return sb.ToString();
        }
        /// <summary>
        /// sys_role转成Sys_Role
        /// </summary>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ConvertTo_Uppercase(string lowercase)
        {
            lowercase = lowercase.ParseToString();
            StringBuilder sb = new StringBuilder();
            string[] arr = lowercase.Split('_');
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i][0].ToString().ToUpper() + arr[i].Substring(1);
            }
            return string.Join("_", arr);
        }
        /// <summary>
        /// UserService转成userService
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstLetterLowercase(string instanceName)
        {
            instanceName = instanceName.ParseToString();
            if (!instanceName.IsEmpty())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(instanceName[0].ToString().ToLower() + instanceName.Substring(1));
                return sb.ToString();
            }
            else
            {
                return instanceName;
            }
        }

        /// <summary>
        /// sys_menu_authorize变成MenuAuthorize
        /// </summary>
        public static string GetClassNamePrefix(string tableName)
        {
            string[] arr = tableName.Split('_');
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < arr.Length; i++)
            {
                sb.Append(arr[i][0].ToString().ToUpper() + arr[i].Substring(1));
            }
            return sb.ToString();
        }

        public static string GetPropertyDatatype(string sDatatype)
        {
            string sTempDatatype = string.Empty;
            sDatatype = sDatatype.ToLower();
            if (sDatatype.Contains("numer("))
            {
                sDatatype = "numer(8,2)";
            }
            switch (sDatatype)
            {
                case "int":
                case "number":
                case "integer":
                case "smallint":
                    sTempDatatype = "int?";
                    break;

                case "bigint":
                    sTempDatatype = "long?";
                    break;

                case "tinyint":
                    sTempDatatype = "bool?";
                    break;

                case "numeric":
                case "real":
                    sTempDatatype = "Single?";
                    break;

                case "float":
                    sTempDatatype = "float?";
                    break;

                case "decimal":
                case "numer(8,2)":
                    sTempDatatype = "decimal?";
                    break;

                case "bit":
                    sTempDatatype = "bool?";
                    break;

                case "datetime":
                case "datetime2":
                case "date":
                case "smalldatetime":
                case "timestamp":
                    sTempDatatype = "DateTime?";
                    break;

                case "money":
                case "smallmoney":
                    sTempDatatype = "double?";
                    break;

                case "char":
                case "varchar":
                case "nvarchar2":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                default:
                    sTempDatatype = "string";
                    break;
            }
            return sTempDatatype;
        }
    }
}

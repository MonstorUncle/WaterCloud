using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WaterCloud.Code
{
    /// <summary>
    /// 流程连线
    /// </summary>
    public class FlowLine
    {
        public string id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string name { get; set; }
        public bool dash { get; set; }
        public double M { get; set; }
        public bool alt { get; set; }

        /// <summary> 分支条件 </summary>
        public List<DataCompare> Compares { get; set; }

        public bool Compare(JObject frmDataJson)
        {
            bool result = true;
            foreach (var compare in Compares)
            {
                compare.FieldName = compare.FieldName.ToLower();
                compare.Value = compare.Value.ToLower();
                decimal value=0;  //参考值
                decimal frmvalue=0; //表单中填写的值
                if (compare.Operation!= DataCompare.Equal&& compare.Operation != DataCompare.NotEqual)
                {
                    value = decimal.Parse(compare.Value);
                    frmvalue = decimal.Parse(frmDataJson.GetValue(compare.FieldName.ToLower()).ToString()); //表单中填写的值
                }
                bool res = false;
				if (compare.Condition=="and")
				{
                    switch (compare.Operation)
                    {
                        case DataCompare.Equal:
                            result &= compare.Value == frmDataJson.GetValue(compare.FieldName).ToString();
                            break;
                        case DataCompare.NotEqual:
                            result &= compare.Value != frmDataJson.GetValue(compare.FieldName).ToString();
                            break;
                        case DataCompare.Larger:
                            result &= frmvalue > value;
                            break;
                        case DataCompare.Less:
                            result &= frmvalue < value;
                            break;
                        case DataCompare.LargerEqual:
                            result &= frmvalue <= value;
                            break;
                        case DataCompare.LessEqual:
                            result &= frmvalue <= value;
                            break;
                        case DataCompare.In:
                            if (compare.FieldName == "申请人" || compare.FieldName == "所属部门")
                            {
                                var arr = compare.Value.Split(',');
                                foreach (var item in frmDataJson.GetValue(compare.FieldName).ToString().Split(','))
                                {
                                    if (arr.Contains(item))
                                    {
                                        res = true;
                                        break;
                                    }
                                }
                                result &= res;
                                break;
                            }
                            else
                            {
                                var arr = compare.Value.Split(',');
                                if (arr.Contains(frmvalue.ToString()))
                                {
                                    res = true;
                                    break;
                                }
                                result &= res;
                                break;
                            }
                            break;
                        case DataCompare.NotIn:
                            if (compare.FieldName == "申请人" || compare.FieldName == "所属部门")
                            {
                                var arr = compare.Value.Split(',');
                                foreach (var item in frmDataJson.GetValue(compare.FieldName).ToString().Split(','))
                                {
                                    if (arr.Contains(item))
                                    {
                                        res = false;
                                        break;
                                    }
                                }
                                result &= res;
                                break;
                            }
                            else
                            {
                                var arr = compare.Value.Split(',');
                                if (arr.Contains(frmvalue.ToString()))
                                {
                                    res = false;
                                    break;
                                }
                                result &= res;
                                break;
                            }
                            break;
                    }
                }
				else
				{
                    switch (compare.Operation)
                    {
                        case DataCompare.Equal:
                            if (compare.FieldName == "申请人" || compare.FieldName == "所属部门")
                            {
                                var arr = compare.Value.Split(',');
                                foreach (var item in frmDataJson.GetValue(compare.FieldName).ToString().Split(','))
                                {
                                    if (arr.Contains(item))
                                    {
                                        res = true;
                                        break;
                                    }
                                }
                                result |=  res;
                                break;
                            }
                            result |= compare.Value == frmDataJson.GetValue(compare.FieldName).ToString();
                            break;
                        case DataCompare.NotEqual:
                            if (compare.FieldName == "申请人" || compare.FieldName == "所属部门")
                            {
                                var arr = compare.Value.Split(',');
                                foreach (var item in frmDataJson.GetValue(compare.FieldName).ToString().Split(','))
                                {
                                    if (arr.Contains(item))
                                    {
                                        res = false;
                                        break;
                                    }
                                }
                                result |= res;
                                break;
                            }
                            result |= compare.Value != frmDataJson.GetValue(compare.FieldName).ToString();
                            break;
                        case DataCompare.Larger:
                            result |= frmvalue > value;
                            break;
                        case DataCompare.Less:
                            result |= frmvalue < value;
                            break;
                        case DataCompare.LargerEqual:
                            result |= frmvalue <= value;
                            break;
                        case DataCompare.LessEqual:
                            result |= frmvalue <= value;
                            break;
                        case DataCompare.In:
                            if (compare.FieldName == "申请人" || compare.FieldName == "所属部门")
                            {
                                var arr = compare.Value.Split(',');
                                foreach (var item in frmDataJson.GetValue(compare.FieldName).ToString().Split(','))
                                {
                                    if (arr.Contains(item))
                                    {
                                        res = true;
                                        break;
                                    }
                                }
                                result |= res;
                                break;
                            }
                            else
                            {
                                var arr = compare.Value.Split(',');
                                if (arr.Contains(frmvalue.ToString()))
                                {
                                    res = true;
                                }
                                result |= res;
                                break;
                            }
                        case DataCompare.NotIn:
                            if (compare.FieldName == "申请人" || compare.FieldName == "所属部门")
                            {
                                var arr = compare.Value.Split(',');
                                foreach (var item in frmDataJson.GetValue(compare.FieldName).ToString().Split(','))
                                {
                                    if (arr.Contains(item))
                                    {
                                        res = false;
                                        break;
                                    }
                                }
                                result |= res;
                                break;
                            }
                            else
                            {
                                var arr = compare.Value.Split(',');
                                if (arr.Contains(frmvalue.ToString()))
                                {
                                    res = false;
                                }
                                result |= res;
                                break;
                            }
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    ///  分支条件
    /// </summary>
    public class DataCompare
    {
        public const string Larger = ">";
        public const string Less = "<";
        public const string LargerEqual = ">=";
        public const string LessEqual = "<=";
        public const string NotEqual = "!=";
        public const string Equal = "=";
        public const string In = "in";
        public const string NotIn = "not in";

        /// <summary>操作类型比如大于/等于/小于</summary>
        public string Operation { get; set; }

        /// <summary> form种的字段名称 </summary>
        public string FieldName { get; set; }

        /// <summary> 字段类型："form"：为表单中的字段，后期扩展系统表等. </summary>
        public string FieldType { get; set; }

        /// <summary>实际的值</summary>
        public string Value { get; set; }
        /// <summary>
        /// 显示值
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 条件关系
        /// </summary>
        public string Condition { get; set; }
    }
}

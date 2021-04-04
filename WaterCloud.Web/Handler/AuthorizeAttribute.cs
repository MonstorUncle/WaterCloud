using System.ComponentModel.DataAnnotations;

namespace WaterCloud.Web
{
	public class AuthorizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string _authorize { get; set; }
        public AuthorizeAttribute(string authorize)
        {
            _authorize = authorize.ToLower();
        }
    }
}
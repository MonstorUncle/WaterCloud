using System.ComponentModel.DataAnnotations;

namespace WaterCloud.WebApi
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
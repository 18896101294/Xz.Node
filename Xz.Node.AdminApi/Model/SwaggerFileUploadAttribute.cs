using System;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// 标上此参数swagger将会显示上传文件功能
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]//此特性可以在方法上和方法参数上使用
    public class SwaggerFileUploadAttribute : Attribute
    {
        /// <summary>
        /// 是否必须
        /// </summary>
        public bool Required { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Required"></param>
        public SwaggerFileUploadAttribute(bool Required = true)
        {
            this.Required = Required;
        }
    }
}

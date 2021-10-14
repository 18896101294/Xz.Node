using System;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// 隐藏接口，不生成到swagger文档展示
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)] //此特性可以在方法上和类上使用
    public partial class HiddenAttribute : Attribute
    {
    }
}

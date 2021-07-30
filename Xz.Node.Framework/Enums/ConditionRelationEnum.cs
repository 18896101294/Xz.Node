using System.ComponentModel;

namespace Xz.Node.Framework.Enums
{
    /// <summary>
    /// 查询关系
    /// </summary>
    public enum ConditionRelationEnum
    {
        /// <summary>
        /// 和
        /// </summary>
        [Description("与")]
        And = 1,
        /// <summary>
        /// 或
        /// </summary>
        [Description("或")]
        Or = 2
    }
}

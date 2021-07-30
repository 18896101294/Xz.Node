using Xz.Node.Framework.Enums;

namespace Xz.Node.Framework.Model
{
    /// <summary>
    /// 排序
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SortInfo()
        {
            this.Direction = ConditionDirectionEnum.ASC;
        }
        /// <summary>
        /// 表的字段名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public ConditionDirectionEnum Direction { get; set; }
        /// <summary>
        /// 表别名
        /// </summary>
        public string Alias { get; set; }
    }
}

using System.Collections.Generic;

namespace Xz.Node.Framework.Model
{
    /// <summary>
    /// 树节点结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IEnumerable<TreeItem<T>> Children { get; set; }
    }
}

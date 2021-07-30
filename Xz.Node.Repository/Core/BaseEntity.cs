using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.Repository.Core
{
    /// <summary>
    /// Entity基类
    /// </summary>
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            //if (KeyIsNull())
            //{
            //    GenerateDefaultKeyVal();
            //}
        }

        /// <summary>
        /// 判断主键是否为空，常用做判定操作是【添加】还是【编辑】
        /// </summary>
        /// <returns></returns>
        public abstract bool KeyIsNull();

        /// <summary>
        /// 创建默认的主键值
        /// </summary>
        public abstract void GenerateDefaultKeyVal();
    }
}

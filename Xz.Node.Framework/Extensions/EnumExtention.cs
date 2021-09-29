using System;
using System.ComponentModel;
using System.Linq;

namespace Xz.Node.Framework.Extensions
{
    /// <summary>
    /// 枚举类扩展
    /// </summary>
    public static class EnumExtention
    {
        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int GetValue(this Enum e)
        {
            return e.GetHashCode();
        }
        /// <summary>
        /// 获取枚举值的Description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : struct
        {
            string result = value.ToString();
            Type type = typeof(T);
            var info = type.GetField(value.ToString());
            var attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attributes != null && attributes.FirstOrDefault() != null)
            {
                result = (attributes.First() as DescriptionAttribute).Description;
            }
            return result;
        }
    }
}

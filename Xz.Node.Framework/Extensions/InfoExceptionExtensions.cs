using System;
using Xz.Node.Framework.Extensions.AutofacManager;

namespace Xz.Node.Framework.Extensions
{
    /// <summary>
    /// 自定义全局提示异常信息，InfoException的异常不会被记录为错误日志
    /// </summary>
    public class InfoException : System.Exception, IDisposable, IDependency
    {
        /// <summary>
        /// 容器管理
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public InfoException()
        {
        }

        /// <summary>
        /// 错误代码定义
        /// </summary>
        public int ErrorCode { get; set; } = -1;

        /// <summary>
        /// 自定义全局提示异常信息，InfoException的异常不会被记录为错误日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="errorCode"></param>
        public InfoException(string msg, int errorCode = -1) : base(msg)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///  初始化构造函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        /// <param name="errorCode"></param>
        public InfoException(string msg, Exception innerException, int errorCode = -1) : base(msg, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 释放函数
        /// </summary>
        public void Dispose()
        {

        }
    }
}

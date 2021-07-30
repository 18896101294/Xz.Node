namespace Xz.Node.Framework.Model
{
    /// <summary>
    /// 前后台交互结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultInfo<T>
    {
        public ResultInfo()
        {
            Code = 200;
            Message = "成功";
        }
        /// <summary>
        /// 错误编码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get => Code == 200; private set { } }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }
}

namespace Xz.Node.App.Base
{
    /// <summary>
    /// 分页入参
    /// </summary>
    public class PageReq
    {
        public PageReq()
        {
            page = 1;
            limit = 20;
        }
        /// <summary>
        /// 页码
        /// </summary>
        /// <example>1</example>
        public int page { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        /// <example>20</example>
        public int limit { get; set; }

        public string key { get; set; }
    }
}

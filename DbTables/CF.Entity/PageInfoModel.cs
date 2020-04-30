using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Entity
{
    /// <summary>
    /// 分页对象
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 分页条数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int? Total { get; set; }

    }
}

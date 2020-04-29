using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WZY.Dapper
{
    public class PagedCollection<T>
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
        public int TotalCount { get; set; }
        /// <summary>
        /// 当前页数据集合
        /// </summary>
        public List<T> DataList { get; set; }
    }
}

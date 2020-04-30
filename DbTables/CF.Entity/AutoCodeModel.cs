using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Entity
{
    public class AutoCodeModel
    {
        /// <summary>
        /// 自动编号对象
        /// </summary>
        public AutoCodeEntity AutoCodeEntity{get;set;}
        /// <summary>
        /// 自动编号计算对象
        /// </summary>
        public AutoCodeCounterEntity AutoCodeCounterEntity { get; set; }
        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// 编号计数位
        /// </summary>
        public string Counter { get; set; }


    }
}

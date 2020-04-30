using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Entity
{
    /// <summary>
    /// layui table 返回数据格式
    /// </summary>
    public class ResonseModel
    {
        public string code { get; set; }="0";
        public string msg { get; set; }
        public int  count { get; set; }
        public object data { get; set; }
    }
}

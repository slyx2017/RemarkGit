using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WZY.Dapper
{
    /// <summary>
    /// 排序对象
    /// </summary>
    public sealed class SortDirection
    {
        public SortDirection(string orderField, Direction direction)
        {
            OrderField = orderField;
            Direction = direction;
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderField { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public Direction Direction { get; set; }
    }
    /// <summary>
    /// 排序方向
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 正序
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// 反序
        /// </summary>
        Descending = 1
    }
}

namespace CF.Entity
{
    /// <summary>
    /// Sys_AutoCodeCounter，Sys_自动编码计数器：
    /// </summary>
    public class AutoCodeCounterEntity
    {
        ///<summary>
        ///CodeName，编码名称：默认为：表名 + _ + 字段名 构成
        ///</summary>
        public string CodeName { get; set; }

        ///<summary>
        ///CountKey，计数依据：
        ///</summary>
        public string CountKey { get; set; }

        ///<summary>
        ///CountValue，当前计数值：
        ///</summary>
        public int CountValue { get; set; }

    }
}

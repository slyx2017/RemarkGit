namespace CF.Entity
{
    /// <summary>
    /// Sys_AutoCode，Sys_自动编码：
    /// </summary>
    public class AutoCodeEntity
    {
        ///<summary>
        ///CodeName，编码名称：默认为：表名 + _ + 字段名 构成
        ///</summary>
        public string CodeName { get; set; }

        ///<summary>
        ///Remark，编码说明：
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///CodeLength，编码长度：最后生成的编码最大长度
        ///</summary>
        public int CodeLength { get; set; }

        ///<summary>
        ///CodeRule，编码规则：<YY>：当前年份（两位）;   <YYYY>：当前年份（四位）;   <MM>：当前月份;   <DD>：当前天数;   <X>：流水号;   <UID>：用户代码;   <DID>：部门代码;   <CID>：公司代码;   <BIZ>：业务类型代码;   除上述字符外的其它所有字符原样保留。
        ///</summary>
        public string CodeRule { get; set; }

        ///<summary>
        ///NumberLength，流水号长度：不足此位数的，左边用 0 补足
        ///</summary>
        public int NumberLength { get; set; }

        ///<summary>
        ///CountingCycle，计数周期：按年计数，每年第一天重新计数;   M：按月计数，每月第一天重新计数;   D：按天计数，每天重新计数;   N：从不重新计数
        ///</summary>
        public string CountingCycle { get; set; }

        ///<summary>
        ///CodePreview，编码预览：
        ///</summary>
        public string CodePreview { get; set; }

        ///<summary>
        ///IsSysInner，是否内部使用：0=系统内部使用，设置界面不能修改，1=设置界面可以修改规则
        ///</summary>
        public bool IsSysInner { get; set; }

    }
}

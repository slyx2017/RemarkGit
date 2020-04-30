using DapperExtensions.Mapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CF.Entity
{
    public class SysCodeEntityMapping : ClassMapper<SysCodeEntity>
    {
        public SysCodeEntityMapping()
        {
            Table("SysCode");
            Map(x => x.id).Key(KeyType.Identity);
            AutoMap();
        }
    }
    /// 字典表
    /// </summary>
    public class SysCodeEntity
    {
        ///<summary>
        ///主键
        ///</summary>
        public int id { get; set; }

        ///<summary>
        ///编码
        ///</summary>
        public string code { get; set; }

        ///<summary>
        ///名称
        ///</summary>
        public string name { get; set; }

        ///<summary>
        ///描述
        ///</summary>
        public string description { get; set; }

        ///<summary>
        ///是否可用
        ///</summary>
        public bool isActive { get; set; }

    }
}

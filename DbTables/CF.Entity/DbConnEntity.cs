using DapperExtensions.Mapper;

namespace CF.Entity
{
    public class DbConnEntityMapping : ClassMapper<DbConnEntity>
    {
        public DbConnEntityMapping()
        {
            // 自定义映射的表名
            Table("MDT_ServerClient");

            // 自定义映射表的主键
            Map(m => m.id).Key(KeyType.Identity);// 主键的类型
            
            // 启用自动映射，一定要调用此方法
            AutoMap();
        }
    }
    /// <summary>
    /// 数据库连接表
    /// </summary>
    public class DbConnEntity
    {
        ///<summary>
        ///主键
        ///</summary>
        public int id { get; set; }

        ///<summary>
        ///服务器编号
        ///</summary>
        public string serverNo { get; set; }

        ///<summary>
        ///服务器名称
        ///</summary>
        public string serverName { get; set; }

        ///<summary>
        ///数据库名称
        ///</summary>
        public string dataBaseName { get; set; }

        ///<summary>
        ///登陆名
        ///</summary>
        public string userId { get; set; }

        ///<summary>
        ///密码
        ///</summary>
        public string password { get; set; }

        ///<summary>
        ///描述
        ///</summary>
        public string description { get; set; }

        ///<summary>
        ///是否可用
        ///</summary>
        public bool isActive { get; set; }
        ///<summary>
        ///是否激活中
        ///</summary>
        public bool isEnable { get; set; }
    }
}

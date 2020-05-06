
using CF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZY.Dapper;

namespace CF.Biz
{
    /// <summary>
    /// 字典Biz类
    /// </summary>
    public class SysCodeBiz:BaseService<SysCodeEntity>
    {
        public SysCodeEntity GetCodeEntity(string keyValue)
        {
            SysCodeEntity entity= GetById(keyValue);
            return entity;
        }
    }
}

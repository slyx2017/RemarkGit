
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
    public class SysCodeBiz
    {
        public SysCodeEntity GetCodeEntity(string keyValue)
        {
            SysCodeEntity entity= DapperHelper.GetById<SysCodeEntity>(keyValue);
            return entity;
        }
    }
}

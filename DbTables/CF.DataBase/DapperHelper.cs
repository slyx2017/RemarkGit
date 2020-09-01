using CF.Entity.HelperModel;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.DataBase
{
    //数据库操作类
    public static class DapperHelper
    {
        /// <summary>
        /// 读取配置文件web.config数据库连接字符串 name="connStr"
        /// </summary>
        static string connectionString = ConfigurationManager.ConnectionStrings["connStr"].ToString();
        public static IDbConnection GetConnection(string connStr = "")
        {
            if (!string.IsNullOrEmpty(connStr))
            {
                connectionString = connStr;
            }
            IDbConnection connection = new SqlConnection(connectionString);
            return connection;
        }
        /// <summary>
        /// 业务数据库
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static IDbConnection GetConnectionBiz(string connStr)
        {
            IDbConnection connection = new SqlConnection(connStr);
            return connection;
        }
        private static string FilterSqlInjectChart(string sqlStr)
        {
            if (string.IsNullOrEmpty(sqlStr))
            {
                return string.Empty;
            }
            sqlStr = sqlStr.Replace(" ", string.Empty);
            sqlStr = sqlStr.Replace(";", string.Empty);
            sqlStr = sqlStr.Replace("'", string.Empty);
            return sqlStr;
        }

        /// <summary>
        /// 查询 全sql 异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task<List<T>> QueryAsync<T>(string sql,object param=null, IDbConnection connection = null)
        {
            var list = new List<T>();
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var collection = await conn.QueryAsync<T>(sql, param);
                list = collection.ToList();
            }
            return list;
        }
        /// <summary>
        /// 查询数据 带参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="obj"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static List<T> Query<T>(string sql, object obj=null, IDbConnection connection = null) where T : class
        {
            List<T> list = new List<T>();
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                list = conn.Query<T>(sql, obj).ToList();
            }
            return list;
        }
        /// <summary>
        /// 查询数据 全sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<T> Query<T>(string sql, IDbConnection connection = null) where T : class
        {
            List<T> list = new List<T>();
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                list = conn.Query<T>(sql).ToList();
            }
            return list;
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static PagedCollection<T> QueryPageList<T>(string sqlQuery,
            object parameters, List<SortDirection> orderBy, int pageIndex, int pageSize,
            int? totalCount = default(int?), IDbConnection connection = null) where T : class
        {
            List<T> item = null;
            string sqlTemplate = @" ORDER BY {0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY";
            StringBuilder sbSort = new StringBuilder();
            if (orderBy != null)
            {
                int i = 0;
                foreach (var sortDirection in orderBy)
                {
                    if (i != 0)
                    {
                        sbSort.Append(", ");
                    }
                    sbSort.Append(FilterSqlInjectChart(sortDirection.OrderField));
                    if (sortDirection.Direction == Direction.Descending)
                    {
                        sbSort.Append(" ").Append("DESC");
                    }
                    i++;
                }
            }
            string sql = sqlQuery + string.Format(sqlTemplate, sbSort.ToString(), (pageIndex - 1) * pageSize, pageSize);

            using (var conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                item = parameters != null ? conn.Query<T>(sql, parameters).ToList() : conn.Query<T>(sql).ToList();
                if (!totalCount.HasValue)
                {
                    sqlTemplate = @"SELECT COUNT(1) AS TotalCount FROM ({0}) AS TCount";
                    sql = string.Format(sqlTemplate, sqlQuery);
                    totalCount = parameters == null ? conn.QueryFirst<int>(sql) : conn.QueryFirst<int>(sql, parameters);
                }
                return new PagedCollection<T> { pageIndex = pageIndex, pageSize = pageSize, Total = totalCount.Value, DataList = item };
            }
        }
        /// <summary>
        /// 根据条件查询一个数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="obj"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static T QueryFirstOrDefault<T>(string sql, object obj=null, IDbConnection connection = null) where T : class
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn.QueryFirstOrDefault<T>(sql, obj);
            }
        }
        /// <summary>
        /// 根据Id获取一个对象
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static T GetConditionById<T>(object id, IDbConnection connection = null) where T : class
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn.Get<T>(id);
            }
        }
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="obj"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static int Execute(string sql, object obj = null, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn.Execute(sql, obj);
            }
        }
        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ExecutePro<T>(string proc, object obj = null,int? commandTimeout=default(int?), IDbConnection connection = null) where T : class
        {
            List<T> list = new List<T>();
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                list = conn.Query<T>(proc, obj, null, true, commandTimeout, CommandType.StoredProcedure).ToList();
            }
            return list;
        }
        /// <summary>
        /// 事务 不带参数 全sql
        /// </summary>
        /// <param name="sqlArray"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static int ExecuteTransaction(string[] sqlArray, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int result = 0;
                        foreach (var sql in sqlArray)
                        {
                            result += conn.Execute(sql, null, transaction);
                        }
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 事务 带参数
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int ExecuteTransaction(Dictionary<string,object> keyValues, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int result = 0;
                        foreach (var sqlItem in keyValues)
                        {
                            result += conn.Execute(sqlItem.Key, sqlItem.Value, transaction);
                        }
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

    }
}

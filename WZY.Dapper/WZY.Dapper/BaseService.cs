using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DapperExtensions;

namespace WZY.Dapper
{
    /// <summary>
    /// Dapper数据库操作类 配置文件web.config数据库连接字符串 name="connStr"
    /// </summary>
    public class BaseService<T> where T: class,new ()
    {
        private static string ConnectionString;
        public BaseService()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["connStr"].ToString();
        }
        
        /// <summary>
        /// 提供数据库连接对象 默认读取配置文件中的值， 如果参数不为空则覆盖配置文件中的值
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <returns></returns>
        public IDbConnection GetConnection(string connStr = "")
        {
            if (!string.IsNullOrEmpty(connStr))
            {
                ConnectionString = connStr;
            }
            IDbConnection connection = new SqlConnection(ConnectionString);
            return connection;
        }

        /// <summary>
        /// 动态设置数据库连接对象 
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <returns></returns>
        public IDbConnection GetConnectionBiz(string connStr)
        {
            IDbConnection connection = new SqlConnection(connStr);
            return connection;
        }
        /// <summary>
        /// 过滤非法字符
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        private string FilterSqlInjectChart(string PropertyName)
        {
            if (string.IsNullOrEmpty(PropertyName))
            {
                return string.Empty;
            }
            PropertyName = PropertyName.Replace(" ", string.Empty);
            PropertyName = PropertyName.Replace(";", string.Empty);
            PropertyName = PropertyName.Replace("'", string.Empty);
            return PropertyName;
        }
        /// <summary>
        /// 根据Id获取一个对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public T GetById(object id, IDbConnection connection = null)
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
        /// 根据Id获取一个对象 异步
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync(object id, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var obj = await conn.GetAsync<T>(id);
                return obj;
            }
        }
        /// <summary>
        /// 根据条件获取一个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault(string sql, object param = null, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn.QueryFirstOrDefault<T>(sql, param);
            }
        }
        /// <summary>
        /// 根据条件获取一个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<T> QueryFirstOrDefaultAsync(string sql, object param = null, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var obj = await conn.QueryFirstOrDefaultAsync<T>(sql, param);
                return obj;
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<T> Query(string sql, object param = null, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var collection = conn.Query<T>(sql, param);
                return collection.ToList();
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsync(string sql, object param = null, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var collection = await conn.QueryAsync<T>(sql, param);

                return collection.ToList();
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public PagedCollection<T> QueryPageList(string sqlQuery,
            object parameters, List<SortDirection> orderBy, int pageIndex, int pageSize,
            int? totalCount = default(int?), IDbConnection connection = null)
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
                return new PagedCollection<T> { pageIndex = pageIndex, pageSize = pageSize, TotalCount = totalCount.Value, DataList = item };
            }
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int Execute(string sql, object param = null, IDbConnection connection = null)
        {
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn.Execute(sql, param);
            }
        }

        /// <summary>
        /// 存储过程
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<T> ExecutePro(string proc, object param = null, int? commandTimeout = default(int?), IDbConnection connection = null)
        {
            List<T> list = new List<T>();
            using (IDbConnection conn = connection ?? GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                list = conn.Query<T>(proc, param, null, true, commandTimeout, CommandType.StoredProcedure).ToList();
            }
            return list;
        }
        /// <summary>
        /// 事务 不带参数 全sql
        /// </summary>
        /// <param name="sqlArray"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int ExecuteTransaction(string[] sqlArray, IDbConnection connection = null)
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
        /// <param name="keyValuePairs"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int ExecuteTransaction(Dictionary<string, object> keyValuePairs, IDbConnection connection = null)
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
                        foreach (var sqlItem in keyValuePairs)
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

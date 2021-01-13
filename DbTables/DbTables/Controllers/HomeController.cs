using CF.Biz;
using CF.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WZY.Dapper;

namespace DbTables.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnBiz dbConnBiz;
        public HomeController()
        {
            dbConnBiz = new DbConnBiz();
        }
        #region 视图
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Setting()
        {
            return View();
        }
        public ActionResult SetForm()
        {
            return View();
        }
        public ActionResult CreateEntityForm()
        {
            return View();
        }
        public ActionResult CreateBizForm()
        {
            return View();
        }
        public ActionResult ColumnsForm()
        {
            return View();
        }
        public ActionResult CreateAutoCodeForm()
        {
            return View();
        }
        public ActionResult CreateIndexALL()
        {
            return View();
        }
        #endregion
        
        /// <summary>
        /// 服务器列表查询
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetServerList(PageModel pageInfo,DbConnEntity queryModel)
        {
            List<SortDirection> sorts = new List<SortDirection>() {
                new SortDirection("id",Direction.Descending),
                new SortDirection("serverName",Direction.Descending)
            };
            PagedCollection<DbConnEntity> pagedCollection= dbConnBiz.GetPageList(queryModel, sorts, pageInfo);
            var result = new ResonseModel
            {
                code = "0",
                msg = "",
                count = pagedCollection.TotalCount,
                data = pagedCollection.DataList
            };
            var json= JsonConvert.SerializeObject(result);
            return Content(json);
        }

        public ActionResult SetServerName(string serverid)
        {
            ResonseModel resonseModel = new ResonseModel() { code="0",msg="操作失败"};
            if (string.IsNullOrEmpty(serverid))
            {
                resonseModel.msg = "参数不能为空";
            }
            else
            {
                int result = dbConnBiz.SetServerName(serverid);
                if (result==0)
                {
                    resonseModel.msg = "设置服务器失败";
                }
                else
                {
                    resonseModel.code = "1";
                    resonseModel.msg = "操作成功";
                }
            }
            return Json(resonseModel);
        }
        public ActionResult GetEntityForm(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                return Json(new DbConnEntity());
            }
            else
            {
                DbConnEntity dbConnEntity= dbConnBiz.GetEntity(keyValue);
                return Json(dbConnEntity);
            }
        }
        public ActionResult SaveForm(DbConnEntity entity)
        {
            ResonseModel resonseModel = new ResonseModel() { code = "0", msg = "操作失败" };
            int result = 0;
            if (entity.id==0)
            {
                entity.isEnable = false;
                entity.isActive = true;
                result = dbConnBiz.Add(entity);
            }
            else
            {
                result = dbConnBiz.Update(entity);
            }
            if (result>0)
            {
                resonseModel.code = "1";
                resonseModel.msg = "操作成功";
                
            }
            return Json(resonseModel);
        }

        public ActionResult RemoveForm(string keyValue)
        {
            ResonseModel resonseModel = new ResonseModel() { code = "0", msg = "操作失败" };
            int result = dbConnBiz.Delete(keyValue);
            if (result > 0)
            {
                resonseModel.code = "1";
                resonseModel.msg = "操作成功";
            }
            return Json(resonseModel);
        }

        #region 获取数据
        /// <summary>
        /// 表字段、说明列表
        /// </summary>
        /// <param name="q"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetList(string q, string name)
        {
            List<TableEntity> listView = await dbConnBiz.GetListAsync(q, name);
            var data = new
            {
                rows = listView,
                total = listView.Count
            };
            var jsonstr = JsonConvert.SerializeObject(data);
            return Content(jsonstr);
        }

        /// <summary>
        /// 表集合查询
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public List<TableEntity> GetTableCollection(string q)
        {
            List<TableEntity> listView = dbConnBiz.GetList(q);
            return listView;
        }
        /// <summary>
        /// 数据库表名列表
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetTableList(string q)
        {
            var listView = GetTableCollection(q);
            var data = new
            {
                rows = listView,
                total = listView.Count
            };
            var jsonstr = JsonConvert.SerializeObject(data);
            return Content(jsonstr);
        }
        /// <summary>
        /// 数据库表字段转JSON
        /// </summary>
        /// <param name="q"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult GetJsonData(string q,string type)
        {
            var jsondata = dbConnBiz.GetJsonData(q,type);
            return Content(jsondata);
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult CreateEntityFile(string q, string ns)
        {
            string filecs = dbConnBiz.CreateEntityContent(q, ns);
            return Content(filecs);
        }

        /// <summary>
        /// 生成业务类
        /// </summary>
        /// <param name="q">表名</param>
        /// <param name="ns">命名空间名字</param>
        /// <returns></returns>
        public ActionResult CreateBizFile(string q, string ns)
        {
            string filecs = dbConnBiz.CreateBizContent(q, ns);
            return Content(filecs);
        }

        /// <summary>
        /// 生成全部
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public ActionResult CreateAllFile(string q, string ns)
        {
            string entitystr =dbConnBiz.CreateEntityContent(q, ns);
            string bizstr = dbConnBiz.CreateBizContent(q, ns);
            string ctrlstr = dbConnBiz.CreateControllerContent(q, ns);
            string indexstr = dbConnBiz.CreateIndexContent(q, ns);
            string formstr = dbConnBiz.CreateFormContent(q, ns);
            var result = new
            {
                entityStr = entitystr,
                bizStr = bizstr,
                ctrlStr = ctrlstr,
                indexStr=indexstr,
                formStr = formstr
            };
            return Json(result);
        } 
        #endregion
    }
}
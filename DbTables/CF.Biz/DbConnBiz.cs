using CF.Entity;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZY.Dapper;

namespace CF.Biz
{
    public class DbConnBiz
    {

        #region 生成Form字符串
        /// <summary>
        /// 生成Form字符串
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public string CreateFormContent(string q, string ns)
        {
            string sqlstr = @"
SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

";
            if (string.IsNullOrEmpty(ns))
            {
                ns = "RootPath.LastPath";
            }
            string rootPath = ns.Substring(ns.LastIndexOf(".") + 1);
            if (!string.IsNullOrEmpty(q))
            {
                sqlstr += " WHERE A.name = '" + q + "'";
            }
            List<TableEntity> collection = new List<TableEntity>();
            collection = DapperHelper.Query<TableEntity>(sqlstr, null, GetBizConnectionString());

            var listView = GetList(q);
            var desc = "";
            if (listView.Count > 0)
            {
                desc = listView.Where(x => x.tablename == q).Select(x => x.description).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(desc))
            {
                desc = q;
            }
            var tableName = q;
            if (q.Contains("_"))
            {
                int LastIndex = q.LastIndexOf('_') + 1;
                q = q.Substring(LastIndex);
            }

            string rn = "\r\n";
            string filecs = "";
            filecs += "@{" + rn;
            filecs += $"     ViewBag.Title = \"{desc+ "编辑"}\";" + rn;
            filecs += "     Layout = \"~/Views/Shared/_Form.cshtml\";" + rn;
            filecs += "}" + rn;
            filecs += rn;

            #region script 拼接
            filecs += "<script>" + rn;
            filecs += "     var keyValue = request(\"keyValue\");" + rn;
            filecs += "     $(function () {" + rn;
            filecs += "         LoadFormData();" + rn;
            filecs += "     });" + rn;
            filecs += rn;
            filecs += "     //初始化表单" + rn;
            filecs += "     function LoadFormData() {" + rn;
            filecs += "         //获取表单" + rn;
            filecs += "         if (!!keyValue) {" + rn;
            filecs += "             $.SetForm({" + rn;
            filecs += "                 url: '/" + rootPath + "/" + q + "/GetFormJson'," + rn;
            filecs += "                 param: { keyValue: keyValue }," + rn;
            filecs += "                 success: function (data) {" + rn;
            filecs += "                     $('#form1').SetWebControls(data);" + rn;
            filecs += "                 }" + rn;
            filecs += "             });" + rn;
            filecs += "         }" + rn;
            filecs += "     }" + rn;
            
            
            filecs += "     //保存表单" + rn;
            filecs += "     function AcceptClick() {" + rn;
            filecs += "         if (!$('#form1').Validform()) {" + rn;
            filecs += "             return false;" + rn;
            filecs += "         }" + rn;
            filecs += "         var postData = $('#form1').GetWebControls_Ext('');" + rn;
            filecs += "         $.SaveForm({" + rn;
            filecs += "             url: '/" + rootPath + "/" + q + "/SaveForm?keyValue='+keyValue," + rn;
            filecs += "             param: postData," + rn;
            filecs += "             loading: '数据处理中...'," + rn;
            filecs += "             success: function (data) {" + rn;
            filecs += "                 $.currentIframe().btn_search();" + rn;
            filecs += "             }" + rn;
            filecs += "         });" + rn;
            filecs += "     }" + rn;
            filecs += rn;
            filecs += "</script>" + rn;
            #endregion
            filecs += rn;

            #region html拼接
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"ui-layout\" id=\"layout\" style=\"height: 100%; width: 100%;\">" + rn);
            sb.Append("    <div class=\"ui-layout-center\">" + rn);
            sb.Append("        <div class=\"center-Panel\">" + rn);
            
            List<string> fieldList = new List<string>() { "CreaterID", "CreaterName", "CreateDT", "UpdaterID", "UpdaterName", "UpdateDT", "IsActive", "" + collection.FirstOrDefault().columnname + "" };
            collection = collection.Where(x => !fieldList.Contains(x.columnname)).ToList();

            int n = 0;
            foreach (var item in collection)
            {
                if (n == 2)
                {
                    n = 0;
                }
                if (n % 2 == 0)
                {
                    sb.Append("            <div class=\"row\">" + rn);
                }
                sb.Append("                <div class=\"col-xs-6\">" + rn);
                sb.Append("                    <label class=\"form-label col-md-4\">" + item.description + "</label>" + rn);
                sb.Append("                    <div class=\"col-md-8\">" + rn);
                if (GetDataType(item.datatype) == "DateTime")
                {
                    sb.Append("                        <input id=\"" + item.columnname + "\" name=\"" + item.columnname + "\" type=\"text\" class=\"form-control input-wdatepicker\"  onfocus=\"WdatePicker({ dateFmt: 'yyyy-MM-dd'})\" autocomplete=\"off\" />" + rn);

                }
                else
                {
                    sb.Append("                        <input id=\"" + item.columnname + "\" name=\"" + item.columnname + "\" type=\"text\" class=\"form-control\" placeholder=\"" + item.description + "\" />" + rn);
                }
                sb.Append("                    </div>" + rn);
                sb.Append("                </div>" + rn);

                n++;
                if (n == 2)
                {
                    sb.Append("            </div>" + rn);
                }
            }
            if (n==1)
            {
                sb.Append("             </div>" + rn);
            }
            sb.Append("        </div>" + rn);
            sb.Append("    </div>" + rn);
            sb.Append("</div>" + rn);


            #endregion

            filecs += sb.ToString();

            return filecs;
        }
        #endregion

        #region 生成Index页面
        /// <summary>
        /// 生成Index字符串
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public string CreateIndexContent(string q, string ns)
        {
            string sqlstr = @"
SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

";
            if (string.IsNullOrEmpty(ns))
            {
                ns = "MySelf.UserInfo";
            }
            string rootPath = ns.Substring(ns.LastIndexOf(".")+1);
            if (!string.IsNullOrEmpty(q))
            {
                sqlstr += " WHERE A.name = '" + q + "'";
            }
            List<TableEntity> collection = new List<TableEntity>();
            collection = DapperHelper.Query<TableEntity>(sqlstr, null, GetBizConnectionString());

            var listView = GetList(q);
            var desc = "";
            if (listView.Count > 0)
            {
                desc = listView.Where(x => x.tablename == q).Select(x => x.description).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(desc))
            {
                desc = q;
            }
            var tableName = q;
            if (q.Contains("_"))
            {
                int LastIndex = q.LastIndexOf('_') + 1;
                q = q.Substring(LastIndex);
            }
            
            string rn = "\r\n";
            string filecs = "";
            filecs += "@{" + rn;
            filecs += $"     ViewBag.Title = \"{desc}\";" + rn;
            filecs += "     Layout = \"~/Views/Shared/_IndexLayout.cshtml\";" + rn;
            filecs += "}" + rn;
            filecs += rn;

            #region script 拼接
            filecs += "<script>" + rn;
            filecs += "     $(function () {" + rn;
            filecs += "           GetGrid();" + rn;
            filecs += "     });" + rn;
            filecs += rn;
            filecs += "     //加载表格" + rn;
            filecs += "     function GetGrid() {" + rn;
            filecs += "             var columnsAll = [" + rn;
            foreach (var item in collection)
            {
                if (item.columnname == collection.FirstOrDefault().columnname || item.columnname == "CreaterID" || item.columnname == "UpdaterID" || item.columnname.EndsWith("ID"))
                {
                    filecs += "                 { title: '" + (string.IsNullOrEmpty(item.description) ? item.columnname : item.description) + "', field: '" + item.columnname + "', hidden: true }," + rn;
                }
                else
                {
                    if (GetDataType(item.datatype) == "bool")
                    {
                        filecs += "                 { title: '" + (string.IsNullOrEmpty(item.description)?item.columnname:item.description) + "', field: '" + item.columnname + "', width: 120, align: 'left'," + rn;
                        filecs += "                     formatter: function (value, row){" + rn;
                        filecs += "                         if (value){" + rn;
                        filecs += "                                 return '是';" + rn;
                        filecs += "                         }" + rn;
                        filecs += "                         else{" + rn;
                        filecs += "                                 return '否';" + rn;
                        filecs += "                         }" + rn;
                        filecs += "                     }" + rn;
                        filecs += "                 }," + rn;
                    }
                    else if(GetDataType(item.datatype) == "DateTime")
                    {
                        filecs += "                 { title: '" + (string.IsNullOrEmpty(item.description) ? item.columnname : item.description) + "', field: '" + item.columnname + "', width: 150, align: 'left',formatter:com.formatDate }," + rn;
                    }
                    else
                    {
                        filecs += "                 { title: '" + (string.IsNullOrEmpty(item.description) ? item.columnname : item.description) + "', field: '" + item.columnname + "', width: 120, align: 'left' }," + rn;
                    }
                }
            }
            filecs = filecs.Remove(filecs.LastIndexOf(","), 1);
            filecs += "             ];" + rn;
            filecs += rn;
            filecs += "             $('#gridTable').datagrid({" + rn;
            filecs += "                 pageSize: 50," + rn;
            filecs += "                 pageList: [50, 100, 200]," + rn;
            filecs += "                 pagination: true," + rn;
            filecs += "                 rownumbers: true," + rn;
            filecs += "                 singleSelect: true," + rn;
            filecs += "                 selectOnCheck: true," + rn;
            filecs += "                 checkOnSelect: true," + rn;
            filecs += "                 sortName: 'CreateDT'," + rn;
            filecs += "                 sortOrder: 'desc'," + rn;
            filecs += "                 idField: '" + collection.FirstOrDefault().columnname + "'," + rn;
            filecs += "                 method: 'post'," + rn;
            filecs += "                 onDblClickRow: function () { btn_edit() }," + rn;
            filecs += "                 columns: [columnsAll]," + rn;
            filecs += "                 url: '/" + rootPath + "/" + q + "/GetPageListJson'," + rn;
            filecs += "                 queryParams: $('.searchPanel').GetWebControls()" + rn;
            filecs += "             });" + rn;
            filecs += "             $('#gridTable').datagrid('resize', { width: ($('.gridPanel').width()), height: ($(window).height() - $('.searchPanel').height() - $('.toolbarPanel').height() - 9) });" + rn;
            filecs += "     }" + rn;
            filecs += rn;

            filecs += "     //查询" + rn;
            filecs += "     function btn_search() {" + rn;
            filecs += "         var rows = $('#gridTable').datagrid('getSelections');" + rn;
            filecs += "         if (rows != undefined && rows.length > 0) {" + rn;
            filecs += "             $('#gridTable').datagrid('clearSelections'); //必选先清除选择项 再重新加载数据" + rn;
            filecs += "         }" + rn;
            filecs += "         var queryParams = {};" + rn;
            filecs += "         queryParams = $('.searchPanel').GetWebControls();" + rn;
            filecs += "         $('#gridTable').datagrid('options').url = '/" + rootPath + "/" + q + "/GetPageListJson';" + rn;
            filecs += "         $('#gridTable').datagrid('options').queryParams = queryParams;" + rn;
            filecs += "         $('#gridTable').datagrid('reload');" + rn;
            filecs += "     }" + rn;
            filecs += rn;

            filecs += "     //新增" + rn;
            filecs += "     function btn_add() {" + rn;
            filecs += "         dialogOpen({" + rn;
            filecs += "             id: 'Form'," + rn;
            filecs += "             title: '新增'," + rn;
            filecs += "             url: '/" + rootPath + "/" + q + "/Form'," + rn;
            filecs += "             width: '600px'," + rn;
            filecs += "             height: '460px'," + rn;
            filecs += "             callBack: function (iframeId) {" + rn;
            filecs += "                 top.frames[iframeId].AcceptClick();" + rn;
            filecs += "             }" + rn;
            filecs += "         });" + rn;
            filecs += "     }" + rn;
            filecs += rn;

            filecs += "     //编辑" + rn;
            filecs += "     function btn_edit() {" + rn;
            filecs += "         var row = $('#gridTable').datagrid('getSelected');" + rn;
            filecs += "         if (row == null) {" + rn;
            filecs += "             dialogMsg('请选择需要编辑的数据！', 0);" + rn;
            filecs += "             return false;" + rn;
            filecs += "         }" + rn;
            filecs += "         var keyValue = row." + collection.FirstOrDefault().columnname + ";" + rn;
            filecs += "         if (keyValue) {" + rn;
            filecs += "             dialogOpen({" + rn;
            filecs += "                 id: 'Form'," + rn;
            filecs += "                 title: '编辑'," + rn;
            filecs += "                 url: '/" + rootPath + "/" + q + "/Form?keyValue=' + keyValue," + rn;
            filecs += "                 width: '600px'," + rn;
            filecs += "                 height: '460px'," + rn;
            filecs += "                 callBack: function (iframeId) {" + rn;
            filecs += "                     top.frames[iframeId].AcceptClick();" + rn;
            filecs += "                 }" + rn;
            filecs += "             });" + rn;
            filecs += "         }" + rn;
            filecs += "     }" + rn;
            filecs += rn;

            filecs += "     //删除" + rn;
            filecs += "     function btn_delete() {" + rn;
            filecs += "         var row = $('#gridTable').datagrid('getSelected');" + rn;
            filecs += "         if (row == null) {" + rn;
            filecs += "             dialogMsg('请选择需要删除的数据！', 0);" + rn;
            filecs += "             return false;" + rn;
            filecs += "         }" + rn;
            filecs += "         var keyValue = row." + collection.FirstOrDefault().columnname + ";" + rn;
            filecs += "         if (keyValue) {" + rn;
            filecs += "             $.RemoveForm({" + rn;
            filecs += "                 url: '/" + rootPath + "/" + q + "/RemoveForm'," + rn;
            filecs += "                 param: { keyValue: keyValue }," + rn;
            filecs += "                 success: function (data) {" + rn;
            filecs += "                     btn_search();" + rn;
            filecs += "                 }" + rn;
            filecs += "             });" + rn;
            filecs += "         }" + rn;
            filecs += "     }" + rn;
            filecs += rn;

            filecs += "     //关闭" + rn;
            filecs += "     function btn_close() {" + rn;
            filecs += "         top.tablist.close();" + rn;
            filecs += "     }" + rn;
            filecs += rn;
            filecs += "</script>" + rn;
            #endregion
            filecs += rn;

            #region html拼接
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"ui-layout\" id=\"layout\" style=\"height: 100%; width: 100%;\">" + rn);
            sb.Append("    <div class=\"ui-layout-north\">" + rn);
            sb.Append("        <div class=\"north-Panel\">" + rn);
            sb.Append("            <div class=\"toolbarPanel\">" + rn);
            sb.Append("                <div class=\"title\">"+desc+"管理</div>" + rn);
            sb.Append("                <div class=\"toolbar\">" + rn);
            sb.Append("                    <div class=\"btn-group\">" + rn);
            sb.Append("                        <a id=\"lr-search\" class=\"btn btn-default\" onclick=\"btn_search()\"><i class=\"fa fa-search\"></i>查询</a>" + rn);
            sb.Append("                    </div>" + rn);
            sb.Append("                    <div class=\"btn-group\">" + rn);
            sb.Append("                        <a id=\"lr-add\" class=\"btn btn-default\" onclick=\"btn_add()\"><i class=\"fa fa-plus\"></i>新增</a>" + rn);
            sb.Append("                        <a id=\"lr-edit\" class=\"btn btn-default\" onclick=\"btn_edit()\"><i class=\"fa fa-pencil-square-o\"></i>编辑</a>" + rn);
            sb.Append("                        <a id=\"lr-delete\" class=\"btn btn-default\" onclick=\"btn_delete()\"><i class=\"fa fa-trash-o\"></i>删除</a>" + rn);
            sb.Append("                    </div>" + rn);
            sb.Append("                    @*<script>$('.toolbar').authorizeButton('PF_0900')</script>*@" + rn);
            sb.Append("                </div>" + rn);
            sb.Append("            </div>" + rn);
            sb.Append("        </div>" + rn);
            sb.Append("    </div>" + rn);
            sb.Append("    <div class=\"ui-layout-center\">" + rn);
            sb.Append("        <div class=\"center-Panel\">" + rn);
            sb.Append("            <div class=\"searchPanel\">" + rn);
            
            List<string> fieldList =new List<string>() { "CreaterID", "CreaterName","CreateDT","UpdaterID","UpdaterName","UpdateDT","IsActive",""+collection.FirstOrDefault().columnname+""};
            collection = collection.Where(x => !fieldList.Contains(x.columnname)).ToList();
            
            int n = 0;
            foreach (var item in collection)
            {
                if (n==4)
                {
                    n = 0;
                }
                if (n % 4 == 0)
                {
                    sb.Append("                <div class=\"row\">" + rn);
                    
                }
                sb.Append("                    <div class=\"col-xs-3\">" + rn);
                sb.Append("                        <label class=\"form-label col-md-4\">"+item.description+"</label>" + rn);
                sb.Append("                        <div class=\"col-md-8\">" + rn);
                if (GetDataType(item.datatype)=="DateTime")
                {
                    sb.Append("                            <input id=\"" + item.columnname + "\" name=\"" + item.columnname + "\" type=\"text\" class=\"form-control input-wdatepicker\"  onfocus=\"WdatePicker({ dateFmt: 'yyyy-MM-dd HH:mm'})\" autocomplete=\"off\" />" + rn);

                }
                else
                {
                    sb.Append("                            <input id=\"" + item.columnname + "\" name=\"" + item.columnname + "\" type=\"text\" class=\"form-control\" placeholder=\"" + item.description + "\" />" + rn);
                }
                sb.Append("                        </div>" + rn);
                sb.Append("                    </div>" + rn);
                
                n++;
                if (n==4)
                {
                    sb.Append("                </div>" + rn);
                }
            }
            sb.Append("                 </div>" + rn);
            sb.Append("            </div>" + rn);
            sb.Append("            <div class=\"gridPanel\">" + rn);
            sb.Append("                <table id=\"gridTable\"></table>" + rn);
            sb.Append("            </div>" + rn);
            sb.Append("        </div>" + rn);
            sb.Append("    </div>" + rn);
            sb.Append("</div>" + rn);


            #endregion

            filecs += sb.ToString();

            return filecs;
        }
        #endregion

        #region 生成实体类字符串
        /// <summary>
        /// 生成实体类字符串
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public string CreateEntityContent(string q, string ns)
        {
            string sqlstr = @"
SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

";

            if (!string.IsNullOrEmpty(q))
            {
                sqlstr += " WHERE A.name = '" + q + "'";
            }
            List<TableEntity> collection = new List<TableEntity>();
            collection = DapperHelper.Query<TableEntity>(sqlstr,null, GetBizConnectionString());

            var listView = GetList(q);
            var desc = "";
            if (listView.Count > 0)
            {
                desc = listView.Where(x => x.tablename == q).Select(x => x.description).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(desc))
            {
                desc = q;
            }
            var tableName = q;
            if (q.Contains("_"))
            {
                int LastIndex = q.LastIndexOf('_') + 1;
                q = q.Substring(LastIndex);
            }
            var entityName = q + "Entity";
            string usinglist = @"using DapperExtensions.Mapper;
using System;";
            string rn = "\r\n";
            string filecs = "";
            string nsname = string.IsNullOrEmpty(ns) ? "MCP.Entity" : ns + ".Entity";
            filecs += usinglist + rn;
            filecs += rn;
            filecs += "namespace " + nsname + "" + rn;
            filecs += "{" + rn;
            filecs += "     /// <summary>" + rn;
            filecs += "     /// 实体数据库映射表" + rn;
            filecs += "     /// </summary>" + rn;
            filecs += "     public sealed class " + entityName + "Mapper : ClassMapper<" + entityName + ">" + rn;
            filecs += "     {" + rn;
            filecs += "             /// <summary>" + rn;
            filecs += "             /// 实体数据库映射表" + rn;
            filecs += "             /// </summary>" + rn;
            filecs += "             public " + entityName + "Mapper()" + rn;
            filecs += "             {" + rn;
            filecs += "                 Table(\"" + tableName + "\");" + rn;
            filecs += "                 Map(i => i." + collection.FirstOrDefault().columnname + ").Key(KeyType.Assigned);" + rn;
            filecs += "                 AutoMap();" + rn;
            filecs += "             }" + rn;
            filecs += "     }" + rn;
            filecs += rn;
            filecs += "     /// <summary>" + rn;
            filecs += "     /// " + desc + "" + rn;
            filecs += "     /// </summary>" + rn;
            filecs += "     public class " + entityName + "" + rn;
            filecs += "     {" + rn;
            var bodystr = "";
            foreach (var item in collection)
            {
                bodystr += "            ///<summary>" + rn;
                bodystr += "            ///" + item.description?.Replace("\r\n", ";") + rn;
                bodystr += "            ///</summary>" + rn;
                if (GetDataType(item.datatype) == "DateTime" || GetDataType(item.datatype) == "decimal")
                {
                    bodystr += "            public " + GetDataType(item.datatype) + "? " + item.columnname + " {get;set;}" + rn;
                }
                else
                {
                    bodystr += "            public " + GetDataType(item.datatype) + " " + item.columnname + " {get;set;}" + rn;
                }
                bodystr += rn;
            }
            filecs += bodystr;
            filecs += "     }" + rn;
            filecs += "}" + rn;
            return filecs;
        }
        #endregion

        #region 生成业务类字符串
        /// <summary>
        /// 生成业务类字符串
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public string CreateBizContent(string q, string ns)
        {
            var listView = GetList(q);
            var desc = "";
            if (listView.Count > 0)
            {
                desc = listView.Where(x => x.tablename == q).Select(x => x.description).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(desc))
            {
                desc = q;
            }
            var tableName = q;
            if (q.Contains("_"))
            {
                int LastIndex = q.LastIndexOf('_') + 1;
                q = q.Substring(LastIndex);
            }
            var entityName = q + "Entity";
            string rn = "\r\n";
            string filecs = "";
            string usinglist = @"using Dapper;
using MCP.ORM.Dapper.Core;
using MCP.ORM.Dapper.Model;
using MCP.Common.Utility;
using MCP.Framework.Operator;
using {0};
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using System.Transactions;
";
            usinglist = string.Format(usinglist, ns + ".Entity");
            string nsname = string.IsNullOrEmpty(ns) ? "MCP.Biz" : ns + ".Biz";
            filecs += usinglist + rn;
            filecs += "namespace " + nsname + "" + rn;
            filecs += "{" + rn;


            filecs += "     /// <summary>" + rn;
            filecs += "     /// " + desc + "业务类" + rn;
            filecs += "     /// </summary>" + rn;
            filecs += "     public class " + q + "Biz: BaseService<" + entityName + ">" + rn;
            filecs += "     {" + rn;


            var bodystr = "";
            bodystr += "            #region 获取数据" + rn;
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 获取数据分页列表" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"queryModel\">查询实体对象</param>" + rn;
            bodystr += "            /// <param name=\"sorts\">排序</param>" + rn;
            bodystr += "            /// <param name=\"pageIndx\"></param>" + rn;
            bodystr += "            /// <param name=\"pagesize\"></param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            public dynamic GetPagedCollection(" + rn;
            bodystr += "                " + entityName + " queryModel," + rn;
            bodystr += "                List<SortDirection> sorts, int pageIndx, int pagesize)" + rn;
            bodystr += "            {" + rn;
            bodystr += "                    StringBuilder sb = new StringBuilder();" + rn;
            bodystr += "                    sb.Append(\"SELECT a.* FROM [dbo].[" + tableName + "] a WHERE a.IsActive = 1 \");" + rn;
            bodystr += "                    DynamicParameters parameters = new DynamicParameters();" + rn;
            bodystr += "                    //创建人" + rn;
            bodystr += "                    if (!string.IsNullOrEmpty(queryModel.CreaterName))" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            sb.Append(\" and a.CreaterName = @CreaterName  \");" + rn;
            bodystr += "                            parameters.Add(\"@CreaterName\", queryModel.CreaterName);" + rn;
            bodystr += "                    }" + rn;
            bodystr += rn;

            bodystr += "                    if (!sorts.Any())" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            sorts.Add(new SortDirection(\"CreateDT\", false));" + rn;
            bodystr += "                    }" + rn;
            bodystr += "                    var collection = base.SqlQueryPage<" + entityName + ">(sb.ToString(), parameters, sorts, pageIndx, pagesize);" + rn;
            bodystr += "                    var data = new" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            total = collection.TotalCount," + rn;
            bodystr += "                            rows = collection.DataList" + rn;
            bodystr += "                    };" + rn;
            bodystr += "                    return data;" + rn;
            bodystr += "            }" + rn;

            //根据主键获取实体对象
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 根据主键获取实体对象" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"keyValue\">主键</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            public " + entityName + " GetFormEntity(string keyValue) " + rn;
            bodystr += "            {" + rn;
            bodystr += "                    return base.GetByCondition<" + entityName + ">(keyValue);" + rn;
            bodystr += "            }" + rn;
            bodystr += "            #endregion" + rn;
            bodystr += rn;

            bodystr += "            #region 提交" + rn;
            //新增
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 新增" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"entity\">实体对象</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            public string Add(" + entityName + " entity) " + rn;
            bodystr += "            {" + rn;
            bodystr += "                    return base.InSert<" + entityName + ">(entity, isEnabled: true);" + rn;
            bodystr += "            }" + rn;
            bodystr += rn;

            //编辑
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 编辑" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"entity\">实体对象</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            public bool Edit(" + entityName + " entity) " + rn;
            bodystr += "            {" + rn;
            bodystr += "                    return base.Update<" + entityName + ">(entity, isEnabled: false);" + rn;
            bodystr += "            }" + rn;
            bodystr += rn;

            //删除
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 删除" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"keyValue\">主键</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            public bool Delete(string keyValue) " + rn;
            bodystr += "            {" + rn;
            bodystr += "                    var userCurr = OperatorProvider.Provider.Current();" + rn;
            bodystr += "                    var entity = GetFormEntity(keyValue);" + rn;
            bodystr += "                    if(entity != null)" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            entity.IsActive = false;" + rn;
            bodystr += "                            entity.UpdateDT = DateTime.Now;" + rn;
            bodystr += "                            entity.UpdaterID = userCurr.UserId;" + rn;
            bodystr += "                            entity.UpdaterName = userCurr.UserName;" + rn;
            bodystr += "                    }" + rn;
            bodystr += "                    return base.Update<" + entityName + ">(entity, isEnabled: false);" + rn;
            bodystr += "            }" + rn;
            bodystr += "            #endregion" + rn;
            filecs += bodystr;
            filecs += "     }\r\n";
            filecs += " }\r\n";
            return filecs;
        }
        #endregion

        #region 生成控制器类字符串
        /// <summary>
        /// 生成控制器类字符串
        /// </summary>
        /// <param name="q"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public string CreateControllerContent(string q, string ns)
        {
            string sqlstr = @"
SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

";

            if (!string.IsNullOrEmpty(q))
            {
                sqlstr += " WHERE A.name = '" + q + "'";
            }
            List<TableEntity> collection = new List<TableEntity>();
            collection = DapperHelper.Query<TableEntity>(sqlstr, null, GetBizConnectionString());

            var listView = GetList(q);
            var desc = "";
            if (listView.Count > 0)
            {
                desc = listView.Where(x => x.tablename == q).Select(x => x.description).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(desc))
            {
                desc = q;
            }
            var tableName = q;
            if (q.Contains("_"))
            {
                int LastIndex = q.LastIndexOf('_') + 1;
                q = q.Substring(LastIndex);
            }
            string rn = "\r\n";
            string filecs = "";
            string usinglist = @"using MCP.ORM.Dapper.Core;
using MCP.ORM.Dapper.Model;
using MCP.Common.Utility;
using MCP.SysManage.External;
using MCP.Framework.Operator;
using {0}.Entity;
using {0}.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
using MCP.OA.Util;
";
            usinglist = string.Format(usinglist, ns);
            filecs += usinglist + rn;
            string nsname = string.IsNullOrEmpty(ns) ? "MCP.WebUI.Controllers" : ns + ".WebUI.Controllers";
            filecs += "namespace " + nsname + "" + rn;
            filecs += "{" + rn;


            filecs += "     /// <summary>" + rn;
            filecs += "     /// " + desc + "控制器类" + rn;
            filecs += "     /// </summary>" + rn;
            filecs += $"     public class {q}Controller : MvcControllerBase" + rn;
            filecs += "     {" + rn;

            var bodystr = "";
            bodystr += $"            private readonly {q}Biz {q.ToLower()}Biz;" + rn;

            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 构造函数" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += $"            public {q}Controller()" + rn;
            bodystr += "            {" + rn;
            bodystr += $"                    {q.ToLower()}Biz = new {q}Biz();" + rn;
            bodystr += "            }" + rn;
            bodystr += rn;

            bodystr += "            #region 视图" + rn;
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 列表" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            public ActionResult Index()" + rn;
            bodystr += "            {" + rn;
            bodystr += "                    return View(); " + rn;
            bodystr += "            }" + rn;
            bodystr += rn;

            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 表单" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            public ActionResult Form()" + rn;
            bodystr += "            {" + rn;
            bodystr += "                    return View(); " + rn;
            bodystr += "            }" + rn;
            bodystr += "            #endregion" + rn;
            bodystr += rn;


            bodystr += "            #region 获取数据" + rn;
            //分页查询
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 分页查询" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"queryModel\">查询实体对象</param>" + rn;
            bodystr += "            /// <param name=\"pageInfo\">分页对象</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            [HttpPost]" + rn;
            bodystr += $"            public ActionResult GetPageListJson({q}Entity queryModel, PageInfo pageInfo)" + rn;
            bodystr += "            {" + rn;
            bodystr += "                    List<SortDirection> orderBy = new List<SortDirection>()" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            new SortDirection(pageInfo.Sort,pageInfo.Order.Equals(\"asc\",StringComparison.CurrentCultureIgnoreCase))" + rn;
            bodystr += "                    };" + rn;
            bodystr += rn;
            bodystr += $"                    var data = {q.ToLower()}Biz.GetPagedCollection(queryModel, orderBy, pageInfo.Page, pageInfo.Rows);" + rn;
            bodystr += "                    return ToJsonResult(data);" + rn;
            bodystr += "            }" + rn;
            bodystr += rn;

            //根据主键获取实体对象
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 获取实体" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"keyValue\">主键</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            [HttpGet]" + rn;
            bodystr += "            public ActionResult GetFormJson(string keyValue)" + rn;
            bodystr += "            {" + rn;
            bodystr += $"                    var data = {q.ToLower()}Biz.GetFormEntity(keyValue);" + rn;
            bodystr += "                    return ToJsonResult(data);" + rn;
            bodystr += "            }" + rn;
            bodystr += "            #endregion" + rn;
            bodystr += rn;

            bodystr += "            #region 提交" + rn;
            //新增、编辑
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 新增、编辑" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"keyValue\">keyValue</param>" + rn;
            bodystr += "            /// <param name=\"model\">model</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += $"            public ActionResult SaveForm(string keyValue, {q}Entity model)" + rn;
            bodystr += "            {" + rn;
            bodystr += "                    DateTime time = DateTime.Now;" + rn;
            bodystr += "                    var opUserInfo = OperatorProvider.Provider.Current();" + rn;
            bodystr += "                    if (string.IsNullOrEmpty(keyValue))" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            model.CreaterID = opUserInfo.UserId;" + rn;
            bodystr += "                            model.CreateDT = time;" + rn;
            bodystr += "                            model.CreaterName = opUserInfo.UserName;" + rn;
            bodystr += "                            model.IsActive = true;" + rn;
            bodystr += "                            model." + collection.FirstOrDefault().columnname + " = Guid.NewGuid().GuidToLongId();" + rn;
            bodystr += $"                            string id = {q.ToLower()}Biz.Add(model);" + rn;
            bodystr += "                            if (!string.IsNullOrEmpty(id))" + rn;
            bodystr += "                            {" + rn;
            bodystr += "                                    return Success(\"添加成功！\");" + rn;
            bodystr += "                            }" + rn;
            bodystr += "                            else" + rn;
            bodystr += "                            {" + rn;
            bodystr += "                                    return Error(\"添加失败！\");" + rn;
            bodystr += "                            }" + rn;
            bodystr += "                    }" + rn;
            bodystr += "                    else" + rn;
            bodystr += "                    {" + rn;
            bodystr += $"                            {q}Entity entity= {q.ToLower()}Biz.GetFormEntity(keyValue);" + rn;
            foreach (var item in collection)
            {
                if (item.columnname != collection.FirstOrDefault().columnname && item.columnname != "CreaterID" && item.columnname != "CreateDT" && item.columnname != "CreaterName" && item.columnname!="IsActive")
                {
                    if (item.columnname == "UpdaterID")
                    {
                        bodystr += "                            entity." + item.columnname + " = opUserInfo.UserId;" + rn;
                    }
                    else if (item.columnname == "UpdaterName")
                    {
                        bodystr += "                            entity." + item.columnname + " = opUserInfo.UserName;" + rn;
                    }
                    else if (item.columnname == "UpdateDT")
                    {
                        bodystr += "                            entity." + item.columnname + " = time;" + rn;
                    }
                    else
                    {
                        bodystr += "                            entity." + item.columnname + " = model." + item.columnname + ";" + rn;
                    }
                }
            }
            bodystr += $"                            bool flag = {q.ToLower()}Biz.Edit(entity);" + rn;
            bodystr += "                            if (flag)" + rn;
            bodystr += "                            {" + rn;
            bodystr += "                                    return Success(\"修改成功！\");" + rn;
            bodystr += "                            }" + rn;
            bodystr += "                            else" + rn;
            bodystr += "                            {" + rn;
            bodystr += "                                    return Error(\"修改失败！\");" + rn;
            bodystr += "                            }" + rn;
            bodystr += "                    }" + rn;
            bodystr += "            }" + rn;
            bodystr += rn;

            //删除
            bodystr += "            /// <summary>" + rn;
            bodystr += "            /// 删除" + rn;
            bodystr += "            /// </summary>" + rn;
            bodystr += "            /// <param name=\"keyValue\">主键</param>" + rn;
            bodystr += "            /// <returns></returns>" + rn;
            bodystr += "            public ActionResult RemoveForm(string keyValue)" + rn;
            bodystr += "            {" + rn;
            bodystr += $"                    var flag = {q.ToLower()}Biz.Delete(keyValue);" + rn;
            bodystr += "                    if (flag)" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            return Success(\"删除成功！\");" + rn;
            bodystr += "                    }" + rn;
            bodystr += "                    else" + rn;
            bodystr += "                    {" + rn;
            bodystr += "                            return Error(\"删除失败！\");" + rn;
            bodystr += "                    }" + rn;
            bodystr += "            }" + rn;
            bodystr += "            #endregion" + rn;
            filecs += bodystr;
            filecs += "     }\r\n";
            filecs += " }\r\n";
            return filecs;
        }
        #endregion

        #region sql server数据类型转C#实体类数据类型
        /// <summary>
        /// sql server数据类型转C#实体类数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetDataType(string value)
        {
            string CSharpDataType = string.Empty;
            switch (value)
            {
                case "bit":
                    CSharpDataType = "bool";
                    break;
                case "tinyint":
                    CSharpDataType = "byte";
                    break;
                case "smallint":
                    CSharpDataType = "short";
                    break;
                case "int":
                    CSharpDataType = "int";
                    break;
                case "bigint":
                    CSharpDataType = "long";
                    break;
                case "smallmoney":
                case "money":
                case "numeric":
                case "decimal":
                    CSharpDataType = "decimal";//decim
                    break;
                case "float":
                    CSharpDataType = "double";
                    break;
                case "real":
                    CSharpDataType = "float";
                    break;
                case "smalldatetime":
                case "datetime":
                case "timestamp":
                case "date":
                    CSharpDataType = "DateTime";
                    break;
                case "char":
                case "text":
                case "varchar":
                case "nchar":
                case "ntext":
                case "nvarchar":
                    CSharpDataType = "string";
                    break;
                case "binary":
                case "varbinary":
                case "image":
                    CSharpDataType = "byte[]";
                    break;
                case "uniqueidentifier":
                    CSharpDataType = "Guid";
                    break;
                case "Variant":
                    CSharpDataType = "object";
                    break;
                default:
                    break;
            }
            return CSharpDataType;

        }
        #endregion

        /// <summary>
        /// 数据库表字段转JSON
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public string GetJsonData(string q,string type)
        {
            List<string> listcolumn = new List<string>();

            string sqlstr = @"
SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

";

            if (!string.IsNullOrEmpty(q))
            {
                sqlstr += " WHERE A.name = '" + q + "'";
            }

            var collection = DapperHelper.Query<TableEntity>(sqlstr,null, GetBizConnectionString());
            string jsondata = "{";
            if (type=="en")
            {
                //listcolumn = collection.Select(x => x.columnname).ToList();
                foreach (var item in collection)
                {
                    jsondata += item.columnname + ":\'" + item.columnname + "\',";

                }
                if (jsondata.EndsWith(","))
                {
                    jsondata = jsondata.Substring(0, jsondata.Length - 1);
                }
            }
            else
            {
                foreach (var item in collection)
                {
                    jsondata += item.columnname + ":\'" + item.description + "\',";

                }
                if (jsondata.EndsWith(","))
                {
                    jsondata = jsondata.Substring(0, jsondata.Length - 1);
                }
                //listcolumn = collection.Select(x => x.columnname).ToList();
            }
            
            jsondata += "}";
            return jsondata;
        }

        /// <summary>
        /// 表集合查询
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public List<TableEntity> GetList(string q)
        {
            string sqlstr = @"
select a.name as tablename,b.value as description 
from sys.tables a
left join sys.extended_properties b on b.minor_id=0 and b.major_id=a.object_id
where a.type='U'

";
            List<TableEntity> listView = new List<TableEntity>();
            if (!string.IsNullOrEmpty(q))
            {
                sqlstr += " And a.name like '%" + q + "%'";
                sqlstr += " order by a.name ";
                listView = DapperHelper.Query<TableEntity>(sqlstr,null, GetBizConnectionString());
            }
            else
            {
                sqlstr += " order by a.name ";
                var collection = DapperHelper.Query<TableEntity>(sqlstr,null, GetBizConnectionString());
                listView = collection.Take(20).ToList();
            }
            return listView;
        }

        /// <summary>
        /// 表字段、说明列表
        /// </summary>
        /// <param name="q">搜索字段</param>
        /// <param name="name">模糊查询符号</param>
        /// <returns></returns>
        public async Task<List<TableEntity>> GetListAsync(string q,string name)
        {
            string sqlstr = @"
SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype 
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

";

            List<TableEntity> listView = new List<TableEntity>();
            if (!string.IsNullOrEmpty(name))
            {
                if (name == "like")
                {
                    sqlstr += " WHERE A.name like '" + q + "%'";
                }
                else
                {
                    sqlstr += " WHERE A.name = '" + q + "'";
                }
            }

            if (!string.IsNullOrEmpty(q))
            {
                listView = await DapperHelper.QueryAsync<TableEntity>(sqlstr,null, GetBizConnectionString());
            }
            else
            {
                var collection = await DapperHelper.QueryAsync<TableEntity>(sqlstr,null, GetBizConnectionString());
                listView = collection.Take(20).ToList();
            }
            return listView;
        }
        
        /// <summary>
        /// 分页 查询
        /// </summary>
        /// <param name="dbConnEntity">参数对象</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pageModel">分页对象</param>
        /// <returns></returns>
        public PagedCollection<DbConnEntity> GetPageList(DbConnEntity dbConnEntity,List<SortDirection> orderBy, PageModel pageModel)
        {
            string sqlstr = @" select id,serverNo,serverName,dataBaseName,isEnable from MDT_ServerClient where isActive=1 ";
            DynamicParameters parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(dbConnEntity.serverNo))
            {
                sqlstr += " and serverNo like @serverNo ";
                parameters.Add("@serverNo", "%" + dbConnEntity.serverNo + "%");
            }
            if (!string.IsNullOrEmpty(dbConnEntity.dataBaseName))
            {
                sqlstr += " and dataBaseName like @dataBaseName ";
                parameters.Add("@dataBaseName", "%" + dbConnEntity.dataBaseName + "%");
            }

            PagedCollection<DbConnEntity> pageCollection = DapperHelper.QueryPageList<DbConnEntity>(sqlstr, parameters, orderBy, pageModel.pageIndex, pageModel.pageSize);
            return pageCollection;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public DbConnEntity GetEntity(string keyValue)
        {
            DbConnEntity connEntity =  DapperHelper.GetById<DbConnEntity>(int.Parse(keyValue));
            return connEntity;
        }
        public int Add(DbConnEntity dbConnEntity)
        {
            string sqlstr = " insert into MDT_ServerClient (serverNo,serverName,dataBaseName,userId,password,description,isActive,isEnable) values(@serverNo,@serverName,@dataBaseName,@userId,@password,@description,@isActive,@isEnable) ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@serverNo", dbConnEntity.serverNo);
            parameters.Add("@serverName", dbConnEntity.serverName);
            parameters.Add("@dataBaseName", dbConnEntity.dataBaseName);
            parameters.Add("@userId", dbConnEntity.userId);
            parameters.Add("@password", dbConnEntity.password);
            parameters.Add("@description", dbConnEntity.description);
            parameters.Add("@isActive", dbConnEntity.isActive);
            parameters.Add("@isEnable", dbConnEntity.isEnable);
            int result  = DapperHelper.Execute(sqlstr, parameters);
            return result;
        }
        public int Update(DbConnEntity dbConnEntity)
        {
            string sqlwhere = "";
            if (string.IsNullOrEmpty(dbConnEntity?.id.ToString()))
            {
                return -1;
            }
            else
            {
                sqlwhere = $" where id={dbConnEntity.id} ";
            }
            string sqlstr = "update MDT_ServerClient set ";
            List<string> strList = new List<string>();

            if (!string.IsNullOrEmpty(dbConnEntity.serverNo))
            {
                strList.Add($" serverNo='{dbConnEntity.serverNo}' ");
            }
            if (!string.IsNullOrEmpty(dbConnEntity.serverName))
            {
                strList.Add($" serverName='{dbConnEntity.serverName}' ");
            }
            if (!string.IsNullOrEmpty(dbConnEntity.dataBaseName))
            {
                strList.Add($" dataBaseName='{dbConnEntity.dataBaseName}' ");
            }
            if (!string.IsNullOrEmpty(dbConnEntity.userId))
            {
                strList.Add($" userId='{dbConnEntity.userId}' ");
            }
            if (!string.IsNullOrEmpty(dbConnEntity.password))
            {
                strList.Add($" password='{dbConnEntity.password}' ");
            }
            if (!string.IsNullOrEmpty(dbConnEntity.description))
            {
                strList.Add($" description='{dbConnEntity.description}' ");
            }
            if (dbConnEntity.isEnable)
            {
                strList.Add($" isEnable=1 ");
            }
            else
            {
                strList.Add($" isEnable=0 ");
            }
            strList.Add(" isActive=1 ");
            string sqlfield = string.Join(",", strList);
            sqlstr += sqlfield + sqlwhere;
            int result = DapperHelper.Execute(sqlstr);
            
            return result;
        }
        public int Delete(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                return -1;
            }
            var ids = keyValue.Replace(",", "','");
            //物理删除
            //string sqlstr = $"delete MDT_ServerClient where id in ('{ids}') ";
            //逻辑删除
            string sqlstr = $"update MDT_ServerClient set isActive=0 where id in ('{ids}') ";
            int result = DapperHelper.Execute(sqlstr);
            return result;
        }

        /// <summary>
        /// 设置服务器
        /// </summary>
        /// <param name="keyValue">主键id</param>
        /// <param name="connstr"></param>
        /// <returns></returns>
        public int SetServerName(string keyValue)
        {
            string sql = $"update MDT_ServerClient set isEnable=1 where id='{keyValue}';update MDT_ServerClient set isEnable=0 where id!='{keyValue}';";
            string[] sqlArray = {
                $"update MDT_ServerClient set isEnable=1 where id='{keyValue}'",
                $"update MDT_ServerClient set isEnable=0 where id!='{keyValue}'"
            };
            int result = 0;

            result = DapperHelper.ExecuteTransaction(sqlArray);
            return result;
        }


        /// <summary>
        /// 获取业务数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetBizConnectionString()
        {
            string dConnecionString = "";
            string strsql = "select * from MDT_ServerClient where isEnable=1 ";
            var entity = DapperHelper.QueryFirstOrDefault<DbConnEntity>(strsql,DapperHelper.GetConnection());
            if (entity != null)
            {
                dConnecionString = $"Server={entity.serverName};Initial Catalog={entity.dataBaseName};User ID={entity.userId};Password={entity.password}";
            }
            
            return DapperHelper.GetConnectionBiz(dConnecionString); ;
        }
    }
}

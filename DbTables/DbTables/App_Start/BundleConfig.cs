using System.Web;
using System.Web.Optimization;

namespace DbTables
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/easyui").Include(
                "~/Scripts/jquery.min.js", 
                "~/Scripts/jquery.easyui.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/easyui").Include(
                      "~/Content/jquery-easyui-1.7.0/themes/default/easyui.css",
                      "~/Content/jquery-easyui-1.7.0/themes/icon.css"));
        }
    }
}

using System.Web.Optimization;

namespace UTM.Keto.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // js bundle (без CDN, только локальные файлы в правильном порядке)
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/jquery.mCustomScrollbar.concat.min.js",
                "~/Scripts/custom.js"
            ));

            // css bundle (без CDN, только локальные файлы в правильном порядке)
            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/style.css",
                "~/Content/css/responsive.css"
            ));

            // включение оптимизации (минификация и бандлинг)
            BundleTable.EnableOptimizations = true;
        }
    }
}
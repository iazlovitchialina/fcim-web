using System.Web.Optimization;

namespace UTM.Keto.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/jquery.mCustomScrollbar.concat.min.js",
                "~/Scripts/custom.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/style.css",
                "~/Content/css/responsive.css"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}
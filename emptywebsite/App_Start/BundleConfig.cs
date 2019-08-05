using System.Web.Optimization;

namespace WEB
{
    public partial class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // while hosted on sitedemo during development, don't optimize
//#if DEBUG
            BundleTable.EnableOptimizations = false;
//#endif

            bundles.Add(new ScriptBundle("~/scripts/login").Include(
                "~/scripts/jquery-{version}.js",
                "~/scripts/toastr.min.js"
                ));
            
            var scriptBundle = new ScriptBundle("~/scripts/main").Include(
                // ----- 3RD PARTY
                "~/scripts/jquery-{version}.js",
                "~/scripts/jquery-ui-{version}.js",
                "~/scripts/tether/tether.js",
                "~/scripts/angular.js",
                "~/scripts/angular-breadcrumb.js", // modified: bootstrap4, trimming
                "~/scripts/angular-messages.min.js",
                "~/scripts/angular-resource.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/angular-scroll.min.js",
                "~/scripts/angular-ui-router.js",
                "~/scripts/angular-ui/ui-bootstrap.js",
                "~/scripts/angular-ui/ui-bootstrap-tpls.js",
                "~/scripts/angular-ui/sortable.js",
                "~/scripts/angular-local-storage.js",
                "~/scripts/umd/popper.js",
                "~/scripts/bootstrap.js",
                "~/scripts/moment.js",
                "~/scripts/nya-bootstrap4-select.js",
                "~/scripts/toastr.min.js",
                "~/scripts/angular-scroll.min.js",
                "~/scripts/angular-loading-bar.js",
                "~/scripts/ng-file-upload.min.js",
                // ----- ENUM FILES
                "~/scripts/typings/Enums.js",
                "~/scripts/typings/Roles.js",
                // ----- COMMON
                "~/app/common/app.js",
                "~/app/common/api.js",
                "~/app/common/api-entity.js",
                "~/app/common/authservice.js",
                "~/app/common/errorservice.js",
                "~/app/common/ng-copy.js",
                "~/app/common/filters.js",
                "~/app/common/localdate.js",
                "~/app/common/masterpagecontroller.js",
                "~/app/common/routes.js",
                "~/app/common/routes-entity.js",
                // ----- DIRECTIVES
                "~/app/directives/draggable.js",
                "~/app/directives/filecontent.js",
                "~/app/directives/pager.js",
                "~/app/directives/pagerinfo.js",
                "~/app/directives/appselectuser.js",
                "~/app/directives/selectuser.js",
                // ----- APP FILES
                "~/app/home/index.js",
                "~/app/users/user.js",
                "~/app/users/users.js",
                "~/app/users/changepassword.js",
                "~/app/settings/settings.js"
                // ----- ADD OTHER FILES HERE
                );

            AddGeneratedBundles(scriptBundle);

            bundles.Add(scriptBundle);

            bundles.Add(new StyleBundle("~/content/login").Include(
                "~/content/bootstrap.min.css",
                "~/content/login.css",
                "~/content/toastr.min.css",
                "~/content/font-awesome.min.css.css"
                ));

            bundles.Add(new StyleBundle("~/content/error").Include(
                "~/content/bootstrap.min.css",
                "~/content/login.css"
                ));

            bundles.Add(new StyleBundle("~/content/main").Include(
                "~/content/bootstrap.min.css",
                "~/content/main.css",
                "~/content/toastr.min.css",
                "~/content/font-awesome.min.css",
                "~/content/loading-bar.css",
                "~/content/nya-bootstrap4-select.css"
                ));


        }
    }
}

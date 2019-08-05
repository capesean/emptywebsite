----- IF YOU'RE SETTING UP FROM A BLANK PROJECT -----
1. Create the new project (empty asp.net web application)
2. Copy in the packages.json file from this project
3. Run: Update-Package -reinstall
4. Copy in folders+files from emptywebsite:
	- app, AppStart, Controllers, images, Migrations, Models, templates, Utilities, Views
	- \Global.asax+.cs, \Startup.cs, \fonts\Roboto-Regular.ttf
5. Set up web.config (copy and replace values)
6. Copy the following files that aren't in Nuget:
	- \Scripts\Models.tst
	- \Scripts\nya-bootstrap-select.js
	- \Scripts\angular-scroll.min.js
	- \Scripts\angular-breadcrumb.js (don't use nuget: this has bootstrap4 template)
	- \Content\nya-bootstrap-select.css
7. Uncomment lines in controllers/api/user - allow firstnames, set ordering, etc.
8. When starting the project, some other lines will error: resolve these as appropriate.
	- this typically means having a User model in CodeGenerator with FirstName, LastName and Enabled fields.
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Data.Entity.Validation;
using System.Configuration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<User, AppRole, Guid, AppUserLogin,
        AppUserRole, AppUserClaim>
    {
        public DbSet<Error> Errors { get; set; }
        public DbSet<ErrorException> ErrorExceptions { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            if (ConfigurationManager.AppSettings["RootUrl"].ToString().StartsWith("http://localhost:"))
            {
                // auto migrate if local
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.DevelopmentConfiguration>());
            }
            else
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.DevelopmentConfiguration>());
                // no migrations, for optimal startup
                /* note: this requires the dbinterceptor to be in web.config, so it gets applied (no seed method to apply it on) */
                //Database.SetInitializer<ApplicationDbContext>(null);
            }

            Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    e.Data.Add(Guid.NewGuid().ToString(), string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        e.Data.Add(ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }


        private void CreateComputedColumn(string tableName, string fieldName, string calculation)
        {
            // drop default
            var sql = $@"declare @Command  nvarchar(1000)
                    select @Command = 'ALTER TABLE dbo.{tableName} drop constraint ' + d.name
                     from sys.tables t
                      join    sys.default_constraints d
                       on d.parent_object_id = t.object_id
                      join    sys.columns c
                       on c.object_id = t.object_id
                        and c.column_id = d.parent_column_id
                     where t.name = '{tableName}'
                      and t.schema_id = schema_id('dbo')
                      and c.name = '{fieldName}'

                    execute (@Command);";
            Database.ExecuteSqlCommand(sql);
            // drop column
            Database.ExecuteSqlCommand($"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{fieldName}') ALTER TABLE {tableName} DROP COLUMN {fieldName};");
            // add column
            Database.ExecuteSqlCommand($"ALTER TABLE {tableName} ADD {fieldName} AS {calculation};");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModelBuilder(modelBuilder);

            // add custom indices here with fluent api

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace WEB.Models
{
    public partial class SettingsDTO
    {
        [MaxLength(50)]
        public string RootUrl { get; set; }
        public string SiteName { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
    }

    public class EnumDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }

    public class RoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }

    public partial class ModelFactory
    {
        public static SettingsDTO Create(Settings settings)
        {
            var dto = new SettingsDTO
            {
                // public settings (i.e. those from web.config that should be public)
                RootUrl = settings.RootUrl,
                SiteName = settings.SiteName
            };

            // add roles
            var db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            using (var roleStore = new RoleStore<AppRole, Guid, AppUserRole>(db))
            using (var roleManager = new RoleManager<AppRole, Guid>(roleStore))
            {
                foreach (var role in roleManager.Roles.ToList())
                {
                    Roles roleEnum;
                    var parseResult = Enum.TryParse(role.Name, out roleEnum);
                    if (!parseResult) throw new InvalidCastException("Invalid role in SettingsDTO.Create: " + role.Name);

                    var roleDTO = new RoleDTO
                    {
                        Id = role.Id,
                        Name = roleEnum.ToString(),
                        Label = roleEnum.GetRoleName()
                    };

                    dto.Roles.Add(roleDTO);
                }
            }

            return dto;
        }

        public void Hydrate(Settings settings, SettingsDTO settingsDTO)
        {
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using WEB.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;
using System.Net.Mail;

namespace WEB.Controllers
{
    [Authorize, RoutePrefix("api/users")]
    public class UsersController : BaseApiController
    {
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Search([FromUri]PagingOptions pagingOptions, [FromUri]string searchText = null, [FromUri]Guid? roleId = null)
        {
            IQueryable<User> results = UserManager.Users;
            if (pagingOptions.IncludeEntities)
            {
            }

            if (roleId != null) results = results.Where(u => u.Roles.Any(c => c.RoleId == roleId));

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                results = results.Where(u =>
                    u.Email.Contains(searchText)
                    //|| u.LastName.Contains(searchText)
                    //|| u.FirstName.Contains(searchText)
                    //|| (u.FullName).Contains(searchText)
                    );
            }

            //results = results.OrderBy(u => u.FirstName + " " + u.LastName);
            results = results.OrderBy(u => u.Id);

            return Ok((await GetPaginatedResponse(results, pagingOptions)).Select(o => ModelFactory.Create(o)));
        }

        [AuthorizeRoles(Roles.Administrator), HttpGet, Route("{id:Guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var user = await db.Users
                .Include(u => u.Roles)
                .SingleOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(ModelFactory.Create(user));
        }

        [AuthorizeRoles(Roles.Administrator), HttpPost, Route("")]
        public async Task<IHttpActionResult> Insert([FromBody]UserDTO userDTO)
        {
            if (userDTO.Id != Guid.Empty) return BadRequest("Invalid User Id");

            return await SaveAsync(userDTO);
        }

        [AuthorizeRoles(Roles.Administrator), HttpPost, Route("{id:Guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody]UserDTO userDTO)
        {
            if (id != userDTO.Id) return BadRequest("Id mismatch");

            return await SaveAsync(userDTO);
        }

        private async Task<IHttpActionResult> SaveAsync(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userDTO.Email = userDTO.Email.ToLower();

            var user = await UserManager.FindByEmailAsync(userDTO.Email);

            if (user != null && user.Id != userDTO.Id)
                return BadRequest("Email address already exists.");

            var password = Utilities.General.GenerateRandomPassword();
            var isNew = userDTO.Id == Guid.Empty;

            if (isNew)
            {
                user = new User()
                {
                    Email = userDTO.Email,
                    UserName = userDTO.Email
                };
            }
            else
            {
                user = await UserManager.FindByIdAsync(userDTO.Id);
                if (user == null) return NotFound();
            }

            ModelFactory.Hydrate(user, userDTO);

            var saveResult = (isNew ? await UserManager.CreateAsync(user, password) : await UserManager.UpdateAsync(user));

            if (!saveResult.Succeeded)
                return GetErrorResult(saveResult);

            // only a system admin can edit the roles
            if (CurrentUserIsInRole(Roles.Administrator))
            {
                if (!isNew)
                    foreach (var role in Enum.GetNames(typeof(Roles)))
                        await UserManager.RemoveFromRoleAsync(user.Id, role);

                if (userDTO.RoleIds != null)
                {
                    using (var roleStore = new RoleStore<AppRole, Guid, AppUserRole>(db))
                    using (var roleManager = new RoleManager<AppRole, Guid>(roleStore))
                    {
                        foreach (var role in userDTO.RoleIds)
                        {
                            var r = await roleManager.FindByIdAsync(role);
                            var result = await UserManager.AddToRoleAsync(user.Id, r.Name);
                        }
                    }
                }
            }

            if (isNew)
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(user.Email));
                message.Subject = "Account Created";
                //message.Body = user.FirstName + Environment.NewLine;
                //message.Body += Environment.NewLine;
                message.Body += "A new account has been created for you on " + Settings.SiteName + "." + Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += "To access the site, please login using your email address and the password below:" + Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += "<strong>EMAIL/USER ID:</strong> " + user.Email + Environment.NewLine;
                message.Body += "<strong>PASSWORD:</strong> " + password + Environment.NewLine;
                message.Body += "<strong>LOGIN URL:</strong> " + Settings.RootUrl + "login" + Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += "You may change your password once you have logged in." + Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += "You can reset your password at any time, should you forget it, by following the reset link on the login page." + Environment.NewLine;

                Utilities.Email.SendMail(message, Settings);
            }

            return await Get(user.Id);
        }

        [AuthorizeRoles(Roles.Administrator), HttpDelete, Route("{id:Guid}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            if (id == CurrentUser.Id) return BadRequest("You are not allowed to delete your own account");

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            //if (DbContext.Indicators.Any(o => o.LastSavedById == id || o.CreatedById == id))
            //    return BadRequest("Unable to delete the user as he/she is the owner of one or more reports");

            await UserManager.DeleteAsync(user);

            return Ok();
        }

        [HttpGet, Route("profile")]
        public async Task<IHttpActionResult> Profile()
        {
            // TODO: Customise the User Profile here"
            var profile = new
            {
                CurrentUser.Email,
                //CurrentUser.FirstName,
                //CurrentUser.LastName,
                //CurrentUser.FullName,
                UserId = CurrentUser.Id,
                CurrentUser.Roles
            };

            return Ok(profile);
        }

        [HttpPost, Route("changepassword")]
        public IHttpActionResult ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = UserManager.ChangePassword(CurrentUser.Id, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);

            if (result.Succeeded) return Ok();

            return BadRequest(result.Errors.Aggregate((a, b) => a + ", " + b));
        }

        public class ChangePasswordModel
        {
            public string CurrentPassword { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; }
            [DataType(DataType.Password)]
            [Display(Name = "Repeat Password")]
            [Compare("NewPassword", ErrorMessage = "The New Password and Repeat Password do not match.")]
            public string RepeatPassword { get; set; }
        }

    }
}

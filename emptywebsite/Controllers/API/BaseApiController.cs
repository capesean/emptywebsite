using System;
using System.Collections.Generic;
using System.Linq;
using WEB.Models;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Threading;
using System.Net;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace WEB.Controllers
{
    public class BaseApiController : ApiController
    {
        internal ApplicationDbContext db
        {
            get
            {
                if (_db == null) _db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
                return _db;
            }
        }
        internal Settings Settings
        {
            get
            {
                if (_settings == null) _settings = new Settings(db);
                return _settings;
            }
        }
        internal User CurrentUser
        {
            get
            {
                if (_currentUser == null) _currentUser = UserManager.FindByName(User.Identity.Name);
                return _currentUser;
            }
        }
        internal ModelFactory ModelFactory
        {
            get
            {
                if (_modelFactory == null) _modelFactory = new ModelFactory();
                return _modelFactory;
            }
        }
        internal AppUserManager UserManager
        {
            get
            {
                if (_userManager == null) _userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
                return _userManager;
            }
        }
        private ApplicationDbContext _db;
        private Settings _settings;
        private User _currentUser;
        private ModelFactory _modelFactory;
        private AppUserManager _userManager;

        internal bool CurrentUserIsInRole(Roles role)
        {
            return UserManager.IsInRole(CurrentUser.Id, role.ToString());
        }

        public BaseApiController() : base()
        {
            // added this to force the user to load before any other calls, else there were errors that an async call was already being made...
            _currentUser = UserManager.FindByName(User.Identity.Name);
        }

        protected async Task<List<T>> GetPaginatedResponse<T>(IQueryable<T> query, PagingOptions pagingOptions)
        {
            if (pagingOptions == null) pagingOptions = new PagingOptions();
            if (pagingOptions.PageIndex < 0) pagingOptions.PageIndex = 0;

            var totalRecords = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pagingOptions.PageSize);

            //var prevLink = pagingOptions.PageIndex > 0 ? Url.Link("DefaultApi", new { pagingOptions.PageSize, page = pagingOptions.PageIndex - 1 }) : "";
            //var nextLink = pagingOptions.PageIndex < totalPages - 1 ? Url.Link("DefaultApi", new { pagingOptions.PageSize, page = pagingOptions.PageIndex + 1 }) : "";
            //var firstLink = Url.Link("DefaultApi", new { pagingOptions.PageSize, page = 0 });
            //var lastLink = Url.Link("DefaultApi", new { pagingOptions.PageSize, page = totalPages - 1 });

            var results = await (pagingOptions.PageSize <= 0
                ? query.ToListAsync()
                : query.Skip(pagingOptions.PageSize * pagingOptions.PageIndex)
                    .Take(pagingOptions.PageSize)
                    .ToListAsync());

            var paginationHeader = new
            {
                pageIndex = pagingOptions.PageIndex,
                pageSize = pagingOptions.PageSize,
                records = results.Count,
                totalRecords = totalRecords,
                totalPages = totalPages//,
                //FirstPageLink = firstLink,
                //PrevPageLink = prevLink,
                //NextPageLink = nextLink,
                //LastPageLink = lastLink
            };

            HttpContext.Current.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

            return results;
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return BadRequest();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        protected ResponseMessageResult Forbidden()
        {
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Forbidden));
        }

        //protected BadRequestObjectResult BadRequest(ModelStateDictionary ModelState, string key, string error)
        //{
        //    ModelState.AddModelError(key, error);
        //    return BadRequest(ModelState);
        //}
    }

    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params Roles[] roles) : base()
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }

    public class PagingOptions
    {
        public PagingOptions()
        {
            PageIndex = 0;
            PageSize = 10;
            OrderBy = null;
            OrderByAscending = true;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByAscending { get; set; }
        public bool IncludeEntities { get; set; } = false;
    }

    public class BadRequestErrors : IHttpActionResult
    {
        private List<string> messages;
        private HttpRequestMessage request;

        public BadRequestErrors(List<string> messages, HttpRequestMessage request)
        {
            this.messages = messages;
            this.request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = request.CreateResponse(HttpStatusCode.BadRequest, messages);
            return Task.FromResult(response);
        }
    }
}
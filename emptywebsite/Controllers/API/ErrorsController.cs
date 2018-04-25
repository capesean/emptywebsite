using System;
using System.Data.Entity;
using System.Web.Http;
using WEB.Models;
using System.Threading.Tasks;

namespace WEB.Controllers
{
    [AuthorizeRoles(Roles.Administrator), RoutePrefix("api/errors")]
    public class ErrorsController : BaseApiController
    {
        [HttpGet, Route("{id:Guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var error = await db.Errors
                .Include(e => e.Exception)
                .SingleOrDefaultAsync(e => e.Id == id);

            if (error == null)
                return NotFound();

            return Ok(ModelFactory.Create(error));
        }
    }
}
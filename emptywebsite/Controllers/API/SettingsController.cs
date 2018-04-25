using System.Web.Http;
using WEB.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace WEB.Controllers
{
    [Authorize, RoutePrefix("api/settings")]
    public class SettingsController : BaseApiController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(ModelFactory.Create(Settings));
        }

        [AuthorizeRoles(Roles.Administrator), HttpPost, Route("")]
        public async Task<IHttpActionResult> Post([FromBody] SettingsDTO settingsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var settings = await db.Settings.SingleOrDefaultAsync();

            db.Entry(settings).State = EntityState.Modified;

            ModelFactory.Hydrate(settings, settingsDTO);

            db.SaveChanges();

            return Ok(ModelFactory.Create(new Settings(db)));
        }
    }
}
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bidding_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        [Route("/Error")]
     /// Error Controller
        public async Task Error()
        {

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            await HttpContext.Response.WriteAsync(" Page: Home.");


        }
    }
}

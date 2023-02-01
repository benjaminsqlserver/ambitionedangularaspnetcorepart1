using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catstagram.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract  class ApiController : ControllerBase
    {
    }
}

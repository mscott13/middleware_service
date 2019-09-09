using System.Web.Http;

namespace middleware_service
{
    class PortController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(Constants.port);
        }
    }
}

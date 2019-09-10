using System.Net.Http;
using System.Web.Http;

namespace middleware_service
{
    public class PortController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Current()
        {
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, Constants.PORT);
        }
    }
}

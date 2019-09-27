using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers
{
    public class ObjectController : ApiController
    {
        IEnumerable <Models._Object_> GetObjects()
        {
         
        }

        public int Get()
        {
            return 1;
        }
    }
}

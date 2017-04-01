using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VikasIoTController.Controllers
{
    public class ValueController : ApiController
    {
        D2CEventRepository objRepo = new D2CEventRepository();
        // GET api/values
        public IEnumerable<D2CEvent> Get()
        {
            return objRepo.GetData();
        }
    }
}

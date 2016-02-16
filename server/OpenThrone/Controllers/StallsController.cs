using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using OpenThrone.Models;

namespace OpenThrone.Controllers
{
    public class StallsController : ApiController
    {
        public Dictionary<string, string> Get()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "stall1", HttpContext.Current.Cache["stall1"].ToString() },
                { "stall2", HttpContext.Current.Cache["stall2"].ToString() },
                { "stall3", HttpContext.Current.Cache["stall3"].ToString() }
            };
            return dictionary;
        }

        public void Post(int id, Status updateStatus)
        {
            HttpContext.Current.Cache["stall" + id] = updateStatus.Occupied;
        }
    }
}

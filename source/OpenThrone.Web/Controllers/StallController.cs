using System.Collections.Generic;
using System.Web.Http;
using OpenThrone.Web.Models;

namespace OpenThrone.Web.Controllers
{
    public class StallController : ApiController
    {
        // GET api/stall
        public IEnumerable<Stall> Get()
        {
            return StallCache.AllStalls();
        }

        // GET api/stall/5
        public Stall Get(int id)
        {
            return StallCache.Stall(id);
        }

        // PUT api/stall/5
        public void Put(int id, [FromBody]Stall stall)
        {
            stall.Id = id;
            StallCache.Update(id, stall);
        }
    }
}

using System.Collections.Generic;
using System.Web.Http;
using log4net;
using Microsoft.AspNet.SignalR;
using OpenThrone.Web.Hubs;
using OpenThrone.Web.Models;

namespace OpenThrone.Web.Controllers
{
    [System.Web.Http.Authorize]
    public class StallController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (StallController));

        // GET api/stall
        [AllowAnonymous]
        public IEnumerable<Stall> Get()
        {
            return StallCache.AllStalls();
        }

        // PUT api/stall/5
        [TokenAuthenticated]
        public void Put(int id, [FromBody] UpdateStallStatus updateStallStatus)
        {
            Log.DebugFormat("Controller received request to change stall {0} status to {1}", id,
                updateStallStatus.Available ? "available" : "occupied");
            var stall = StallCache.GetStall(id);
            stall.Available = updateStallStatus.Available;
            StallCache.UpdateStall(stall);
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients.All.stallAvailabilityChange(stall);
        }
    }

    public class UpdateStallStatus
    {
        public bool Available { get; set; }
    }
}
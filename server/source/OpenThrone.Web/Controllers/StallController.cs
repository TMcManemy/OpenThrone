using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using OpenThrone.Web.Hubs;
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

        // PUT api/stall/5
        public void Put(int id, [FromBody] UpdateStallStatus updateStallStatus)
        {
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

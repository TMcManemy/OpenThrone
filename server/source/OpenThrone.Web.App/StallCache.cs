using System.Collections.Generic;
using System.Linq;
using OpenThrone.Web.Models;

namespace OpenThrone.Web
{
    public class StallCache
    {
        private static Dictionary<int, Stall> _cache;

        public static void Initialize()
        {
            var initialState = new[]
            {
                new Stall {Id = 1, Available = true, Location = "asynchrony"},
                new Stall {Id = 2, Available = true, Location = "asynchrony"},
                new Stall {Id = 3, Available = true, Location = "asynchrony"},
                new Stall {Id = 4, Available = true, Location = "wwt"},
            };
            _cache = initialState.ToDictionary(s => s.Id);
        }

        public static IEnumerable<Stall> AllStalls()
        {
            return _cache.Values;
        }

        public static void UpdateStall(Stall stall)
        {
            _cache[stall.Id] = stall;
        }

        public static Stall GetStall(int id)
        {
            return _cache[id];
        }
    }
}
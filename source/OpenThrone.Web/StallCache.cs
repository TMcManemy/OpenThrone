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
                    new Stall {Id = 1, Available = true},
                    new Stall {Id = 2, Available = true},
                    new Stall {Id = 3, Available = true},
                };
            _cache = initialState.ToDictionary(s => s.Id);
        }

        public static IEnumerable<Stall> AllStalls()
        {
            return _cache.Values;
        }

        public static Stall Stall(int id)
        {
            return _cache[id];
        }

        public static void Update(int id, Stall stall)
        {
            _cache[id] = stall;
        }
    }
}
using NUnit.Framework;
using OpenThrone.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenThrone.Web
{
    [TestFixture]
    public class StallCacheTests
    {
        [SetUp]
        public void Setup()
        {
            // Initialize the cache before each test
            StallCache.Initialize();
        }

        [Test]
        public void Initialize_CreatesThreeStalls()
        {
            // Arrange & Act
            var allStalls = StallCache.AllStalls().ToList();

            // Assert
            Assert.AreEqual(3, allStalls.Count, "Should create exactly 3 stalls");
        }

        [Test]
        public void Initialize_SetsCorrectStallIds()
        {
            // Arrange & Act
            var allStalls = StallCache.AllStalls().OrderBy(s => s.Id).ToList();

            // Assert
            Assert.AreEqual(1, allStalls[0].Id, "First stall should have ID 1");
            Assert.AreEqual(2, allStalls[1].Id, "Second stall should have ID 2");
            Assert.AreEqual(3, allStalls[2].Id, "Third stall should have ID 3");
        }

        [Test]
        public void Initialize_SetsAllStallsAsAvailable()
        {
            // Arrange & Act
            var allStalls = StallCache.AllStalls();

            // Assert
            Assert.IsTrue(allStalls.All(s => s.Available), "All stalls should be available after initialization");
        }

        [Test]
        public void Initialize_SetsCorrectLocation()
        {
            // Arrange & Act
            var allStalls = StallCache.AllStalls();

            // Assert
            Assert.IsTrue(allStalls.All(s => s.Location == "asynchrony"), "All stalls should have location 'asynchrony'");
        }

        [Test]
        public void AllStalls_ReturnsAllStalls()
        {
            // Arrange & Act
            var allStalls = StallCache.AllStalls();

            // Assert
            Assert.IsNotNull(allStalls, "AllStalls should not return null");
            Assert.AreEqual(3, allStalls.Count(), "AllStalls should return all 3 stalls");
        }

        [Test]
        public void AllStalls_ReturnsEnumerable()
        {
            // Arrange & Act
            var allStalls = StallCache.AllStalls();

            // Assert
            Assert.IsInstanceOf<IEnumerable<Stall>>(allStalls, "AllStalls should return an IEnumerable<Stall>");
        }

        [Test]
        public void GetStall_ReturnsCorrectStallById()
        {
            // Arrange & Act
            var stall = StallCache.GetStall(1);

            // Assert
            Assert.IsNotNull(stall, "GetStall should return a stall");
            Assert.AreEqual(1, stall.Id, "Should return stall with ID 1");
            Assert.AreEqual("asynchrony", stall.Location, "Should have correct location");
            Assert.IsTrue(stall.Available, "Should be available");
        }

        [Test]
        public void GetStall_ReturnsCorrectStallForEachId()
        {
            // Arrange & Act & Assert
            for (int i = 1; i <= 3; i++)
            {
                var stall = StallCache.GetStall(i);
                Assert.AreEqual(i, stall.Id, $"GetStall({i}) should return stall with ID {i}");
            }
        }

        [Test]
        public void GetStall_ThrowsKeyNotFoundExceptionForInvalidId()
        {
            // Arrange & Act & Assert
            Assert.Throws<KeyNotFoundException>(() => StallCache.GetStall(999), 
                "GetStall should throw KeyNotFoundException for invalid ID");
        }

        [Test]
        public void UpdateStall_UpdatesAvailability()
        {
            // Arrange
            var stall = StallCache.GetStall(1);
            stall.Available = false;

            // Act
            StallCache.UpdateStall(stall);

            // Assert
            var updatedStall = StallCache.GetStall(1);
            Assert.IsFalse(updatedStall.Available, "Stall availability should be updated to false");
        }

        [Test]
        public void UpdateStall_UpdatesLocation()
        {
            // Arrange
            var stall = StallCache.GetStall(2);
            stall.Location = "new-location";

            // Act
            StallCache.UpdateStall(stall);

            // Assert
            var updatedStall = StallCache.GetStall(2);
            Assert.AreEqual("new-location", updatedStall.Location, "Stall location should be updated");
        }

        [Test]
        public void UpdateStall_PersistsChangesAcrossMultipleCalls()
        {
            // Arrange
            var stall = StallCache.GetStall(3);
            stall.Available = false;
            stall.Location = "updated-location";

            // Act
            StallCache.UpdateStall(stall);
            var retrievedStall = StallCache.GetStall(3);

            // Assert
            Assert.IsFalse(retrievedStall.Available, "Availability change should persist");
            Assert.AreEqual("updated-location", retrievedStall.Location, "Location change should persist");
        }

        [Test]
        public void UpdateStall_CanUpdateMultipleStalls()
        {
            // Arrange
            var stall1 = StallCache.GetStall(1);
            var stall2 = StallCache.GetStall(2);
            stall1.Available = false;
            stall2.Available = false;

            // Act
            StallCache.UpdateStall(stall1);
            StallCache.UpdateStall(stall2);

            // Assert
            Assert.IsFalse(StallCache.GetStall(1).Available, "Stall 1 should be unavailable");
            Assert.IsFalse(StallCache.GetStall(2).Available, "Stall 2 should be unavailable");
            Assert.IsTrue(StallCache.GetStall(3).Available, "Stall 3 should still be available");
        }

        [Test]
        public void UpdateStall_CanToggleAvailabilityMultipleTimes()
        {
            // Arrange
            var stall = StallCache.GetStall(1);

            // Act & Assert
            stall.Available = false;
            StallCache.UpdateStall(stall);
            Assert.IsFalse(StallCache.GetStall(1).Available, "Should be unavailable after first update");

            stall.Available = true;
            StallCache.UpdateStall(stall);
            Assert.IsTrue(StallCache.GetStall(1).Available, "Should be available after second update");

            stall.Available = false;
            StallCache.UpdateStall(stall);
            Assert.IsFalse(StallCache.GetStall(1).Available, "Should be unavailable after third update");
        }
    }
}

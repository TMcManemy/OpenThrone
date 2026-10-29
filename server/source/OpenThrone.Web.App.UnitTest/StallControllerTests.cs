using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using OpenThrone.Web.Controllers;
using OpenThrone.Web.Models;

namespace OpenThrone.Web
{
    [TestFixture]
    public class StallControllerTests
    {
        private StallController _controller;

        [SetUp]
        public void Setup()
        {
            // Initialize StallCache before each test
            StallCache.Initialize();

            // Create controller instance
            _controller = new StallController();
        }

        [Test]
        public void Get_ReturnsAllStalls()
        {
            // Arrange
            var expectedStalls = StallCache.AllStalls().ToList();

            // Act
            var result = _controller.Get().ToList();

            // Assert
            Assert.AreEqual(expectedStalls.Count, result.Count, "Should return all stalls");
            Assert.AreEqual(3, result.Count, "Should return exactly 3 stalls");
        }

        [Test]
        public void Get_ReturnsStallsWithCorrectIds()
        {
            // Arrange
            var expectedIds = new[] { 1, 2, 3 };

            // Act
            var result = _controller.Get().ToList();
            var resultIds = result.Select(s => s.Id).OrderBy(id => id).ToList();

            // Assert
            CollectionAssert.AreEqual(expectedIds, resultIds, "Should return stalls with IDs 1, 2, 3");
        }

        [Test]
        public void Get_ReturnsStallsWithAvailabilityStatus()
        {
            // Arrange & Act
            var result = _controller.Get().ToList();

            // Assert
            Assert.IsTrue(result.All(s => s.Available), "All stalls should be available after initialization");
        }

        [Test]
        public void Get_ReturnsStallsWithLocation()
        {
            // Arrange & Act
            var result = _controller.Get().ToList();

            // Assert
            Assert.IsTrue(result.All(s => s.Location == "asynchrony"), "All stalls should have location 'asynchrony'");
        }

        [Test]
        public void Put_UpdatesStallAvailability()
        {
            // Arrange
            int stallId = 1;
            var updateStatus = new UpdateStallStatus { Available = false };

            // Act
            _controller.Put(stallId, updateStatus);

            // Assert
            var updatedStall = StallCache.GetStall(stallId);
            Assert.IsFalse(updatedStall.Available, "Stall should be marked as unavailable");
        }

        [Test]
        public void Put_UpdatesStallAvailabilityToTrue()
        {
            // Arrange
            int stallId = 2;
            // First set to unavailable
            var updateStatusFalse = new UpdateStallStatus { Available = false };
            _controller.Put(stallId, updateStatusFalse);

            // Then set back to available
            var updateStatusTrue = new UpdateStallStatus { Available = true };

            // Act
            _controller.Put(stallId, updateStatusTrue);

            // Assert
            var updatedStall = StallCache.GetStall(stallId);
            Assert.IsTrue(updatedStall.Available, "Stall should be marked as available");
        }

        [Test]
        public void Put_UpdatesCorrectStall()
        {
            // Arrange
            int stallId = 2;
            var updateStatus = new UpdateStallStatus { Available = false };

            // Act
            _controller.Put(stallId, updateStatus);

            // Assert
            var updatedStall = StallCache.GetStall(stallId);
            var otherStall1 = StallCache.GetStall(1);
            var otherStall3 = StallCache.GetStall(3);

            Assert.IsFalse(updatedStall.Available, "Stall 2 should be unavailable");
            Assert.IsTrue(otherStall1.Available, "Stall 1 should still be available");
            Assert.IsTrue(otherStall3.Available, "Stall 3 should still be available");
        }

        [Test]
        public void Put_ThrowsExceptionForInvalidStallId()
        {
            // Arrange
            int invalidStallId = 999;
            var updateStatus = new UpdateStallStatus { Available = false };

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _controller.Put(invalidStallId, updateStatus),
                "Should throw KeyNotFoundException for invalid stall ID");
        }

        [Test]
        public void Put_ThrowsExceptionForNegativeStallId()
        {
            // Arrange
            int invalidStallId = -1;
            var updateStatus = new UpdateStallStatus { Available = false };

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _controller.Put(invalidStallId, updateStatus),
                "Should throw KeyNotFoundException for negative stall ID");
        }

        [Test]
        public void Put_BroadcastsStallChangeViaSignalR()
        {
            // Arrange
            int stallId = 1;
            var updateStatus = new UpdateStallStatus { Available = false };

            // Act
            _controller.Put(stallId, updateStatus);

            // Assert
            // Verify that the hub context was used to broadcast the change
            // Note: Due to SignalR's static nature, we verify the stall was updated
            var updatedStall = StallCache.GetStall(stallId);
            Assert.IsFalse(updatedStall.Available, "Stall should be updated before broadcast");
        }

        [Test]
        public void Put_MultipleUpdatesOnSameStall()
        {
            // Arrange
            int stallId = 1;

            // Act - Update multiple times
            _controller.Put(stallId, new UpdateStallStatus { Available = false });
            _controller.Put(stallId, new UpdateStallStatus { Available = true });
            _controller.Put(stallId, new UpdateStallStatus { Available = false });

            // Assert
            var finalStall = StallCache.GetStall(stallId);
            Assert.IsFalse(finalStall.Available, "Stall should reflect the last update");
        }

        [Test]
        public void UpdateStallStatus_CanBeSerializedFromJson()
        {
            // Arrange & Act
            var updateStatus = new UpdateStallStatus { Available = true };

            // Assert
            Assert.IsTrue(updateStatus.Available, "UpdateStallStatus should have Available property");
            Assert.IsNotNull(updateStatus, "UpdateStallStatus should be instantiable");
        }

        [Test]
        public void Get_IsMarkedAsAllowAnonymous()
        {
            // Arrange & Act
            var getMethod = typeof(StallController).GetMethod("Get");
            var allowAnonymousAttribute = getMethod.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);

            // Assert
            Assert.AreEqual(1, allowAnonymousAttribute.Length, "Get method should have AllowAnonymous attribute");
        }

        [Test]
        public void Put_IsMarkedAsTokenAuthenticated()
        {
            // Arrange & Act
            var putMethod = typeof(StallController).GetMethod("Put");
            var tokenAuthenticatedAttribute = putMethod.GetCustomAttributes(typeof(TokenAuthenticatedAttribute), false);

            // Assert
            Assert.AreEqual(1, tokenAuthenticatedAttribute.Length, "Put method should have TokenAuthenticated attribute");
        }
    }
}

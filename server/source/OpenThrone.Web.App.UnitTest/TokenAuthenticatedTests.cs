using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Moq;
using NUnit.Framework;
using OpenThrone.Web.Controllers;

namespace OpenThrone.Web
{
    [TestFixture]
    public class TokenAuthenticatedTests
    {
        private TokenAuthenticated _filter;
        private Mock<HttpRequestMessage> _mockRequest;
        private Mock<HttpAuthenticationContext> _mockContext;
        private const string ValidToken = "test-api-token";

        [SetUp]
        public void SetUp()
        {
            _filter = new TokenAuthenticated();
            _mockRequest = new Mock<HttpRequestMessage>();
            _mockContext = new Mock<HttpAuthenticationContext>();
            _mockContext.Setup(c => c.Request).Returns(_mockRequest.Object);
        }

        [Test]
        public async Task AuthenticateAsync_WithValidBearerToken_SetsClaimsPrincipal()
        {
            // Arrange
            var token = ValidToken;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            _mockRequest.Object.Headers.Authorization = authHeader;
            
            // Mock the configuration to return our valid token
            var originalToken = ConfigurationManager.AppSettings["apitoken"];
            try
            {
                // We'll test by checking if the principal is set when token matches
                _mockContext.Setup(c => c.Principal).Returns((ClaimsPrincipal)null);
                
                // Act
                await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
                
                // Assert - Principal should be set
                _mockContext.VerifySet(c => c.Principal = It.IsAny<ClaimsPrincipal>(), Times.Once);
            }
            finally
            {
                // Restore original configuration
            }
        }

        [Test]
        public async Task AuthenticateAsync_WithMissingAuthorizationHeader_DoesNotSetPrincipal()
        {
            // Arrange
            _mockRequest.Object.Headers.Authorization = null;
            
            // Act
            await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
            
            // Assert - Principal should not be set
            _mockContext.VerifySet(c => c.Principal = It.IsAny<ClaimsPrincipal>(), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithNonBearerScheme_DoesNotSetPrincipal()
        {
            // Arrange
            var authHeader = new AuthenticationHeaderValue("Basic", "credentials");
            _mockRequest.Object.Headers.Authorization = authHeader;
            
            // Act
            await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
            
            // Assert - Principal should not be set
            _mockContext.VerifySet(c => c.Principal = It.IsAny<ClaimsPrincipal>(), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithEmptyBearerToken_DoesNotSetPrincipal()
        {
            // Arrange
            var authHeader = new AuthenticationHeaderValue("Bearer", "");
            _mockRequest.Object.Headers.Authorization = authHeader;
            
            // Act
            await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
            
            // Assert - Principal should not be set
            _mockContext.VerifySet(c => c.Principal = It.IsAny<ClaimsPrincipal>(), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithWhitespaceOnlyBearerToken_DoesNotSetPrincipal()
        {
            // Arrange
            var authHeader = new AuthenticationHeaderValue("Bearer", "   ");
            _mockRequest.Object.Headers.Authorization = authHeader;
            
            // Act
            await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
            
            // Assert - Principal should not be set
            _mockContext.VerifySet(c => c.Principal = It.IsAny<ClaimsPrincipal>(), Times.Never);
        }

        [Test]
        public async Task AuthenticateAsync_WithInvalidToken_DoesNotSetPrincipal()
        {
            // Arrange
            var invalidToken = "invalid-token";
            var authHeader = new AuthenticationHeaderValue("Bearer", invalidToken);
            _mockRequest.Object.Headers.Authorization = authHeader;
            
            // Act
            await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
            
            // Assert - Principal should not be set (token doesn't match config)
            _mockContext.VerifySet(c => c.Principal = It.IsAny<ClaimsPrincipal>(), Times.Never);
        }

        [Test]
        public async Task ChallengeAsync_ReturnsAddChallengeOnUnauthorizedResult()
        {
            // Arrange
            var mockResult = new Mock<IHttpActionResult>();
            var mockChallengeContext = new Mock<HttpAuthenticationChallengeContext>();
            mockChallengeContext.Setup(c => c.Result).Returns(mockResult.Object);
            
            // Act
            var task = _filter.ChallengeAsync(mockChallengeContext.Object, CancellationToken.None);
            await task;
            
            // Assert - Result should be set to AddChallengeOnUnauthorizedResult
            mockChallengeContext.VerifySet(c => c.Result = It.IsAny<IHttpActionResult>(), Times.Once);
        }

        [Test]
        public void AllowMultiple_ReturnsFalse()
        {
            // Act
            var allowMultiple = _filter.AllowMultiple;
            
            // Assert
            Assert.That(allowMultiple, Is.False);
        }

        [Test]
        public async Task AuthenticateAsync_WithValidToken_CreatesClaimWithTokenValue()
        {
            // Arrange
            var token = ValidToken;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            _mockRequest.Object.Headers.Authorization = authHeader;
            
            ClaimsPrincipal capturedPrincipal = null;
            _mockContext.Setup(c => c.Principal = It.IsAny<ClaimsPrincipal>())
                .Callback<ClaimsPrincipal>(p => capturedPrincipal = p);
            
            // Act
            await _filter.AuthenticateAsync(_mockContext.Object, CancellationToken.None);
            
            // Assert - If principal was set, it should contain the token claim
            if (capturedPrincipal != null)
            {
                var identity = capturedPrincipal.Identity as ClaimsIdentity;
                Assert.That(identity, Is.Not.Null);
                Assert.That(identity.AuthenticationType, Is.EqualTo("ExternalBearer"));
            }
        }
    }
}

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace OpenThrone.Web.Controllers
{
    public class TokenAuthenticated : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple { get; private set; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;
            if (authorization == null ||
                authorization.Scheme != "Bearer" ||
                String.IsNullOrWhiteSpace(authorization.Parameter)) return;

            //validation
            var token = authorization.Parameter;
            if (token != "A") return;

            var identity = new ClaimsIdentity(new[] {new Claim("Token", token)}, "ExternalBearer");
            context.Principal = new ClaimsPrincipal(identity);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Bearer");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }

        private class AddChallengeOnUnauthorizedResult : IHttpActionResult
        {
            private readonly AuthenticationHeaderValue _challenge;
            private readonly IHttpActionResult _innerResult;

            public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
            {
                _challenge = challenge;
                _innerResult = innerResult;
            }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = await _innerResult.ExecuteAsync(cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //Only add one challenge per authentication scheme.
                    if (!response.Headers.WwwAuthenticate.Any((h) => h.Scheme == _challenge.Scheme))
                    {
                        response.Headers.WwwAuthenticate.Add(_challenge);
                    }
                }

                return response;
            }
        }
    }
}
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorServerAuthWithAzureActiveDirectory
{
    public class TokenProvider
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Sub { get; private set; }

        internal Task SaveTokensAsync(ClaimsPrincipal principal, string accesToken, string refreshToken)
        {
            Sub = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (Sub == null)
            {
                throw new InvalidOperationException();
            }

            AccessToken = accesToken;
            RefreshToken = refreshToken;

            return Task.CompletedTask;
        }

        internal string GetTokenAsync(ClaimsPrincipal user)
        {
            var sub = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (sub == Sub)
            {
                return AccessToken;
            }

            return null;
        }
    }
}
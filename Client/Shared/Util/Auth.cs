using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using RecipEase.Shared;

namespace RecipEase.Client.Shared.Util
{
    public static class Auth
    {
        public const string NoAuthHttpClientName = "ServerAPI.NoAuthenticationClient";

        /// <summary>
        /// Returns the user id of the currently logged in user, or null if no user is logged in. 
        /// </summary>
        public static async Task<string> GetAuthenticatedUser(Task<AuthenticationState> authStateTask)
        {
            var authState = await authStateTask;
            var user = authState.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated) return null;

            var userId = user.FindFirst(CustomClaimsTypes.UserId)?.Value;
            return userId;
        }

        public static HttpClient GetNoAuthHttpClient(IHttpClientFactory clientFactory)
        {
            return clientFactory.CreateClient(NoAuthHttpClientName);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductManagement.API.Filters
{
    public class AuthHeaderFilter : IAuthorizationFilter
    {
        private const string HeaderName = "X-Auth-Token";
        private const string StaticValue = "static_secret"; 

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.Equals(context.HttpContext.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var value) || value != StaticValue)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}

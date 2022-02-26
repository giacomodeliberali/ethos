using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ethos.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Ethos.Web
{
    public class AuthorizeAttribute : ActionFilterAttribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Gets or sets a comma delimited list of roles that are allowed to access the resource.
        /// </summary>
        public string? Roles { get; set; }

        public AuthorizeAttribute()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Guard.Against.Null(context, nameof(context));

            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            var policyBuilder = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser();

            if (!string.IsNullOrEmpty(Roles))
            {
                policyBuilder.RequireRole(Roles.Split(","));
            }

            var policy = policyBuilder.Build();

            var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context.HttpContext);
            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context.HttpContext, context);

            if (authorizeResult.Challenged)
            {
                // Return custom 401 result
                context.Result = new JsonResult(new ExceptionDto()
                {
                    Message = "Authorization failed.",
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDayApi.Helpers
{
    // Custom attribute to enforce role-based access via session
    // Usage: [RoleAuthorize("Organiser")] or [RoleAuthorize("Organiser", "Participant")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32(SessionKeys.UserId);
            var userRole = session.GetString(SessionKeys.UserRole);

            // Not logged in
            if (userId == null || string.IsNullOrEmpty(userRole))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "You must be logged in to access this resource."
                });
                return;
            }

            // Role check — if allowed roles specified, enforce them
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(userRole))
            {
                context.Result = new ObjectResult(new
                {
                    message = $"Access denied. This endpoint requires one of the following roles: {string.Join(", ", _allowedRoles)}"
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
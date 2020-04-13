using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cine.Modules.Identity.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Cine.Modules.Identity.Api.Middlewares
{
    internal sealed class JwtAuthorizationMiddleware : IMiddleware
    {
        private readonly ITokensService _service;

        public JwtAuthorizationMiddleware(ITokensService service)
        {
            _service = service;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers["Authorization"];

            if (token == StringValues.Empty)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            token = token.Single().Split(' ').Last();
            var isValid = _service.Validate(token);

            if (isValid)
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
    }
}

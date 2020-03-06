using THD.Core.Api.Business.Interface;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace THD.Core.Api.Private
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(
            RequestDelegate next
            )
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISystemService _ISystemService)
        {
            var jsonString = "{\"message\": \"Invalid Token\"}";
            context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();

            string authHeader = context.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
                //Extract credentials                
                string LicenseKey = authHeader.Substring("Bearer ".Length).Trim();

                bool IsTokenKey = await _ISystemService.LicenseKeyValidate(LicenseKey);
                if (IsTokenKey)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                    return;
                }

            }
            else
            {
                // no authorization header
                context.Response.StatusCode = 401; //Unauthorized.
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                return;
            }
        }

    }
}

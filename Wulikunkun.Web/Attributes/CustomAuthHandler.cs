using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Wulikunkun.Web.Models;

namespace Web.Attributes
{
    public class CustomAuthHandler : IAuthenticationHandler
    {
        HttpContext _context;
        AuthenticationScheme _authenticationScheme;
        WangKunDbContext _dbContext;
        public const string SchemeName = "CustomAuth";

        /* 经过测试这里的数据库上下文可以成功注入 */
        public CustomAuthHandler(WangKunDbContext wangKunDbContext)
        {
            _dbContext = wangKunDbContext;
        }
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            if (_context.Session.IsAvailable)
            {
                string userName = _context.Session.GetString("username");
                User user = _dbContext.Users.FirstOrDefault(item => item.Name == userName);
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserName",user.Name),
                    new Claim("IsActive",user.IsActive.ToString())
                }, SchemeName);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                AuthenticationTicket authenticationTicket = new AuthenticationTicket(claimsPrincipal, _authenticationScheme.Name);
                return Task.FromResult(AuthenticateResult.Success(authenticationTicket));

            }
            else
                return Task.FromResult(AuthenticateResult.Fail("未登陆"));
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _authenticationScheme = scheme;
            _context = context;
            return Task.CompletedTask;
        }
    }
}

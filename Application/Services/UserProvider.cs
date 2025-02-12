using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services
{
    public class UserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor _context;

        public UserProvider(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Guid GetUserId()
        {
            string id = _context.HttpContext.User.Claims
                       .FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(id);
        }

        public string GetName()
        {
            return _context.HttpContext.User.Claims
                       .FirstOrDefault(i => i.Type == ClaimTypes.Name).Value;
        }

        public string GetEmail()
        {
            return _context.HttpContext.User.Claims
                       .FirstOrDefault(i => i.Type == ClaimTypes.Email).Value;
        }
    }
}

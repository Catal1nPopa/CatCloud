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
            try
            {
                string id = _context.HttpContext.User.Claims
                       .FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(id);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException();
            }
        }

        public string GetName()
        {
            try
            {
            return _context.HttpContext.User.Claims
                       .FirstOrDefault(i => i.Type == ClaimTypes.Name).Value;
            }
            catch(Exception ex)
            {
                throw new UnauthorizedAccessException();
            }
        }

        public string GetEmail()
        {
            try
            {
            return _context.HttpContext.User.Claims
                       .FirstOrDefault(i => i.Type == ClaimTypes.Email).Value;
            }
            catch(Exception)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}

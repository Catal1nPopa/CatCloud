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
                var claim = _context?.HttpContext?.User?.Claims
                .FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);

                if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                {
                    throw new UnauthorizedAccessException("User ID claim is missing or session has expired.");
                }

                return Guid.Parse(claim.Value);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException();
            }
        }

        public string GetName()
        {
            var claim = _context?.HttpContext?.User?.Claims
                .FirstOrDefault(i => i.Type == ClaimTypes.Name);

            if (claim == null)
            {
                throw new UnauthorizedAccessException("Name claim is missing or session has expired.");
            }

            return claim.Value;
        }

        public string GetEmail()
        {
            var claim = _context?.HttpContext?.User?.Claims
                .FirstOrDefault(i => i.Type == ClaimTypes.Email);

            if (claim == null)
            {
                throw new UnauthorizedAccessException("Email claim is missing or session has expired.");
            }

            return claim.Value;
        }
    }
}

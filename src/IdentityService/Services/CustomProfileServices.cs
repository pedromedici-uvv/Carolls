using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService.Services
{
    public class CustomProfileServices : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManage;
        public CustomProfileServices(UserManager<ApplicationUser> userManager)
        {
            _userManage = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
          var user = await _userManage.GetUserAsync(context.Subject);
          var existingClaims = await _userManage.GetClaimsAsync(user);


            var claims = new List<Claim>
            {
                new Claim("username", user.UserName),
            };

            context.IssuedClaims.AddRange(claims);
            context.IssuedClaims.Add(existingClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name));
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}

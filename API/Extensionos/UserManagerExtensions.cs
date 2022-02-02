using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensionos;

public static class UserManagerExtensions
{
    public static async Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(this UserManager<AppUser> input, ClaimsPrincipal user)
    {
        string email = user.FindFirstValue(ClaimTypes.Email);
        return await input.Users.Include(user => user.Address).SingleOrDefaultAsync(user => user.Email == email);
    }

    public static async Task<AppUser> FindByEmailFromClaimsPrinciple(this UserManager<AppUser> input, ClaimsPrincipal user)
    {
        string email = user.FindFirstValue(ClaimTypes.Email);
        return await input.Users.SingleOrDefaultAsync(user => user.Email == email);
    }
}

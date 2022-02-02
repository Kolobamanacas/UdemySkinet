using API.Dtos;
using API.Errors;
using API.Extensionos;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> userManager;
    private readonly SignInManager<AppUser> signInManager;
    private readonly ITokenService tokenService;
    private readonly IMapper mapper;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.tokenService = tokenService;
        this.mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        AppUser user = await userManager.FindByEmailFromClaimsPrinciple(User);

        return new UserDto
        {
            Email = user.Email,
            Token = tokenService.CreateToken(user),
            DisplayName = user.DisplayName
        };
    }

    [HttpGet("emailexists")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        return await userManager.FindByEmailAsync(email) != null;
    }

    [HttpGet("address")]
    [Authorize]
    public async Task<ActionResult<AddressDto>> GetUserAddress()
    {
        AppUser user = await userManager.FindUserByClaimsPrincipalWithAddressAsync(User);
        return mapper.Map<Address, AddressDto>(user.Address);
    }

    [HttpPut("address")]
    [Authorize]
    public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
    {
        AppUser user = await userManager.FindUserByClaimsPrincipalWithAddressAsync(User);
        user.Address = mapper.Map<AddressDto, Address>(address);
        IdentityResult result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest("Problem updating the user");
        }

        return Ok(mapper.Map<Address, AddressDto>(user.Address));
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        AppUser user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return Unauthorized(new ApiResponse(401));
        }

        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
        {
            return Unauthorized(new ApiResponse(401));
        }

        return new UserDto
        {
            Email = user.Email,
            Token = tokenService.CreateToken(user),
            DisplayName = user.DisplayName
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        AppUser user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        IdentityResult result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new ApiResponse(400));
        }

        return new UserDto
        {
            Email = user.Email,
            Token = tokenService.CreateToken(user),
            DisplayName = user.DisplayName
        };
    }
}

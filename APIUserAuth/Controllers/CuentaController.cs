using APIUserAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIUserAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public CuentaController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        private UserToken BuildToken(UserInfo userInfo, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                // Los Claims son informaciones fiables que viajan dentro del token
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("mi valor", "lo que quiera"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        [HttpPost("crear")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo userInfo)
        {
            var user = new ApplicationUser { UserName = userInfo.Email, Email = userInfo.Email };
            var result = await _userManager.CreateAsync(user, userInfo.Password);
            if (result.Succeeded)
            {
                return BuildToken(userInfo, new List<string>());
            }
            else
            {
                return BadRequest("Nombre de usuario o password incorrectos");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> LoginUser([FromBody] UserInfo userInfo)
        {
            var user = new ApplicationUser { UserName = userInfo.Email, Email = userInfo.Email };
            var result = await _userManager.CreateAsync(user, userInfo.Password);
            if (result.Succeeded)
            {
                var usuario = await _userManager.FindByEmailAsync(userInfo.Email);
                var roles = await _userManager.GetRolesAsync(usuario);
                return BuildToken(userInfo, roles);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Datos erróneos.");
                return BadRequest(ModelState);
            }
        }
    }
}

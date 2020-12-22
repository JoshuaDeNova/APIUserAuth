using APIUserAuth.Context;
using APIUserAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIUserAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsuariosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("AsignarRol")]
        public async Task<ActionResult> AsignarRolUsuario(EditarRolDTO editarRolDTO)
        {
            var usuario = await _userManager.FindByIdAsync(editarRolDTO.UserId);
            if(usuario == null) { return NotFound(); }
            await _userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.RoleName));
            await _userManager.AddToRoleAsync(usuario, editarRolDTO.RoleName);
            return Ok();
        }

        [HttpPost("EliminarRol")]
        public async Task<ActionResult> RemoverRolUsuario(EditarRolDTO editarRolDTO)
        {
            var usuario = await _userManager.FindByIdAsync(editarRolDTO.UserId);
            if (usuario == null) { return NotFound(); }
            await _userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.RoleName));
            await _userManager.RemoveFromRoleAsync(usuario, editarRolDTO.RoleName);
            return Ok();
        }
    }
}

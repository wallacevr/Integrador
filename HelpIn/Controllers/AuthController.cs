using HelpIn; // Seu DbContext
using Microsoft.AspNetCore.Mvc;
using HelpIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
[Route("login")]
public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string tipoUsuario, string email, string senha)
    {
        if (string.IsNullOrEmpty(tipoUsuario) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            TempData["MensagemErro"] = "Preencha todos os campos.";
            return View();
        }

        if (tipoUsuario == "Voluntario")
        {
            var user = await _context.Voluntarios.FirstOrDefaultAsync(v => v.Email == email);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<Voluntario>();
                var resultado = passwordHasher.VerifyHashedPassword(user, user.Senha, senha);

                if (resultado == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, "Voluntário"),  // Corrigido o nome da Role
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    TempData["MensagemSucesso"] = $"Bem-vindo, {user.Nome}!";
                    return RedirectToAction("painel-voluntario", "voluntario");
                }
            }
        }
        else if (tipoUsuario == "Ong")
        {
            var user = await _context.Ongs.FirstOrDefaultAsync(o => o.Email == email);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<Ong>();
                var resultado = passwordHasher.VerifyHashedPassword(user, user.Senha, senha);

                if (resultado == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, "Ong"),
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    TempData["MensagemSucesso"] = $"Bem-vindo, {user.Nome}!";
                    return RedirectToAction("PainelOng", "Ong");
                }
            }
        }

        TempData["MensagemErro"] = "E-mail ou senha inválidos.";
        return View();
    }
}



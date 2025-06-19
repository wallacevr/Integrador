using HelpIn; // onde está seu ApplicationDbContext
using Microsoft.AspNetCore.Mvc;
using HelpIn.Models; // caso você use uma pasta para as models (ajuste conforme seu projeto)
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
[Route("voluntario")]
public class VoluntarioController : Controller
{
    private readonly ApplicationDbContext _context;


    [HttpGet]
    [Route("cadastro-voluntario")]
    [AllowAnonymous]  // <-- Não exige login
    public IActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    [Route("cadastro-voluntario")]
    [AllowAnonymous]  // <-- Não exige login
    public async Task<IActionResult> Cadastro(Voluntario voluntario)
    {
        if (ModelState.IsValid)
        {
            // Hash da senha
            var passwordHasher = new PasswordHasher<Voluntario>();
            voluntario.Senha = passwordHasher.HashPassword(voluntario, voluntario.Senha);
            // Upload do currículo (opcional)
            if (voluntario.Curriculo != null && voluntario.Curriculo.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(voluntario.Curriculo.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await voluntario.Curriculo.CopyToAsync(stream);

                voluntario.CurriculoUrl = "/uploads/" + fileName;
            }
            var emailExiste = await _context.Voluntarios
                .AnyAsync(v => v.Email == voluntario.Email);

            if (emailExiste)
            {
                ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                return View(voluntario);
            }
            await _context.Voluntarios.AddAsync(voluntario);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Voluntário cadastrado com sucesso!";
            return RedirectToAction("Cadastro");
        }

        return View(voluntario);
    }
    public VoluntarioController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("painel-voluntario")]
   [Authorize(Roles = "Voluntário")]  // <-- Só acessa se estiver logado como ONG
    public IActionResult PainelVoluntario()
    {
        return View();
    }
    
}

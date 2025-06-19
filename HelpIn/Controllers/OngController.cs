using HelpIn; // onde está seu ApplicationDbContext
using Microsoft.AspNetCore.Mvc;
using HelpIn.Models; // caso você use uma pasta para as models (ajuste conforme seu projeto)
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
[Route("ong")]
public class OngController : Controller
{
    private readonly ApplicationDbContext _context;

    public OngController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [AllowAnonymous]  // <-- Não exige login
    public IActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]  // <-- Não exige login
    public async Task<IActionResult> Cadastro(Ong ong)
    {
        if (ModelState.IsValid)
        {
            // Upload da logo (opcional)
            if (ong.Logo != null && ong.Logo.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ong.Logo.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ong.Logo.CopyToAsync(stream);

                ong.LogoUrl = "/uploads/" + fileName;
            }
            var emailExiste = await _context.Voluntarios
                .AnyAsync(v => v.Email == ong.Email);

            if (emailExiste)
            {
                ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                return View(ong);
            }
            // ✅ Aqui é onde os dados são salvos no banco:
            await _context.Ongs.AddAsync(ong);           // ← adiciona ao contexto
            await _context.SaveChangesAsync();           // ← grava no banco

            TempData["Mensagem"] = "ONG cadastrada com sucesso!";
            return RedirectToAction("Cadastro");
        }

        // Se a validação falhar, retorna a view com os erros
        return View(ong);
    }

    [Route("painel-ong")]
    [Authorize(Roles = "Ong")]  // <-- Só acessa se estiver logado como ONG
    public IActionResult PainelOng()
    {
        return View();
    }

}

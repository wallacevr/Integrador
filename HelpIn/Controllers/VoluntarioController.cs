using HelpIn; // onde está seu ApplicationDbContext
using Microsoft.AspNetCore.Mvc;
using HelpIn.Models; // caso você use uma pasta para as models (ajuste conforme seu projeto)
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
 using HelpIn.Services;
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
    
   

    [HttpGet("ongs-proximas")]
    [Authorize(Roles = "Voluntário")]
    public async Task<IActionResult> OngsProximas(int page = 1, int pageSize = 5)
    {
        Console.WriteLine(User.Identity.Name);
        var voluntario = await _context.Voluntarios
            .FirstOrDefaultAsync(v => v.Email == User.Identity.Name);
        Console.WriteLine(voluntario);
        if (voluntario == null || string.IsNullOrEmpty(voluntario.Cep))
        {
            TempData["MensagemErro"] = "CEP do voluntário não encontrado.";
            return RedirectToAction("PainelVoluntario");
        }

        var geoService = new GeocodingService();
        double volLat, volLon;

        // Se não tiver lat/lon salvo ainda
        if (voluntario.Latitude == null || voluntario.Longitude == null)
        {
            try
            {
                (volLat, volLon) = await geoService.GetCoordinatesByCep(voluntario.Cep);
                voluntario.Latitude = volLat;
                voluntario.Longitude = volLon;
                _context.Voluntarios.Update(voluntario);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["MensagemErro"] = "Não foi possível localizar seu CEP.";
                return RedirectToAction("PainelVoluntario");
            }
        }
        else
        {
            volLat = voluntario.Latitude.Value;
            volLon = voluntario.Longitude.Value;
        }

        var ongs = await _context.Ongs.ToListAsync();
        var listaOngs = new List<(Ong ong, double distanciaKm)>();

        foreach (var ong in ongs)
        {
            if (ong.Latitude == null || ong.Longitude == null)
            {
                try
                {
                    (double lat, double lon) = await geoService.GetCoordinatesByCep(ong.cep);
                    ong.Latitude = lat;
                    ong.Longitude = lon;
                    _context.Ongs.Update(ong);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    continue; // Ignora ONG que não localizou CEP
                }
            }

            double distancia = CalcularDistancia(volLat, volLon, ong.Latitude.Value, ong.Longitude.Value);
            listaOngs.Add((ong, distancia));
        }

        var totalOngs = listaOngs.Count;
        var resultadoPaginado = listaOngs
            .OrderBy(x => x.distanciaKm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.TotalPages = (int)Math.Ceiling(totalOngs / (double)pageSize);
        ViewBag.CurrentPage = page;

        return View(resultadoPaginado);
    }

    private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371;
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

}

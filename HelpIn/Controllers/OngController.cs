using HelpIn;
using HelpIn.Models;
using HelpIn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace HelpIn.Controllers
{
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
                    // Hash da senha
                    var passwordHasher = new PasswordHasher<Ong>();
                ong.Senha = passwordHasher.HashPassword(ong, ong.Senha);
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

        [HttpGet("voluntarios-proximos")]
        [Authorize(Roles = "Ong")]
        public async Task<IActionResult> VoluntariosProximos(int page = 1, int pageSize = 5)
        {
            var ong = await _context.Ongs
                .FirstOrDefaultAsync(o => o.Email == User.Identity.Name);
            Console.WriteLine(User.Identity.Name);
            if (ong == null || string.IsNullOrEmpty(ong.Cep))
            {
                TempData["MensagemErro"] = "CEP da ONG não encontrado.";
                return RedirectToAction("PainelOng");
            }

            var geoService = new GeocodingService();
            double ongLat, ongLon;

            if (ong.Latitude == null || ong.Longitude == null)
            {
                try
                {
                    (ongLat, ongLon) = await geoService.GetCoordinatesByCep(ong.Cep);
                    ong.Latitude = ongLat;
                    ong.Longitude = ongLon;
                    _context.Ongs.Update(ong);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["MensagemErro"] = "Erro ao localizar o CEP da ONG.";
                    return RedirectToAction("PainelOng");
                }
            }
            else
            {
                ongLat = ong.Latitude.Value;
                ongLon = ong.Longitude.Value;
            }

            var voluntarios = await _context.Voluntarios.ToListAsync();
            var listaVoluntarios = new List<(Voluntario voluntario, double distanciaKm)>();

            foreach (var voluntario in voluntarios)
            {
                if (!string.IsNullOrEmpty(voluntario.Cep))
                {
                    try
                    {
                        double volLat, volLon;

                        if (voluntario.Latitude == null || voluntario.Longitude == null)
                        {
                            (volLat, volLon) = await geoService.GetCoordinatesByCep(voluntario.Cep);
                            voluntario.Latitude = volLat;
                            voluntario.Longitude = volLon;
                            _context.Voluntarios.Update(voluntario);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            volLat = voluntario.Latitude.Value;
                            volLon = voluntario.Longitude.Value;
                        }

                        double distancia = CalcularDistancia(ongLat, ongLon, volLat, volLon);
                        listaVoluntarios.Add((voluntario, distancia));
                    }
                    catch
                    {
                        continue; // Ignora voluntário com erro de geolocalização
                    }
                }
            }

            var totalVoluntarios = listaVoluntarios.Count;
            var resultadoPaginado = listaVoluntarios
                .OrderBy(x => x.distanciaKm)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling(totalVoluntarios / (double)pageSize);
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
}

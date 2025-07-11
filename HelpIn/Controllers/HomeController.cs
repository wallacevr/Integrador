using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HelpIn.Models;

namespace HelpIn.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet("faq")]
    public IActionResult Faq()
    {
        return View();
    }
      [HttpGet("funcionalidades")]
    public IActionResult Funcionalidades()
    {
        return View();
    }
    [HttpGet("nossoimpacto")]
    public IActionResult NossoImpacto()
    {
        return View();
    }
    
    public IActionResult EmConstrucao()
    {
        return View();
    }
    [HttpGet("sobre")]
    public IActionResult Sobre()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    // Rota para a página com os botões
    public IActionResult EscolhaCadastro()
    {
        return View();
    }
    // Rotas que os botões irão acessar
    public IActionResult CadastroOng()
    {
        return View(); // View CadastroOng.cshtml
    }

    public IActionResult CadastroVoluntario()
    {
        return View(); // View CadastroVoluntario.cshtml
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

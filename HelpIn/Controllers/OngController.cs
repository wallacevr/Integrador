using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class OngController : Controller
{
    [HttpGet]
    public IActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Cadastro(Ong ong)
    {
        if (ModelState.IsValid)
        {
            // TODO: Salvar ONG no banco ou exibir mensagem de sucesso
            TempData["Mensagem"] = "ONG cadastrada com sucesso!";
            return RedirectToAction("Cadastro");
        }

        return View(ong);
    }
}

using HelpIn;
using HelpIn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpIn.Controllers
{
    [Authorize(Roles = "Ong")]
    [Route("ong/vagas")]
    public class VagaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VagaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listar todas as vagas da ONG logada
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Email == User.Identity.Name);
            if (ong == null) return Unauthorized();

            var vagas = await _context.Vagas
                .Where(v => v.OngId == ong.Id)
                .ToListAsync();

            return View(vagas);
        }

        // GET: Formulário de criação
        [HttpGet("criar")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Salvar nova vaga
        [HttpPost("criar")]
        public async Task<IActionResult> Create(Vaga vaga)
        {
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Email == User.Identity.Name);
            if (ong == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                vaga.OngId = ong.Id;
                vaga.DataCriacao = DateTime.Now;

                _context.Vagas.Add(vaga);
                await _context.SaveChangesAsync();

                TempData["MensagemSucesso"] = "Vaga criada com sucesso!";
                return RedirectToAction("Index");
            }

            TempData["MensagemErro"] = "Erro ao salvar vaga. Verifique os campos.";
            return View(vaga);
        }

       // GET: Exibir
        [HttpGet("visualizar/{id}")]
        public async Task<IActionResult> show(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null) return NotFound();
            return View(vaga);
        }
        // GET: Editar
        [HttpGet("editar/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null) return NotFound();
            return View(vaga);
        }

        // POST: Salvar edição
        [HttpPost("editar/{id}")]
        public async Task<IActionResult> Edit(int id, Vaga vaga)
        {
            if (id != vaga.Id) return NotFound();

            if (ModelState.IsValid)
            {

                var vagaExistente = await _context.Vagas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                if (vagaExistente == null) return NotFound();

                // Mantém o OngId original
                vaga.OngId = vagaExistente.OngId;
                vaga.DataCriacao = vagaExistente.DataCriacao; // Opcional: preservar data original       
                _context.Update(vaga);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Vaga atualizada!";
                return RedirectToAction("Index");
            }

            TempData["MensagemErro"] = "Erro ao atualizar.";
            return View(vaga);
        }

        // GET: Confirmar exclusão
        [HttpGet("excluir/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null) return NotFound();
            return View(vaga);
        }

        // POST: Excluir
        [HttpPost("excluir/{id}"), ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga != null)
            {
                _context.Vagas.Remove(vaga);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Vaga excluída com sucesso.";
            }
            return RedirectToAction("Index");
        }
    }
}

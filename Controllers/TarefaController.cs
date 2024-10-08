using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound(); 
            }
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefas = _context.Tarefas.ToList();
            if (tarefas.Count == 0)
            {
                return NoContent(); 
            }
            return Ok(tarefas); 
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            if (string.IsNullOrEmpty(titulo))
            {
                return BadRequest("O título não pode estar vazio.");
            }

            var tarefas = _context.Tarefas
                                  .Where(t => t.Titulo.Contains(titulo))
                                  .ToList();

            if (tarefas.Count == 0)
            {
                return NotFound("Nenhuma tarefa encontrada com o título fornecido.");
            }

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefas = _context.Tarefas.Where(x => x.Data.Date == data.Date).ToList();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefas = _context.Tarefas
                                  .Where(x => x.Status == status)
                                  .ToList();

            if (tarefas.Count == 0)
            {
                return NotFound("Nenhuma tarefa encontrada com o status fornecido.");
            }
            return Ok(tarefas);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
            {
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });
            }

            try
            {
                _context.Tarefas.Add(tarefa);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = "Erro ao salvar a tarefa: " + ex.Message });
            }

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            if (tarefaBanco == null)
                return NotFound();

            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

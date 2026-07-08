using CatchDebits.API.DTOs.Pessoa;
using CatchDebits.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatchDebits.API.Controllers;

/// <summary>
/// Controller REST para o CRUD de Pessoas.
/// Não contém lógica de negócio — apenas orquestra chamadas ao Service.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _service;

    public PessoasController(IPessoaService service) => _service = service;

    /// <summary>
    /// GET /api/pessoas — Lista todas as pessoas cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PessoaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var pessoas = await _service.ListarTodosAsync();
        return Ok(pessoas);
    }

    /// <summary>
    /// POST /api/pessoas — Cadastra uma nova pessoa.
    /// Retorna 201 Created com o header Location apontando para o recurso.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] PessoaRequestDto dto)
    {
        var criada = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Listar), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// DELETE /api/pessoas/{id} — Deleta a pessoa e suas transações (Cascade automático).
    /// Retorna 204 No Content em caso de sucesso.
    /// Retorna 404 Not Found se o Id não existir.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        var deletado = await _service.DeletarAsync(id);

        if (!deletado)
            return NotFound(new { mensagem = $"Pessoa com Id {id} não encontrada." });

        return NoContent();
    }
}
using HyDrata.GestaoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/produtores")]
public class ProdutoresApiController : ControllerBase
{
    private readonly IProdutorRepository _repo;

    public ProdutoresApiController(IProdutorRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var produtores = _repo.GetAll().Select(p => new
        {
            p.Id, p.Nome, p.Cpf, p.Email, p.Telefone, p.Status, p.DataCadastro
        });
        return Ok(produtores);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var produtor = _repo.GetById(id);
        if (produtor == null) return NotFound($"Produtor {id} não encontrado.");

        return Ok(new
        {
            produtor.Id, produtor.Nome, produtor.Cpf,
            produtor.Email, produtor.Telefone, produtor.Status, produtor.DataCadastro
        });
    }

    [HttpGet("{id:int}/propriedades")]
    public IActionResult GetComPropriedades(int id)
    {
        var produtor = _repo.GetByIdComPropriedades(id);
        if (produtor == null) return NotFound($"Produtor {id} não encontrado.");

        return Ok(new
        {
            produtor.Id, produtor.Nome, produtor.Cpf,
            produtor.Email, produtor.Telefone, produtor.Status, produtor.DataCadastro,
            Propriedades = produtor.Propriedades.Select(p => new
            {
                p.Id, p.Nome, p.Cidade, p.Estado, p.AreaHectares, p.Status,
                Plano = p.Plano == null ? null : new { p.Plano.Id, p.Plano.Nome }
            })
        });
    }

    [HttpGet("email/{email}")]
    public IActionResult GetByEmail(string email)
    {
        var produtor = _repo.GetByEmail(email);
        if (produtor == null) return NotFound($"Produtor com e-mail '{email}' não encontrado.");

        return Ok(new
        {
            produtor.Id, produtor.Nome, produtor.Cpf,
            produtor.Email, produtor.Telefone, produtor.Status, produtor.DataCadastro
        });
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CadastrarProdutorDto dto)
    {
        if (_repo.CpfExiste(dto.Cpf))
            return Conflict(new { erro = $"CPF '{dto.Cpf}' já está cadastrado." });

        if (dto.Email != null && _repo.EmailExiste(dto.Email))
            return Conflict(new { erro = $"E-mail '{dto.Email}' já está cadastrado." });

        var produtor = new Produtor
        {
            Nome     = dto.Nome,
            Cpf      = dto.Cpf,
            Email    = dto.Email,
            Telefone = dto.Telefone,
            Senha    = dto.Senha,
            Status   = dto.Status
        };

        _repo.Add(produtor);

        return CreatedAtAction(nameof(GetById), new { id = produtor.Id }, new
        {
            produtor.Id, produtor.Nome, produtor.Cpf,
            produtor.Email, produtor.Telefone, produtor.Status, produtor.DataCadastro
        });
    }

    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, [FromBody] AtualizarProdutorDto dto)
    {
        var produtor = _repo.GetById(id);
        if (produtor == null) return NotFound($"Produtor {id} não encontrado.");

        if (dto.Email != null && _repo.EmailExiste(dto.Email, ignorarId: id))
            return Conflict(new { erro = $"E-mail '{dto.Email}' já está em uso por outro produtor." });

        produtor.Nome     = dto.Nome;
        produtor.Email    = dto.Email;
        produtor.Telefone = dto.Telefone;
        produtor.Senha    = dto.Senha;
        produtor.Status   = dto.Status;

        _repo.Update(produtor);

        return Ok(new
        {
            produtor.Id, produtor.Nome, produtor.Cpf,
            produtor.Email, produtor.Telefone, produtor.Status, produtor.DataCadastro
        });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Deletar(int id)
    {
        var produtor = _repo.GetById(id);
        if (produtor == null) return NotFound($"Produtor {id} não encontrado.");

        try
        {
            _repo.Delete(id);
            return NoContent();
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("ORA-02292") == true)
        {
            return Conflict(new { erro = "Não é possível remover: produtor possui propriedades vinculadas." });
        }
    }
}

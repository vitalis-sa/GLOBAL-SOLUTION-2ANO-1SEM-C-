using HyDrata.GestaoApi.Repositories;
using Microsoft.EntityFrameworkCore;

public class ProdutorRepository : IProdutorRepository
{
    private readonly AppDbContext _context;

    public ProdutorRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Produtor> GetAll()
        => _context.Produtores
            .AsNoTracking()
            .ToList();

    public Produtor? GetById(int id)
        => _context.Produtores
            .FirstOrDefault(p => p.Id == id);

    public Produtor? GetByIdComPropriedades(int id)
        => _context.Produtores
            .Include(p => p.Propriedades)
                .ThenInclude(pr => pr.Plano)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);

    public Produtor? GetByEmail(string email)
        => _context.Produtores
            .AsNoTracking()
            .FirstOrDefault(p => p.Email == email);

    public bool CpfExiste(string cpf, int? ignorarId = null)
        => _context.Produtores
            .Any(p => p.Cpf == cpf && (!ignorarId.HasValue || p.Id != ignorarId.Value));

    public bool EmailExiste(string email, int? ignorarId = null)
        => _context.Produtores
            .Any(p => p.Email == email && (!ignorarId.HasValue || p.Id != ignorarId.Value));

    public void Add(Produtor produtor)
    {
        produtor.DataCadastro = DateTime.UtcNow;
        _context.Produtores.Add(produtor);
        _context.SaveChanges();
    }

    public void Update(Produtor produtor)
    {
        _context.Produtores.Update(produtor);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var produtor = GetById(id);
        if (produtor != null)
        {
            _context.Produtores.Remove(produtor);
            _context.SaveChanges();
        }
    }
}

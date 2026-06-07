using HyDrata.GestaoApi.Repositories;
using Microsoft.EntityFrameworkCore;

public class PropriedadeRepository : IPropriedadeRepository
{
    private readonly AppDbContext _context;

    public PropriedadeRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Propriedade> GetAll()
        => _context.Propriedades
            .Include(p => p.Produtor)
            .Include(p => p.Plano)
            .AsNoTracking()
            .ToList();

    public IEnumerable<Propriedade> GetByProdutorId(int produtorId)
        => _context.Propriedades
            .Include(p => p.Plano)
            .Include(p => p.Produtor)
            .Where(p => p.ProdutorId == produtorId)
            .AsNoTracking()
            .ToList();

    public Propriedade? GetById(int id)
        => _context.Propriedades
            .Include(p => p.Produtor)
            .Include(p => p.Plano)
            .FirstOrDefault(p => p.Id == id);

    public void Add(Propriedade propriedade)
    {
        var conn = _context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) conn.Open();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT SQ_PROPRIEDADE.NEXTVAL FROM DUAL";
            propriedade.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }
        propriedade.DataCadastro = DateTime.UtcNow;
        _context.Propriedades.Add(propriedade);
        _context.SaveChanges();
    }

    public void Update(Propriedade propriedade)
    {
        _context.Propriedades.Update(propriedade);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var propriedade = GetById(id);
        if (propriedade != null)
        {
            _context.Propriedades.Remove(propriedade);
            _context.SaveChanges();
        }
    }
}

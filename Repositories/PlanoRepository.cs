using HyDrata.GestaoApi.Repositories;

public class PlanoRepository : IPlanoRepository
{
    private readonly AppDbContext _context;

    public PlanoRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Plano> GetAll()
        => _context.Planos
            .ToList();

    public Plano? GetById(int id)
        => _context.Planos
            .FirstOrDefault(p => p.Id == id);

    public void Add(Plano plano)
    {
        plano.Id = _context.Database.SqlQueryRaw<int>("SELECT SQ_PLANO.NEXTVAL as \"Value\" FROM DUAL").First();
        _context.Planos.Add(plano);
        _context.SaveChanges();
    }

    public void Update(Plano plano)
    {
        _context.Planos.Update(plano);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var plano = GetById(id);
        if (plano != null)
        {
            _context.Planos.Remove(plano);
            _context.SaveChanges();
        }
    }
}

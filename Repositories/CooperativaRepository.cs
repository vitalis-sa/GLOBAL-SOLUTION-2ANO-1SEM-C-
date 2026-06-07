using HyDrata.GestaoApi.Repositories;

public class CooperativaRepository : ICooperativaRepository
{
    private readonly AppDbContext _context;

    public CooperativaRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Cooperativa> GetAll()
        => _context.Cooperativas
            .ToList();

    public Cooperativa? GetById(int id)
        => _context.Cooperativas
            .FirstOrDefault(c => c.Id == id);

    public void Add(Cooperativa cooperativa)
    {
        cooperativa.Id = _context.Database.SqlQueryRaw<int>("SELECT SQ_COOPERATIVA.NEXTVAL as \"Value\" FROM DUAL").First();
        cooperativa.DataCadastro = DateTime.UtcNow;
        _context.Cooperativas.Add(cooperativa);
        _context.SaveChanges();
    }

    public void Update(Cooperativa cooperativa)
    {
        _context.Cooperativas.Update(cooperativa);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var cooperativa = GetById(id);
        if (cooperativa != null)
        {
            _context.Cooperativas.Remove(cooperativa);
            _context.SaveChanges();
        }
    }
}

using HyDrata.GestaoApi.Repositories;
using Microsoft.EntityFrameworkCore;

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
        var conn = _context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) conn.Open();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT SQ_COOPERATIVA.NEXTVAL FROM DUAL";
            cooperativa.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }
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

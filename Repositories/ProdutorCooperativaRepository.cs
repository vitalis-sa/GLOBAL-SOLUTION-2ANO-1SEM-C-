using HyDrata.GestaoApi.Repositories;
using Microsoft.EntityFrameworkCore;

public class ProdutorCooperativaRepository : IProdutorCooperativaRepository
{
    private readonly AppDbContext _context;

    public ProdutorCooperativaRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ProdutorCooperativa> GetAll()
        => _context.ProdutorCooperativas
            .Include(pc => pc.Produtor)
            .Include(pc => pc.Cooperativa)
            .AsNoTracking()
            .ToList();

    public IEnumerable<ProdutorCooperativa> GetByProdutorId(int produtorId)
        => _context.ProdutorCooperativas
            .Include(pc => pc.Cooperativa)
            .Where(pc => pc.ProdutorId == produtorId)
            .AsNoTracking()
            .ToList();

    public IEnumerable<ProdutorCooperativa> GetByCooperativaId(int cooperativaId)
        => _context.ProdutorCooperativas
            .Include(pc => pc.Produtor)
            .Where(pc => pc.CooperativaId == cooperativaId)
            .AsNoTracking()
            .ToList();

    public ProdutorCooperativa? Get(int produtorId, int cooperativaId)
        => _context.ProdutorCooperativas
            .Include(pc => pc.Produtor)
            .Include(pc => pc.Cooperativa)
            .FirstOrDefault(pc => pc.ProdutorId == produtorId && pc.CooperativaId == cooperativaId);

    public bool ExisteAssociacao(int produtorId, int cooperativaId)
        => _context.ProdutorCooperativas
            .Count(pc => pc.ProdutorId == produtorId && pc.CooperativaId == cooperativaId) > 0;

    public void Add(ProdutorCooperativa associacao)
    {
        associacao.DataAssociacao = DateTime.UtcNow;
        _context.ProdutorCooperativas.Add(associacao);
        _context.SaveChanges();
    }

    public void Delete(ProdutorCooperativa associacao)
    {
        _context.ProdutorCooperativas.Remove(associacao);
        _context.SaveChanges();
    }
}

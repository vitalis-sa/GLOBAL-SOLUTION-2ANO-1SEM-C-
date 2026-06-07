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
    {
        var query = _context.Produtores.Where(p => p.Cpf == cpf);
        if (ignorarId.HasValue)
        {
            query = query.Where(p => p.Id != ignorarId.Value);
        }
        return query.Count() > 0;
    }

    public bool EmailExiste(string email, int? ignorarId = null)
    {
        var query = _context.Produtores.Where(p => p.Email == email);
        if (ignorarId.HasValue)
        {
            query = query.Where(p => p.Id != ignorarId.Value);
        }
        return query.Count() > 0;
    }

    public void Add(Produtor produtor)
    {
        var conn = _context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) conn.Open();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT SQ_PRODUTOR.NEXTVAL FROM DUAL";
            produtor.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }
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
            var conn = _context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
BEGIN
    DELETE FROM ALERTA WHERE PROPRIEDADE_ID IN (SELECT ID FROM PROPRIEDADE WHERE PRODUTOR_ID = :id);
    DELETE FROM LEITURA_CLIMA WHERE DISPOSITIVO_IOT_ID IN (SELECT ID FROM DISPOSITIVO_IOT WHERE PROPRIEDADE_ID IN (SELECT ID FROM PROPRIEDADE WHERE PRODUTOR_ID = :id));
    DELETE FROM LEITURA_LUZ WHERE DISPOSITIVO_IOT_ID IN (SELECT ID FROM DISPOSITIVO_IOT WHERE PROPRIEDADE_ID IN (SELECT ID FROM PROPRIEDADE WHERE PRODUTOR_ID = :id));
    DELETE FROM DISPOSITIVO_IOT WHERE PROPRIEDADE_ID IN (SELECT ID FROM PROPRIEDADE WHERE PRODUTOR_ID = :id);
    DELETE FROM PRODUTOR_COOPERATIVA WHERE PRODUTOR_ID = :id;
    DELETE FROM PROPRIEDADE WHERE PRODUTOR_ID = :id;
    DELETE FROM PRODUTOR WHERE ID = :id;
END;";
                var pId = cmd.CreateParameter();
                pId.ParameterName = "id";
                pId.Value = id;
                cmd.Parameters.Add(pId);
                
                cmd.ExecuteNonQuery();
            }
        }
    }
}

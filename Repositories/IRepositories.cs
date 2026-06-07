namespace HyDrata.GestaoApi.Repositories;

public interface IProdutorRepository
{
    IEnumerable<Produtor> GetAll();
    Produtor? GetById(int id);
    Produtor? GetByIdComPropriedades(int id);
    Produtor? GetByEmail(string email);
    bool CpfExiste(string cpf, int? ignorarId = null);
    bool EmailExiste(string email, int? ignorarId = null);
    void Add(Produtor produtor);
    void Update(Produtor produtor);
    void Delete(int id);
}

public interface ICooperativaRepository
{
    IEnumerable<Cooperativa> GetAll();
    Cooperativa? GetById(int id);
    void Add(Cooperativa cooperativa);
    void Update(Cooperativa cooperativa);
    void Delete(int id);
}

public interface IPlanoRepository
{
    IEnumerable<Plano> GetAll();
    Plano? GetById(int id);
    void Add(Plano plano);
    void Update(Plano plano);
    void Delete(int id);
}

public interface IPropriedadeRepository
{
    IEnumerable<Propriedade> GetAll();
    IEnumerable<Propriedade> GetByProdutorId(int produtorId);
    Propriedade? GetById(int id);
    void Add(Propriedade propriedade);
    void Update(Propriedade propriedade);
    void Delete(int id);
}

public interface IProdutorCooperativaRepository
{
    IEnumerable<ProdutorCooperativa> GetAll();
    IEnumerable<ProdutorCooperativa> GetByProdutorId(int produtorId);
    IEnumerable<ProdutorCooperativa> GetByCooperativaId(int cooperativaId);
    ProdutorCooperativa? Get(int produtorId, int cooperativaId);
    bool ExisteAssociacao(int produtorId, int cooperativaId);
    void Add(ProdutorCooperativa associacao);
    void Delete(ProdutorCooperativa associacao);
}

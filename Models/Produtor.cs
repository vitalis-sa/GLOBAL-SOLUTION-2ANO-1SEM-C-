using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PRODUTOR")]
public class Produtor
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NOME")]
    [Required, StringLength(150)]
    public string Nome { get; set; } = null!;

    [Column("CPF")]
    [Required, StringLength(14)]
    public string Cpf { get; set; } = null!;

    [Column("EMAIL")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("TELEFONE")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("SENHA")]
    [Required, StringLength(255)]
    public string Senha { get; set; } = null!;

    [Column("STATUS")]
    [StringLength(30)]
    public string Status { get; set; } = "ATIVO";

    [Column("DATA_CADASTRO")]
    public DateTime DataCadastro { get; set; }

    // 1:N — Um Produtor possui muitas Propriedades
    public List<Propriedade> Propriedades { get; set; } = [];

    // N:N — Um Produtor pode estar em muitas Cooperativas
    public List<ProdutorCooperativa> ProdutorCooperativas { get; set; } = [];
}

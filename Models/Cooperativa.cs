using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("COOPERATIVA")]
public class Cooperativa
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NOME")]
    [Required, StringLength(100)]
    public string Nome { get; set; } = null!;

    [Column("EMAIL")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("TELEFONE")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("STATUS")]
    [StringLength(30)]
    public string Status { get; set; } = "ATIVA";

    [Column("DATA_CADASTRO")]
    public DateTime DataCadastro { get; set; }

    // N:N — Uma Cooperativa pode ter muitos Produtores
    public List<ProdutorCooperativa> ProdutorCooperativas { get; set; } = [];
}

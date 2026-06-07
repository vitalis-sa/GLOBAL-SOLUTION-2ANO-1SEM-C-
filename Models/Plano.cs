using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PLANO")]
public class Plano
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NOME")]
    [Required, StringLength(50)]
    public string Nome { get; set; } = null!;

    [Column("VALOR_MENSALIDADE")]
    [Required]
    public decimal ValorMensalidade { get; set; }

    [Column("DESCRICAO")]
    [StringLength(300)]
    public string? Descricao { get; set; }

    [Column("STATUS")]
    [StringLength(30)]
    public string Status { get; set; } = "ATIVO";

    // 1:N — Um Plano pode ser usado em muitas Propriedades
    public List<Propriedade> Propriedades { get; set; } = [];
}

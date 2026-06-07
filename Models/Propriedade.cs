using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PROPRIEDADE")]
public class Propriedade
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PRODUTOR_ID")]
    [Required]
    public int ProdutorId { get; set; }
    [ForeignKey("ProdutorId")]
    public Produtor? Produtor { get; set; }

    [Column("PLANO_ID")]
    [Required]
    public int PlanoId { get; set; }
    [ForeignKey("PlanoId")]
    public Plano? Plano { get; set; }

    [Column("NOME")]
    [Required, StringLength(150)]
    public string Nome { get; set; } = null!;

    [Column("AREA_HECTARES")]
    [Required]
    public double AreaHectares { get; set; }

    [Column("CIDADE")]
    [StringLength(100)]
    public string? Cidade { get; set; }

    [Column("ESTADO")]
    [StringLength(2)]
    public string? Estado { get; set; }

    [Column("LATITUDE")]
    public double? Latitude { get; set; }

    [Column("LONGITUDE")]
    public double? Longitude { get; set; }

    [Column("STATUS")]
    [StringLength(30)]
    public string Status { get; set; } = "ATIVA";

    [Column("DATA_CADASTRO")]
    public DateTime DataCadastro { get; set; }
}

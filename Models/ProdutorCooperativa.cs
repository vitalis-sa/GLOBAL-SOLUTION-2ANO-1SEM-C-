using System.ComponentModel.DataAnnotations.Schema;

// Tabela de junção para o relacionamento N:N entre Produtor e Cooperativa.
[Table("PRODUTOR_COOPERATIVA")]
public class ProdutorCooperativa
{
    [Column("PRODUTOR_ID")]
    public int ProdutorId { get; set; }
    [ForeignKey("ProdutorId")]
    public Produtor? Produtor { get; set; }

    [Column("COOPERATIVA_ID")]
    public int CooperativaId { get; set; }
    [ForeignKey("CooperativaId")]
    public Cooperativa? Cooperativa { get; set; }

    [Column("DATA_ASSOCIACAO")]
    public DateTime DataAssociacao { get; set; }
}

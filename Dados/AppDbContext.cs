using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produtor>             Produtores             { get; set; }
    public DbSet<Cooperativa>          Cooperativas           { get; set; }
    public DbSet<Plano>                Planos                 { get; set; }
    public DbSet<Propriedade>          Propriedades           { get; set; }
    public DbSet<ProdutorCooperativa>  ProdutorCooperativas   { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produtor>(entity =>
        {
            entity.Property(e => e.Id)
                  .ValueGeneratedNever();

            entity.HasIndex(e => e.Cpf)
                  .IsUnique()
                  .HasDatabaseName("UQ_PRODUTOR_CPF");

            entity.HasIndex(e => e.Email)
                  .IsUnique()
                  .HasDatabaseName("UQ_PRODUTOR_EMAIL");

            entity.Property(e => e.DataCadastro)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Cooperativa>(entity =>
        {
            entity.Property(e => e.Id)
                  .ValueGeneratedNever();

            entity.Property(e => e.DataCadastro)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Plano>(entity =>
        {
            entity.Property(e => e.Id)
                  .ValueGeneratedNever();

            entity.Property(e => e.ValorMensalidade)
                  .HasPrecision(10, 2);
        });

        modelBuilder.Entity<Propriedade>(entity =>
        {
            entity.Property(e => e.Id)
                  .ValueGeneratedNever();

            entity.Property(e => e.AreaHectares);

            entity.Property(e => e.Latitude);

            entity.Property(e => e.Longitude);

            entity.Property(e => e.DataCadastro)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Produtor)
                  .WithMany(p => p.Propriedades)
                  .HasForeignKey(e => e.ProdutorId)
                  .HasConstraintName("FK_PROPRIEDADE_PRODUTOR")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Plano)
                  .WithMany(p => p.Propriedades)
                  .HasForeignKey(e => e.PlanoId)
                  .HasConstraintName("FK_PROPRIEDADE_PLANO")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProdutorCooperativa>(entity =>
        {
            entity.HasKey(e => new { e.ProdutorId, e.CooperativaId });

            entity.Property(e => e.DataAssociacao)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Produtor)
                  .WithMany(p => p.ProdutorCooperativas)
                  .HasForeignKey(e => e.ProdutorId)
                  .HasConstraintName("FK_PC_PRODUTOR")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Cooperativa)
                  .WithMany(c => c.ProdutorCooperativas)
                  .HasForeignKey(e => e.CooperativaId)
                  .HasConstraintName("FK_PC_COOPERATIVA")
                  .OnDelete(DeleteBehavior.Cascade); 
        });
    }
}

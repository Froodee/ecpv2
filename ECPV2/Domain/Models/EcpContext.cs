using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ECPV2.Domain.Models;

public partial class EcpContext : DbContext
{
    public EcpContext()
    {
    }

    public EcpContext(DbContextOptions<EcpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Avi> Avis { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<Connexion> Connexions { get; set; }

    public virtual DbSet<Employé> Employés { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<Produit> Produits { get; set; }

    public virtual DbSet<Promo> Promos { get; set; }

    public virtual DbSet<Typeproduit> Typeproduits { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.151.253;Database=ECP;User ID=sa;Password=P@ssw0rd2025;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("ADMIN");

            entity.Property(e => e.Iduser)
                .ValueGeneratedNever()
                .HasColumnName("IDUSER");
            entity.Property(e => e.Adruser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADRUSER");
            entity.Property(e => e.Cpuser).HasColumnName("CPUSER");
            entity.Property(e => e.Nomuser)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NOMUSER");
            entity.Property(e => e.Numuser).HasColumnName("NUMUSER");
            entity.Property(e => e.Villeuser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("VILLEUSER");

            entity.HasOne(d => d.IduserNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ADMIN_UTILISATEURS");
        });

        modelBuilder.Entity<Avi>(entity =>
        {
            entity.HasKey(e => new { e.Refpds, e.Iduser });

            entity.ToTable("AVIS");

            entity.Property(e => e.Refpds).HasColumnName("REFPDS");
            entity.Property(e => e.Iduser).HasColumnName("IDUSER");
            entity.Property(e => e.Commentaire)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("COMMENTAIRE");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Avis)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AVIS_ADMIN");

            entity.HasOne(d => d.RefpdsNavigation).WithMany(p => p.Avis)
                .HasForeignKey(d => d.Refpds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AVIS_PRODUITS");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("CLIENT");

            entity.Property(e => e.Iduser)
                .ValueGeneratedNever()
                .HasColumnName("IDUSER");
            entity.Property(e => e.Adruser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADRUSER");
            entity.Property(e => e.Cpuser).HasColumnName("CPUSER");
            entity.Property(e => e.Nomuser)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NOMUSER");
            entity.Property(e => e.Numuser).HasColumnName("NUMUSER");
            entity.Property(e => e.Siret).HasColumnName("SIRET");
            entity.Property(e => e.Villeuser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("VILLEUSER");

            entity.HasOne(d => d.IduserNavigation).WithOne(p => p.Client)
                .HasForeignKey<Client>(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLIENT_UTILISATEURS");
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.Numcde);

            entity.ToTable("COMMANDE");

            entity.Property(e => e.Numcde)
                .ValueGeneratedNever()
                .HasColumnName("NUMCDE");
            entity.Property(e => e.Datecde).HasColumnName("DATECDE");
            entity.Property(e => e.Iduser).HasColumnName("IDUSER");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COMMANDE_UTILISATEURS");
        });

        modelBuilder.Entity<Connexion>(entity =>
        {
            entity.HasKey(e => e.Idu);

            entity.ToTable("CONNEXION");

            entity.Property(e => e.Idu)
                .ValueGeneratedNever()
                .HasColumnName("IDU");
            entity.Property(e => e.Login)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LOGIN");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("PASSWORD");

            entity.HasMany(d => d.Idusers).WithMany(p => p.Idus)
                .UsingEntity<Dictionary<string, object>>(
                    "Lienn",
                    r => r.HasOne<Utilisateur>().WithMany()
                        .HasForeignKey("Iduser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LIENN_UTILISATEURS"),
                    l => l.HasOne<Connexion>().WithMany()
                        .HasForeignKey("Idu")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LIENN_CONNEXION"),
                    j =>
                    {
                        j.HasKey("Idu", "Iduser");
                        j.ToTable("LIENN");
                        j.IndexerProperty<short>("Idu").HasColumnName("IDU");
                        j.IndexerProperty<short>("Iduser").HasColumnName("IDUSER");
                    });
        });

        modelBuilder.Entity<Employé>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("EMPLOYÉ");

            entity.Property(e => e.Iduser)
                .ValueGeneratedNever()
                .HasColumnName("IDUSER");
            entity.Property(e => e.Adruser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADRUSER");
            entity.Property(e => e.Cpuser).HasColumnName("CPUSER");
            entity.Property(e => e.Dateemp).HasColumnName("DATEEMP");
            entity.Property(e => e.Nomuser)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NOMUSER");
            entity.Property(e => e.Nosecu)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("NOSECU");
            entity.Property(e => e.Numuser).HasColumnName("NUMUSER");
            entity.Property(e => e.Villeuser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("VILLEUSER");

            entity.HasOne(d => d.IduserNavigation).WithOne(p => p.Employé)
                .HasForeignKey<Employé>(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EMPLOYÉ_UTILISATEURS");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.Numfact);

            entity.ToTable("FACTURE");

            entity.Property(e => e.Numfact)
                .ValueGeneratedNever()
                .HasColumnName("NUMFACT");
            entity.Property(e => e.Datefact).HasColumnName("DATEFACT");
            entity.Property(e => e.Numcde).HasColumnName("NUMCDE");

            entity.HasOne(d => d.NumcdeNavigation).WithMany(p => p.Factures)
                .HasForeignKey(d => d.Numcde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FACTURE_COMMANDE");
        });

        modelBuilder.Entity<Produit>(entity =>
        {
            entity.HasKey(e => e.Refpds);

            entity.ToTable("PRODUITS");

            entity.Property(e => e.Refpds)
                .ValueGeneratedNever()
                .HasColumnName("REFPDS");
            entity.Property(e => e.Codetyp).HasColumnName("CODETYP");
            entity.Property(e => e.Descpds)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DESCPDS");
            entity.Property(e => e.Designpds)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DESIGNPDS");
            entity.Property(e => e.Idpromo).HasColumnName("IDPROMO");
            entity.Property(e => e.Libpds)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LIBPDS");
            entity.Property(e => e.Prixpds)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRIXPDS");
            entity.Property(e => e.Qtepds).HasColumnName("QTEPDS");

            entity.HasOne(d => d.CodetypNavigation).WithMany(p => p.Produits)
                .HasForeignKey(d => d.Codetyp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRODUITS_TYPEPRODUIT");

            entity.HasOne(d => d.IdpromoNavigation).WithMany(p => p.Produits)
                .HasForeignKey(d => d.Idpromo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRODUITS_PROMO");

            entity.HasMany(d => d.Numcdes).WithMany(p => p.Refpds)
                .UsingEntity<Dictionary<string, object>>(
                    "Panier",
                    r => r.HasOne<Commande>().WithMany()
                        .HasForeignKey("Numcde")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PANIER_COMMANDE"),
                    l => l.HasOne<Produit>().WithMany()
                        .HasForeignKey("Refpds")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PANIER_PRODUITS"),
                    j =>
                    {
                        j.HasKey("Refpds", "Numcde");
                        j.ToTable("PANIER");
                        j.IndexerProperty<short>("Refpds").HasColumnName("REFPDS");
                        j.IndexerProperty<short>("Numcde").HasColumnName("NUMCDE");
                    });

            entity.HasMany(d => d.Refpds1s).WithMany(p => p.RefpdsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "Composition",
                    r => r.HasOne<Produit>().WithMany()
                        .HasForeignKey("Refpds1")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COMPOSITION_PRODUITS1"),
                    l => l.HasOne<Produit>().WithMany()
                        .HasForeignKey("Refpds")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COMPOSITION_PRODUITS"),
                    j =>
                    {
                        j.HasKey("Refpds", "Refpds1");
                        j.ToTable("COMPOSITION");
                        j.IndexerProperty<short>("Refpds").HasColumnName("REFPDS");
                        j.IndexerProperty<short>("Refpds1").HasColumnName("REFPDS_1");
                    });

            entity.HasMany(d => d.RefpdsNavigation).WithMany(p => p.Refpds1s)
                .UsingEntity<Dictionary<string, object>>(
                    "Composition",
                    r => r.HasOne<Produit>().WithMany()
                        .HasForeignKey("Refpds")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COMPOSITION_PRODUITS"),
                    l => l.HasOne<Produit>().WithMany()
                        .HasForeignKey("Refpds1")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COMPOSITION_PRODUITS1"),
                    j =>
                    {
                        j.HasKey("Refpds", "Refpds1");
                        j.ToTable("COMPOSITION");
                        j.IndexerProperty<short>("Refpds").HasColumnName("REFPDS");
                        j.IndexerProperty<short>("Refpds1").HasColumnName("REFPDS_1");
                    });
        });

        modelBuilder.Entity<Promo>(entity =>
        {
            entity.HasKey(e => e.Idpromo);

            entity.ToTable("PROMO");

            entity.Property(e => e.Idpromo)
                .ValueGeneratedNever()
                .HasColumnName("IDPROMO");
            entity.Property(e => e.Datedeb).HasColumnName("DATEDEB");
            entity.Property(e => e.Datefin).HasColumnName("DATEFIN");
            entity.Property(e => e.Reduc)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("REDUC");
        });

        modelBuilder.Entity<Typeproduit>(entity =>
        {
            entity.HasKey(e => e.Codetyp);

            entity.ToTable("TYPEPRODUIT");

            entity.Property(e => e.Codetyp)
                .ValueGeneratedNever()
                .HasColumnName("CODETYP");
            entity.Property(e => e.Libtyp)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LIBTYP");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("UTILISATEURS");

            entity.Property(e => e.Iduser)
                .ValueGeneratedNever()
                .HasColumnName("IDUSER");
            entity.Property(e => e.Adruser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADRUSER");
            entity.Property(e => e.Cpuser).HasColumnName("CPUSER");
            entity.Property(e => e.Nomuser)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NOMUSER");
            entity.Property(e => e.Numuser).HasColumnName("NUMUSER");
            entity.Property(e => e.Villeuser)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("VILLEUSER");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Encyclopaedia.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Encyclopaedia.Data
{

    // On herite de IdentityDbContext pour intégrer la gestion des utilisateurs et des rôles avec ASP.NET Core Identity.
    public class EncyclopaediaDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        // Le contructeur qui recoit la configuration de la base de données via program.cs, et le transmet à la classe de base.
        public EncyclopaediaDbContext(DbContextOptions<EncyclopaediaDbContext> options)
            : base(options) { }

        // Les DbSet représentent les tables de la base de données pour chaque entité.
        public DbSet<Language> Languages { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<DomainTranslation> DomainTranslations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleTranslation> ArticleTranslations { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagTranslation> TagTranslations { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Clé composite pour ArticleTag
            modelBuilder.Entity<ArticleTag>()
                .HasKey(x => new { x.ArticleId, x.TagId });

            // Clé composite pour DomainTranslation
            modelBuilder.Entity<DomainTranslation>()
                .HasKey(x => new { x.DomainId, x.LanguageId });

            // Clé composite pour CategoryTranslation
            modelBuilder.Entity<CategoryTranslation>()
                .HasKey(x => new { x.CategoryId, x.LanguageId });

            // Clé composite pour TagTranslation
            modelBuilder.Entity<TagTranslation>()
                .HasKey(x => new { x.TagId, x.LanguageId });
        }
    }
       
}
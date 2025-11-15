using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.Models; // 1. IMPORTANTE: Garante que sua classe Category (com as Data Annotations) seja usada.

// 2. Implementação do DbContext com a lógica HasData()
public class AppDbContext : DbContext
{
    // Crie a propriedade DbSet para sua entidade
    public DbSet<Category> Categories { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // O EF Core já detecta as anotações [Table], [Key], [StringLength] da sua classe Category.
        modelBuilder.Entity<Category>(entity =>
        {
            // 3. Define que a coluna CategoryId é uma chave primária,
            // e que o EF Core não deve tentar gerenciá-la como uma identidade durante o HasData().
            // Isso permite que HasData insira os valores de ID que definimos.
            entity.Property(e => e.CategoryId)
                  .ValueGeneratedNever(); 

            // 4. Usa o HasData() para carregar as categorias
            // As IDs (Category.CategoryId) são obrigatórias para o HasData() e devem ser únicas.
            entity.HasData(GetCategorySeedData());
        });
    }

    private static List<Category> GetCategorySeedData()
    {
        // Usamos a lista exata de nomes solicitados, com descrições em português.
        return new List<Category>
        {
            new Category { CategoryId = 1, Name = "Canvas & Brush", Description = "Materiais para pintura em tela, cavaletes e suprimentos para belas-artes." },
            new Category { CategoryId = 2, Name = "Clay & Glaze", Description = "Argilas especiais, barros, ferramentas de modelagem e esmaltes para cerâmica." },
            new Category { CategoryId = 3, Name = "Ethereal Glassworks", Description = "Kits e materiais para vidro soprado, fusão e criação de mosaicos de vidro." },
            new Category { CategoryId = 4, Name = "Forged Metals", Description = "Ferramentas e metais para forja, soldagem e criação de peças decorativas de metal." },
            new Category { CategoryId = 5, Name = "Glimmerstone Jewels", Description = "Pedras brutas, contas, fios e fechos para montagem de joias artesanais." },
            new Category { CategoryId = 6, Name = "Heritage Leathers", Description = "Couros de diferentes texturas, carimbos, tintas e agulhas para trabalho em couro." },
            new Category { CategoryId = 7, Name = "Paper & Ink", Description = "Papéis de arroz, tintas nanquim, penas e materiais para caligrafia e encadernação." },
            new Category { CategoryId = 8, Name = "Timbercraft", Description = "Madeiras selecionadas, tornos e ferramentas de precisão para marcenaria e entalhe." },
            new Category { CategoryId = 9, Name = "Wicker & Weave", Description = "Vime, bambu e outras fibras naturais para cestaria, trançado e tecelagem." },
            new Category { CategoryId = 10, Name = "Wool & Whimsey", Description = "Lãs naturais, fios exóticos e acessórios para tricô, crochê e feltragem a seco." },
            new Category { CategoryId = 11, Name = "Stitch & Thread", Description = "Tecidos diversos, linhas de bordar, agulhas e máquinas para costura e quilting." }
        };
    }
}
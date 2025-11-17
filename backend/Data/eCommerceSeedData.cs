using backend.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace backend.Data
{
    // Classe responsável por fornecer todos os dados de Seed para o DbContext
    public static class SeedData
    {
        // =================================================================
        // CONSTANTES DE DESENVOLVIMENTO
        // =================================================================
        
        // MOCK HASH: Senha fixa para todos os clientes e vendedores (representa "123456")
        public const string MOCK_PASSWORD_HASH = "HASH_PARA_123456_DEV";

        // MOCK IMAGES: URL base simulada para imagens de produtos
        private const string MockImageUrlBase = "https://mock.cdn.ecommerce.com/";
        private const string BaseImagePath = "products/img";

        // =================================================================
        // FUNÇÕES DE SEEDING DE DADOS
        // =================================================================

        public static IEnumerable<Currency> GetCurrencySeedData()
        {
            return new List<Currency>
            {
                new Currency { CurrencyCode = "BRL", Name = "Real Brasileiro", Symbol = "R$", ExchangeRateToBRL = 1.00m },
                new Currency { CurrencyCode = "USD", Name = "Dólar Americano", Symbol = "$", ExchangeRateToBRL = 5.20m }
            };
        }

        public static IEnumerable<Client> GetClientSeedData()
        {
            // Seed de Clientes com os nomes fornecidos e dados de endereço detalhados
            return new List<Client>
            {
                new Client 
                { 
                    UserId = 1, 
                    Name = "Daniel Wilson", 
                    Email = "daniel.wilson@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "11987654321",
                    Address1 = "Rua do Cliente Feliz, 100",
                    Address2 = "Apto 101",
                    City = "São Paulo",
                    State = "SP",
                    Country = "Brazil",
                    ZipCode = "01000-001"
                },
                new Client 
                { 
                    UserId = 2, 
                    Name = "Isabella Moore", 
                    Email = "isabella.moore@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "21991234567",
                    Address1 = "Av. Atlântica, 2000",
                    Address2 = "Bloco B, Sala 5",
                    City = "Rio de Janeiro",
                    State = "RJ",
                    Country = "Brazil",
                    ZipCode = "22010-020"
                },
                new Client 
                { 
                    UserId = 3, 
                    Name = "John Smith", 
                    Email = "john.smith@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "31987654321",
                    Address1 = "Rua da Liberdade, 333",
                    Address2 = "Casa",
                    City = "Belo Horizonte",
                    State = "MG",
                    Country = "Brazil",
                    ZipCode = "30140-010"
                },
                new Client 
                { 
                    UserId = 4, 
                    Name = "Olivia Miller", 
                    Email = "olivia.miller@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "41999887766",
                    Address1 = "Rua das Flores, 500",
                    Address2 = "Fundos",
                    City = "Curitiba",
                    State = "PR",
                    Country = "Brazil",
                    ZipCode = "80020-000"
                },
                new Client 
                { 
                    UserId = 5, 
                    Name = "Sophia Davis", 
                    Email = "sophia.davis@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "51987659876",
                    Address1 = "Av. Principal, 800",
                    Address2 = "Sala 12",
                    City = "Porto Alegre",
                    State = "RS",
                    Country = "Brazil",
                    ZipCode = "90020-006"
                },
                new Client 
                { 
                    UserId = 6, 
                    Name = "Emily Johnson", 
                    Email = "emily.johnson@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "61988776655",
                    Address1 = "Praça dos Três Poderes, 1",
                    Address2 = "",
                    City = "Brasília",
                    State = "DF",
                    Country = "Brazil",
                    ZipCode = "70100-000"
                },
                new Client 
                { 
                    UserId = 7, 
                    Name = "James Taylor", 
                    Email = "james.taylor@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "81977665544",
                    Address1 = "Rua do Sol, 456",
                    Address2 = "Ponto de Referência: Próximo à praia",
                    City = "Recife",
                    State = "PE",
                    Country = "Brazil",
                    ZipCode = "50000-000"
                },
                new Client 
                { 
                    UserId = 8, 
                    Name = "Michael Brown", 
                    Email = "michael.brown@email.com", 
                    PasswordHash = MOCK_PASSWORD_HASH, 
                    PhoneNumber = "71966554433",
                    Address1 = "Avenida da Bahia, 123",
                    Address2 = "Edifício Comercial",
                    City = "Salvador",
                    State = "BA",
                    Country = "Brazil",
                    ZipCode = "40000-000"
                }
            };
        }

        public static IEnumerable<Seller> GetSellerSeedData()
        {
            return new List<Seller>
            {
                new Seller { SellerId = 1, Name = "Tech Solutions Ltda", Email = "tech.vendor@loja.com", PasswordHash = MOCK_PASSWORD_HASH, Cnpj = "11.111.111/0001-11" },
                new Seller { SellerId = 2, Name = "Casa e Decoração Premium", Email = "casa.decora@loja.com", PasswordHash = MOCK_PASSWORD_HASH, Cnpj = "22.222.222/0001-22" },
                new Seller { SellerId = 3, Name = "Moda e Acessórios", Email = "moda.acessorios@loja.com", PasswordHash = MOCK_PASSWORD_HASH, Cnpj = "33.333.333/0001-33" }
            };
        }

        public static IEnumerable<Category> GetCategorySeedData()
        {
            return new List<Category>
            {
                new Category { CategoryId = 1, Name = "Eletrônicos", Description = "Smartphones, TVs e Gadgets" },
                new Category { CategoryId = 2, Name = "Livros", Description = "Ficção, Não-Ficção e Guias Técnicos" },
                new Category { CategoryId = 3, Name = "Casa e Cozinha", Description = "Móveis e utensílios domésticos" },
                new Category { CategoryId = 4, Name = "Moda Masculina", Description = "Roupas, calçados e acessórios masculinos" }
            };
        }
        
        public static IEnumerable<Product> GetProductSeedData()
        {
            return new List<Product>
            {
                // Produtos do Seller 1 (Eletrônicos)
                new Product { ProductId = 1, Name = "Smartphone X Pro", Description = "Modelo premium com câmera 108MP.", Price = 3999.99m, Stock = 50, CategoryId = 1, SellerId = 1 },
                new Product { ProductId = 2, Name = "Smart TV LED 55'' 4K", Description = "Experiência cinematográfica em casa.", Price = 2500.00m, Stock = 20, CategoryId = 1, SellerId = 1 },
                new Product { ProductId = 3, Name = "Fone de Ouvido Noise Canceling", Description = "Áudio imersivo e cancelamento ativo de ruído.", Price = 799.00m, Stock = 100, CategoryId = 1, SellerId = 1 },
                new Product { ProductId = 4, Name = "Notebook Gamer 15''", Description = "Alto desempenho para jogos e trabalho.", Price = 5500.00m, Stock = 15, CategoryId = 1, SellerId = 1 },

                // Produtos do Seller 2 (Casa e Cozinha)
                new Product { ProductId = 5, Name = "Conjunto de Panelas Inox (10pçs)", Description = "Durabilidade e design para sua cozinha.", Price = 450.00m, Stock = 80, CategoryId = 3, SellerId = 2 },
                new Product { ProductId = 6, Name = "Cadeira Ergonômica Office", Description = "Suporte lombar e conforto total.", Price = 650.00m, Stock = 30, CategoryId = 3, SellerId = 2 },
                new Product { ProductId = 7, Name = "Cafeteira Expresso Automática", Description = "Café gourmet com um toque.", Price = 1200.00m, Stock = 40, CategoryId = 3, SellerId = 2 },
                new Product { ProductId = 8, Name = "Luminária de Chão LED", Description = "Design moderno e luz ajustável.", Price = 250.00m, Stock = 70, CategoryId = 3, SellerId = 2 },

                // Produtos do Seller 3 (Moda Masculina e Livros)
                new Product { ProductId = 9, Name = "Livro: Arquitetura Limpa", Description = "Guia essencial sobre design de software.", Price = 99.90m, Stock = 200, CategoryId = 2, SellerId = 3 },
                new Product { ProductId = 10, Name = "Camisa Social Slim Fit", Description = "100% algodão, ideal para o dia a dia.", Price = 180.00m, Stock = 120, CategoryId = 4, SellerId = 3 }
            };
        }

        public static IEnumerable<ProductImage> GetProductImageSeedData()
        {
            var images = new List<ProductImage>();
            int imageIdCounter = 1;
            var products = GetProductSeedData();

            foreach (var product in products)
            {
                var sellerId = product.SellerId;
                var categoryId = product.CategoryId;
                
                // 1. Imagem Principal
                string mainAlt = $"Main image of {product.Name}.";
                // The main image uses the ProductId and '_main.jpg'
                string mainUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{product.ProductId}_main.jpg";

                images.Add(new ProductImage
                {
                    ProductImageId = imageIdCounter++,
                    ProductId = product.ProductId,
                    Url = mainUrl,
                    Alt = mainAlt
                });

                // 2. Imagens de Detalhe (Simula 1 ou 2 imagens)
                int detailCount = product.ProductId % 2 == 0 ? 2 : 1; 
                
                for (int i = 1; i <= detailCount; i++)
                {
                    string detailAlt = $"Detail {i} of {product.Name}.";
                    string detailUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{product.ProductId}_detail_{i}.jpg";

                    images.Add(new ProductImage
                    {
                        ProductImageId = imageIdCounter++,
                        ProductId = product.ProductId,
                        Url = detailUrl,
                        Alt = detailAlt
                    });
                }
            }
            
            return images;
        }
    }
}
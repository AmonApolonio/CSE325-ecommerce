using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class ClientDetailsDto
{
    // Campo Chave (Necessário para a verificação de ID no Controller)
    public long UserId { get; set; }

    // Campos de Texto (Obrigatórios pela sua classe Client)
    [Required]
    public string Name { get; set; } = null!;
    
    [Required] // Embora seja somente leitura no Blazor, a API ainda pode esperar
    public string Email { get; set; } = null!; 

    public string? PhoneNumber { get; set; }

    [Required]
    public string Address1 { get; set; } = null!;
    public string? Address2 { get; set; }

    [Required]
    public string City { get; set; } = null!;
    
    [Required]
    public string State { get; set; } = null!;
    
    [Required]
    public string Country { get; set; } = null!;
    public string? ZipCode { get; set; }
}
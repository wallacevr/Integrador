using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace HelpIn.Models;
[Index(nameof(Email), IsUnique = true)]
public class Voluntario
{
    
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Telefone { get; set; }

    [Required]
    public string InteresseAtuacao { get; set; }

    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }

    // Upload de currículo (opcional)
    public string? CurriculoUrl { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [NotMapped]
    public IFormFile Curriculo { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [NotMapped]
    [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}

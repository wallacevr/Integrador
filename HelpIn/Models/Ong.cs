using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
[Index(nameof(Email), IsUnique = true)]
public class Ong
{
    public int Id { get; set; } // 🔑 Chave primária obrigatória
    [Required]
    public string Nome { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Telefone { get; set; }

    [Required]
    public string AreaAtuacao { get; set; }

    public string Cidade { get; set; }

    public string Sobre { get; set; }

  // ✅ Salvo no banco de dados:
    public string? LogoUrl { get; set; }
      // ❌ Não salvo no banco (apenas no tempo de upload):
    [NotMapped]
    public IFormFile Logo { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [NotMapped]
    [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}

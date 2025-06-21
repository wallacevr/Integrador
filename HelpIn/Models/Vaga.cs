using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpIn.Models
{
    public class Vaga
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public string AreaAtuacao { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [ForeignKey("OngId")]
        public int OngId { get; set; }

        public Ong? Ong { get; set; } // ← Agora opcional (nullable)
    }
}


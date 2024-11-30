using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;


namespace INDUENDUM_API.Models
{
    public class Product
    {
        [Key] // Çelësi primar
        public int Id { get; set; }

        [Required(ErrorMessage = "Emri i produktit është i detyrueshëm.")]
        [StringLength(100, ErrorMessage = "Emri i produktit duhet të ketë maksimumi 100 karaktere.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Përshkrimi është i detyrueshëm.")]
        [StringLength(500, ErrorMessage = "Përshkrimi duhet të ketë maksimumi 500 karaktere.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çmimi është i detyrueshëm.")]
        [Column(TypeName = "decimal(18, 2)")] // Formati për vlera monetare
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Ju lutemi vendosni një URL të vlefshme.")]
        [StringLength(255, ErrorMessage = "URL duhet të ketë maksimumi 255 karaktere.")] // Kufizim për URL
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsOnSale { get; set; } = false; // Default është false

        // Lidhja me ApplicationUser (kompani)
        [Required(ErrorMessage = "ID e kompanisë është e detyrueshme.")]
        [ForeignKey(nameof(Company))] // Përdor nameof për lidhjen me ApplicationUser
        public string CompanyId { get; set; } = string.Empty;

        public ApplicationUser? Company { get; set; } // Kompania që menaxhon këtë produkt

        // Relacioni me Collection (Many-to-Many)
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

        // Data e krijimit për auditim
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Data e përditësimit për auditim
        public DateTime? UpdatedAt { get; set; }
    }
}

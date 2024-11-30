namespace INDUENDUM_API.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Collection
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Emri i koleksionit duhet të jetë deri në 100 karaktere.")]
        public string Name { get; set; } = string.Empty;

        // Marrëdhënia me përdoruesin (ApplicationUser)
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        // Marrëdhënia me produktet (Many-to-Many)
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

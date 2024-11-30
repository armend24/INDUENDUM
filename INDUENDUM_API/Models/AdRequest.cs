using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace INDUENDUM_API.Models
{
    public class AdRequest
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Detajet e kërkesës janë të detyrueshme.")]
        [StringLength(500, ErrorMessage = "Detajet e kërkesës mund të jenë deri në 500 karaktere.")]
        public string RequestDetails { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID e kompanisë është e detyrueshme.")]
        [ForeignKey("Company")]
        public string CompanyId { get; set; } = string.Empty;

        [Required]
        public ApplicationUser Company { get; set; } = null!;

        public bool IsApproved { get; set; } = false;
    }

}



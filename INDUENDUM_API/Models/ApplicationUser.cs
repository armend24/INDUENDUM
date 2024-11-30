using Microsoft.AspNetCore.Identity;

namespace INDUENDUM_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Property shtesë për emrin e plotë të përdoruesit
        [PersonalData] // Etiketë për të treguar që kjo fushë mund të ruhet dhe editohet nga përdoruesi
        public string FullName { get; set; } = string.Empty;

        // Shembull për shtimin e ndonjë fushe tjetër specifike nëse është e nevojshme në projektin tuaj
        [PersonalData]
        public string? Address { get; set; } // Fusha opsionale për adresën e përdoruesit

        [PersonalData]
        public DateTime? DateOfBirth { get; set; } // Fusha opsionale për datën e lindjes
    }
}

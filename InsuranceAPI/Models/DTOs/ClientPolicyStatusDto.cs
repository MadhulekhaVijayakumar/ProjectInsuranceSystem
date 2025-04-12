namespace InsuranceAPI.Models.DTOs
{
    public class ClientPolicyStatusDto
    {
        public int ProposalId { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public string InsuranceType { get; set; }
        public DateTime ProposalCreatedAt { get; set; }
        public string ProposalStatus { get; set; }

        public decimal? CalculatedPremium { get; set; } // From InsuranceDetails
        public DateTime? InsuranceStartDate { get; set; }
        public decimal? InsuranceSum { get; set; }

        public string? InsurancePolicyNumber { get; set; } // From Insurance (if payment done)
        public string? InsuranceStatus { get; set; } // "quote generated", "active", etc.
        public DateTime? InsuranceCreatedAt { get; set; }
    }
}

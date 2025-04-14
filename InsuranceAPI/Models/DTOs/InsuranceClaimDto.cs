namespace InsuranceAPI.Models.DTOs
{
    public class InsuranceClaimDto
    {
        public int ClaimId { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string InsurancePolicyNumber { get; set; }
    }
}

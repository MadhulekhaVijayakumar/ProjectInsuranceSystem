using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.DTOs
{
    public class CreateProposalRequest
    {
        // Client Info
        
       

        // Vehicle Info
        [Required, MaxLength(20)]
        public string VehicleType { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string ChassisNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string EngineNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string MakerName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ModelName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string VehicleColor { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FuelType { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public int SeatCapacity { get; set; }

        // Proposal Info
        [Required]
        public string InsuranceType { get; set; } = string.Empty;

        [Required]
        public DateTime InsuranceValidUpto { get; set; }

        [Required]
        public DateTime FitnessValidUpto { get; set; }

        // Insurance Details Info
        [Required]
        public DateTime InsuranceStartDate { get; set; }

        [Required]
        public decimal InsuranceSum { get; set; }

        [Required]
        public string DamageInsurance { get; set; } = string.Empty;

        [Required]
        public string LiabilityOption { get; set; } = string.Empty;

        [Required]
        public string Plan { get; set; } = string.Empty;
    }
}

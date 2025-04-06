using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public User? User {  get; set; }//NAvigation


    }
}

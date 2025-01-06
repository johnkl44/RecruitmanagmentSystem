using System.ComponentModel;

namespace RecruitmentManagementSystem.Models
{
    public class ContactUsModel
    {
        int? ContactId { get; set; }
        [DisplayName("Your Name")]
        public string? Name { get; set; }
        [DisplayName("Your Email Id")]
        public string? EmailId { get; set; }
        [DisplayName("Your Message")]
        public string? UserMessage { get; set; }
        public string? ResponseMessage { get; set; }

        public string? ResponseStatus { get; set; }


        
    }
}

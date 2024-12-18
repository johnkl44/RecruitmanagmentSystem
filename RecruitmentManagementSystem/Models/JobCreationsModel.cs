using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentManagementSystem.Models
{
    public class JobCreationsModel
    {
        [Key]
        public int JobId { get; set; }
        [DisplayName("Job title")]
        [Required(ErrorMessage = "Job title is required.")]
        public string? JobTitle { get; set; }
        [DisplayName("Job description")]
        [Required(ErrorMessage = "Job description required.")] 
        public string? JobDescription { get; set; }
        [DisplayName("Required skill set")]
        [Required(ErrorMessage = "Skill set required.")] 
        public string? RequiredSkills { get; set; }
        [DisplayName("Experience")]
        [Required(ErrorMessage = "Experience required.")] 
        public string? Experience { get; set; }
        [DisplayName("Salary Range")]
        [Required(ErrorMessage = "Salary Expectations required.")] 
        public string? SalaryRange { get; set; }
        [DisplayName("Deadline")]
        [Required(ErrorMessage = "Deadline required.")] 
        public DateTime Deadline { get; set; }
        public string? JobStatus { get; set; }
        public int Author { get; set; }
        public IFormFile? PosterPhoto { get; set; }
        public string? PosterPhotoBase64 { get; set; }

        public byte[]? Poster {  get; set; }

        public DateTime? PostingDate { get; set; }
    }
}

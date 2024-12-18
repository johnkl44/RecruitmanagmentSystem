using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Protocol;
using Microsoft.AspNetCore.Mvc;

namespace RecruitmentManagementSystem.Models
{
    public class ApplicationModel
    {
        [Key]
        public int ApplicationId { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public IFormFile? ResumeFile { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
        public string? ApplicationStatus { get; set; }
        public DateTime AppliedDate { get; set; }
        public byte[]? Photo {  get; set; }
        public byte[]? Resume {  get; set; }

    }
}

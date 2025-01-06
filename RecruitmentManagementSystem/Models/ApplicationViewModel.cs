namespace RecruitmentManagementSystem.Models
{
    public class ApplicationViewModel
    {
        public int? ApplicationID { get; set; }
        public int? CandidateID { get; set; }
        public string? CandidateFirstName { get; set; }
        public string? CandidateLastName { get; set; }
        public string? CandidateEmail { get; set; }
        public int? JobID { get; set; }
        public string? JobTitle { get; set; }
        public string? ApplicationStatus { get; set; }
        public DateTime AppliedDate { get; set; }
        public string? PosterPhoto { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? ResumeFile { get; set; }
        public byte[]? Photo { get; set; }
        public byte[]? Resume { get; set; }
        public byte[]? Poster { get;set; }
    }

}

using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Net;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruitmentManagementSystem.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "First name is required.")]
        public string? FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        public string? LastName { get; set; }
        [DisplayName("Date of Birth")]
        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }
        [DisplayName("Gender")]
        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }
        [DisplayName("Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        public string? PhoneNumber { get; set; }
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Email address is required.")]
        public string? Email { get; set; }
        [DisplayName("Address")]
        [Required(ErrorMessage = "Address required.")]
        public string? Address { get; set; }
        [DisplayName("State")]
        [Required(ErrorMessage = "State required.")]
        public string? State { get; set; }
        [DisplayName("City")]
        [Required(ErrorMessage = "City required.")]
        public string? City { get; set; }
        [DisplayName("Username")]
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DisplayName("Password")]
        public string? Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]

        public string? ConfirmPassword { get; set; }
        [DisplayName("Role")]
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SelectListItem>? States { get; set; } 
        public List<SelectListItem>? Cities { get; set; }

        public string Encode(string password)
        {
            try
            {
                byte[] EncodeDataByte = new byte[password.Length];
                EncodeDataByte = System.Text.Encoding.UTF8.GetBytes(password);
                string EncryptPassword = Convert.ToBase64String(EncodeDataByte);
                return EncryptPassword;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Encoding Password", ex);
            }
        }

        public string Decode(string encodedPassword)
        {
            try
            {
                byte[] decodedBytes = Convert.FromBase64String(encodedPassword);
                string decodedPassword = System.Text.Encoding.UTF8.GetString(decodedBytes);
                return decodedPassword;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Decoding Password", ex);
            }
        }

    }

}

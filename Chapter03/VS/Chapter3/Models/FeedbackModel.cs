using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Chapter3.Models
{
    public class FeedbackModel
    {
        [Display(Name = "Name", Prompt = "Enter your full name"),
         Required]
        public string Name { get; set; }

        [Display(Name = "Average Score", Prompt = "Your average score"),
         Range(1.0, 100.0), UIHint("Number"),
         Required]
        public decimal Score { get; set; }

        [Display(Name = "Birthday"),
         DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Home page", Prompt = "Personal home page"),
         DataType(DataType.Url),
         Required]
        public string Homepage { get; set; }

        [Display(Name = "Email", Prompt = "Preferred e-mail address"),
         DataType(DataType.EmailAddress),
         Required]
        public string Email { get; set; }

        [Display(Name = "Phone number", Prompt = "Contact phone number"),
         DataType(DataType.PhoneNumber),
         Required]
        public string Phone { get; set; }

        [Display(Name = "Overall Satisfaction"), UIHint("Range")]
        public string Satisfaction { get; set; }
    }
}

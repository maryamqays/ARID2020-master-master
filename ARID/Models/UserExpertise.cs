using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class UserExpertise
    {
        public int Id { get; set; }

        [Display(Name = "ExpertiseId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int ExpertiseId { get; set; }
        public  Expertise Expertise { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }
    }
}

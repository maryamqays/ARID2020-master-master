using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ProfileLink
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "ProfileType")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public ProfileType ProfileType { get; set; }

        [Display(Name = "ProfileUrl")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [Url]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ProfileUrl { get; set; }

        [Display(Name = "AccessType")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DefaultValue(AccessType.ForPublic)]
        public AccessType AccessType { get; set; }
    }
}

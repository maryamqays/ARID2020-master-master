using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class TeachingExperience
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "ArDescription")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string ArDescription { get; set; }

        [Display(Name = "EnDescription")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnDescription { get; set; }

        [Display(Name = "ArTitle")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(200, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string ArTitle { get; set; }

        [Display(Name = "EnTitle")]
        [StringLength(200, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnTitle { get; set; }

        [Display(Name = "IsCurrent")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public bool IsCurrent { get; set; }

        [Display(Name = "FromYear")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime FromYear { get; set; }

        [Display(Name = "ToYear")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime? ToYear { get; set; }

        [Display(Name = "Indx")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [Range(1, 1000, ErrorMessage = "RangeError")]
        public int Indx { get; set; }
    }
}

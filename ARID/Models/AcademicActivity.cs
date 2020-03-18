using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class AcademicActivity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [Display(Name = "ActivityType")]
        public ActivityType ActivityType { get; set; }

        [Display(Name = "ArTitle")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArTitle { get; set; }

        [Display(Name = "EnTitle")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnTitle { get; set; }

        [Display(Name = "ArDescription")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArDescription { get; set; }

        [Display(Name = "EnDescription")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnDescription { get; set; }

        [Display(Name = "ActivityDate")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.Date)]
        public DateTime ActivityDate { get; set; }

        [Display(Name = "ActivityURL")]
        [Url(ErrorMessage = "UrlError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ActivityURL { get; set; }

        [Display(Name = "RelationType")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public RelationType RelationType { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public Language Language { get; set; }

        [Display(Name = "CountryId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Display(Name = "Photo")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "ArProjectName")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public String ArProjectName { get; set; }

        [Display(Name = "EnProjectName")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public String EnProjectName { get; set; }

        [Display(Name = "ArDescription")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string ArDescription { get; set; }

        [Display(Name = "EnDescription")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string EnDescription { get; set; }

        [Display(Name = "ArDetails")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ArDetails { get; set; }

        [Display(Name = "EnDetails")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string EnDetails { get; set; }

        [Display(Name = "CountryId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Display(Name = "FromYear")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime FromYear { get; set; }

        [Display(Name = "ToYear")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime ToYear { get; set; }

        [Display(Name = "ProjectState")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DefaultValue(ProjectState.New)]
        public ProjectState ProjectState { get; set; }
    }
}

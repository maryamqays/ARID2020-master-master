using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    //[RequiredIf("ToYear", "IsCurrent", false)]
    public class EducationalLevel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "AcademicDegreeId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int AcademicDegreeId { get; set; }
        public AcademicDegree AcademicDegree { get; set; }

        [Display(Name = "SpecialityId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; }

        [Display(Name = "ArCertificateName")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string ArCertificateName { get; set; }

        [Display(Name = "EnCertificateName")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string EnCertificateName { get; set; }

        [Display(Name = "FacultyId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }

        [Display(Name = "UniversityId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int UniversityId { get; set; }
        public University University { get; set; }

        [Display(Name = "CountryId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Display(Name = "ArDescription")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(200, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string ArDescription { get; set; }

        [Display(Name = "EnDescription")]
        [StringLength(200, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string EnDescription { get; set; }

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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using ARID.Data;
using System.Runtime.InteropServices;

namespace ARID.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [StringLength(450)]
        [Display(Name = "ReferredById")]
        public string ReferredById { get; set; }
        public  ApplicationUser ReferredBy { get; set; }

        [Display(Name = "ArName")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string ArName { get; set; }

        [Display(Name = "EnName")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string EnName { get; set; }

        [Display(Name = "OtherNames")]
        [StringLength(300, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 6)]
        public string OtherNames { get; set; }

        [Display(Name = "DateofBirth")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.Date)]
        public DateTime DateofBirth { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DefaultValue(GenderType.Unknown)]
        public GenderType Gender { get; set; }

        [Display(Name = "SecondEmail")]
        [EmailAddress(ErrorMessage = "EmailError")]
        [StringLength(100)]
        public string SecondEmail { get; set; }

        [Display(Name = "RegDate")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.DateTime)]
        public DateTime RegDate { get; set; }

        [Display(Name = "ARID")]
        [DefaultValue(0)]
        public int ARID { get; set; }

        [Display(Name = "ARIDDate")]
        [DataType(DataType.DateTime)]
        public DateTime? ARIDDate { get; set; }

        [Display(Name = "Status")]
        [DefaultValue(StatusType.New)]
        public StatusType Status { get; set; }

              [Display(Name = "UILanguage")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public string UILanguage { get; set; }

        [Display(Name = "ProfileImage")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ProfileImage { get; set; }

        [Display(Name = "FeaturedImage")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string FeaturedImage { get; set; }

        [Display(Name = "CVURL")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string CVURL { get; set; }

        [Display(Name = "Summary")]
        [StringLength(4000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Summary { get; set; }

        [Display(Name = "ContactMeDetail")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string ContactMeDetail { get; set; }

        [Display(Name = "CountryId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Display(Name = "CityId")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public int CityId { get; set; }
        public  City City { get; set; }

        [Display(Name = "UniversityId")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public int UniversityId { get; set; }
        public  University University { get; set; }

        [Display(Name = "FacultyId")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public int FacultyId { get; set; }
        public  Faculty Faculty { get; set; }

        [Display(Name = "Visitors")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public int Visitors { get; set; }

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "HoldingBalance")]
        public decimal HoldingBalance { get; set; }

        [Display(Name = "DAL")]
        [StringLength(450)]
        public string DAL { get; set; }

        [Display(Name = "DALEnabled")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public bool DALEnabled { get; set; }

        [Display(Name = "LastLogin")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "IsFreelancer")]
        public bool IsFreelancer { get; set; }

        //أعمل/أنتمي لمؤسسة غير جامعية
        [Display(Name = "IsNotUniversity")] 
        public bool IsNotUniversity { get; set; }

        [Display(Name = "FirmName")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string FirmName { get; set; }

     

        /// <summary>
        /// To be amended later to enable events use.
        /// </summary>
        /// <param name="sender">The sender of the code into the event</param>
        /// <param name="ev">to pass the parameters to the event</param>
        public delegate void CountEventHandler(object sender, CountEventArgs ev);

        private CountEventHandler count;
        public event CountEventHandler CountUnReadNotifications
        {
            add
            {
                count += value;
            }
            remove
            {
                count -= value;
            }
        }

        [NotMapped]
        public int UnReadNotifications
        {
            get
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return Common.ARIDFormat(this.ARID);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Address
    {
        //Home/Office
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "العنوان كاملا متضمنا اسم البناية")]
        [StringLength(100, ErrorMessage = "عدد الأحرف : من 6 الى 100", MinimumLength = 6)]
        public string FullAddress { get; set; }

        [Display(Name = "رقم الهاتف")]
        [StringLength(15, ErrorMessage = "عدد الأحرف : من 8 الى 15", MinimumLength = 8)]
        public string PhoneNumber { get; set; }

        [Display(Name = "الاسم المختصر للعنوان")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(10, ErrorMessage = "عدد الأحرف : من 4 الى 10", MinimumLength = 4)]
        public string AddressSaveName { get; set; }

        [Display(Name = "الاسم بالعربي")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(50, ErrorMessage = "عدد الأحرف : من 6 الى 50", MinimumLength = 6)]
        public string ArName { get; set; }

        [Display(Name = "الاسم بالانجليزي")]
        [StringLength(50, ErrorMessage = "عدد الأحرف : من 6 الى 50", MinimumLength = 6)]
        public string EnName { get; set; }

        [Display(Name = "العضو")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "IsDeleted")]
        public bool IsDeleted { get; set; }

        [Display(Name = "البلد")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Display(Name = "المدينة")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int CityId { get; set; }
        public  City City { get; set; }

        [Display(Name = "الجامعة")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public int UniversityId { get; set; }
        public  University University { get; set; }

        [Display(Name = "الكلية")]
        //[Required(ErrorMessage = "RequiredFieldError")]
        public int FacultyId { get; set; }
        public  Faculty Faculty { get; set; }

    }
}

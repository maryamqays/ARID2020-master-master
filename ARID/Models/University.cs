using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class University
    {
     
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم الجامعة باللغة العربية")]
        public string ArUniversityName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم الجامعة باللغة الانجليزية")]
        public string EnUniversityName { get; set; }

        [Url]
        [StringLength(100)]
        [Display(Name = "الموقع الالكتروني")]
        public string Website { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "عدد الكادر التدريسي")]
        public int StaffNo { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "عدد الطلبة")]
        public int StudentNo { get; set; }

        [StringLength(500)]
        [Display(Name = "شعار الجامعة")]
        public string LogoHD { get; set; }

        [Required]
        [Range(1000, 10000)]
        [Display(Name = "سنة تأسيس الجامعة")]
        public int YearofEstablishment { get; set; }

        [Display(Name = "حكومية")]
        public bool Governmental { get; set; }

        [Display(Name = "شبه خاصة")]
        public bool SemiPrivate { get; set; }

        [Display(Name = "خاصة")]
        public bool Private { get; set; }

        [Display(Name = "الدولة")]
        public int CountryId { get; set; }
        public  Country Country { get; set; }

        //[Display(Name = "User")]
        //[StringLength(450)]
        //public string ApplicationUserId { get; set; }
       
        [Display(Name = "Indexed")]
        public bool IsIndexed { get; set; }

    }
}

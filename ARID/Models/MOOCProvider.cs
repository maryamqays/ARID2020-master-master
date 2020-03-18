using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class MOOCProvider
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ألاسم مطلوب")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "الاسم ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "البريد الالكتروني مطلوب")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وصف مجهز الخدمة مطلوب")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name = "الموقع الالكتروني")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Website { get; set; }

        [Display(Name = "Facebook")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Fb { get; set; }

        [Display(Name = "Twitter")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Twitter { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }


        [StringLength(250, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الشعار")]
        public string Logo { get; set; }

    }
}

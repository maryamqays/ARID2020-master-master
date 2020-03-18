using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerPortfolio
    {
        [Key]
        public int Id { get; set; }

        [StringLength(450)]
        [Display(Name = "مقدم الطلب")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(500, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 100 حرف", MinimumLength = 10)]
        [Display(Name = "العنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "المهارات")]
        [StringLength(2000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 2000 حرف", MinimumLength = 10)]
        public string Skills { get; set; }

        [Display(Name = "الوصف")]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [StringLength(5000, ErrorMessage = "هذا الحقل يجب ان يتراوح بين 10 الى 5000 حرف", MinimumLength = 10)]
        public string Description { get; set; }

        [Url]
        [StringLength(500, ErrorMessage = "الحد الأقصى لهذا الحقل 500 حرف", MinimumLength = 0)]
        [Display(Name = "رابط يوضح اعمالك في هذا المجال")]
        public string ExternalLink { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [Display(Name = "صورة")]
        public string Image { get; set; }

        [StringLength(100)]
        [Display(Name = "ملف")]
        public string File { get; set; }

        [Display(Name = "إعجاب")]
        public int Likes { get; set; }

        [Display(Name = "عدد المشاهدات")]
        public int Views { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "تاريخ الانجاز")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfAchievement { get; set; }
    }
}

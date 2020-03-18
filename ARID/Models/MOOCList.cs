using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class MOOCList
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError")]
        [Display(Name = "العنوان")]
        public string Title { get; set; }

        [Display(Name = "المنصة")]
        public int MOOCProviderId { get; set; }
        public  MOOCProvider MOOCProvider { get; set; }

        [Required]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

        [Display(Name = "هل هذا المساق يقدم مجانا في المنصة المستضيفة ؟")]
        public bool IsFree { get; set; }

        [Display(Name = "مبلغ عنه")]
        public bool IsReported { get; set; }

        [Required]
        [Display(Name = "اللغة")]
        public Language Language { get; set; }

        [Display(Name = "الشهادة")]
        public Certificate Certificate { get; set; }

        [Required]
        [Display(Name = "المدة")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Duration { get; set; }

        [Display(Name = "الجامعة")]
        public int UniversityId { get; set; }
        public University University { get; set; }

        [Url]
        [Required]
        [Display(Name = "الرابط")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Link { get; set; }

        [Url]
        [Display(Name = "رابط يوتيوب")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string YouTubeLink { get; set; }

        [Display(Name = "الكلمات المفتاحية")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Tags { get; set; }

        [Display(Name = "المحاضر")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Instructor { get; set; }

        [StringLength(250, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "صورة")]
        public string Image { get; set; }

        [Display(Name = "تاريخ البدء")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }


        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "تاريخ الانشاء")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }
    }
}

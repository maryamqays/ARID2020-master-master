using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SuggestedReviewer
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "امكانية التحكيم باللغة الانجليزية؟")]
        public bool CanReviewEnglish { get; set; }

        [Display(Name = "امكانية التحكيم باللغة العربية؟")]
        public bool CanReviewArabic { get; set; }

        [StringLength(450)]
        [Display(Name = "سبب الاقتراح")]
        public string ReasonForSuggesion { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الورقة البحثية")]
        public int ManuscriptId { get; set; }
        public  Manuscript Manuscript { get; set; }

        [Required]
        [Display(Name = "المحكم")]
        [StringLength(450)]
        public string ReviewerUserId { get; set; }
        public ApplicationUser ReviewerUser { get; set; }

        [Required]
        [Display(Name = "حالة الترشيح")]
        public SuggestionStatus SuggestionStatus { get; set; }
    }
}

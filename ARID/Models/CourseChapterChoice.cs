using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseChapterChoice
    {
        [key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(100, ErrorMessage = "عدد الحروف يجب أن يكون بين 5 و 100 حرف", MinimumLength = 5)]
        [Display(Name = "نص الخيار")]
        public string Option { get; set; }

        [Display(Name = "السؤال")]
        public int CourseChapterExamId { get; set; }
        public CourseChapterExam CourseChapterExam { get; set; }

        [Display(Name = "هل هذه الإجابة هي الإجابة الصحيحة")]
        public bool IsCorrectAnswer { get; set; }

        [Display(Name = "محذوفة؟")]
        public bool IsDeleted { get; set; }

        [Display(Name = "الدرجة")]
        public int Mark { get; set; }

    }
}

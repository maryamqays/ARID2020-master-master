using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseChapterExamLog
    {
        [key]
        public int Id { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "تاريخ الاجابة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime AnswerDate { get; set; }

        [Display(Name = "الإختيار")]
        public Guid CourseChapterChoiceId { get; set; }
        public CourseChapterChoice CourseChapterChoice { get; set; }

    }
}

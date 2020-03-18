using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public CourseRegisteration CourseRegisteration { get; set; }
        public CourseSponsor CourseSponsor { get; set; }
        public CourseChapter CourseChapter { get; set; }
        public CourseChapterExam CourseChapterExam { get; set; }
        public CourseChapterChoice CourseChapterChoice { get; set; }
        public CourseChapterContent CourseChapterContent { get; set; }
        public IEnumerable<CourseRegisteration> CourseRegisterations { get; set; }
        public IEnumerable<CourseChapter> CourseChapters { get; set; }
        public IEnumerable<CourseSponsor> CourseSponsors { get; set; }
        public IEnumerable<CourseChapterContent> CourseChapterContents { get; set; }
        public IEnumerable<CourseChapterExam> CourseChapterExams { get; set; }
        public IEnumerable<CourseChapterChoice> CourseChapterChoices { get; set; }

    }
}

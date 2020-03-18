using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseChapterViewModel
    {
        public CourseChapter CourseChapter { get; set; }
        public CourseChapterContent CourseChapterContent { get; set; }
        public IEnumerable<CourseChapter> CourseChapters { get; set; }
        public IEnumerable<CourseChapterContent> CourseChapterContents { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}

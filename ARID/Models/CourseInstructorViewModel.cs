using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseInstructorViewModel
    {
        public CourseInstructor CourseInstructor { get; set; }
        public IEnumerable<CourseInstructor> CourseInstructors { get; set; }
        public IEnumerable<Course> courses { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}

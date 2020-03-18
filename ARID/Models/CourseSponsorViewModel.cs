using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CourseSponsorViewModel
    {
        public CourseSponsor CourseSponsor { get; set; }
        public IEnumerable<CourseSponsor> CourseSponsors { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}

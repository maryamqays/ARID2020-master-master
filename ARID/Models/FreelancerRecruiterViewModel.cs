using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerRecruiterViewModel
    {
        public FreelancerRecruiter FreelancerRecruiter { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers  { get; set; }
        public IEnumerable<FreelancerRecruiter> FreelancerRecruiters { get; set; }
    }
}

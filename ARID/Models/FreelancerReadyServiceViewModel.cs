using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerReadyServiceViewModel
    {
        public FreelancerReadyService FreelancerReadyService { get; set; }
        public Ticket Ticket { get; set; }
        public IEnumerable<FreelancerReadyService> FreelancerReadyServices { get; set; }
        public IEnumerable<FreelancerReadyServiceExtension> FreelancerReadyServiceExtensions { get; set; }
        public IEnumerable<FreelancerRating> FreelancerRatings { get; set; }
    }
}

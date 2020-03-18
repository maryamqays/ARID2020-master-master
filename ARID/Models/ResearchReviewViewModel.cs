using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ResearchReviewViewModel
    {
        public SubmissionReviewer ResearchReview { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SubmissionReviewer> ResearchReviews { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
    }
}

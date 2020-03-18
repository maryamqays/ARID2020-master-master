using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ResearchReviewerViewModel
    {
        public SubmissionReviewer ResearchReview { get; set; }
        public IEnumerable<SubmissionReviewer> ResearchReviews { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}

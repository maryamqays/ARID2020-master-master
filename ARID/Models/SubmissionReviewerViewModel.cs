using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SubmissionReviewerViewModel
    {
        public SubmissionReviewer SubmissionReviewer { get; set; }
        public RegisterFromOutsideViewModel RegisterFromOutsideViewModel { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SubmissionReviewer> SubmissionReviewers { get; set; }
        public IEnumerable<ArticleAuthorship> articleAuthorships { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public IEnumerable<SubmissionFile> SubmissionFiles { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SuggestedReviewerViewModel
    {
        public SuggestedReviewer SuggestedReviewer { get; set; }
        public Manuscript Manuscript { get; set; }
        public RegisterFromOutsideViewModel RegisterFromOutsideViewModel { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SuggestedReviewer> SuggestedReviewers  { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers  { get; set; }
        public IEnumerable<Manuscript> Manuscripts  { get; set; }
        public IEnumerable<ArticleAuthorship> ArticleAuthorships  { get; set; }
        public IEnumerable<SubmissionFile> SubmissionFiles  { get; set; }
        public IEnumerable<Submission> Submissions { get; set; }
        public IEnumerable<JournalFileType> JournalFileTypes { get; set; }
    }
}

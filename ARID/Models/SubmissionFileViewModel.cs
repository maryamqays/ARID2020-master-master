using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SubmissionFileViewModel
    {
        public SubmissionFile SubmissionFile { get; set; }
        public Manuscript manuscript { get; set; }
        public Submission Submission { get; set; }
        public IEnumerable<SubmissionFile> SubmissionFiles { get; set; }
        public IEnumerable<SuggestedReviewer> SuggestedReviewers { get; set; }
        public IEnumerable<ArticleAuthorship> ArticleAuthorships { get; set; }
        public IEnumerable<Submission> Submissions { get; set; }
        public IEnumerable<Manuscript> Manuscripts { get; set; }
        public IEnumerable<JournalFileType> JournalFileTypes { get; set; }
    }
}

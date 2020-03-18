using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ManuscriptViewModel
    {
        public Manuscript Manuscript { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public ArticleAuthorship ArticleAuthorship { get; set; }
        public SuggestedReviewer SuggestedReviewer { get; set; }
        public Submission Submission { get; set; }
        public SubmissionFile SubmissionFile { get; set; }
        public SubmissionReviewer SubmissionReviewer { get; set; }
        public IEnumerable<Manuscript> CoAuthorManuscripts { get; set; }
        public IEnumerable<Manuscript> Manuscripts { get; set; }
        public IEnumerable<Manuscript> ManuscriptsManuscripts { get; set; }
        public IEnumerable<SuggestedReviewer> SuggestedReviewers { get; set; }
        public IEnumerable<ArticleAuthorship> ArticleAuthorships { get; set; }
        public IEnumerable<Submission> Submissions { get; set; }
        public IEnumerable<SubmissionFile> SubmissionFiles { get; set; }
        public IEnumerable<JournalFileType> JournalFileTypes { get; set; }
        public IEnumerable<SubmissionReviewer> SubmissionReviewers { get; set; }
        public IEnumerable<AreaEditor> AreaEditors { get; set; }
    }
}

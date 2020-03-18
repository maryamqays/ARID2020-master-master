using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalViewModel
    {
        public Journal Journal { get; set; }
        public JournalIssue JournalIssue { get; set; }
        public Manuscript Manuscript { get; set; }
        public PagingInfo PagingInfo  { get; set; }
        public ApplicationUser ApplicationUser  { get; set; }
        public List<JournalIssue> JournalIssues { get; set; }
        public IEnumerable<ApplicationUser> JournalReviewers { get; set; }
        public IEnumerable<ApplicationUser> JournalAuthors { get; set; }
        public IEnumerable<ApplicationUser> OtherJournalsAuthors { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<ApplicationUser> OtherJournalsReviewers { get; set; }
        public List<JournalPage> JournalPages { get; set; }
        public IEnumerable<Manuscript> ManuscriptsManuscripts { get; set; }
        public IEnumerable<Manuscript> CoAuthorManuscripts { get; set; }
        public IEnumerable<AreaEditor> AreaEditors { get; set; }
        public IEnumerable<Manuscript> Manuscripts { get; set; }
        public IEnumerable<SubmissionReviewer> SubmissionReviewers { get; set; }
        public IEnumerable<EducationalLevel> EducationalLevels { get; set; }
    }
}

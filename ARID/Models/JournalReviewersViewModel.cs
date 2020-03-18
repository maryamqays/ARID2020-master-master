using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalReviewersViewModel
    {
        public JournalReviewer JournalReviewerEOI  { get; set; }
        public ApplicationUser ApplicationUser  { get; set; }
        public IEnumerable <JournalReviewer> JournalReviewerEOIs  { get; set; }
        public IEnumerable <ApplicationUser> ApplicationUsers  { get; set; }
        public IEnumerable <AreaEditor> AreaEditors { get; set; }
      //  public IEnumerable <ResearchAuthor> ResearchAuthors  { get; set; }
        //public IEnumerable <JournalRepository> JournalRepositories  { get; set; }
        public IEnumerable <JournalFileType> JournalFileTypes  { get; set; }
      //  public IEnumerable <AttachedManuscriptFile> AttachedManuscriptFiles  { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class MOOCListViewModel
    {
        public MOOCList MOOCList { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "دولة الجامعة")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        public IEnumerable<MOOCList> MOOCLists { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}

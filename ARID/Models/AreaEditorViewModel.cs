using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class AreaEditorViewModel
    {
        public AreaEditor AreaEditor { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public IEnumerable<AreaEditor> AreaEditors { get; set; }
    }
}

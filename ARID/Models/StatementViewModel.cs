using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class StatementViewModel
    {
        public Statement Statement { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Statement> Statements { get; set; }
    }
}

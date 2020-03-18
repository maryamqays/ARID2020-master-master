using System;
using System.ComponentModel.DataAnnotations;

namespace ARID.DTOs
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }

        public string ARID { get; set; }

        public string ArName { get; set; }

        public string EnName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string University { get; set; }

        public string Faculty { get; set; }

        public int Visitors { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ArticleType
    {
        [Key]
        public int Id { get; set; }

                [Required]
        [StringLength(100)]
        [Display(Name = "نوع المقالة")]
        public string Type { get; set; }
            }
}

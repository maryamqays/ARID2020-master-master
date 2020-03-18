using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SkillViewModel
    {
        public Skill Skill { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
        public IEnumerable<UserSkill> UserSkills { get; set; }
    }
}

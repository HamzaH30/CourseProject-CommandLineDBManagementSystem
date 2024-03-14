using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    internal class Goal
    {
        // Self Properties
        public int Id { get; set; }

        /*
         * Time is stored as a decimal, because it represented the time within the match that the goal is scored
         * Storing time as total minutes (including seconds as a fraction)
         * 
         * For example, if a goal is scored 87 minutes and 45 seconds into the match, then it would be represented as: 87.75
         */
        public decimal Time { get; set; } 

        // Foreign Properties
        public int PlayerId { get; set; }
        public int MatchId { get; set; }

        // Navigation properties
        public virtual Match Match { get; set; }
        public virtual Player Player { get; set; }
    }
}

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
        public DateTime Time { get; set; }

        // Foreign Properties
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.Models
{
    internal class Match
    {
        // Self Properties
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int Attendance { get; set; }

        // Foreign Properties
        public int StadiumId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }
}

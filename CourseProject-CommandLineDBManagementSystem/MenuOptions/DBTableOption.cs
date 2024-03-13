using MenuClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class DBTableOption : IMenuItem
    {
        public string SelectionText { get; init; }

        public DBTableOption(string selectionText)
        {
            SelectionText = selectionText;
        }

        public void PerformMenuAction()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuClasses;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class CreateEntry : IMenuItem
    {
        public string SelectionText { get; init; }
        public bool continueToCreateEntryMenu;

        public CreateEntry()
        {
            SelectionText = "Create a new entry for a table";
            continueToCreateEntryMenu = false;
        }

        public void PerformMenuAction()
        {
            continueToCreateEntryMenu = true;
        }
    }
}

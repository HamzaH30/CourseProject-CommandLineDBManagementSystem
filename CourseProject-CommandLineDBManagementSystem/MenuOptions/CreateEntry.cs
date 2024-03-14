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
        public Menu MenuInstance { get; init; }
        public bool ContinueToCreateEntryMenu { get; private set; }


        public CreateEntry(Menu menuInstance)
        {
            SelectionText = "Create a new entry for a table";
            ContinueToCreateEntryMenu = false;
            MenuInstance = menuInstance;
        }

        public void PerformMenuAction()
        {
            ContinueToCreateEntryMenu = true;
            MenuInstance.ExitMenu();
        }
    }
}

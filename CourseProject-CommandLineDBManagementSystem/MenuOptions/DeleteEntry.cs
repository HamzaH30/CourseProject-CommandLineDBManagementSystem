using MenuClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject_CommandLineDBManagementSystem.MenuOptions
{
    public class DeleteEntry : IMenuItem
    {
        public string SelectionText { get; init; }
        public Menu MenuInstance { get; init; }
        public bool ContinueToDeleteMenu { get; private set; }

        public DeleteEntry(Menu menuInstance)
        {
            SelectionText = "Delete entries from a Table";
            MenuInstance = menuInstance;
            ContinueToDeleteMenu = false;
        }

        public void PerformMenuAction()
        {
            MenuInstance.ExitMenu();
            ContinueToDeleteMenu = true;
        }
    }
}

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
        private bool _continueToCreateEntryMenu;
        private List<string> _tableNames;


        public CreateEntry(Menu menuInstance, List<string> tableNames)
        {
            SelectionText = "Create a new entry for a table";
            _continueToCreateEntryMenu = false;
            MenuInstance = menuInstance;
            _tableNames = tableNames;
        }

        public void PerformMenuAction()
        {
            MenuInstance.ExitMenu();
            CreateEntryMenu();
        }

        private void CreateEntryMenu()
        {
            Menu chooseTableMenu = new Menu("Create", "Choose a table");
            _tableNames.ForEach(name => chooseTableMenu.AddToMenu(new DBTableOption(name)));
            chooseTableMenu.Display();
        }
    }
}

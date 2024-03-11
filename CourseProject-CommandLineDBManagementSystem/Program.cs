using CourseProject_CommandLineDBManagementSystem.MenuOptions;
using MenuClasses;

namespace CourseProject_CommandLineDBManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu startingMenu = new Menu("Welcome to the Console-Based DBMS", "Choose one of the following options:");
            startingMenu.AddToMenu(new CreateEntry());


        }
    }
}

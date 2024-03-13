using CourseProject_CommandLineDBManagementSystem.Data;
using CourseProject_CommandLineDBManagementSystem.MenuOptions;
using MenuClasses;
using Microsoft.EntityFrameworkCore;

namespace CourseProject_CommandLineDBManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using ApplicationDBContext dbContext = new ApplicationDBContext();

            var tableNames = dbContext.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .Distinct()
                .ToList();

            Menu startingMenu = new Menu("Welcome to the Console-Based DBMS", "Choose one of the following options:");
            startingMenu.AddToMenu(new CreateEntry(startingMenu, tableNames));

            startingMenu.Display();


        }
    }
}

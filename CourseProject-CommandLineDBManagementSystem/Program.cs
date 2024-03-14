using CourseProject_CommandLineDBManagementSystem.Data;
using CourseProject_CommandLineDBManagementSystem.MenuOptions;
using MenuClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.NetworkInformation;
using System.Reflection;

namespace CourseProject_CommandLineDBManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using ApplicationDBContext dbContext = new ApplicationDBContext();



            Menu startingMenu = new Menu("Welcome to the Console-Based DBMS", "Choose one of the following options:");
            CreateEntry createEntry = new CreateEntry(startingMenu);
            startingMenu.AddToMenu(createEntry);
            startingMenu.Display();

            if (createEntry.ContinueToCreateEntryMenu)
            {
                CreateOperation<ApplicationDBContext>();
            }

        }

        private static void CreateOperation<TContext>() where TContext : ApplicationDBContext, new()
        {
            using TContext dbContext = new TContext();

            var entityNames = dbContext.Model.GetEntityTypes()
                .Select(entityType => entityType.ClrType.Name) // Get the name of the class type (ClrType) that EF core is using to map each entity to its corresponding table.
                .Distinct()
                .ToList();

            Menu chooseTableMenu = new Menu("Create", "Choose a table");
            entityNames.ForEach(entityName => chooseTableMenu.AddToMenu(new DBTableOption(entityName, chooseTableMenu, CreateTableEntry)));
            chooseTableMenu.Display();
        }

        /// <summary>
        /// This function is used as a callback function.
        /// 
        /// First, this function finds that table the user is trying to add an entry to.
        /// It does this by finding the entityType.
        /// 
        /// Secondly, it creates a new instance of the selected entity. Representing the user's entry
        /// 
        /// Thirdly, it loops through all the properties (attributes) of the entity and asks to user to give a valid input
        /// 
        /// Lastly, it adds this entity instance to the DbSet and saves changes to add the entry to the Database
        /// </summary>
        /// <param name="entityName"></param>
        public static void CreateTableEntry(string entityName)
        {
            using var dbContext = new ApplicationDBContext();

            // Find the entity type by matching the name of each entityType with the name of the table the user selected : https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api
            var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => entityType.ClrType.Name == entityName);

            if (entityType == null)
            {
                Console.WriteLine("Entity type / table not found");
                return;
            }

            // Create a new instance of the selected entity type
            var entityInstance = Activator.CreateInstance(entityType.ClrType); // ClrType is the class type that the entity is an instance of

            // Loop through all the properties of the entity
            foreach (var prop in entityType.ClrType.GetProperties())
            {
                if (CanUserEditThisProperty(entityType, prop))
                {
                    if (entityName == "Goal" && prop.Name == "Time")
                    {
                        TimeSpan timeOfGoal = PromptUserForGoalTime();

                        // Convert TimeSpan to total minutes as a decimal (minutes + seconds/60)
                        decimal totalMinutes = (decimal)timeOfGoal.TotalMinutes;

                        prop.SetValue(entityInstance, totalMinutes);
                    }
                    else 
                    {
                        // Prompt user
                        Console.WriteLine($"Enter value for {prop.Name} ({prop.PropertyType.Name}): ");
                        string input = Console.ReadLine();

                        // Convert the user's input into the correct datatype for this property
                        var value = Convert.ChangeType(input, prop.PropertyType);

                        // Set the value of the property to what the user has entered in
                        prop.SetValue(entityInstance, value);
                    }
                }
            }

            // Once the user has entered in all the required information, add the entity instance to the DbSet and save changes
            dbContext.Add(entityInstance);
            dbContext.SaveChanges();

            Console.WriteLine("Entity added successfully.");
        }

        private static TimeSpan PromptUserForGoalTime()
        {
            int minutes = 0;
            int seconds = 0;

            // Prompt for minutes with validation
            bool validMinutes = false;
            while (!validMinutes)
            {
                Console.Write("What minute was the goal scored in? ");
                validMinutes = int.TryParse(Console.ReadLine(), out minutes);

                if (!validMinutes || minutes < 0)
                {
                    Console.WriteLine("Please enter a valid non-negative number for minutes.");
                    validMinutes = false;
                }
            }

            // Prompt for seconds with validation
            bool validSeconds = false;
            while (!validSeconds)
            {
                Console.Write("What second was the goal scored in? ");
                validSeconds = int.TryParse(Console.ReadLine(), out seconds);

                if (!validSeconds || seconds < 0 || seconds >= 60)
                {
                    Console.WriteLine("Please enter a valid number for seconds (0-59).");
                    validSeconds = false;
                }
            }

            // Combine into a TimeSpan and return
            return new TimeSpan(0, minutes, seconds);
        }


        /// <summary>
        /// Check if the property is a value type or string, and not part of the primary key
        /// </summary>
        private static bool CanUserEditThisProperty(IEntityType? entityType, PropertyInfo prop)
        {
            return (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string)) &&
                   !IsPropertyAPrimaryKey(entityType, prop);
        }

        private static bool IsPropertyAPrimaryKey(IEntityType entityType, PropertyInfo prop)
        {
            return entityType.GetProperties()
                .Where(p => p.IsPrimaryKey())
                .Select(p => p.Name)
                .ToList()
                .Contains(prop.Name);
        }
    }
}

using CourseProject_CommandLineDBManagementSystem.Data;
using CourseProject_CommandLineDBManagementSystem.MenuOptions;
using MenuClasses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace CourseProject_CommandLineDBManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using ApplicationDBContext dbContext = new ApplicationDBContext();
            bool continueRunning = true;

            while (continueRunning)
            {
                Menu startingMenu = new Menu("Welcome to the Console-Based DBMS", "Choose one of the following options:");
                
                CreateEntry createEntry = new CreateEntry(startingMenu);
                ReadOption readOption = new ReadOption(startingMenu);
                
                startingMenu.AddToMenu(createEntry);
                startingMenu.AddToMenu(readOption);

                startingMenu.Display();

                if (createEntry.ContinueToCreateEntryMenu)
                {
                    CreateOperation<ApplicationDBContext>();
                }
                else if (readOption.ContinueToReadMenu)
                {
                    ReadOperation<ApplicationDBContext>();
                }

                // Check if user requested to exit through the menu
                if (startingMenu.UserRequestedExit)
                {
                    continueRunning = false;
                }
                else
                {
                    // Ask the user if they want to perform another operation, as before
                    Console.Write("Do you want to perform another operation? (y/n): ");
                    string userDecision = Console.ReadLine();

                    if (userDecision != null && userDecision.ToLower() != "y")
                    {
                        continueRunning = false;
                    }
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Simple method to convert PascalCase strings to a more readable format
        /// </summary>
        private static string ToReadableName(string pascalCaseString)
        {
            if (string.IsNullOrEmpty(pascalCaseString))
            {
                return pascalCaseString;
            }

            StringBuilder readableName = new StringBuilder();
            readableName.Append(pascalCaseString[0]);

            for (int i = 1; i < pascalCaseString.Length; i++)
            {
                if (char.IsUpper(pascalCaseString[i]))
                {
                    readableName.Append(' ');
                }
                readableName.Append(pascalCaseString[i]);
            }

            return readableName.ToString();
        }


        private static void ReadOperation<TContext>() where TContext : ApplicationDBContext, new()
        {
            using TContext dbContext = new TContext();

            var entityNames = dbContext.Model.GetEntityTypes()
                .Select(entityType => entityType.ClrType.Name) // Get the name of the class type (ClrType) that EF core is using to map each entity to its corresponding table.
                .Distinct()
                .ToList();

            Menu chooseTableMenu = new Menu("Read", "Choose a table");
            entityNames.ForEach(entityName => chooseTableMenu.AddToMenu(new DBTableOption(ToReadableName(entityName), chooseTableMenu, ReadTableEntry)));
            chooseTableMenu.Display();
        }

        private static void CreateOperation<TContext>() where TContext : ApplicationDBContext, new()
        {
            using TContext dbContext = new TContext();

            var entityNames = dbContext.Model.GetEntityTypes()
                .Select(entityType => entityType.ClrType.Name) // Get the name of the class type (ClrType) that EF core is using to map each entity to its corresponding table.
                .Distinct()
                .ToList();

            Menu chooseTableMenu = new Menu("Create", "Choose a table");
            entityNames.ForEach(entityName => chooseTableMenu.AddToMenu(new DBTableOption(ToReadableName(entityName), chooseTableMenu, CreateTableEntry)));
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
                        Console.Write($"Enter a value for {ToReadableName(prop.Name)} ({prop.PropertyType.Name}): ");
                        string input = Console.ReadLine();

                        // Convert the user's input into the correct datatype for this property
                        var value = Convert.ChangeType(input, prop.PropertyType);

                        // Set the value of the property to what the user has entered in
                        prop.SetValue(entityInstance, value);
                    }
                }
            }

            try
            {
                // Once the user has entered in all the required information, add the entity instance to the DbSet and save changes
                dbContext.Add(entityInstance);
                dbContext.SaveChanges();
                Console.WriteLine("----\r\nThe record was inserted into the database.");
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception is due to a duplicate key
                if (IsDuplicateKeyException(ex))
                {
                    Console.WriteLine("Error: A record with the same key already exists in the database.");
                }
                else
                {
                    // Handle or log other database update exceptions
                    Console.WriteLine("An error occurred while updating the database. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("An unexpected error occurred.");
            }
        }

        public static void ReadTableEntry(string entityName)
        {
            using var dbContext = new ApplicationDBContext();

            /*
             * Find the entity type that corresponds to the provided table name. Referenced from: https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api
             * (Entity types are classes mapped to the database tables)
             */
            var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => entityType.ClrType.Name == entityName);

            // If the entity type isn't found, it prints an error message and returns early.
            if (entityType == null)
            {
                Console.WriteLine("Entity type / table not found");
                return;
            }

            /*
             * Find the DbSet property (DbSet<entityType>) within the DbContext that corresponds to the entity name.
             * 
             * Referenced from https://dev.to/nakullukan/how-to-crud-dynamically-in-entity-framework-3fi3
             */
            var dbSetProperty = dbContext.GetType().GetProperty(entityName + "s"); // The DbSet is named as plural of entity, e.g., "Teams" for "Team" entity

            // If the DbSet property isn't found, it prints an error message and returns early.
            if (dbSetProperty == null)
            {
                Console.WriteLine($"DbSet<{entityName}> not found.");
                return;
            }

            // Retrieves the value of the DbSet property, which is an IQueryable representing the collection of entities in the database.
            IEnumerable dbSet = (IEnumerable)dbSetProperty.GetValue(dbContext);

            // Retrieve a list of navigation property names to filter them out later
            List<string> navigationProperties = entityType.GetNavigations().Select(navigation => navigation.Name).ToList();

            Console.WriteLine($"\n\rEntries in {ToReadableName(entityName)}: ");

            // Iterates over each entity in the DbSet property
            foreach (var entity in dbSet)
            {
                // Retrieves the properties of the current entity (the properties in the corresponding class, e.g. Team or Stadium)
                PropertyInfo[] entityProperties = entity.GetType().GetProperties();

                foreach (var prop in entityProperties)
                {
                    // Skip navigation properties
                    if (!navigationProperties.Contains(prop.Name))
                    {
                        var propValue = prop.GetValue(entity);
                        Console.WriteLine($"{ToReadableName(prop.Name)}: {propValue}");
                    }
                }
                Console.WriteLine("-----------");
            }
        }

        /// <summary>
        /// Referenced from: https://stackoverflow.com/questions/3967140/duplicate-key-exception-from-entity-framework
        /// </summary>
        private static bool IsDuplicateKeyException(DbUpdateException exception)
        {
            // Check if the InnerException exists and is of type SqlException
            if (exception.InnerException != null && exception.InnerException.GetType() == typeof(SqlException))
            {
                // Explicitly cast the InnerException to a SqlException
                SqlException sqlEx = (SqlException)exception.InnerException;

                // 2627 is 'Unique constraint violation', 2601 is 'Cannot insert duplicate key row'
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;
            }

            return false;
        }

        /// <summary>
        /// This is a specific method for when the user wants to write an entry into the Goal table.
        /// It prompts the user specific information regarding the time of the goal.
        /// </summary>
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

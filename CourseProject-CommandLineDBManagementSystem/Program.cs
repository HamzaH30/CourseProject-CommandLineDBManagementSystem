/*
 * Name: Hamza Haque
 * Course: Advanced Database and ORM Concepts
 * Assignment: Advanced Database Course Project: Command Line Database Management System
 */

using CourseProject_CommandLineDBManagementSystem.Data;
using CourseProject_CommandLineDBManagementSystem.MenuOptions;
using MenuClasses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CourseProject_CommandLineDBManagementSystem
{
    internal class Program
    {
        /*
         * TODO:
         * - Create a separate file that creates some temporary starting data, instead of having to manually create data
         * - Finish the Update & Delete operations
         * - Complete README + documentation
         */

        static void Main(string[] args)
        {
            using ApplicationDBContext dbContext = new ApplicationDBContext();
            bool continueRunning = true;

            while (continueRunning)
            {
                // Final try-catch. Meant to catch any specific errors that have not been handled in any of the numerous methods.
                try
                {
                    InitializeStartingMenu(out Menu startingMenu, out CreateEntry createEntry, out ReadOption readOption, out UpdateOption updateOption, out DeleteEntry deleteEntry);
                    startingMenu.Display();
                    HandleStartingMenuOptions(createEntry, readOption, updateOption, deleteEntry);

                    // Check if user requested to exit through the menu
                    if (startingMenu.UserRequestedExit)
                    {
                        continueRunning = false;
                    }
                    else
                    {
                        continueRunning = AskToContinue();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured. Please try again.\r\nError: {ex.Message}\r\n");
                }
            }
        }

        private static void HandleStartingMenuOptions(CreateEntry createEntry, ReadOption readOption, UpdateOption updateOption, DeleteEntry deleteEntry)
        {
            if (createEntry.ContinueToCreateEntryMenu)
            {
                CreateOperation<ApplicationDBContext>();
            }
            else if (readOption.ContinueToReadMenu)
            {
                ReadOperation<ApplicationDBContext>();
            }
            else if (updateOption.ContinueToUpdateMenu)
            {
                UpdateOperation<ApplicationDBContext>();
            }
            else if (deleteEntry.ContinueToDeleteMenu)
            {
                DeleteOperation<ApplicationDBContext>();
            }
        }

        private static void InitializeStartingMenu(out Menu startingMenu, out CreateEntry createEntry, out ReadOption readOption, out UpdateOption updateOption, out DeleteEntry deleteEntry)
        {
            startingMenu = new Menu("Welcome to the Console-Based DBMS", "Choose one of the following options:");

            createEntry = new CreateEntry(startingMenu);
            readOption = new ReadOption(startingMenu);
            updateOption = new UpdateOption(startingMenu);
            deleteEntry = new DeleteEntry(startingMenu);

            startingMenu.AddToMenu(createEntry);
            startingMenu.AddToMenu(readOption);
            startingMenu.AddToMenu(updateOption);
            startingMenu.AddToMenu(deleteEntry);
        }

        private static bool AskToContinue()
        {
            Console.Write("> Do you want to perform another operation? (y/n): ");
            var userDecision = Console.ReadLine()?.Trim().ToLower();

            if (userDecision == "y")
            {
                Console.WriteLine();
                return true;
            }
            else if (userDecision == "n")
            {
                Console.WriteLine();
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'y' for yes or 'n' for no.");

                // Recursively call until valid input is received
                return AskToContinue();
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

            Menu chooseTableMenu = new Menu("==Read==", "Choose a table");
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

            Menu chooseTableMenu = new Menu("==Create==", "Choose a table");
            entityNames.ForEach(entityName => chooseTableMenu.AddToMenu(new DBTableOption(ToReadableName(entityName), chooseTableMenu, CreateTableEntry)));
            chooseTableMenu.Display();
        }

        /// <summary>
        /// Displays a menu to update an entry in any table that contains at least one entry. 
        /// It dynamically generates a list of tables (DbSets) that have entries and allows the user to choose one for updating.
        /// Upon choosing a table, it further navigates to updating a specific entry within that table.
        /// 
        /// This method ensures that the user can only attempt to update tables that are not empty.
        /// </summary>
        private static void UpdateOperation<TContext>() where TContext : ApplicationDBContext, new()
        {
            using TContext dbContext = new TContext();

            Menu chooseTableMenu = new Menu("==Update==", "Choose a table");

            /*
             * Filter out all properties of the dbContext object that are generic types and specifically are of the type DbSet<>, 
             * regardless of what type DbSet<> is holding.
             */
            var allDbSetProps = dbContext.GetType().GetProperties()
                .Where(
                    prop => prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                );

            foreach (var dbSetPropInfo in allDbSetProps)
            {
                // Retrieve the value of the DbSet property, which is an IQueryable representing the collection of entries in this table (DbSet<entityType> collection is the code representation of this table).
                IEnumerable entities = (IEnumerable)dbSetPropInfo.GetValue(dbContext);

                // Get the names of tables that have at least one data entry by checking if there are any elements in the entities collection
                if (entities.Cast<object>().Any())
                {
                    // Only create a menu item for those tables that have at least one data entry
                    chooseTableMenu.AddToMenu(new DBTableOption(ToReadableName(dbSetPropInfo.Name.TrimEnd('s')), chooseTableMenu, UpdateTableEntry));
                }
            }

            chooseTableMenu.Display();
        }

        private static void DeleteOperation<TContext>() where TContext : ApplicationDBContext, new()
        {
            using TContext dbContext = new TContext();

            // First ask the user what table they want to select
            var entityNames = dbContext.Model.GetEntityTypes()
                .Select(entityType => entityType.ClrType.Name) // Get the name of the class type (ClrType) that EF core is using to map each entity to its corresponding table.
                .Distinct()
                .ToList();

            Menu chooseTableMenu = new Menu("==Delete Entry==", "Choose a table");
            entityNames.ForEach(entityName => chooseTableMenu.AddToMenu(new DBTableOption(ToReadableName(entityName), chooseTableMenu, DeleteTableEntry)));
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
            var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => ToReadableName(entityType.ClrType.Name) == entityName);

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
                        Console.Write($"> Enter a value for {ToReadableName(prop.Name)} ({prop.PropertyType.Name}): ");
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

        /// <summary>
        /// This function updates a specific entry in a given table (entity) within the database by allowing the user to select and modify one of its properties.
        /// This function dynamically fetches the entity type based on the provided entity name and displays a menu of editable properties to the user.
        /// The user can then select a property to update, enter a new value for it, and save the changes to the database.
        /// </summary>
        public static void UpdateTableEntry(string entityName)
        {
            using var dbContext = new ApplicationDBContext();

            // Find the entity type by matching the name of each entityType with the name of the table the user selected : https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api
            var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => ToReadableName(entityType.ClrType.Name) == entityName);

            if (entityType == null)
            {
                Console.WriteLine("Entity type / table not found");
                return;
            }

            // Get the primary key property (the schema shows that there are no composite keys, they are all surrogate keys)
            var primaryKeyProp = entityType.FindPrimaryKey()?.Properties.FirstOrDefault()?.PropertyInfo;

            // Error-handling
            if (primaryKeyProp == null)
            {
                Console.WriteLine($"No primary key found for {entityName}.");
                return;
            }

            // Find which entry the user wants to edit
            Console.WriteLine($"Updating an entry in {ToReadableName(entityName)}:");
            Console.Write($"> Enter the {ToReadableName(primaryKeyProp.Name)} of the entry you wish to edit: ");
            string keyValue = Console.ReadLine();

            /*
             * All the primary keys are integers according to the DB Schema. 
             * In the future, to expand, if there is a primary key of another datatype, then we could use Convert.ChangeType()
             */
            int primaryKey = int.Parse(keyValue);

            var dataEntityToUpdate = dbContext.Find(entityType.ClrType, primaryKey);

            // Error-handling
            if (dataEntityToUpdate == null)
            {
                Console.WriteLine("Entry not found!");
                return;
            }

            // Get the properties of this data entry in the table (entity) that the user can edit
            var editableProps = entityType.ClrType.GetProperties()
                .Where(p => p.Name != primaryKeyProp.Name && CanUserEditThisProperty(entityType, p)) // Explicitly making sure that the primary key property is not editable by the user
                .ToList();

            if (editableProps.Count == 0)
            {
                Console.WriteLine("There are no editable properties for this entry.");
                return;
            }

            // Showing the update menu and prompting the user to update the property
            Menu choosePropertyMenu = new Menu("Update", "Choose a property to update:");

            foreach (var prop in editableProps)
            {
                // Pass the callback function in.
                choosePropertyMenu.AddToMenu(new EditablePropertyOption(choosePropertyMenu, ToReadableName(prop.Name), () =>
                {
                    /*
                     * This lambda expression is "capturing" / storing the "prop" reference for each EditablePropertyOption instance.
                     * Therefore, the callback function doesn't need to accept any parameters due to the way lambda expressions work.
                     */

                    // Specific method of updating the time of goal property
                    if (entityName == "Goal" && prop.Name == "Time")
                    {
                        TimeSpan timeOfGoal = PromptUserForGoalTime();
                        decimal totalMinutes = (decimal)timeOfGoal.TotalMinutes;
                        prop.SetValue(dataEntityToUpdate, totalMinutes);
                    }
                    else
                    {
                        // For all other properties, prompt the user to give a new value
                        Console.Write($"> Enter a new value for {ToReadableName(prop.Name)} ({prop.PropertyType.Name}) [Current value: {prop.GetValue(dataEntityToUpdate)}]: ");
                        string input = Console.ReadLine();
                        var value = Convert.ChangeType(input, prop.PropertyType);
                        prop.SetValue(dataEntityToUpdate, value);
                    }

                    // Error-handling
                    try
                    {
                        dbContext.Update(dataEntityToUpdate);
                        dbContext.SaveChanges();
                        Console.WriteLine("The record has been updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while updating the record: {ex.Message}");
                    }
                }));
            }

            choosePropertyMenu.Display();
        }

        /// <summary>
        /// This function is used as a callback function.
        /// 
        /// First, given the name of the table that the user wants to view, it finds the corresponding entity type (class).
        /// 
        /// Secondly, it will find the corresponding DbSet property (e.g, DbSet<Goal>, DbSet<Team>, etc.) that stores a collection
        /// of all the entries (entities) in this table
        /// 
        /// Finally, it will print out the entries in a readable format.
        /// </summary>
        /// <param name="entityName"></param>
        public static void ReadTableEntry(string entityName)
        {
            using var dbContext = new ApplicationDBContext();

            /*
             * Step 1: Find the corresponding entity type for the given name. Referenced from: https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api
             * (Entity types are classes mapped to the database tables)
             */
            var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => ToReadableName(entityType.ClrType.Name) == entityName);

            // If the entity type isn't found, it prints an error message and returns early.
            if (entityType == null)
            {
                Console.WriteLine("Entity type / table not found");
                return;
            }

            /*
             * Step 2A: Find the DbSet property (DbSet<entityType>) within the DbContext that corresponds to the entity name.
             * 
             * Referenced from https://dev.to/nakullukan/how-to-crud-dynamically-in-entity-framework-3fi3
             */
            // Ensure entityName is one word by removing spaces
            string dbSetName = entityName.Replace(" ", "") + "s";
            var dbSetProperty = dbContext.GetType().GetProperty(dbSetName); // The DbSet is named as plural of entity, e.g., "Teams" for "Team" entity

            // If the DbSet property isn't found, it prints an error message and returns early.
            if (dbSetProperty == null)
            {
                Console.WriteLine($"DbSet<{entityName}> not found.");
                return;
            }

            // Step 2B: Retrieve the value of the DbSet property, which is an IQueryable representing the collection of entities in the database.
            IEnumerable dbSet = (IEnumerable)dbSetProperty.GetValue(dbContext);


            // Step 3: Print the entries in a readable format.
            Console.WriteLine($"\n\rEntries in {ToReadableName(entityName)}: ");
            PrintTableEntries(entityType, dbSet);
        }

        /// <summary>
        /// This function is used as a callback function.
        /// 
        /// Deletes an entry from a specified entity table based on the entity name and a user-specified ID.
        /// </summary>
        public static void DeleteTableEntry(string entityName)
        {
            using var dbContext = new ApplicationDBContext();

            // Attempt to find the entityType and dbSet using the provided entity name.
            var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => ToReadableName(entityType.ClrType.Name) == entityName);

            if (entityType == null)
            {
                Console.WriteLine("Entity type/table not found.");
                return;
            }

            string dbSetName = entityName.Replace(" ", "") + "s"; // Entity names are singular and DbSet names are plural.

            // Case the result to IEnumerable. If the casting fails, dbSet will be null
            IEnumerable? dbSet = dbContext.GetType().GetProperty(dbSetName)?.GetValue(dbContext) as IEnumerable;
            
            if (dbSet == null)
            {
                Console.WriteLine($"DbSet<{entityName}> not found.");
                return;
            }

            // Display the entries to the user.
            PrintTableEntries(entityType, dbSet);

            // Prompt for the primary key of the entry to delete.
            var primaryKeyProp = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();
            
            if (primaryKeyProp == null)
            {
                Console.WriteLine($"No primary key found for {entityName}.");
                return;
            }

            Console.Write($"\r\n> Enter the ID of the entry you wish to delete, or enter 0 to cancel: ");

            if (!int.TryParse(Console.ReadLine(), out int primaryKeyValue))
            {
                Console.WriteLine("Invalid input. Operation aborted.");
                return;
            }

            // Check if user wants to cancel the operation
            if (primaryKeyValue == 0)
            {
                Console.WriteLine("Operation cancelled. Returning to the main menu.");
                return;
            }

            // Find the entity by its primary key.
            var entityToRemove = dbContext.Find(entityType.ClrType, primaryKeyValue);
            
            if (entityToRemove == null)
            {
                Console.WriteLine("Entry not found!");
                return;
            }

            // Remove the entity from the DbSet and save changes.
            dbContext.Remove(entityToRemove);
            dbContext.SaveChanges();

            Console.WriteLine("Entry successfully deleted.");
        }

        /// <summary>
        /// This function is responsible for printing out the entries of a table  in the database in a readable, printable table.
        /// 
        /// Firstly, it determines the maximum width of each column.
        /// Then, it prints the header and data rows.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dbSet"></param>
        private static void PrintTableEntries(IEntityType? entityType, IEnumerable dbSet)
        {
            // Retrieve a list of navigation property names to filter them out later
            List<string> navigationProperties = entityType.GetNavigations().Select(navigation => navigation.Name).ToList();

            // Determine the maximum width for each column (looking at the headers and cells)
            Dictionary<string, int> maxColumnWidths = DetermineMaxColWidth(entityType, dbSet, navigationProperties);

            // Print header row
            int colGap = PrintHeader(maxColumnWidths);

            // Print the data rows
            PrintDataRows(dbSet, navigationProperties, maxColumnWidths, colGap);
        }

        private static void PrintDataRows(IEnumerable dbSet, List<string> navigationProperties, Dictionary<string, int> maxColumnWidths, int colGap)
        {
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

                        // Print each value, padded to the right to align with the column width
                        Console.Write($"{propValue.ToString().PadRight(maxColumnWidths[prop.Name] + colGap)}|");
                    }
                }

                // New line after printing each entity's data
                Console.WriteLine();
            }
        }

        private static int PrintHeader(Dictionary<string, int> maxColumnWidths)
        {
            int colGap = 2;

            // Print the header row
            foreach (string columnName in maxColumnWidths.Keys)
            {
                // Add padding for readability, creating a small gap between columns.
                Console.Write($"{ToReadableName(columnName).PadRight(maxColumnWidths[columnName] + colGap)}|");
            }

            // New line after printing the header
            Console.WriteLine();

            // Print a divider
            foreach (int width in maxColumnWidths.Values)
            {
                Console.Write($"{new string('-', width + colGap)}+");
            }

            // New line after printing the divider
            Console.WriteLine();
            return colGap;
        }

        /// <summary>
        /// This function calculates the maximum column widths for a table display by considering both 
        /// the property names (headers) and their values across all entities, excluding navigation properties. 
        /// 
        /// It initializes each column width based on the property name length and then updates these widths based on the 
        /// longest value found for each property in the dataset, 
        /// returning a dictionary mapping property names to their maximum widths.
        /// </summary>
        private static Dictionary<string, int> DetermineMaxColWidth(IEntityType? entityType, IEnumerable dbSet, List<string> navigationProperties)
        {
            // Dictionary to hold the maximum length of values for each property
            Dictionary<string, int> maxColumnWidths = new Dictionary<string, int>();

            // Including the headers in the width calculation
            foreach (var prop in entityType.ClrType.GetProperties())
            {
                if (!navigationProperties.Contains(prop.Name))
                {
                    // Initialize each column width with the length of the property name (header name)
                    maxColumnWidths[prop.Name] = ToReadableName(prop.Name).Length;
                }
            }

            // Iterate over each entity to find the maximum length of values for each property
            foreach (var entity in dbSet)
            {
                /* Note:
                 * entity.GetType().GetProperties() and entityType.ClrType.GetProperties() virtually do the same thing.
                 * 
                 * The first way is a .NET specific approach, and can be used on any object.
                 * The second way is an EF core specific approach, and can only be used (the ClrType) within the context of the EF core model
                 */
                foreach (var prop in entity.GetType().GetProperties())
                {
                    if (!navigationProperties.Contains(prop.Name))
                    {
                        var propValue = prop.GetValue(entity);
                        int valueLength = propValue.ToString().Length;

                        if (valueLength > maxColumnWidths[prop.Name])
                        {
                            // Update if the current value is longer
                            maxColumnWidths[prop.Name] = valueLength;
                        }
                    }
                }
            }

            return maxColumnWidths;
        }

        /// <summary>
        /// Referenced from: https://stackoverflow.com/questions/3967140/duplicate-key-exception-from-entity-framework
        /// 
        /// This function checks if a DbUpdateException thrown by Entity Framework is due to a SQL unique constraint violation, 
        /// specifically identifying exceptions related to duplicate key errors by examining SQL error numbers 2627 and 2601.
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
        /// This is a specific method for when the user wants to write an entry or update an entry into the Goal table.
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
                Console.Write("> What minute was the goal scored in? ");
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
                Console.Write("> What second was the goal scored in? ");
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
using System;
using Spectre.Console;
using Newtonsoft.Json;

namespace Program
{

    class Task {

        public int    priority               {get;set;}
        public string task                   {get;set;}
        public string note                   {get;set;}
        public string status                 {get;set;}
        public string category               {get;set;}
        public string date                   {get;set;}
        public int    estimated_time         {get;set;}
    }

  class Program
  {

    // Global Variables
    public static List<Task> tasks = new List<Task>();
    public static string notification = "";
    public static string orderBy = "Priority";
    public static string searchKeyword = "";









    static List<Task> GetTasksFromJSON(string path) {

        string jsonString = File.ReadAllText(path);
        // Task task = JsonConvert.DeserializeObject<Task>(jsonString);
        List<Task> task = JsonConvert.DeserializeObject<List<Task>>(jsonString);
        return task;
    }

    static string CountFinishedTask(List<Task> taskList) {

        int counter = 0;

        foreach (var item in taskList)
        {
            if (item.status == "DONE") {
                counter += 1;
            }
        }

        return counter.ToString();
    }

    static string CountUnfinishedTask(List<Task> taskList) {

        int counter = 0;

        foreach (var item in taskList)
        {
            if (item.status != "DONE") {
                counter += 1;
            }
        }

        return counter.ToString();
    }

    static void Main(string[] args)
    {
        ShowMainScreen();
    }

    static void ShowMainScreen() {

        // Clear all console
        Console.Clear();


        AnsiConsole.Markup("\n \n");

        AnsiConsole.Write(
            new FigletText("Raldin Casidar Studio")
                .Centered()
                .Color(Color.Green)
        );

        AnsiConsole.Markup("\n");
        AnsiConsole.Write( new Rule("[red]Task Management System v2.0[/]") );
        AnsiConsole.Markup("\n");

        var panel = new Panel("[lime]:information: What is this program?[/]: [white]This program is designed to manage your task list with ease. It is designed to be a simple and efficient tool.[/]\n\n[lime]:information: Developed by[/]: [white]Raldin Casidar Disomimba of BSCS-1A[/]");
        panel.Header = new PanelHeader("Information");
        panel.Padding = new Padding(1, 1, 1, 1);
        panel.Expand = true;

        AnsiConsole.Write(panel);



        // Ask for user action
        var userAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select your [red]action[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(new[] {

                    "Manage Task",
                    "Credits",
                    "Exit",

                })
        );

        if (userAction == "Manage Task") { ShowManageTaskScreen(); }
        if (userAction == "Credits") { ShowCreditScreen(); }
    }



    static void ShowManageTaskScreen() {

        Console.Clear();

        AnsiConsole.Markup("\n \n");


        // ================== TITLE ==================

        var TitleRule = new Rule("[red]TASK MANAGEMENT SYSTEM[/]");
        AnsiConsole.Write(TitleRule);

        // Margin
        AnsiConsole.Write("\n");

        // Get the data from the JSON file
        tasks = GetTasksFromJSON("Tasks.json");


        // Order by the selected option
        if (orderBy == "Priority") { tasks = tasks.OrderBy(x => x.priority).ToList(); }
        if (orderBy == "Name") { tasks = tasks.OrderBy(x => x.task).ToList(); }
        if (orderBy == "Date") { tasks = tasks.OrderBy(x => x.date).ToList(); }
        if (orderBy == "Category") { tasks = tasks.OrderBy(x => x.category).ToList(); }
        if (orderBy == "Status") { tasks = tasks.OrderBy(x => x.status).ToList(); }
        if (orderBy == "Estimated Time") { tasks = tasks.OrderBy(x => x.estimated_time).ToList(); }

        if (searchKeyword != "") {
            tasks = tasks.Where(x => 
                // Search for a keyword in all fields
                x.task.ToLower().Contains(searchKeyword.ToLower()) || 
                x.note.ToLower().Contains(searchKeyword.ToLower()) || 
                x.category.ToLower().Contains(searchKeyword.ToLower()) || 
                x.status.ToLower().Contains(searchKeyword.ToLower()) || 
                x.date.ToLower().Contains(searchKeyword.ToLower()) || 
                x.estimated_time.ToString().ToLower().Contains(searchKeyword.ToLower()) || 
                x.priority.ToString().ToLower().Contains(searchKeyword.ToLower())
            ).ToList();
        }


        // ================== GRID OF TOTAL NUMBERS ==================

        var grid = new Grid();

        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();

        // Add header row 
        grid.AddRow(new FigletText[]{
            new FigletText(tasks.Count.ToString()).Color(Color.Blue).Centered(), 
            new FigletText(CountFinishedTask(tasks)).Color(Color.Green).Centered(), 
            new FigletText(CountUnfinishedTask(tasks)).Color(Color.Red).Centered()
        });
        grid.AddRow(new Text[]{
            new Text("Total Task").Centered(), 
            new Text("Finished").Centered(), 
            new Text("Not Finished").Centered()
        });


        // Write to Console
        AnsiConsole.Write(grid);

        // Margin
        AnsiConsole.Write("\n\n");



        // ================== TABLE OF TASKS ==================

        var table = new Table();

        string title = (searchKeyword != "") ? $"[yellow]Search for: {searchKeyword}[/]" : $"[yellow]Task Lists [/][blue]Ordered by: {orderBy}[/]";

        // Styles
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.White);
        table.Expand();
        table.Title( title );
        table.ShowRowSeparators();

        // Headers
        table.AddColumn("[yellow]Priority[/]");
        table.AddColumn("[yellow]Status[/]");
        table.AddColumn("[yellow]Task[/]");
        table.AddColumn("[yellow]Category[/]");
        table.AddColumn("[yellow]Due Date[/]");
        table.AddColumn("[yellow]Estimated Time (in minutes)[/]");


        // Display the task data in the table
        foreach (var task in tasks)
        {
            string isDone = (task.status == "DONE") ? "green" : "red";

            table.AddRow(task.priority.ToString(), $"{task.task}\n[grey][yellow]Note:[/] {task.note}[/]", $"[{isDone}]{task.status}[/]", $"[green]{task.category}[/]", $"[dodgerblue1]{task.date}[/]", $"[hotpink_1]{task.estimated_time}[/]");
        }

        // Render the table
        AnsiConsole.Write(table);


        // ================== SHOW NOTIFICATION ==================

        if (notification != "") {

            AnsiConsole.Markup("\n");

            var notificationPanel = new Panel($"[green]{notification}[/]");
            notificationPanel.Header = new PanelHeader("Notification");
            notificationPanel.Padding = new Padding(2, 2, 2, 2);
            notificationPanel.BorderColor(Color.Green);
            notificationPanel.Expand = true;


            AnsiConsole.Write(notificationPanel);

            // Margin
            AnsiConsole.Write("\n\n");


            notification = "";

        }



        // ================== ASK FOR USER ACTION ==================

        if (searchKeyword != "") { 
            var userAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nSelect your [red]action[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(new[] {
                    "Go back"
                })
            );

                searchKeyword = "";

                ShowManageTaskScreen();
        } 
        else 
        {
            var userAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select your [red]action[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                    .AddChoices(new[] {

                        "Mark as DONE",
                        "Mark as TODO",
                        "Add Task",
                        "Edit Task",
                        "Delete Task",
                        "Search Tasks",
                        "Sort by Priority",
                        "Sort by Task Name",
                        "Sort by Category",
                        "Sort by Status",
                        "Sort by Due Date",
                        "Sort by Estimated Time",
                        "Export to TXT File",
                        "Go Back"

                    })
            );


            if (userAction == "Mark as DONE") { MarkAsDone(); }
            if (userAction == "Mark as TODO") { MarkAsTodo(); }
            if (userAction == "Add Task") { ShowAddTaskScreen(); }
            if (userAction == "Edit Task") { ShowEditTaskScreen(); }
            if (userAction == "Delete Task") { ShowDeleteTaskScreen(); }
            if (userAction == "Search Tasks") { ShowSearchTaskScreen(); }
            if (userAction == "Sort by Priority") { orderBy = "Priority"; ShowManageTaskScreen(); }
            if (userAction == "Sort by Task Name") { orderBy = "Name"; ShowManageTaskScreen(); }
            if (userAction == "Sort by Category") { orderBy = "Category"; ShowManageTaskScreen(); }
            if (userAction == "Sort by Due Date") { orderBy = "Date"; ShowManageTaskScreen(); }
            if (userAction == "Sort by Status") { orderBy = "Status"; ShowManageTaskScreen(); }
            if (userAction == "Sort by Estimated Time") { orderBy = "Estimated Time"; ShowManageTaskScreen(); }
            if (userAction == "Export to TXT File") { ExportToTXTFile(); }
            // if (userAction == "Credits") { ShowCreditScreen(); }
            if (userAction == "Go Back") { ShowMainScreen(); }

        }

    }

    static void ExportToTXTFile() {

        string text = @"
            TASK MANAGEMENT SYSTEM V2.0 (Raldin Casidar Studios)            
        ============================================================\n\n";

        foreach (var task in tasks) {
            text = text + @$"
==================
Priority: {task.priority}
Task: {task.task}
Category: {task.category}
Status: {task.status}
Date: {task.date}
Estimated Time: {task.estimated_time}
Note: {task.note}
==================

            ";
        }

        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
            "Tasks List.txt");

        File.WriteAllText(filePath, text);

        notification = "File exported successfuly on Desktop: (Tasks List.txt)";


        ShowManageTaskScreen();
    }


    static void ShowSearchTaskScreen() {

        string searchPrompt = AnsiConsole.Ask<string>("[green]Search tasks on any field? [yellow](Leave blank to show all)[/][/]");
        

        searchKeyword = searchPrompt;

        ShowManageTaskScreen();

    }


    static void ShowAddTaskScreen() {

        AnsiConsole.Markup( "\n" );
        AnsiConsole.Write( new Rule("[green]Add New Task[/]") );
        AnsiConsole.Markup( "\n" );

        string name = AnsiConsole.Ask<string>("[green]What is your task Name?[/]");
        string category = AnsiConsole.Ask<string>("[green]Cool! What category is it?[/]");
        string due_date = AnsiConsole.Ask<string>("[green]When is the due date? [yellow](YYYY-MM-DD)[/][/]");
        string estimated_time = AnsiConsole.Ask<string>("[green]What is your estimated time? [yellow](in minutes)[/][/]");
        string note = AnsiConsole.Ask<string>("[green]What is your task note/description?[/]");


        // Add new task
        tasks.Add( new Task() { priority = tasks.Count + 1, task = name, category = category, status = "TODO", date = due_date, estimated_time = Int32.Parse(estimated_time), note = note } );

        // Save to JSON file
        SaveTaskChangesToJSON("Tasks.json");


        // Display success notification
        notification = "Task added successfully!";

        // Go back to managing tasks
        ShowManageTaskScreen();
    }

    static Task GetTaskWhereNameIs(string name) {

        foreach (var task in tasks) {
            if (task.task == name) {
                return task;
            }
        }

        return new Task();
    }

    static int GetTaskIndexWhereNameIs(string name) {

        return tasks.FindIndex(c => c.task == name);

        // return new Task();
    }

    static void ShowEditTaskScreen() {

        List<string> choices = new List<string>();

        foreach (var task in tasks) {
            choices.Add(task.task);
        }

        // Cancel button
        choices.Add("Cancel");

        var whichTask = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select which [red]task[/] you want to [yellow]edit[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(choices)
        );


        if (whichTask == "Cancel") { ShowManageTaskScreen(); return; }


        Task selectedTask = GetTaskWhereNameIs(whichTask);


        AnsiConsole.Markup( "\n" );
        AnsiConsole.Write( new Rule("[green]Edit Task Information[/]") );
        AnsiConsole.Markup( "\n" );
        AnsiConsole.Markup( "[yellow]Tips:[/] You can leave blank to the field if you dont want to change it's value" );
        AnsiConsole.Markup( "\n" );


        string name = AnsiConsole.Ask<string>("[green]What is your task Name?[/]", selectedTask.task);
        string category = AnsiConsole.Ask<string>("[green]Cool! What category is it?[/]", selectedTask.category);
        string due_date = AnsiConsole.Ask<string>("[green]When is the due date? [yellow](YYYY-MM-DD)[/][/]", selectedTask.date);
        string estimated_time = AnsiConsole.Ask<string>("[green]What is your estimated time? [yellow](in minutes)[/][/]", selectedTask.estimated_time.ToString());
        string note = AnsiConsole.Ask<string>("[green]What is your task note/description?[/]", selectedTask.note);


        // Edit task
        selectedTask.task = name;
        selectedTask.category = category;
        selectedTask.date = due_date;
        selectedTask.estimated_time = Int32.Parse(estimated_time);
        selectedTask.note = note;

        // Add new task
        tasks[GetTaskIndexWhereNameIs(selectedTask.task)] = selectedTask;

        // Save to JSON file
        SaveTaskChangesToJSON("Tasks.json");


        // Display success notification
        notification = "Task edited successfully!";

        // Go back to managing tasks
        ShowManageTaskScreen();
    }

    static void MarkAsDone() {

        List<string> choices = new List<string>();

        foreach (var task in tasks.Where(x => x.status == "TODO")) {
            choices.Add(task.task);
        }

        // Cancel button
        choices.Add("Cancel");

        var whichTask = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select which [red]task[/] you want to [yellow]mark as DONE[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(choices)
        );


        if (whichTask == "Cancel") { ShowManageTaskScreen(); return; }


        Task selectedTask = GetTaskWhereNameIs(whichTask);
        // Edit task
        selectedTask.status = "DONE";

        // Add new task
        tasks[GetTaskIndexWhereNameIs(selectedTask.task)] = selectedTask;

        // Save to JSON file
        SaveTaskChangesToJSON("Tasks.json");


        // Display success notification
        notification = "Task mark as DONE successfully!";

        // Go back to managing tasks
        ShowManageTaskScreen();
    }

    static void MarkAsTodo() {

        List<string> choices = new List<string>();

        foreach (var task in tasks.Where(x => x.status == "DONE")) {
            choices.Add(task.task);
        }

        // Cancel button
        choices.Add("Cancel");

        var whichTask = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select which [red]task[/] you want to [yellow]mark as TODO[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(choices)
        );


        if (whichTask == "Cancel") { ShowManageTaskScreen(); return; }


        Task selectedTask = GetTaskWhereNameIs(whichTask);
        // Edit task
        selectedTask.status = "TODO";

        // Add new task
        tasks[GetTaskIndexWhereNameIs(selectedTask.task)] = selectedTask;

        // Save to JSON file
        SaveTaskChangesToJSON("Tasks.json");


        // Display success notification
        notification = "Task mark as TODO successfully!";

        // Go back to managing tasks
        ShowManageTaskScreen();
    }

    static void ShowDeleteTaskScreen() {

        List<string> choices = new List<string>();

        foreach (var task in tasks) {
            choices.Add(task.task);
        }

        // Cancel button
        choices.Add("Cancel");

        var whichTask = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select which [red]task[/] you want to [yellow]edit[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(choices)
        );


        if (whichTask == "Cancel") { ShowManageTaskScreen(); return; }


        Task selectedTask = GetTaskWhereNameIs(whichTask);


        tasks.RemoveAt(GetTaskIndexWhereNameIs(selectedTask.task));

        // Save to JSON file
        SaveTaskChangesToJSON("Tasks.json");


        // Display success notification
        notification = "Task deleted successfully!";

        // Go back to managing tasks
        ShowManageTaskScreen();
    }







    static void SaveTaskChangesToJSON(string Json_Path) {
        // serialize JSON to a string and then write string to a file
        File.WriteAllText(Json_Path, JsonConvert.SerializeObject(tasks, Formatting.Indented));
    }









    static void ShowCreditScreen() {

        Console.Clear();

        AnsiConsole.Markup("\n \n");

        AnsiConsole.Write(
            new FigletText("Raldin Casidar Studio")
                .Centered()
                .Color(Color.Green)
        );
 
        AnsiConsole.Markup("\n");
        AnsiConsole.Write( new Rule("[red]Task Management System v2.0[/]") );
        AnsiConsole.Markup("\n");

        var panel = new Panel("[green]This Task Management was made by [yellow]Raldin Casidar[/]. Raldin Casidar is a student at JRMSU Main Campus and this project is made in compliance to the Final Project of sir [yellow]Edgardo Olmoguez[/]. \n \nPlease contact me at [yellow]raldin.disomimba13@gmail.com[/] if you seen any problem.[/]");
        panel.Header = new PanelHeader("About this Project");
        panel.Padding = new Padding(1, 1, 1, 1);
        panel.Expand = true;

        AnsiConsole.Write(panel);




        // Ask for user action
        var userAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select your [red]action[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more menu)[/]")
                .AddChoices(new[] {

                    "Go Back",

                })
        );

        if (userAction == "Go Back") { ShowMainScreen(); }
    }
  }
}
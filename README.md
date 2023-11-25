# HomeBudget# 3Budgetteers-HomeBudget

This is a home budgetting app created in the context of an application development course. The main goal of this project was to get experience working in an agile environment using SCRUM methodologies.

The team members were :
- William Dunwoody
- Breanna De Forest
- Cristiano Fazi

### Installation Steps

1. **Clone the Repository:**

`git clone https://github.com/WillDunw/HomeBudget.git`

2. **Open the Project in Visual Studio:**
- Open Visual Studio.
- Choose "Open a project or solution" and navigate to the directory where you cloned the repository.
- Select the `.sln` file and click "Open."

3. **Restore NuGet Packages:**
- Once the project is open, right-click on the solution in the Solution Explorer.
- Select "Restore NuGet Packages" to ensure all necessary packages are installed.

4. **Build and Run the Application:**
- Set the WPF project as the startup project.
- Click the "Start" button or press `F5` to build and run the application.

5. **Explore the App:**
- Once the application is running, explore the various features and functionalities.

## Main Features

- Budget file storing: the app allows you to choose which database you would like to use, so multiple different people can use the app on one device.
- Expenses CRUD: the app permits you to perfom any CRUD operation on any expense.
- Categories CRUD: the app also allows you to perform CRUD operations on expense categories, allowing you to organise your expenses the way you want.
- Expense viewing: you are able to view your expenses and all their details in a single window.
- Expense searching: while viewing all your expenses, you are given the option to search through them, to find one that matches your criteria.
- Downloading expenses: if you need your expenses for other purposes, the app allows you to download your expenses in a CSV file format, to be viewed in Excel or any other means.

## Main Technologies

- The UI was built using WPF and XAML.
- The app itself was built using C#.
- All the database interactions were done using a local database in the form of SQLite.
- The app was built following the MVP design pattern.

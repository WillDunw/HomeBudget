﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// Used to hold collections of Expense objects and to read/write from files that pertain to
    /// expense information. It contains a default file name, a list that holds all of the Expense
    /// objects, a file name, and a directory name. It has methods to manage the list of expenses,
    /// and some to read/write to files that have or will soon hold information about the expenses.
    /// Only the default file name is static, so to access anything else in the class, an instance
    /// must be created first.
    /// </summary>
    public class Expenses
    {
        private static String DefaultFileName = "budget.txt";
        private List<Expense> _Expenses = new List<Expense>();
        private string _FileName;
        private string _DirName;

        // ====================================================================
        // Properties
        // ====================================================================
        
        /// <value>
        /// Returns the file name that will hold information about the expenses.
        /// </value>
        public String FileName { get { return _FileName; } }

        /// <value>
        /// Returns the directory name that the file can be found in.
        /// </value>
        public String DirName { get { return _DirName; } }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================

        /// <summary>
        /// Prepares to get information about the expenses from a file. It clears the current expenses
        /// list and resets the directory and file name data before using a method from the BudgetFiles
        /// class to get a valid filepath. Using this file path, another method is called to actually 
        /// read the information from the file.
        /// 
        /// <example>
        /// Here is how to use the method:
        /// 
        /// <code>
        /// 
        /// Expenses exp = new Expenses();
        /// 
        /// string expenseFileName = "../expenses.exp";
        /// 
        /// exp.ReadFromFile(expenseFileName);
        /// 
        /// </code>
        /// 
        /// </example>
        /// 
        /// </summary>
        /// <param name="filepath">Represents the file path of the file that will be read.</param>
        /// <exception cref="FileNotFoundException">Thrown when a valid file path cannot be found or
        /// created.</exception>
        /// <exception cref="Exception">Thrown when the file cannot be read.</exception>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current expenses,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            _Expenses.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // read the expenses from the xml file
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);


        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================

        /// <summary>
        /// Prepares to write the expenses information to a file. It creates a filepath if one was not
        /// passed into the method and if the file and directory name backing fields aren't null, 
        /// otherwise the file path stays null and has to use default values. It then validated or 
        /// creates a valid file path using a method from the BudgetFiles class before calling another
        /// method to actually write the information to a file.
        /// 
        /// <example>
        /// Here is how to use the method:
        /// 
        /// <code>
        /// 
        /// string expFileName = "./expFile.exp";
        /// 
        /// Expenses exp = new Expenses(expFileName);
        /// exp.Add(DateTime.Now, 4, 3.99, "Hot Chocolate");
        /// 
        /// exp.SaveToFile(expFileName);
        /// 
        /// </code>
        /// </example>
        /// 
        /// </summary>
        /// <param name="filepath">Represents the file path of the file that will be written to.</param>
        /// <exception cref="Exception">Thrown when the file doesn't exist or is read only.</exception>
        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }



        // ====================================================================
        // Add expense
        // ====================================================================
        private void Add(Expense expense)
        {
            //Connecting to the database
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = Database.dbConnection.CreateCommand();

            //Writing the insert command
            sqlite_cmd.CommandText = "INSERT INTO expenses (Date, Description, Amount, CategoryId) VALUES (@Date, @Description, @Amount, @CategoryId);";
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@Date", expense.Date.ToString("yyyy-MM-dd")));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@Description", expense.Description));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@Amount", expense.Amount));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@CategoryId", (int)expense.Category));
            sqlite_cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates or adds a new Expense objects to the expenses list. To create a new expense, it 
        /// needs an id, a date, a category, an amount, and a description. the date, category, amount,
        /// and description are all passed into the method in parameters, but the id has to be
        /// calculated based on the expense ids that already are in the database. It then adds the 
        /// expense to the database.
        /// 
        /// <example>
        /// Here's an example of how to use the method:
        /// 
        /// <code>
        /// 
        /// Expenses exp = new Expenses();
        /// exp.Add(DateTime.Now, 9, 66.96, "Dining room chair");
        /// exp.Add(DateTime.Now, 9, 34.99, "2 gallon of pink paint");
        /// exp.Add(DateTime.Now, 14, 12.50, "Poutine");
        /// 
        /// </code>
        /// </example>
        /// 
        /// </summary>
        /// <param name="date">Represents the date the expense happened on.</param>
        /// <param name="category">Represents the category the expense belongs to.</param>
        /// <param name="amount">Represents the monetary value of the expense.</param>
        /// <param name="description">Represents a description of what the expense is.</param>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            //Connecting to the database
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = Database.dbConnection.CreateCommand();

            //Writing the insert command
            sqlite_cmd.CommandText = "INSERT INTO expenses (Date, Description, Amount, CategoryId) VALUES (@Date, @Description, @Amount, @CategoryId);";
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@Date", date.ToString("yyyy-MM-dd")));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@Description", description));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@Amount", amount));
            sqlite_cmd.Parameters.Add(new SQLiteParameter("@CategoryId", category));
            sqlite_cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Delete expense
        // ====================================================================
        /// <summary>
        /// Removes an expense from the list. It uses the id that is passed as a parameter to find the
        /// index of the expense with the same id. Once the index is found, the expense at its place
        /// gets removed from the list.
        /// 
        /// <example>
        /// Here's an example of how to use the method:
        /// 
        /// <code>
        /// 
        /// Expenses exp = new Expenses();
        /// exp.Add(DateTime.Now, 10, 14.87, "T-Shirt");
        /// exp.Add(DateTime.Now, 13, 60.00, "Monthly bus pass");
        /// 
        /// exp.Delete(1);
        /// 
        /// </code>
        /// 
        /// </example>
        /// 
        /// </summary>
        /// <param name="Id">Represents the expense id of the expense that will be deleted.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the id passed into the method
        /// doesn't match with any expense ids in the list.</exception>
        public void Delete(int Id)
        {
            int i = _Expenses.FindIndex(x => x.Id == Id);
            _Expenses.RemoveAt(i);
        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Creates a copy of the expenses list to return to the caller of the method. It creates
        /// a new list and copies each expense to the new list. This is done because lists are 
        /// passed by reference and the list should not be editable outside of the methods from
        /// this class.
        /// 
        /// <example>
        /// Here's an example of how to use this method:
        /// 
        /// <code>
        /// 
        /// Expenses exp = new Expenses();
        /// exp.Add(DateTime.Now, 3, 9.87, "Eggs");
        /// exp.Add(DateTime.Now, 3, 8.31, "Goldfish");
        /// <![CDATA[
        /// List<Expense> copyOfList = exp.List();
        /// 
        /// copyOfList.RemoveAt(1);
        /// ]]>
        /// </code>
        /// 
        /// Since its a copy, the RemoveAt call won't change anything in the actual expenses instance.
        /// </example>
        /// 
        /// </summary>
        /// <returns>A copy of the expenses list.</returns>
        public List<Expense> List()
        {
            List<Expense> newList = new List<Expense>();
            foreach (Expense expense in _Expenses)
            {
                newList.Add(new Expense(expense));
            }
            return newList;
        }


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {


            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                // Loop over each Expense
                foreach (XmlNode expense in doc.DocumentElement.ChildNodes)
                {
                    // set default expense parameters
                    int id = int.Parse((((XmlElement)expense).GetAttributeNode("ID")).InnerText);
                    String description = "";
                    DateTime date = DateTime.Parse("2000-01-01");
                    int category = 0;
                    Double amount = 0.0;

                    // get expense parameters
                    foreach (XmlNode info in expense.ChildNodes)
                    {
                        switch (info.Name)
                        {
                            case "Date":
                                date = DateTime.Parse(info.InnerText);
                                break;
                            case "Amount":
                                amount = Double.Parse(info.InnerText);
                                break;
                            case "Description":
                                description = info.InnerText;
                                break;
                            case "Category":
                                category = int.Parse(info.InnerText);
                                break;
                        }
                    }

                    // have all info for expense, so create new one
                    this.Add(new Expense(id, date, category, amount, description));

                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadFromFileException: Reading XML " + e.Message);
            }
        }


        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            // ---------------------------------------------------------------
            // loop over all categories and write them out as XML
            // ---------------------------------------------------------------
            try
            {
                // create top level element of expenses
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Expenses></Expenses>");

                // foreach Category, create an new xml element
                foreach (Expense exp in _Expenses)
                {
                    // main element 'Expense' with attribute ID
                    XmlElement ele = doc.CreateElement("Expense");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = exp.Id.ToString();
                    ele.SetAttributeNode(attr);
                    doc.DocumentElement.AppendChild(ele);

                    // child attributes (date, description, amount, category)
                    XmlElement d = doc.CreateElement("Date");
                    XmlText dText = doc.CreateTextNode(exp.Date.ToString("M/dd/yyyy hh:mm:ss tt"));
                    ele.AppendChild(d);
                    d.AppendChild(dText);

                    XmlElement de = doc.CreateElement("Description");
                    XmlText deText = doc.CreateTextNode(exp.Description);
                    ele.AppendChild(de);
                    de.AppendChild(deText);

                    XmlElement a = doc.CreateElement("Amount");
                    XmlText aText = doc.CreateTextNode(exp.Amount.ToString());
                    ele.AppendChild(a);
                    a.AppendChild(aText);

                    XmlElement c = doc.CreateElement("Category");
                    XmlText cText = doc.CreateTextNode(exp.Category.ToString());
                    ele.AppendChild(c);
                    c.AppendChild(cText);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Reading XML " + e.Message);
            }
        }

    }
}


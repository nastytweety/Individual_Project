using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;



namespace Individual_Project
{
    public class App
    {
        /// <summary>
        /// This function checks the username and password of the users and if succeful creates an instance of the user
        /// </summary>
        public void CheckUserPass()
        {
            DbContext db = new DbContext();
            db.CreateDatabase();
            Console.WriteLine("Dwse username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Dwse password: ");
            string password = Console.ReadLine();


            string[] data = new string[4];

            if ((data = db.Check(username, password)) != null)
            {
                if (data[3] == "SuperAdmin")
                {
                    SuperAdmin temp = new SuperAdmin(int.Parse(data[0]), data[1], data[2], data[3]);
                    temp.Menu();
                }
                else if (data[3] == "User" || data[3] == "user")
                {
                    User temp = new User(int.Parse(data[0]), data[1], data[2], data[3]);
                    temp.Menu();
                }
                else if (data[3] == "MessageViewer" || data[3] == "Messageviewer")
                {
                    Viewer temp = new Viewer(int.Parse(data[0]), data[1], data[2], data[3]);
                    temp.Menu();
                }
                else if (data[3] == "MessageEditor")
                {
                    MessageEditor temp = new MessageEditor(int.Parse(data[0]), data[1], data[2], data[3]);
                    temp.Menu();
                }
                else if (data[3] == "MessageHandler")
                {
                    MessageHandler temp = new MessageHandler(int.Parse(data[0]), data[1], data[2], data[3]);
                    temp.Menu();
                }

            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// This function presents the main menu of the application
        /// </summary>
        public void Menu()
        {
            bool exit = true;
            int choice;
            while (exit)
            {
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Exit App");
                int.TryParse(Console.ReadLine(), out choice);
                if (choice == 1)
                {
                    CheckUserPass();
                }
                else if (choice == 2)
                {
                    exit = false;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("===Wrong Choice!Please try again using number 1 or 2===");
                    Console.WriteLine();
                }
            }
        }
    }
}

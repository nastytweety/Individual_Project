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
    public class User
    {
        protected List<string[]> Loaded_Messages = new List<string[]>();
        private string path = "c:\\Projects\\messages.txt";

        protected int UserID;
        protected string Login;
        protected string Password;
        protected string Role;

        public User(int UUserID, String ULogin, String UPassword, String URole)
        {
            UserID = UUserID;
            Login = ULogin;
            Password = UPassword;
            Role = URole;
        }
        /// <summary>
        /// Returns the role of a user
        /// </summary>
        /// <returns>The Role</returns>
        protected string FindRole()
        {
            return Role;
        }


        /// <summary>
        /// Checks if a role is valid
        /// </summary>
        /// <param name="role">the role</param>
        /// <returns> true if its valid false otherwise</returns>
        protected bool RoleCheck(string role)
        {
            if(role=="User"||role=="user"||role=="MessageViewer"||role=="MessageEditor"||role=="MessageHandler")
            {
                return true;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("===No such role===");
                Console.WriteLine();
                return false;
            }
        }

       
        /// <summary>
        /// The user Menu
        /// </summary>
        public void Menu()
        {
            int Choice;
            bool Logout = false;
            while (!Logout)
            {
                Console.WriteLine("1. Write new message");
                Console.WriteLine("2. Open inbox");
                Console.WriteLine("3. Open sent messages");
                Console.WriteLine("4. Enter Chat");
                Console.WriteLine("5. Logout");
                if (int.TryParse(Console.ReadLine(), out Choice))
                {
                    switch (Choice)
                    {
                        case 1:
                            WriteMessage();
                            break;
                        case 2:
                            ShowMessage(1);
                            break;
                        case 3:
                            ShowMessage(0);
                            break;
                        case 4:
                            EnterChat();
                            break;
                        case 5:
                            Console.WriteLine();
                            Logout = true;
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("===Wrong Choice!Try again using 1,2,3,===");
                            Console.WriteLine();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the receiver exists and calls two more functions to store message to database and file
        /// </summary>
        protected void WriteMessage()
        {
            string sender = this.Login;
            DbContext db = new DbContext();
            Console.WriteLine("Insert receivers username");
            string receiver = Console.ReadLine();
            if(!db.CheckIfExists(receiver))
            {
                return;
            }
            string message = db.WriteToDatabase(receiver,sender);
            WriteToFile(receiver, sender, message);
        }
      
        /// <summary>
        /// Stores the message to file message.txt
        /// </summary>
        /// <param name="receiver">name of the receiver</param>
        /// <param name="sender">name of sender</param>
        /// <param name="message">message</param>
        /// 
        protected void WriteToFile(string receiver,string sender,string message)
        {
            if (!File.Exists(path))
            {
                StreamWriter sw = File.CreateText(path);
                using (sw)
                {
                    sw.WriteLine("Date: " + System.DateTime.Now);
                    sw.WriteLine("Receiver: " + receiver);
                    sw.WriteLine("Sender: " + sender);
                    sw.WriteLine("Message: " + message);
                    sw.WriteLine();
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine("Date: " + System.DateTime.Now);
                    sw.WriteLine("Receiver: " + receiver);
                    sw.WriteLine("Sender: " + sender);
                    sw.WriteLine("Message: " + message);
                    sw.WriteLine();
                }
            }
        }

        /// <summary>
        /// Shows a message
        /// </summary>
        /// <param name="choice">If choice=0 the user is the sender .if choice =1 the user is the receiver</param>
        protected void ShowMessage(int choice)
        {
            DbContext db = new DbContext();
            db.ShowMessage(choice, this.Login);
        }

        /// <summary>
        /// This function enables the chat mode with another user
        /// </summary>
        protected void EnterChat()
        {
            DbContext db = new DbContext();
            string choice;
            Console.WriteLine("Chat with who?");
            string username = Console.ReadLine();
            if (!db.CheckIfExists(username))
            {
                return;
            }
            db.EnterChat(username,this.Login);
            
            do
            {
                Console.WriteLine("Do you want to write a message?Y/N");
                choice = Console.ReadLine();
            } while (choice != "Y" && choice != "N");
            if (choice == "Y")
            {
                db.WriteToDatabase(username, Login);
            }
            else
            {
                Console.WriteLine();
                return;
            }
        }
    }
}

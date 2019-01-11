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
        protected string connectionstring = "Server=localhost;Database=IndividualProject;Trusted_Connection=True;";
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
        /// <returns></returns>
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
        /// Returns the UserID of a user
        /// </summary>
        /// <param name="Login">The user Login</param>
        /// <returns>the UserID</returns>
        protected int GetUserID(string Login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select UserID from Users where Login='{Login}' and Active = '1'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (int)reader[0];
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns the Login given a UserID
        /// </summary>
        /// <param name="UserID"> the UserID</param>
        /// <returns>The Login</returns>
        protected string GetLogin(int UserID)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select Login from Users where UserID='{UserID}' and Active='1'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader[0].ToString();
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Checks if a Login exists in the database
        /// </summary>
        /// <param name="Login">the login to be checked</param>
        /// <returns>returns true if the login exists.False otherwise</returns>
        public bool CheckIfExists(string Login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();

                var cmd = new SqlCommand($"select login from Users where login='{Login}' and Active ='1'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Username does not exist===");
                        Console.WriteLine();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
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
            Console.WriteLine("Insert receivers username");
            string receiver = Console.ReadLine();
            if(!CheckIfExists(receiver))
            {
                return;
            }
            string message = WriteToDatabase(receiver);
            WriteToFile(receiver, sender, message);
        }
        /// <summary>
        /// Strores the message to the database
        /// </summary>
        /// <param name="receiver">the name of the receiver</param>
        /// <returns>the message</returns>
        protected string WriteToDatabase(string receiver)
        {
            string sender = this.Login;
            Console.WriteLine("Insert Message");
            string message = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("insert into Messages values(@Message,@Date,@Sender,@Receiver)", dbcon);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@Date", System.DateTime.Now);
                cmd.Parameters.AddWithValue("@Sender", GetUserID(sender));
                cmd.Parameters.AddWithValue("@Receiver", GetUserID(receiver));
                var affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Message Sent!!!===");
                    Console.WriteLine();
                }
            }
            return message;
        }
        /// <summary>
        /// Stores the message to file message.txt
        /// </summary>
        /// <param name="receiver">name of the receiver</param>
        /// <param name="sender">name of sender</param>
        /// <param name="message">message</param>
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
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand();
                if (choice == 1)
                {
                    cmd = new SqlCommand($"select * from Messages where ReceiverID = '{GetUserID(this.Login).ToString()}'", dbcon);
                }
                else if (choice == 0)
                {
                    cmd = new SqlCommand($"select * from Messages where SenderID = '{GetUserID(this.Login).ToString()}'", dbcon);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        string[] data = new string[5];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();

                        Console.WriteLine($"ID: {data[0]} Message: {data[1]}, Date: {data[2]}, Sender: {GetLogin(int.Parse(data[3]))},SenderID: {data[3]},Receiver: {GetLogin(int.Parse(data[4]))}");
                    }
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// This function enables the chat mode with another user
        /// </summary>
        protected void EnterChat()
        {
            Console.WriteLine("Chat with who?");
            string login = Console.ReadLine();
            if (!CheckIfExists(login))
            {
                return;
            }

            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand();
                cmd = new SqlCommand($"select * from Messages where (ReceiverID = '{GetUserID(this.Login).ToString()}' and SenderID = '{GetUserID(login)}')"+
                    $"or (SenderID = '{GetUserID(this.Login).ToString()}' and ReceiverID = '{GetUserID(login)}') ", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\t\t===CHAT MODE===");
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        string[] data = new string[5];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();

                        
                        if (GetLogin(int.Parse(data[3])) == this.Login)
                        {
                            Console.WriteLine($" {data[1]}  ");
                        }
                        else
                        {
                            Console.WriteLine($" \t\t {data[1]} ");
                        }
                    }
                    Console.WriteLine();
                }
            }
            string choice;
            do
            {
                Console.WriteLine("Do you want to write a message?Y/N");
                choice = Console.ReadLine();
            } while (choice != "Y" && choice != "N");
            if(choice=="Y")
            {
                WriteToDatabase(login);
            }
            else
            {
                Console.WriteLine();
                return;
            }  
        }
    }
}

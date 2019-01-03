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
        protected string Login;
        protected string Password;
        protected string Role;

        public User(String ULogin, String UPassword, String URole)
        {
            Login = ULogin;
            Password = UPassword;
            Role = URole;
        }

        protected string FindRole()
        {
            return Role;
        }

        protected bool RoleCheck(string role)
        {
            if(role=="User"||role=="user"||role=="MessageViewer"||role=="MessageEditor"||role=="MessageHandler")
            {
                return true;
            }
            else
            {
                Console.WriteLine("No such role");
                return false;
            }
        }


        public void Menu()
        {
            int Choice;
            bool Logout = false;
            while (!Logout)
            {
                Console.WriteLine("1. Write new message");
                Console.WriteLine("2. Read messages");
                Console.WriteLine("3. Logout");
                if (int.TryParse(Console.ReadLine(), out Choice))
                {
                    switch (Choice)
                    {
                        case 1:
                            WriteMessage();
                            break;
                        case 2:
                            ShowMessage();
                            break;
                        case 3:
                            Logout = true;
                            break;
                        default:
                            Console.WriteLine("Wrong Choice");
                            break;
                    }
                }
            }
        }


        protected void WriteMessage()
        {
            string path = "c:\\Projects\\messages.txt";
            string sender = this.Login;

            Console.WriteLine("Insert receivers username");
            string receiver = Console.ReadLine();
            Console.WriteLine("Insert Message");
            string message = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("insert into Messages values(@Message,@Date,@SenderID,@ReceiverID)", dbcon);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@Date", System.DateTime.Now);
                cmd.Parameters.AddWithValue("@SenderID", sender);
                cmd.Parameters.AddWithValue("@ReceiverID", receiver);
                var affectedRows = cmd.ExecuteNonQuery();
                //Console.WriteLine($"{affectedRows} Affected Rows");
            }


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
                using (StreamWriter sw = new StreamWriter(path,true))
                {
                    sw.WriteLine("Date: " + System.DateTime.Now);
                    sw.WriteLine("Receiver: " + receiver);
                    sw.WriteLine("Sender: " + sender);
                    sw.WriteLine("Message: " + message);
                    sw.WriteLine();
                }
            }
        }



        protected void ShowMessage()
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select * from Messages where ReceiverID = '{this.Login}'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] data = new string[5];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();

                        Console.WriteLine($"ID: {data[0]} Message: {data[1]}, Date: {data[2]}, SenderID: {data[3]},ReceiverID: {data[4]}");
                    }
                }
            }
        }
    }
}

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
    public class Viewer : User
    {
        public Viewer(int UUserID,string ULogin,string UPassword,string URole) : base(UUserID, ULogin,UPassword,URole)
        {

        }

        public new void Menu()
        {
            int Choice;
            bool Logout = false;
            while (!Logout)
            {
                Console.WriteLine("1. Write new message");
                Console.WriteLine("2. Read messages");
                Console.WriteLine("3. View messages");
                Console.WriteLine("4. Logout");
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
                            ShowUsersMessages();
                            break;
                        case 4:
                            Console.WriteLine();
                            Logout = true;
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("===Wrong Choice===");
                            Console.WriteLine();
                            break;
                    }
                }
            }
        }

        protected void ShowUsersMessages()
        {
            int counter = 0;
            SqlConnection dbcon = new SqlConnection(connectionstring);
            Console.WriteLine("Insert Username");
            string login = Console.ReadLine();

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===User does not exist===");
                        Console.WriteLine();
                        return;
                    }
                }

                cmd = new SqlCommand($"select * from Messages where ReceiverID = '{GetUserID(login)}' or SenderID = '{GetUserID(login)}'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        counter++;
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

        protected bool ShowUsersMessages(string login)
        {
            int counter = 0;
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===User does not exist===");
                        Console.WriteLine();
                        return false;
                    }
                }

                cmd = new SqlCommand($"select * from Messages where ReceiverID = '{GetUserID(login)}' or SenderID = '{GetUserID(login)}'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        counter++;
                        string[] data = new string[5];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();

                        Console.WriteLine($"ID: {data[0]} Message: {data[1]}, Date: {data[2]}, SenderID: {data[3]},ReceiverID: {data[4]}");
                    }
                    Console.WriteLine();
                }
                return true;
            }
        }
    }
}

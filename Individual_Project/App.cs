using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace Individual_Project
{
    public class App
    {
        private string connectionstring = "Server=localhost; Database=IndividualProject; Trusted_Connection=True;";
        private string initialconnectionstring = "Server=localhost; Database=master; Trusted_Connection=True;";
        private List<string> Usernames_Passwords = new List<string>();
        private List<string[]> User_Data = new List<string[]>();

        public void CreateDatabase()
        {
            SqlConnection dbcon = new SqlConnection(initialconnectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("select * from sys.Databases where Name = 'IndividualProject'", dbcon);
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    try
                    {
                        cmd = new SqlCommand("create database IndividualProject", dbcon);
                        var affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("use IndividualProject", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("create table Users(Login nvarchar(50) NOT NULL," +
                            "Password nvarchar(64) NOT NULL, Role nvarchar(50) NOT NULL)", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("create table Messages(MessageId int NOT NULL PRIMARY KEY IDENTITY(1,1),Message nvarchar(MAX) NOT NULL,Date datetime NOT NULL," +
                            " SenderID nvarchar(50) NOT NULL,ReceiverID nvarchar(50) NOT NULL)", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("insert into Users values ('admin','admin','SuperAdmin')", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    Console.WriteLine("Database exists");
                }
            }
        }


        public void LoadUserPass()
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            Usernames_Passwords.Clear();
            User_Data.Clear();
            CreateDatabase();

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("select * from Users", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] data = new string[3];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        Usernames_Passwords.Add(data[0] + " " + data[1]);
                        User_Data.Add(data);
                    }
                }
            }
        }

        public void CheckUserPass()
        {
            LoadUserPass();
            bool login = false;
            Console.WriteLine("Dwse username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Dwse password: ");
            string password = Console.ReadLine();
            string userpass = username + " " + password;


            foreach (var user in Usernames_Passwords)
            {
                if (user == userpass)
                {
                    Console.WriteLine("Login Successful");
                    login = true;
                    foreach (string[] data in User_Data)
                    {
                        if (username == data[0])
                        {
                            if (data[2] == "SuperAdmin")
                            {
                                SuperAdmin temp = new SuperAdmin(data[0], data[1], data[2]);
                                temp.Menu();
                            }
                            else if(data[2]== "User" || data[2]=="user")
                            {
                                User temp = new User(data[0], data[1], data[2]);
                                temp.Menu();
                            }
                            else if (data[2] == "Viewer" || data[2] == "viewer")
                            {
                                Viewer temp = new Viewer(data[0], data[1], data[2]);
                                temp.Menu();
                            }
                            else if (data[2] == "MessageEditor")
                            {
                                MessageEditor temp = new MessageEditor(data[0], data[1], data[2]);
                                temp.Menu();
                            }
                            else if(data[2]=="MessageHandler")
                            {
                                MessageHandler temp = new MessageHandler(data[0], data[1], data[2]);
                                temp.Menu();
                            }

                        }
                    }
                }
            }
            if (login == false)
            {
                Console.WriteLine("Login Failed");
            }
        }


        public void Menu()
        {
            bool exit = true;
            int choice;
            while (exit)
            {
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Exit");
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
                    Console.WriteLine("Wrong Choice!Please try again using number 1 or 2");
                }
            }
        }
    }
}

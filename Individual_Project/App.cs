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
        private string connectionstring = Properties.Settings.Default.connectionstring;
        private string initialconnectionstring = Properties.Settings.Default.initialconnectionstring;

        /// <summary>
        /// This function checks if the database exists and creates then database if it does not.
        /// The function adds admin super user
        /// </summary>
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
                        cmd = new SqlCommand("create table Users(UserID int PRIMARY KEY IDENTITY(1,1),Login nvarchar(50)," +
                            "Password varbinary(512),Salt nvarchar(32), Role nvarchar(50), Active bit)", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("create table Messages(MessageId int PRIMARY KEY IDENTITY(1,1),Message nvarchar(MAX) ,Date datetime ," +
                            "SenderID int REFERENCES Users(UserID)," +
                            "ReceiverID int REFERENCES Users(UserID))", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("CREATE PROCEDURE dbo.AddUser @pLogin NVARCHAR(50), @pPassword nvarchar(50),@pRole NVARCHAR(50)"+
                            "AS BEGIN SET NOCOUNT ON DECLARE @pSalt AS NVARCHAR(32) BEGIN TRY "+
                            "SET @pSalt = CONVERT(nvarchar(32), LEFT(REPLACE(NEWID(), '-', ''), 32)) "+
                            "INSERT INTO dbo.[Users](Login, Password, Salt, Role, Active) "+
                            "VALUES(@pLogin, HASHBYTES('SHA2_256', CONCAT(@pPassword, @pSalt)), @pSalt, @pRole, 1) "+
                            "END TRY  BEGIN CATCH  END CATCH END", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("EXEC dbo.AddUser @pLogin = 'admin', @pPassword = 'admin',@pRole = 'SuperAdmin'", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        /// <summary>
        /// This function checks the username and password of the users and if succeful creates an instance of the user
        /// </summary>
        public void CheckUserPass()
        {
            CreateDatabase();
            Console.WriteLine("Dwse username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Dwse password: ");
            string password = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select UserID, Login, Password, Role from Users where Login = @login and HASHBYTES('SHA2_256',CONCAT(@password ,Salt)) = Password and Active='1'", dbcon);
                cmd.Parameters.AddWithValue("@login", username);
                cmd.Parameters.AddWithValue("@password", password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Login Successful===");
                        Console.WriteLine();
                        string[] data = new string[4];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        //data[4] = reader[4].ToString();
                        //data[5] = reader[5].ToString();

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
                        Console.WriteLine();
                        Console.WriteLine("===Login Failed===");
                        Console.WriteLine();
                    }
                }
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

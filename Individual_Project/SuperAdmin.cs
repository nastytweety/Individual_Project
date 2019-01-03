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

    public class SuperAdmin : User
    {
        public SuperAdmin(string ULogin, string UPassword, string URole) : base(ULogin, UPassword, URole)
        {

        }

        public void UpdateUser()
        {
            string role;
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine("Username does not exist");
                        return;
                    }
                }
                Console.WriteLine("Insert new username");
                string newlogin = Console.ReadLine();
                Console.WriteLine("Insert new password");
                string password = Console.ReadLine();
                Console.WriteLine("The roles are <<1.User>> <<2.MessageViewer>> <<3.MesageEditor>> <<4.MessageHandler>>");
                do
                {
                    Console.WriteLine("Insert new role");
                    role = Console.ReadLine();
                } while (RoleCheck(role) == false);


                var cmd2 = new SqlCommand("update Users set Login = @newlogin,Password = @password, Role = @role where login = @login", dbcon);
                cmd2.Parameters.AddWithValue("@login", login);
                cmd2.Parameters.AddWithValue("@newlogin", newlogin);
                cmd2.Parameters.AddWithValue("@password", password);
                cmd2.Parameters.AddWithValue("@role", role);
                var affectedRows = cmd2.ExecuteNonQuery();
                Console.WriteLine($"{affectedRows} Affected Rows");
            }
        }

        public void CreateUser()
        {
            string role;
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            Console.WriteLine("Insert password");
            string password = Console.ReadLine();
            Console.WriteLine("The roles are <<1.User>> <<2.MessageViewer>> <<3.MesageEditor>> <<4.MessageHandler>>");
            do
            {
                Console.WriteLine("Insert new role");
                role = Console.ReadLine();
            } while (RoleCheck(role) == false);

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Username exists");
                        return;
                    }
                }
                var cmd2 = new SqlCommand("insert into Users values(@login,@password,@role)", dbcon);
                cmd2.Parameters.AddWithValue("@login", login);
                cmd2.Parameters.AddWithValue("@password", password);
                cmd2.Parameters.AddWithValue("@role", role);
                var affectedRows = cmd2.ExecuteNonQuery();

                Console.WriteLine($"{affectedRows} Affected Rows");
            }
        }

        public void DeleteUser()
        {
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}'", dbcon);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    reader.Close();
                    var cmd2 = new SqlCommand($"delete from Users where login='{login}'", dbcon);
                    cmd2.Parameters.AddWithValue("@login", login);
                    var affectedRows = cmd2.ExecuteNonQuery();
                    //Console.WriteLine($"{affectedRows} Affected Rows");
                }
                else
                {
                    Console.WriteLine($"User {login} does not exist");
                }

            }
        }

        public void ViewUser()
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

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
                        Console.WriteLine($"Login: {data[0]}, Pass: {data[1]}, Role: {data[2]}");
                    }
                }
            }
        }

        public new void Menu()
        {
            int Choice;
            bool Logout = false;
            while (!Logout)
            {
                Console.WriteLine("1. Write new message");
                Console.WriteLine("2. Read own messages");
                Console.WriteLine("3. Create User");
                Console.WriteLine("4. Delete User");
                Console.WriteLine("5. Update User");
                Console.WriteLine("6. View Users");
                Console.WriteLine("7. Logout");
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
                            CreateUser();
                            break;
                        case 4:
                            DeleteUser();
                            break;
                        case 5:
                            UpdateUser();
                            break;
                        case 6:
                            ViewUser();
                            break;
                        case 7:
                            Logout = true;
                            break;
                        default:
                            Console.WriteLine("Wrong Choice");
                            break;
                    }
                }
            }
        }
    }
}

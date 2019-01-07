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

    public class SuperAdmin : User
    {
        public SuperAdmin(int UUserID,string ULogin, string UPassword, string URole) : base(UUserID,ULogin, UPassword, URole)
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
                var cmd = new SqlCommand($"select login from Users where login='{login}' and Active = '1'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Username does not exist===");
                        Console.WriteLine();
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


                var cmd2 = new SqlCommand($"update Users set Login = '{newlogin}',Password = HASHBYTES('SHA2_256',HASHBYTES('SHA2_256','{password}')), Role = '{role}' where UserID = '{GetUserID(login)}'", dbcon);
                var affectedRows = cmd2.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===User updated===");
                    Console.WriteLine();
                }
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
                Console.WriteLine("Insert role");
                role = Console.ReadLine();
            } while (RoleCheck(role) == false);

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}' and Active ='1'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Username exists===");
                        Console.WriteLine();
                        return;
                    }
                }
                var cmd2 = new SqlCommand($"insert into Users values('{login}',HASHBYTES('SHA2_256',HASHBYTES('SHA2_256','{password}')),'{role}','1')", dbcon);
                var affectedRows = cmd2.ExecuteNonQuery();
                if(affectedRows>0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===User created===");
                    Console.WriteLine();
                }
                
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
                    var cmd2 = new SqlCommand($"update Users set Active = '0' where login='{login}'", dbcon);
                    cmd2.Parameters.AddWithValue("@login", login);
                    var affectedRows = cmd2.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===User deleted===");
                        Console.WriteLine();
                    }
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
                var cmd = new SqlCommand("select * from Users where Active = '1'", dbcon);
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
                        Console.WriteLine($"UserID: {data[0]}, Login: {data[1]}, Role: {data[3]}");
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

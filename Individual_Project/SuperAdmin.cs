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

        /// <summary>
        /// Updates the details of a user.Login password and role can be updated.The messages of the user are updates too.
        /// </summary>
        public void UpdateUser()
        {
            string role;
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!CheckIfExists(login))
            {
                return;
            }

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                Console.WriteLine("Insert new username");
                string newlogin = Console.ReadLine();
                if (CheckIfExists(newlogin))
                {
                    Console.WriteLine();
                    Console.WriteLine("===Username exists===");
                    Console.WriteLine();
                    return;
                }
                Console.WriteLine("Insert new password");
                string password = Console.ReadLine();
                Console.WriteLine("The roles are <<1.User>> <<2.MessageViewer>> <<3.MesageEditor>> <<4.MessageHandler>>");
                do
                {
                    Console.WriteLine("Insert new role");
                    role = Console.ReadLine();
                } while (!RoleCheck(role));


                var cmd2 = new SqlCommand("update Users set Login = @login, Password =  HASHBYTES('SHA2_256',CONCAT(@password,Salt)), Role = @role where UserID = @userid", dbcon);
                cmd2.Parameters.AddWithValue("@login", newlogin);
                cmd2.Parameters.AddWithValue("@password", password);
                cmd2.Parameters.AddWithValue("@role", role);
                cmd2.Parameters.AddWithValue("@userid", GetUserID(login));
                var affectedRows = cmd2.ExecuteNonQuery();
             
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===User updated===");
                    Console.WriteLine();
                }
            }
        }
        /// <summary>
        /// Creates a new user
        /// </summary>
        public void CreateUser()
        {
            string role;
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (CheckIfExists(login))
            {
                Console.WriteLine();
                Console.WriteLine("===Username exists===");
                Console.WriteLine();
                return;
            }
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
                var cmd2 = new SqlCommand($"EXEC dbo.AddUser @pLogin = @login, @pPassword = @password ,@pRole = @role", dbcon);
                cmd2.Parameters.AddWithValue("@login", login);
                cmd2.Parameters.AddWithValue("@password", password);
                cmd2.Parameters.AddWithValue("@role", role);
                var affectedRows = cmd2.ExecuteNonQuery();
                if(affectedRows>0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===User created===");
                    Console.WriteLine();
                }
                
            }
        }

        /// <summary>
        /// Deletes a user by setting the Active field to 0
        /// </summary>
        public void DeleteUser()
        {
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!CheckIfExists(login))
            {
                return;
            }

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd2 = new SqlCommand($"update Users set Active = '0' where login= @login", dbcon);
                cmd2.Parameters.AddWithValue("@login", login);
                var affectedRows = cmd2.ExecuteNonQuery();
                if (affectedRows > 0)
                { 
                    Console.WriteLine();
                    Console.WriteLine("===User deleted===");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Shows a list of active users
        /// </summary>
        public void ViewUser()
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("select UserID,Login,Role from Users where Active = '1'", dbcon);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] data = new string[3];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        Console.WriteLine($"UserID: {data[0]}, Login: {data[1]}, Role: {data[2]}");
                    }
                }
            }
        }

        /// <summary>
        /// The super admin menu
        /// </summary>
        public new void Menu()
        {
            int Choice;
            bool Logout = false;
            while (!Logout)
            {
                Console.WriteLine("1. Write new message");
                Console.WriteLine("2. Open inbox");
                Console.WriteLine("3. Open sent messages");
                Console.WriteLine("4. Enter Chat");
                Console.WriteLine("5. Create User");
                Console.WriteLine("6. Delete User");
                Console.WriteLine("7. Update User");
                Console.WriteLine("8. View Users");
                Console.WriteLine("9. Logout");
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
                            CreateUser();
                            break;
                        case 6:
                            DeleteUser();
                            break;
                        case 7:
                            UpdateUser();
                            break;
                        case 8:
                            ViewUser();
                            break;
                        case 9:
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

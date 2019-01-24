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
            DbContext db = new DbContext();
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!db.CheckIfExists(login))
            {
                return;
            }

            Console.WriteLine("Insert new username");
            string newlogin = Console.ReadLine();
            if (db.CheckIfExists(newlogin))
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
            db.Update(login, newlogin, password, role);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        public void CreateUser()
        {
            string role;
            DbContext db = new DbContext();
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (db.CheckIfExists(login))
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

            db.Insert(login,password,role);
        }

        /// <summary>
        /// Deletes a user by setting the Active field to 0
        /// </summary>
        public void DeleteUser()
        {
            DbContext db = new DbContext();
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!db.CheckIfExists(login))
            {
                return;
            }

            db.Delete(login);
        }

        /// <summary>
        /// Shows a list of active users
        /// </summary>
        public void ViewUser()
        {
            DbContext db = new DbContext();
            db.View();
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

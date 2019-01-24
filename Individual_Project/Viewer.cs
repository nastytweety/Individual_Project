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
        /// <summary>
        /// The Menu
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
                Console.WriteLine("5. View Messages");
                Console.WriteLine("6. Logout");
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
                            ShowUserAllMessages(null);
                            break;
                        case 6:
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

        /// <summary>
        /// Shows the incoming and outgoing messages of a user
        /// </summary>
        /// <param name="ulogin">If param=null then function asks for username</param>
        protected void ShowUserAllMessages(string ulogin)
        {
            string login;
            DbContext db = new DbContext();
            if (ulogin==null)
            {
                Console.WriteLine("Insert Username");
                login = Console.ReadLine();
                if (!db.CheckIfExists(login))
                {
                    return;
                }
            }
            else
            {
                login = ulogin;
            }
            db.ShowAll(login);
        }
    }
}

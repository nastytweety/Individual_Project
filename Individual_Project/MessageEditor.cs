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
    public class MessageEditor : Viewer
    {
        public MessageEditor(int UUserID,string ULogin, string UPassword, string URole) : base(UUserID,ULogin, UPassword, URole)
        {

        }

        
        /// <summary>
        /// Checks whether a Message belongs to a certain login. 
        /// </summary>
        /// <param name="login">The username</param>
        /// <param name="number">the messageID</param>
        /// <returns>true if message belongs to certain login</returns>
        public bool CheckValidChoice(string login,int number)
        {
            DbContext db = new DbContext(); 
            if(db.GetLogin(db.GetReceiverByNumber(number))==login ||db.GetLogin(db.GetSenderByNumber(number))==login)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the all the required information in order to edit a message and calls the UpdateMessage function.
        /// </summary>
        public void EditMessage()
        {
            int choice = 0;
            DbContext db = new DbContext();
            string message;

            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!db.CheckIfExists(login))
            {
                return;
            }

            ShowUserAllMessages(login);
            Console.WriteLine("Which message do you wish to edit? Choose by number");

            bool x = int.TryParse(Console.ReadLine(), out choice);
            while (x == false  || !CheckValidChoice(login, choice))
            {
                Console.WriteLine();
                Console.WriteLine("===Wrong Choice!Choose a valid number===");
                Console.WriteLine();
                x = int.TryParse(Console.ReadLine(), out choice);
            }

            Console.WriteLine(db.GetMessageByNumber(choice));
            Console.WriteLine("Edit message here:");
            message = Console.ReadLine();
            db.UpdateMessage(login, message, choice);
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
                Console.WriteLine("6. Edit Messages");
                Console.WriteLine("7. Logout");
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
                            EditMessage();
                            break;
                        case 7:
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

    }
}

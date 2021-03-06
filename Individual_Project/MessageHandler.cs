﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Individual_Project
{
    public class MessageHandler : MessageEditor
    {
        public MessageHandler(int UUserID,string ULogin, string UPassword, string URole) : base(UUserID,ULogin, UPassword, URole)
        {

        }
        /// <summary>
        /// Deletes a specific message or all the messages of a user.
        /// </summary>
        public void DeleteMessages()
        {
            int choice;
            DbContext db = new DbContext();
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!db.CheckIfExists(login))
            {
                return;
            }

            do
            {
                Console.WriteLine("Press 1 to delete all messages");
                Console.WriteLine("Press 2 to delete specific message");
                int.TryParse(Console.ReadLine(), out choice);
                if (choice < 1 || choice > 2)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Wrong choice.Try again===");
                    Console.WriteLine();
                }
            }
            while (choice < 1 || choice > 2);


            if (choice == 1)
            {
                db.DeleteAll(this.Login);
            }
            else
            {
                int Choice;
                ShowUserAllMessages(login);
                Console.WriteLine("Select message to delete by number");

                bool x = int.TryParse(Console.ReadLine(), out Choice);
                while (x == false || !CheckValidChoice(login, Choice))
                {
                    Console.WriteLine();
                    Console.WriteLine("===Wrong Choice!!Choose a valid number===");
                    Console.WriteLine();
                    x = int.TryParse(Console.ReadLine(), out Choice);
                }
                db.Delete(Choice);
            }

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
                Console.WriteLine("7. Delete Messages");
                Console.WriteLine("8. Logout");
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
                            DeleteMessages();
                            break;
                        case 8:
                            Console.WriteLine();
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

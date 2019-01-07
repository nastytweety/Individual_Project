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
    public class MessageHandler : MessageEditor
    {
        public MessageHandler(int UUserID,string ULogin, string UPassword, string URole) : base(UUserID,ULogin, UPassword, URole)
        {

        }

        public void DeleteMessages()
        {
            int choice;
            Console.WriteLine("Insert username");
            string login = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select login from Users where login='{login}' and Active='1'", dbcon);

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
                do
                {
                    Console.WriteLine("Press 1 to delete outgoing messages");
                    Console.WriteLine("Press 2 to delete incoming messages");
                    Console.WriteLine("Press 3 to delete all messages");
                    Console.WriteLine("Press 4 to delete specific message");
                    int.TryParse(Console.ReadLine(), out choice);
                    if(choice<1||choice>4)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Wrong choice.Try again===");
                        Console.WriteLine();
                    }
                }
                while (choice < 1 || choice >4);

                if (choice == 1)
                {
                    var cmd2 = new SqlCommand($"delete from Messages where SenderID='{GetUserID(login)}'", dbcon);
                    cmd2.Parameters.AddWithValue("@login", login);
                    var affectedRows = cmd2.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Messages Deleted===");
                        Console.WriteLine();
                    }
                }
                else if(choice ==2)
                {
                    var cmd2 = new SqlCommand($"delete from Messages where ReceiverID='{GetUserID(login)}'", dbcon);
                    cmd2.Parameters.AddWithValue("@login", login);
                    var affectedRows = cmd2.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Messages Deleted===");
                        Console.WriteLine();
                    }
                }
                else if (choice == 3)
                {
                    var cmd2 = new SqlCommand($"delete from Messages where ReceiverID='{GetUserID(login)}' or SenderID='{GetUserID(login)}'", dbcon);
                    cmd2.Parameters.AddWithValue("@login", login);
                    var affectedRows = cmd2.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Messages Deleted===");
                        Console.WriteLine();
                    }
                }
                else
                {
                    int Choice;
                    ShowUsersMessages();
                    Console.WriteLine("Select message to delete by number");

                    bool x = int.TryParse(Console.ReadLine(), out Choice);
                    while (x == false || !CheckValidChoice(login, Choice))
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Wrong Choice!!Choose a valid number===");
                        Console.WriteLine();
                        x = int.TryParse(Console.ReadLine(), out Choice);
                    }
                     var cmd2 = new SqlCommand($"delete from Messages where MessageID='{Choice}'", dbcon);
                     var affectedRows = cmd2.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Message Deleted===");
                        Console.WriteLine();
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
                Console.WriteLine("2. Read messages");
                Console.WriteLine("3. View messages");
                Console.WriteLine("4. Edit messages");
                Console.WriteLine("5. Delete messages");
                Console.WriteLine("6. Logout");
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
                            EditMessage();
                            break;
                        case 5:
                            DeleteMessages();
                            break;
                        case 6:
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

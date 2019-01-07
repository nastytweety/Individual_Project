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

        public string GetMessageByNumber(int number)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select Message from Messages where MessageID = '{number}'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader[0].ToString();
                    }
                }
            }
            return " ";
        }

        public int GetSenderByNumber(int number)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select SenderID from Messages where MessageID = '{number}'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return int.Parse(reader[0].ToString());
                    }
                } 
            }
            return 0;
        }


        public int GetReceiverByNumber(int number)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select ReceiverID from Messages where MessageID = '{number}'", dbcon);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return int.Parse(reader[0].ToString());
                    }
                }
            }
            return 0;
        }

        public bool CheckValidChoice(string login,int number)
        {
            if(GetLogin(GetReceiverByNumber(number))==login || GetLogin(GetSenderByNumber(number))==login)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateMessage(string login, string message, int messageid)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();


                var cmd = new SqlCommand("update Messages set Message = @message , Date = @date, SenderID = @senderid , ReceiverID=@receiverid where MessageID = @messageid", dbcon);
                cmd.Parameters.AddWithValue("@message", message);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@senderid", GetSenderByNumber(messageid));
                cmd.Parameters.AddWithValue("@receiverid", GetReceiverByNumber(messageid));
                cmd.Parameters.AddWithValue("@messageid", messageid);
                var affectedRows = cmd.ExecuteNonQuery();
                if(affectedRows>0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Message Updated===");
                    Console.WriteLine();
                }
            }   
        }

        public void EditMessage()
        {
            int choice = 0;
            string login;

            string message;

            do
            {
                Console.WriteLine("Insert Username");
                login = Console.ReadLine();

            }
            while (!ShowUsersMessages(login));

            Console.WriteLine("Which message do you wish to edit? Choose by number");

            bool x = int.TryParse(Console.ReadLine(), out choice);
            while (x == false  || !CheckValidChoice(login, choice))
            {
                Console.WriteLine();
                Console.WriteLine("===Wrong Choice!Choose a valid number===");
                Console.WriteLine();
                x = int.TryParse(Console.ReadLine(), out choice);
            }

            Console.WriteLine(GetMessageByNumber(choice));
            Console.WriteLine("Edit message here:");
            message = Console.ReadLine();
            UpdateMessage(login, message, choice);
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
                Console.WriteLine("5. Logout");
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

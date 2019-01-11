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
        /// It returns the text of a message given the message id
        /// </summary>
        /// <param name="number">The number ID of the message</param>
        /// <returns>The text of the message</returns>
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
        /// <summary>
        /// Returns the SenderID given the MessageID
        /// </summary>
        /// <param name="number">The messageID</param>
        /// <returns>The senderID</returns>
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

        /// <summary>
        /// Returns the ReceiverID given the MessageID
        /// </summary>
        /// <param name="number">The messageID</param>
        /// <returns>The RecieverID</returns>
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
        /// <summary>
        /// Checks whether a Message belongs to a certain login. 
        /// </summary>
        /// <param name="login">The username</param>
        /// <param name="number">the messageID</param>
        /// <returns>true if message belongs to certain login</returns>
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
        /// <summary>
        /// Updates a message
        /// </summary>
        /// <param name="login">The username</param>
        /// <param name="message">The message text</param>
        /// <param name="messageid">The messageID</param>
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

        /// <summary>
        /// Gets the all the required information in order to edit a message and calls the UpdateMessage function.
        /// </summary>
        public void EditMessage()
        {
            int choice = 0;

            string message;

            Console.WriteLine("Insert username");
            string login = Console.ReadLine();
            if (!CheckIfExists(login))
            {
                return;
            }

            ShowUsersMessages(login);
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
                            ShowUsersMessages(null);
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

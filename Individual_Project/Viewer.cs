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
                            ShowUsersMessages(null);
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
        protected void ShowUsersMessages(string ulogin)
        {
            string login;
            if (ulogin==null)
            {
                Console.WriteLine("Insert Username");
                login = Console.ReadLine();
                if (!CheckIfExists(login))
                {
                    return;
                }
            }
            else
            {
                login = ulogin;
            }
            
           
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            { 
                dbcon.Open();
                var cmd = new SqlCommand("select * from Messages where ReceiverID = @user or SenderID = @user", dbcon);
                cmd.Parameters.AddWithValue("@user", GetUserID(login));
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        string[] data = new string[5];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();

                        Console.WriteLine($"ID: {data[0]} Message: {data[1]}, Date: {data[2]}, Sender: {GetLogin(int.Parse(data[3]))},SenderID: {data[3]},Receiver: {GetLogin(int.Parse(data[4]))}");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}

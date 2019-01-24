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
    public class DbContext 
    {
        private string connectionstring = Properties.Settings.Default.connectionstring;
        private string initialconnectionstring = Properties.Settings.Default.initialconnectionstring;

        /// <summary>
        /// This function checks if the database exists and creates then database if it does not.
        /// The function adds admin super user
        /// </summary>
        public void CreateDatabase()
        {
            SqlConnection dbcon = new SqlConnection(initialconnectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("select * from sys.Databases where Name = 'IndividualProject'", dbcon);
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    try
                    {
                        cmd = new SqlCommand("create database IndividualProject", dbcon);
                        var affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("use IndividualProject", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("create table Users(UserID int PRIMARY KEY IDENTITY(1,1),Login nvarchar(50)," +
                            "Password varbinary(512),Salt nvarchar(32), Role nvarchar(50), Active bit)", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("create table Messages(MessageId int PRIMARY KEY IDENTITY(1,1),Message nvarchar(MAX) ,Date datetime ," +
                            "SenderID int REFERENCES Users(UserID)," +
                            "ReceiverID int REFERENCES Users(UserID))", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("CREATE PROCEDURE dbo.AddUser @pLogin NVARCHAR(50), @pPassword nvarchar(50),@pRole NVARCHAR(50)" +
                            "AS BEGIN SET NOCOUNT ON DECLARE @pSalt AS NVARCHAR(32) BEGIN TRY " +
                            "SET @pSalt = CONVERT(nvarchar(32), LEFT(REPLACE(NEWID(), '-', ''), 32)) " +
                            "INSERT INTO dbo.[Users](Login, Password, Salt, Role, Active) " +
                            "VALUES(@pLogin, HASHBYTES('SHA2_256', CONCAT(@pPassword, @pSalt)), @pSalt, @pRole, 1) " +
                            "END TRY  BEGIN CATCH  END CATCH END", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("EXEC dbo.AddUser @pLogin = 'admin', @pPassword = 'admin',@pRole = 'SuperAdmin'", dbcon);
                        affectedRows = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        /// <summary>
        /// Checks username and passwords during the Login.
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        /// <returns></returns>
        public string[] Check(string username, string password)
        {
            string[] data = new string[4];
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select UserID, Login, Password, Role from Users where Login = @login and HASHBYTES('SHA2_256',CONCAT(@password ,Salt)) = Password and Active='1'", dbcon);
                cmd.Parameters.AddWithValue("@login", username);
                cmd.Parameters.AddWithValue("@password", password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Login Successful===");
                        Console.WriteLine();
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        return data;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Login Failed===");
                        Console.WriteLine();
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// Returns the UserID of a user
        /// </summary>
        /// <param name="Login">The user Login</param>
        /// <returns>the UserID</returns>
        public int GetUserID(string Login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand($"select UserID from Users where Login=@login and Active = '1'", dbcon);
                cmd.Parameters.AddWithValue("@login", Login);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (int)reader[0];
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns the Login given a UserID
        /// </summary>
        /// <param name="UserID"> the UserID</param>
        /// <returns>The Login</returns>
        public string GetLogin(int UserID)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("select Login from Users where UserID=@UserID and Active='1'", dbcon);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader[0].ToString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a Login exists in the database
        /// </summary>
        /// <param name="Login">the login to be checked</param>
        /// <returns>returns true if the login exists.False otherwise</returns>
        public bool CheckIfExists(string Login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();

                var cmd = new SqlCommand("select login from Users where login=@login and Active ='1'", dbcon);
                cmd.Parameters.AddWithValue("@login", Login);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("===Username does not exist===");
                        Console.WriteLine();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Strores the message to the database
        /// </summary>
        /// <param name="receiver">the name of the receiver</param>
        /// <param name="Login">the name of the sender</param>
        /// <returns>the message</returns>
        public string WriteToDatabase(string receiver,string Login)
        {
            string sender = Login;
            Console.WriteLine("Insert Message");
            string message = Console.ReadLine();

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand("insert into Messages values(@Message,@Date,@Sender,@Receiver)", dbcon);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@Date", System.DateTime.Now);
                cmd.Parameters.AddWithValue("@Sender", GetUserID(sender));
                cmd.Parameters.AddWithValue("@Receiver", GetUserID(receiver));
                var affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Message Sent!!!===");
                    Console.WriteLine();
                }
            }
            return message;
        }

        /// <summary>
        /// Shows a message
        /// </summary>
        /// <param name="choice">If choice=0 the user is the sender .if choice =1 the user is the receiver</param>
        /// <param name="Login">The user that is going to be sender or receiver</param>
        public void ShowMessage(int choice,string Login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand();
                if (choice == 1)
                {
                    cmd = new SqlCommand($"select * from Messages where ReceiverID = @receiver", dbcon);
                    cmd.Parameters.AddWithValue("@receiver", GetUserID(Login).ToString());
                }
                else if (choice == 0)
                {
                    cmd = new SqlCommand($"select * from Messages where SenderID = @sender", dbcon);
                    cmd.Parameters.AddWithValue("@sender", GetUserID(Login).ToString());
                }

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

        /// <summary>
        /// Shows incoming and outgoing messages of a user
        /// </summary>
        /// <param name="login">The user</param>
        public void ShowAll(string login)
        {
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

        /// <summary>
        /// This function enables the chat mode with another user
        /// </summary>
        /// <param name="username">The sender</param>
        /// <param name="Login">The receiver</param>
        public void EnterChat(string username,string Login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);

            using (dbcon)
            {
                dbcon.Open();
                var cmd = new SqlCommand();
                cmd = new SqlCommand("select * from Messages where (ReceiverID = @receiver and SenderID = @sender)" +
                    "or (SenderID = @receiver and ReceiverID = @sender) ", dbcon);
                cmd.Parameters.AddWithValue("@receiver", GetUserID(Login).ToString());
                cmd.Parameters.AddWithValue("@sender", GetUserID(username));
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\t\t===CHAT MODE===");
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        string[] data = new string[5];
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();


                        if (GetLogin(int.Parse(data[3])) == Login)
                        {
                            Console.WriteLine($" {data[1]}  ");
                        }
                        else
                        {
                            Console.WriteLine($" \t\t {data[1]} ");
                        }
                    }
                    Console.WriteLine();
                }
            }
           
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
                var cmd = new SqlCommand("select Message from Messages where MessageID = @number", dbcon);
                cmd.Parameters.AddWithValue("@number", number);
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
                var cmd = new SqlCommand("select SenderID from Messages where MessageID = @number", dbcon);
                cmd.Parameters.AddWithValue("@number", number);
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
                var cmd = new SqlCommand($"select ReceiverID from Messages where MessageID = @number", dbcon);
                cmd.Parameters.AddWithValue("@number", number);
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
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Message Updated===");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Delete all the messages of a user
        /// </summary>
        /// <param name="login">the user</param>
        public void DeleteAll(string login)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();

                var cmd2 = new SqlCommand("delete from Messages where ReceiverID= @login or SenderID= @login", dbcon);
                cmd2.Parameters.AddWithValue("@login", GetUserID(login));
                var affectedRows = cmd2.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Messages Deleted===");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Deletes specific message 
        /// </summary>
        /// <param name="choice">The messageid to be deleted</param>
        public void Delete(int choice)
        {

            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd2 = new SqlCommand("delete from Messages where MessageID= @choice", dbcon);
                cmd2.Parameters.AddWithValue("@choice", choice);
                var affectedRows = cmd2.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===Message Deleted===");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Deletes specific user
        /// </summary>
        /// <param name="login">the user</param>
        public void Delete(string login)
        {
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
        /// Updates all details of a specific user
        /// </summary>
        /// <param name="login">the old login</param>
        /// <param name="newlogin">the new login</param>
        /// <param name="password">the new password</param>
        /// <param name="role">the new role</param>
        public void Update(string login,string newlogin,string password,string role)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();

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
        /// Inserts a new user in the database
        /// </summary>
        /// <param name="login">the login of the user</param>
        /// <param name="password">the password of the user</param>
        /// <param name="role">the role of the user</param>
        public void Insert(string login,string password,string role)
        {
            SqlConnection dbcon = new SqlConnection(connectionstring);
            using (dbcon)
            {
                dbcon.Open();
                var cmd2 = new SqlCommand($"EXEC dbo.AddUser @pLogin = @login, @pPassword = @password ,@pRole = @role", dbcon);
                cmd2.Parameters.AddWithValue("@login", login);
                cmd2.Parameters.AddWithValue("@password", password);
                cmd2.Parameters.AddWithValue("@role", role);
                var affectedRows = cmd2.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("===User created===");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Shows all active users of the database.
        /// </summary>
        public void View()
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
    }
}

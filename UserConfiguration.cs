using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using ReversiSerializableTypes.UserData;

namespace Reversi_Online_Server_1._1
{
    class UserConfiguration
    {
        ClientDialog client;
        public UserConfiguration(ClientDialog client)
        {
            this.client = client;
        }
        public bool SignUp(EditableUserData editableUserData)
        {
            SqlCommand command = new SqlCommand("SELECT username FROM users WHERE username=@username;", ReversiDatabaseManager.Database.Connection);
            command.Parameters.AddWithValue("username", editableUserData.Profile.Username);
            bool username_reserved;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                username_reserved = reader.HasRows;
            }
            
            if (!username_reserved)
            {
                command.CommandText = "INSERT INTO users(username, password, name, surname, email, join_datetime) VALUES(@username, @password, @name, @surname, @email, @join_datetime);";
                //command.Parameters.AddWithValue("username", editable.Profile.Username);
                command.Parameters.AddWithValue("password", editableUserData.Private.Password);
                command.Parameters.AddWithValue("name", editableUserData.Private.Name);
                command.Parameters.AddWithValue("surname", editableUserData.Private.Surname);
                command.Parameters.AddWithValue("email", editableUserData.Private.Email);
                command.Parameters.AddWithValue("join_datetime", DateTime.Now);
                command.ExecuteNonQuery();

                /*command.CommandText = "INSERT INTO profile_images(username, profile_image) VALUES(@username, @profile_image);";
                //command.Parameters.AddWithValue("username", editable.Profile.Username);
                byte[] image_data = new Serializer().Serialize(editable.Profile.ProfilePhoto);
                command.Parameters.AddWithValue("profile_image", image_data);
                command.ExecuteNonQuery();*/
                editableUserData.Profile.ProfilePhoto.Save($@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures)}\{editableUserData.Profile.Username}.png");
                
                
                this.client.Username = editableUserData.Profile.Username;
                this.MarkOnline();
                client.SendMessage("SUCCESS", ("description", "Account created successfully. You've been automatically logged in"));
                return true;
            }
            else
            {
                client.SendMessage("FAIL", ("description", "Username is already reserved"));
                return false;
            }
        }

        public bool LogIn(LoginUserData loginUserData)
        {
            bool valid = LoginDataMatch(loginUserData);
            if (valid)
            {
                if (this.client.Authenticated) MarkOffline();
                this.client.Username = loginUserData.Username;
                this.MarkOnline();
                this.client.SendMessage("SUCCESS", ("description", "Successfully logged in"));
            }
            else
            {
                this.client.SendMessage("FAIL", ("description", "Username or password is incorrect"));
            }
            return valid;
        }

        public void MarkOnline()
        {
            if (!this.client.Authenticated) throw new NullReferenceException("client should be already authenticated");
            SqlCommand command = new SqlCommand("UPDATE users SET online=1, ipv4=@ip, last_login_datetime=@now WHERE username=@username;", ReversiDatabaseManager.Database.Connection);
            command.Parameters.AddWithValue("username", this.client.Username);
            command.Parameters.AddWithValue("ip", this.client.IPEndPoint.Address.ToString());
            command.Parameters.AddWithValue("now", DateTime.Now);
            command.ExecuteNonQuery();
        }
        [RequiresAuthentication]
        public void MarkOffline()
        {
            if (!this.client.Authenticated) throw new InvalidOperationException("client should be already authenticated");
            SqlCommand command = new SqlCommand("UPDATE users SET online=0 WHERE username=@username;", ReversiDatabaseManager.Database.Connection);
            command.Parameters.AddWithValue("username", this.client.Username);
            command.ExecuteNonQuery();
        }

        public static bool LoginDataMatch(LoginUserData data)
        {
            SqlCommand command = new SqlCommand("SELECT password FROM users WHERE username=@username", ReversiDatabaseManager.Database.Connection);
            command.Parameters.AddWithValue("username", data.Username);
            bool valid = true;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows) valid = false;
                else
                {
                    reader.Read();
                    if ((string)reader["password"] != data.Password)
                    {
                        valid = false;
                    }
                }
            }
            return valid;
        }
    }
}

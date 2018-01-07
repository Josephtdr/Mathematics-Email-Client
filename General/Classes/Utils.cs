using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Net;
namespace The_Email_Client 
{
    public class Profiles {
        public string ID { get; set; }
        public string Email { get; set; }
        public string Port { get; set; }
        public string Server { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public int Settings_ID { get; set; }
        
        public void GetInfofromDB(Profiles Profile, string UserName) {
            
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE UserName='{UserName}'", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    Profile.ID = Common.Cleanstr(reader[0]);
                    Profile.Name = Common.Cleanstr(reader[1]);
                    Profile.Settings_ID = Convert.ToInt16(reader[5]);
                }
                cnctDTB.Close();

                cnctDTB.Open();
                cmd = new OleDbCommand($"SELECT * FROM Settings WHERE ID={Profile.Settings_ID}", cnctDTB);
                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    Profile.Port = Common.Cleanstr(reader[1]);
                    Profile.Server = Common.Cleanstr(reader[2]);
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        public void UpdateDatabasefromProfile(Profiles profile) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up a connection to the database
            OleDbCommand cmd = new OleDbCommand("", cnctDTB);
            try {
                cnctDTB.Open();//opens the connection
                cmd.CommandText = $"UPDATE Profiles SET Name ='{ profile.Name }', UserName ='{ profile.UserName }' WHERE ID ={ profile.ID }";
                cmd.ExecuteNonQuery();//updates profile with name and username
                cnctDTB.Close(); cnctDTB.Open();//closed then re opens the connection, to allow another command to run
                if (profile.Settings_ID != 1) { //checks if user did not previously have default settings
                    if (profile.Port == "587" && profile.Server == "smtp.gmail.com") {//checks if they now have default settings
                        cmd.CommandText = $"DELETE FROM Settings WHERE ID = {profile.Settings_ID};";
                        cmd.ExecuteNonQuery();//deletes there now redundant old settings
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.CommandText = $"UPDATE Profiles SET Settings_ID = 1 WHERE ID = {profile.ID};";
                        cmd.ExecuteNonQuery();//updates their profile to reflect their default settings
                    }
                    else {
                        cmd.CommandText = $"UPDATE Settings SET Port ='{ profile.Port }', Server ='{ profile.Server }' WHERE ID ={ profile.Settings_ID };";
                        cmd.ExecuteNonQuery();//updates their current none default settings
                    }
                }
                else if (!(profile.Port == "587" && profile.Server == "smtp.gmail.com")) {//checks for defualt settings
                    cmd.CommandText = $"INSERT INTO Settings (Port, Server) VALUES " +
                        $"('{profile.Port}','{profile.Server}');";
                    cmd.ExecuteNonQuery();//creates a new none default settings value for their profile
                    cnctDTB.Close(); cnctDTB.Open();
                    cmd.CommandText = $"SELECT ID FROM Settings WHERE Port = '{profile.Port}' AND Server = '{profile.Server}';";
                    OleDbDataReader Reader = cmd.ExecuteReader();//selects all instances from the database with the users settings
                    int checkint = 1;
                    int tempsettingsID = 0;
                    while (Reader.Read()) {
                        if (checkint <= 1)
                            Common.Profile.Settings_ID = Convert.ToInt16(Reader[0]);
                        else 
                            tempsettingsID = Convert.ToInt16(Reader[0]);
                        ++checkint;
                    }
                    if (tempsettingsID != 0) {
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.CommandText = $"DELETE FROM Settings WHERE Settings_ID = {tempsettingsID};";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }
    }

    public class Encryption {
        public static string HashString(string stringtohash) {
            //Create the salt value with a cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(stringtohash, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
        
        public static bool VerifyPasswordorEmail(string UserName, string stringtoverify, bool Password)
        {
            string passwordemail = "";
            if (Password) passwordemail = "Password";
            else passwordemail = "Email";
            //Fetch the stored value
            string savedHash = "";
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT {passwordemail} FROM Profiles WHERE UserName='{UserName}';", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) savedHash = Common.Cleanstr(reader[0]);
            }
            catch (Exception err)
            {
                Common.Profile = new Profiles();
                MessageBox.Show(err.Message);
            }
            finally { cnctDTB.Close(); }
            if (string.IsNullOrEmpty(savedHash)) {
                if(Password)
                    MessageBox.Show($"Username {UserName} does not exist.");
                return false;
            }
            //Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(savedHash);
            //Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            //Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(stringtoverify, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Compare the results
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    MessageBox.Show($"Incorrect {passwordemail}", "Error!");
                    return false;
                }
            if (Password) Common.Profile.Password = stringtoverify;
            else Common.Profile.Email = stringtoverify;
            Common.Profile.UserName = UserName;
            return true;
        }
    }

    public class Email
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string[] Recipients { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> AttachmentNames { get; set; }

        public Email()
        {
            AttachmentNames = new List<string>();
        }

        public void Send()
        {
            try
            {
                MailAddress fromAddress = new MailAddress(UserEmail, UserName);

                SmtpClient smtp = new SmtpClient
                {
                    Host = Server,
                    Port = Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, UserPassword),
                };

                using (MailMessage message = new MailMessage() { From = fromAddress, Subject = Subject, Body = Body })
                {

                    if (Recipients != null) foreach (string recipient in Recipients) message.To.Add(new MailAddress(recipient));
                    if (CC != null) if(CC[0] != "") foreach (string cc in CC) message.CC.Add(new MailAddress(cc));
                    if (BCC != null) if(BCC[0] != "") foreach (string bcc in BCC) message.Bcc.Add(new MailAddress(bcc));

                    if(AttachmentNames != null)
                        foreach (string Attachment in AttachmentNames)
                        {
                            Attachment attachment = new Attachment(Attachment, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = attachment.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(Attachment);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(Attachment);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(Attachment);
                            disposition.FileName = System.IO.Path.GetFileName(Attachment);
                            disposition.Size = new FileInfo(Attachment).Length;
                            disposition.DispositionType = DispositionTypeNames.Attachment;
                            message.Attachments.Add(attachment);
                            Console.WriteLine(Attachment);
                        }
                    
                    smtp.Send(message);
                 }
            }
            catch (Exception error) { System.Windows.Forms.MessageBox.Show(error.ToString()); }
        }
    }

}

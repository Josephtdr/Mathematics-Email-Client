using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Net;
using System.Windows.Forms;
namespace The_Email_Client
{
    public class Settings
    {
        public string Email { get; set; }
        public string Port { get; set; }
        public string Server { get; set; }
        public string PasswordID { get; set; }
        public string Name { get; set; }

        public Settings Defaults()
        {//ll
            Settings Defaults = new Settings()
            {
                Email = "testofcsharperinoemailerino@gmail.com",
                PasswordID = "1",
                Name = "Stool",
                Port = "587",
                Server = "smtp.gmail.com"
            };
            return Defaults;
        }

        public Settings UpdateSettingsfromDB(string email)
        {
            Settings newsettings = new Settings();

            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE Email='{email}'", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    newsettings = new Settings
                    {
                        Email = Common.Cleanstr(reader[1]),
                        PasswordID = Common.Cleanstr(reader[5]),
                        Name = Common.Cleanstr(reader[2]),
                        Port = Common.Cleanstr(reader[3]),
                        Server = Common.Cleanstr(reader[4])
                    };
                }
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
            finally
            {
                cnctDTB.Close();
            }
            return newsettings;
        }

        public void UpdateDatabasefromSettings(Settings settings)
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"UPDATE Profiles SET Name ='{ settings.Name }', Port ={ Convert.ToInt16(settings.Port)}, Server='{ settings.Server }' Where Email='{ Common.Email }'", cnctDTB);
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
            finally
            {
                cnctDTB.Close();
            }
        }
    }

    public class PasswordHashing
    {
        public static string HashPassword(string password)
        {
            //Create the salt value with a cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public static bool VerifyPassword(string email, string password)
        {
            //Fetch the stored value
            string savedPasswordHash = "sXgXELuRUY1AA+zdIJhemHznP9tWElMqyC93jBCcpD1TsPh5";
            int passID = 0;
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE Email='{email}';", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) { passID = Convert.ToInt16(Common.Cleanstr(reader[5])); }

                cnctDTB.Close();
                cnctDTB.Open();
                cmd.CommandText = $"SELECT Password FROM Passwords WHERE ID={passID};";
                reader = cmd.ExecuteReader();
                while (reader.Read()) savedPasswordHash = Common.Cleanstr(reader[0]);
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }

            //Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            //Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            //Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Compare the results
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    System.Windows.MessageBox.Show("Incorrect Password", "Error!");
                    return false;
                }

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
                    if (CC[0] != "") foreach (string cc in CC) message.CC.Add(new MailAddress(cc));
                    if (BCC[0] != "") foreach (string bcc in BCC) message.Bcc.Add(new MailAddress(bcc));

                    foreach (string Attachment in AttachmentNames)
                    {
                        if (Attachment != null)
                        {
                            Attachment attachment = new Attachment(Attachment, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = attachment.ContentDisposition;
                            disposition.CreationDate = File.GetCreationTime(Attachment);
                            disposition.ModificationDate = File.GetLastWriteTime(Attachment);
                            disposition.ReadDate = File.GetLastAccessTime(Attachment);
                            disposition.FileName = System.IO.Path.GetFileName(Attachment);
                            disposition.Size = new FileInfo(Attachment).Length;
                            disposition.DispositionType = DispositionTypeNames.Attachment;
                            message.Attachments.Add(attachment);
                        }
                    }


                    smtp.Send(message);

                }
            }
            catch (Exception error)
            {
                System.Windows.Forms.MessageBox.Show(error.Message);
            }
        }
    }
}

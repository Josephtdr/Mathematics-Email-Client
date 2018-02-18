using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Net;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace The_Email_Client 
{
    public class Student {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int ID { get; set; }
        public bool InClass { get; set; }
    }
    public class Class {
        public string Name { get; set; }
        public int ID { get; set; }
    }
    public class Profiles {
        public string ID { get; set; }
        public string Email { get; set; }
        public string Port { get; set; }
        public string Server { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public int Settings_ID { get; set; }
        
        private static void DeleteRedundantSettings(int Settings_ID) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up a connection to the database
            try {
                cnctDTB.Open();//opens the connection
                OleDbCommand cmd = new OleDbCommand($"SELECT ID FROM Profiles WHERE Settings_ID = {Settings_ID};", cnctDTB);
                OleDbDataReader Reader = cmd.ExecuteReader();
                if (!Reader.HasRows) { //checks no other profiles use the same old settings values
                    cnctDTB.Close(); cnctDTB.Open();
                    cmd.CommandText = $"DELETE FROM Settings WHERE ID = {Settings_ID};";
                    cmd.ExecuteNonQuery();//if none have it, deletes there now redundant old settings
                }
                cnctDTB.Close();//closes the connection}
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }
        private static void CreateNewSettings(Profiles profile) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up a connection to the database
            try {
                cnctDTB.Open(); //opens the connection
                OleDbCommand cmd = new OleDbCommand($"INSERT INTO Settings (Port, Server) VALUES " +
                    $"({profile.Port},'{profile.Server}');", cnctDTB);
                cmd.ExecuteNonQuery();//creates a new none default settings value for their profile
                cnctDTB.Close(); cnctDTB.Open();
                cmd.CommandText = $"SELECT ID FROM Settings WHERE Port = {profile.Port} AND Server = '{profile.Server}';";
                OleDbDataReader Reader = cmd.ExecuteReader();//selects all instances from the database with the users exact settings
                while (Reader.Read())
                    Common.Profile.Settings_ID = Convert.ToInt16(Reader[0]); //updates their local profile to reflect their new settings
                cnctDTB.Close(); cnctDTB.Open();
                cmd.CommandText = $"UPDATE Profiles SET Settings_ID = {Common.Profile.Settings_ID} WHERE ID = {profile.ID};";
                cmd.ExecuteNonQuery(); //updates the users profile to reflect their new settings
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        public void GetInfofromDB(Profiles Profile, string UserName) { //updates local profile from database    
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection
            try {
                cnctDTB.Open(); //opens connection
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE UserName='{UserName}';", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader(); //locates users profile in database from their username
                while (reader.Read()) {
                    Profile.ID = Common.Cleanstr(reader[0]); //updates users ID
                    Profile.Name = Common.Cleanstr(reader[1]); //updates users Name
                    Profile.Settings_ID = Convert.ToInt16(reader[5]); //updates users settings ID
                }
                cnctDTB.Close(); cnctDTB.Open(); //reopensconnection
                cmd = new OleDbCommand($"SELECT * FROM Settings WHERE ID={Profile.Settings_ID};", cnctDTB);
                reader = cmd.ExecuteReader();
                while (reader.Read()) {//locates users settings in database from their settings ID
                    Profile.Port = Common.Cleanstr(reader[1]); //updates users port
                    Profile.Server = Common.Cleanstr(reader[2]); //updates users server
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }//displays error in the event of an error
            finally { cnctDTB.Close(); } //closes connection
        }

        public void UpdateDatabasefromProfile(Profiles profile) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up a connection to the database
            OleDbCommand cmd = new OleDbCommand("", cnctDTB);
            try {
                cnctDTB.Open();//opens the connection
                cmd.CommandText = $"UPDATE Profiles SET Name ='{ profile.Name }', UserName ='{ profile.UserName }' WHERE ID ={ profile.ID };";
                cmd.ExecuteNonQuery();//updates profile with name and username
                cnctDTB.Close(); cnctDTB.Open();//closed then re opens the connection, to allow another command to run
                if (profile.Settings_ID != 1) { //checks if user did not previously have default settings
                    if (profile.Port == "587" && profile.Server == "smtp.gmail.com") {//checks if they now have default settings
                        cmd.CommandText = $"UPDATE Profiles SET Settings_ID = 1 WHERE ID = {profile.ID};";
                        cmd.ExecuteNonQuery();//updates their profile to reflect their default settings

                        DeleteRedundantSettings(profile.Settings_ID);//Deletes any reduant settings
                        Common.Profile.Settings_ID = 1; //updates their local profile to reflect their default settings
                    }
                    else {
                        int tempsettingsid = profile.Settings_ID;
                        cmd.CommandText = $"SELECT ID FROM Settings WHERE Port = {profile.Port} AND Server = '{profile.Server}';";
                        OleDbDataReader tempReader = cmd.ExecuteReader();
                        if (tempReader.HasRows) { //checks if the users new settings already exist in the database
                            while (tempReader.Read())
                                Common.Profile.Settings_ID = Convert.ToInt16(tempReader[0]);
                            cnctDTB.Close(); cnctDTB.Open();
                            cmd.CommandText = $"UPDATE Profiles SET Settings_ID = {Common.Profile.Settings_ID} WHERE ID = {profile.ID};";
                            cmd.ExecuteNonQuery(); //updates the users profile to reflect their new settings
                        }
                        else {
                            CreateNewSettings(profile);
                        }
                        DeleteRedundantSettings(tempsettingsid); //Deletes any reduant settings
                    }
                }
                else if (!(profile.Port == "587" && profile.Server == "smtp.gmail.com")) { //checks if the user has default settings
                    cmd.CommandText = $"SELECT ID FROM Settings WHERE Port = {profile.Port} AND Server = '{profile.Server}';";
                    OleDbDataReader tempReader = cmd.ExecuteReader();
                    if (tempReader.HasRows) { //checks if the users new settings already exist in the database
                        while (tempReader.Read())
                            Common.Profile.Settings_ID = Convert.ToInt16(tempReader[0]);
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.CommandText = $"UPDATE Profiles SET Settings_ID = {Common.Profile.Settings_ID} WHERE ID = {profile.ID};";
                        cmd.ExecuteNonQuery(); //updates the users profile to reflect their new settings
                    }
                    else {
                        CreateNewSettings(profile);
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
            //Combine the salt and stringhash bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //Turn the combined salt+hash into a string for storage
            string savedHash = Convert.ToBase64String(hashBytes);
            return savedHash; //Returns the hashed value.  
        }
        
        public static bool VerifyPasswordorEmail(string UserName, string stringtoverify, bool Password) {
            //Check if fucntion is to verfiy the users password or their email
            string passwordemail = (Password == true ? "Password" : "Email");
            //Fetch the stored value
            string savedHash = "";
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //Set up Database Connection
            try { //Gets the users hashed password or email from the database 
                cnctDTB.Open(); //Open database connection  
                OleDbCommand cmd = new OleDbCommand($"SELECT {passwordemail} FROM Profiles WHERE UserName='{UserName}';", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) savedHash = Common.Cleanstr(reader[0]);
            }
            catch (Exception err) {
                Common.Profile = new Profiles(); //Clears Profile incase it has been written to 
                MessageBox.Show(err.Message); //displays error to user
            }
            finally { cnctDTB.Close(); } //Close database connection
            if (string.IsNullOrEmpty(savedHash)) {
                if(Password)
                    MessageBox.Show($"Username {UserName} does not exist.");
                return false;
                //If no hashed password was found, inform the user they have entered an incorrect User Name and return false
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
                if (hashBytes[i + 16] != hash[i]) {
                    MessageBox.Show($"Incorrect {passwordemail}", "Error!");
                    return false;
                    //If the user has input an incorrect possword or email inform them then return false
                }
            //Updates the users password or email to their local profile along with their username
            if (Password) Common.Profile.Password = stringtoverify;
            else Common.Profile.Email = stringtoverify;
            Common.Profile.UserName = UserName;
            return true;//If the code reaches here returns true as it has all run successfully 
        }
    }
    public class Email {
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

        public Email() {
            AttachmentNames = new List<string>();
        }

        public void Send()
        {
            try {
                MailAddress fromAddress = new MailAddress(UserEmail, UserName);
                SmtpClient smtp = new SmtpClient {
                    Host = Server,
                    Port = Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, UserPassword),
                };
                using (MailMessage message = new MailMessage() { From = fromAddress, Subject = Subject, Body = Body }) {
                    if (Recipients != null) foreach (string recipient in Recipients) message.To.Add(new MailAddress(recipient));
                    if (CC != null) if(CC[0] != "") foreach (string cc in CC) message.CC.Add(new MailAddress(cc));
                    if (BCC != null) if(BCC[0] != "") foreach (string bcc in BCC) message.Bcc.Add(new MailAddress(bcc));

                    if(AttachmentNames != null)
                        foreach (string Attachment in AttachmentNames) {
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
    public class PDF {
        //Organises equations such that the are split into strings of under 37 characters
        public static List<Tuple<Equation, bool>> SortEquationList(List<Equation> EquList, bool dif) {
            //Creates a touple of equations and bools, the bool when true means this portion of an equation is the inital part
            List<Tuple<Equation, bool>> tupleList = new List<Tuple<Equation, bool>>();
            List<Term> tempTermList; //Variable to temporarily store up to 37 characters worth of terms in an equation
            int length; //Variable which stores the current length of string of the temptermlist
            int iterations; //Variable which will indicate if a line is the initial portion of an equation
            foreach (Equation equ in EquList) {//loops through each equation in EquList
                iterations = 0;
                length = 0;
                tempTermList = new List<Term>();
                foreach (Term term in equ.Components) {//loops through each term in the equation
                    length += term.ToString().Length;//adds the length of the terms string to length
                    if (length < 37)//Checks if the overall length is too long
                        tempTermList.Add(term);//Adds the term to the current list of terms 
                    else {//happens if adding the term to the current list of terms would make it too long
                        //Covertes the list of terms into an equation of the correct type, then into a Tuple which is added to the list of tuples 
                        if (dif) tupleList.Add(new Tuple<Equation, bool>(new Diferentiation(tempTermList), iterations == 0 ? true : false));
                        else tupleList.Add(new Tuple<Equation, bool>(new Integration(tempTermList), iterations == 0 ? true : false));
                        tempTermList = new List<Term>();//resets the current list of terms
                        tempTermList.Add(term);//adds the term which could not be previously added to this new list of terms
                        length = term.ToString().Length;//adds the length of said term to the length variable
                        ++iterations;//itterates the itterations variable
                    }
                }
                if (tempTermList[0] != null) {//occurs once there a no more terms to loop through and checks if the list of terms is empty
                    //Covertes the list of terms into an equation of the correct type, then into a Tuple which is added to the list of tuples 
                    if (dif) tupleList.Add(new Tuple<Equation, bool>(new Diferentiation(tempTermList), iterations == 0 ? true : false));
                    else tupleList.Add(new Tuple<Equation, bool>(new Integration(tempTermList), iterations == 0 ? true : false));
                }
            }
            return tupleList; //returns the list of tuples
        }
        //Writes the questions and answers to a PDF document
        public static void CreateLines(ref PdfDocument document, List<Tuple<Equation, bool>> TupleList, string Type) {
            PdfPage page; //initialises the variable which tracks the current page
            XGraphics gfx;
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold); //sets chosen font and style
            int numpages = TupleList.Count / 14; //Works out the number of pages which will be required for the number of given questions
            int QuestionNum = 0; //variable is used to track the current question as so it can be displayed in the PDF
            string line; //initialises the variable which stores the current string for each line of a page
            for (int i = 0; i < numpages + 1; i++) {//Loops through each of the required number of pages
                page = document.AddPage();//Creates a new page
                gfx = XGraphics.FromPdfPage(page);//Updates the new graphics to display on said page
                //Checks if it is the first Page and if so writes a title to said page
                if (i == 0) gfx.DrawString($"{Type}:", font, XBrushes.Black, new XRect(0, 10, page.Width, page.Height), XStringFormat.TopCenter);
                for (int j = 0; j < 14; j++) {//loops through each line in a page
                    if (j + (14 * i) < TupleList.Count) {//makes sure the current line is not outside the bounds of the list of questions/answers
                        //creates the string to be written to a line depending upon if it is displaying a question or an answer
                        line = (Type == "Questions" ? TupleList[j + (14 * i)].Item1.ToString() : TupleList[j + (14 * i)].Item1.FprimeEquationToString());
                        //itterates the QuestionNum variable if the previous question has been fully written to the page
                        QuestionNum += TupleList[j + (14 * i)].Item2 == true ? 1 : 0;
                        //Draws the new line to the page removing any unnecessary characters
                        gfx.DrawString($"{  (TupleList[j + (14 * i)].Item2 == true ? $"{QuestionNum}." : "") }\t " +
                            $"{ Regex.Replace(line, "[^0-9-/+/^/x ]+", "") }",
                            font, XBrushes.Black, new XRect(10, 10 + ((j + 2) * 50), page.Width, page.Height), XStringFormat.TopLeft);
                    }
                }
            }
        }

        public static string CreatePDF(List<Equation> EquList, bool dif) { //Creates a quation/answer PDF from a list of equations 
            PdfDocument document = new PdfDocument(); //Creates a blank pdf document
            //Formats the list of questions such that no questions will be so long they will exceed the edge of the page
            List<Tuple<Equation, bool>> TupleList = SortEquationList(EquList, dif);

            //Creates Question Pages
            CreateLines(ref document, TupleList, "Questions");
            //Creates Answer Pages
            CreateLines(ref document, TupleList, "Answers");

            //Creates a string following the format of 'day/month/year [current time]'
            string datetime = DateTime.Now.ToString().Replace('/', '-').Replace(':', '.');
            //Creates the name of the file
            string filename = $"{datetime}.pdf";
            //Creates the file
            document.Save(filename);
            //Returns the filename
            return filename;
        }

        //Function to create a PDF from a list of Equations
        public static bool CreatePDFfromList(List<Equation> EquList, bool email, Class ClasstoEmail) {
            //Checks if the user has selected to email the PDF and if so checks to make sure they have chosen a class to send the PDF too
            if (email && ClasstoEmail == null) {
                MessageBox.Show("You must select a class to email.", "Error!");
                return false;//informs the user they have not selected a class to email their PDF to 
            }
            //A Function which will create the PDF from the question list and then return the name of said PDF
            string filename = CreatePDF(EquList, EquList[0].GetType() == typeof(Diferentiation) ? true : false);
            if (email)
                SendPDFEmail(filename, ClasstoEmail);//Emails PDF to selected class or recipients
            else
                Process.Start(filename);//Opens PDF
            return true;//Returns true indicating function has run successfully 
        }
        //Emails a pdf to a specific class or students 
        public static void SendPDFEmail(string filename, Class ClasstoEmail) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            string emaillist = "";
            try {
                cnctDTB.Open();//open connection to database
                //Gets emails from students within relavent class
                OleDbCommand cmd = new OleDbCommand($"SELECT Students.Email FROM Class_Lists, Students WHERE " +
                    $"Class_Lists.Student_ID = Students.ID AND Class_Lists.Class_ID = {ClasstoEmail.ID};", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {//sets emails in correct format 
                    emaillist += $"{Common.Cleanstr(reader[0])};";
                }
            }
            //Informs user there is an error in the case of an error
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }//closes database connection

            //Sets up a new email with all applicable values
            Email tempemail = new Email {
                Server = Common.Profile.Server,
                Port = Convert.ToInt16(Common.Profile.Port),
                UserEmail = Common.Profile.Email,
                UserPassword = Common.Profile.Password,
                UserName = Common.Profile.Name,
                Recipients = (emaillist.Split(';')).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList().ToArray(),
                Subject = $"{filename} Worksheet for class {4}",
                Body = "",
                AttachmentNames = new List<string>() { filename }
            };
            tempemail.Send();//Sends email
        }
    }

}

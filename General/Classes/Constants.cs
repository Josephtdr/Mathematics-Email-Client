using System;

namespace The_Email_Client {
    /// <summary>
    /// Class used to store constant values throughout my code
    /// </summary>
    internal class Constants {
        //String used to connect to the database
        public const string DBCONNSTRING = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=.\..\..\Email Client Database.accdb;Persist Security Info=False";
        //Regex pattern for a valid email
        public const string VALIDEMAILPATTERN = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
        //regex pattern for a valid password
        public const string VALIDPASSWORDPATTERN = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$";
        public const string DEFAULTEMAIL = "martinsmathematicsclient@gmail.com";//default email address
        public const string DEFAULTPASSWORD = "capitalutcreading";//default password
        public const int DEFAULTPORT = 587;//default port
        public const string DEFAULTSERVER = "smtp.gmail.com";//default server
        public static Random Rnd { get; } = new Random();
    }
}
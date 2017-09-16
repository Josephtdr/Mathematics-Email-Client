namespace The_Email_Client
{
    internal class Constants
    {
        public const string DBCONNSTRING = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=AddressBook.accdb;Persist Security Info=False";
        public const string VALIDEMAILPATTERN = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
        public const string VALIDPASSWORDPATTERN = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$";
    }
}
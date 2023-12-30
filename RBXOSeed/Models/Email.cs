using RBXOSeed.Data;
using System.ComponentModel.DataAnnotations;

namespace RBXOSeed.Models
{
    public class Email : AuditFields
    {
        [EmailAddress]
        public string EmailAddress { get; set; }

        public static LiteDB.ILiteCollection<Email>? GetAll()
        {
            try
            {
                var emails = DbContext.DB.GetCollection<Email>(DbContext.RSRV_EMAIL);
                return emails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string SaveEmail(Email email)
        {
            var emails = GetAll();
            if (emails == null)
            {
            }
            else
            {
                var emailRec = emails.FindOne(x => x.EmailAddress == email.EmailAddress);
                if (emailRec != null)
                {
                    return "Record Already Exist";
                }
                else
                {
                    //update
                }
            }

            return "Error Saving Email";
        }

        public static void DeleteEmail(string emailAddress)
        {
            var emails = GetAll();
            if (emails == null)
            {

            }
            else
            {
                emails.DeleteManySafe(x => x.EmailAddress == emailAddress);
            }
        }
    }
}

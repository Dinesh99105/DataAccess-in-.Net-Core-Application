using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace MongoDBUI
{
    class Program
    {

        private static MongoDBDataAccess db;
        private static readonly string tableName = "Contacts";
        static void Main(string[] args)
        {
            db = new MongoDBDataAccess("MongoContactsDB",GetConnectionString());

            ContactModel user = new ContactModel
            {
                FirstName = "Charity",
                LastName = "Corey"
            };
            user.EmailAddresses.Add(new EmailAddressModel {EmailAddress = "nope@aol.com" });
            user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "me@timothycorey.com" });
            user.PhoneNumbers.Add(new PhoneNumberModel {PhoneNumber = "555-1212" });
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "555-9876" });

            //CreateContact(user);
            //GetAllContacts();
            //GetContactById("f3ffa6e2-7169-461f-a559-e561c258d746");
            //UpdateContactFirstName("Dinesh", "f3ffa6e2-7169-461f-a559-e561c258d746");
            //RemovePhoneNumberFromUser("555-1212", "f3ffa6e2-7169-461f-a559-e561c258d746");
            // GetAllContacts();
            RemoveUser("f3ffa6e2-7169-461f-a559-e561c258d746");

            Console.WriteLine("Done Processing MongoDB");
            Console.ReadLine();
        }

        private static void CreateContact(ContactModel contact)
        {
            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static void GetContactById(string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName,guid);
            Console.WriteLine($"{contact.Id}:{contact.FirstName}{contact.LastName}");
        }

        private static void GetAllContacts()
        {
            var contacts = db.LoadRecords<ContactModel>(tableName);

            foreach(var contact in contacts)
            {
                Console.WriteLine($"{contact.Id}:{contact.FirstName}{contact.LastName}");
            }
        }

        private static void UpdateContactFirstName(string firstName , string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            contact.FirstName = firstName;
            db.UpsertRecord(tableName,contact.Id,contact);
        }

        private static void RemovePhoneNumberFromUser(string phoneNumber , string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            contact.PhoneNumbers = contact.PhoneNumbers.Where(x => x.PhoneNumber != phoneNumber).ToList();
            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static void RemoveUser(string id)
        {
            Guid guid = new Guid(id);
            db.DeleteRecord<ContactModel>(tableName, guid);
        }

        private static string GetConnectionString(string connectionStringName = "Default")
        {
            string output = "";
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            output = config.GetConnectionString(connectionStringName);
            return output;
        }
    }
}

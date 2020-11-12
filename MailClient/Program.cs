using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using MailKit.Net.Imap;
using Consul;
using Aspose.Email.Clients.ActiveSync.TransportLayer;
using S22.Imap;
using ImapClient = S22.Imap.ImapClient;

// This is where the magic happens
//Ти хто ?
namespace MailClient
{
    class Program
    {
        // Global variables
        private static string Host = "";
        private static int Port = 0;
        public static string FromAddress, FromPass;

        

       
        public static void EmailIsSentPopupCall()
        {
            // Create a new instance of the class
            EmailIsSentPopup popup = new EmailIsSentPopup();

            // Show the popup
            popup.Show();

            // Make the popup active
            popup.Activate();
        }

        /// <summary>
        /// Shows an error popup, not the best thing to see...
        /// </summary>
        /// <param name="ErrorMessage">The error message</param>
        public static void ErrorPopupCall(string ErrorMessage)
        {
            // Create a new instance of the ErrorPopup class
            ErrorPopup Error = new ErrorPopup();

            // Add the content of the ErrorLabel
            Error.ErrorLabel.Content = ErrorMessage;

            // Show the error
            Error.Show();

            // Make the error popup active
            Error.Activate();
        }

        public static void LoggedInPopupCall()
        {
            // Create a new instance of LoginConfirmedPopup 
            LoginConfirmedPopup popup = new LoginConfirmedPopup();

            // Show the popup
            popup.Show();
        }

        //####################
        //#Send email methods#
        //####################

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="ToAddress">The address of the recipient of the email</param>
        /// <param name="FromAddress">The address of the sender of the email</param>
        /// <param name="FromPass">The password of the sender of the email</param>
        /// <param name="Subject">The subject of the email</param>
        /// <param name="Body">The body of the email</param>
        public static void SendEmail(string ToAddress, string FromAddress, string FromPass, string Subject, string Body, string CC, string S_Attachment)
        {
            // First check if all the TextBoxes are filled in and then check the host.
            if (CheckArguments(ToAddress, FromAddress, FromPass, Subject, Body) == true && CheckEmailHost(FromAddress) == true)
            {
                // Створіть новий екземпляр класу SmtpClient
                SmtpClient smtpClient = new SmtpClient
                {
                    // Встановыть хост
                    Host = Host,

                    //Встановіть порт
                    Port = Port,

                    // Увімкнути SSL
                    EnableSsl = true,

                    // Призначте спосіб доставки
                    DeliveryMethod = SmtpDeliveryMethod.Network,

                    //Присвоїти облікові дані
                    Credentials = new NetworkCredential(FromAddress, FromPass),

                    // Встановіть час очікування
                    Timeout = 20000
                };

                //Створіть новий MailMessage, який називається Message, та додайте властивості
                MailMessage Message = new MailMessage(FromAddress, ToAddress, Subject, Body);

                // Якщо щось є на вкладці CC, додайте це до повідомлення
                if (!(string.IsNullOrEmpty(CC)))
                {
                    // Спочатку перетворіть рядок CC у MailAddress CC
                    MailAddress Copy = new MailAddress(CC);

                    // Додайте копію до повідомлення
                    Message.CC.Add(Copy);
                } 

                // Якщо щось є в S_Attachment, додайте це до повідомлення
                if (!(string.IsNullOrEmpty(S_Attachment)))
                {
                    // Створіть новий вкладення
                    Attachment File = new Attachment(S_Attachment);

                    // Додайте його до повідомлення
                    Message.Attachments.Add(File);
                }

                // Надішліть повідомлення
                try
                {
                    smtpClient.Send(Message);
                }
                catch (Exception exception)
                {
                    // Створіть повідомлення про помилку
                    string ErrorMessage = "ERROR 20001:" + "\n" + exception.ToString();

                    // Показати повідомлення про помилку користувачеві
                    ErrorPopupCall(ErrorMessage);

                    // Прибрати
                    Message.Dispose();

                    // Припиніть виконувати цей метод
                    return;
                }

                // Прибирати
                Message.Dispose();

                // Зателефонуйте цим методом, щоб повідомити користувача про те, що повідомлення надіслано
                EmailIsSentPopupCall();
            }
            else
            {
                // Якщо ні, зупиніть виконання цього методу.
                return;
            }
        }

        /// <summary>
        /// Перевіряє хост і чи справді введена адреса з адреси є адресою.
        /// </summary>
        /// <param name="FromAddress">The address of the sender of the email</param>
        private static bool CheckEmailHost(string FromAddress)
        {
            // Спочатку розділіть FromAddress між @
            string[] splitFromAddress = FromAddress.Split('@');

            // Потім перевірте, чи існує splitFromAddress [1]
            if (splitFromAddress.Length == 2)
            {
                // Цей комутатор перевіряє, який це хост, і призначає змінні Host і Port відповідним Host і Port
                switch (splitFromAddress[1])
                {
                    case "gmail.com":
                        Host = "smtp.gmail.com";
                        Port = 587;
                        return true;
                    case "yahoo.com":
                        Host = "smtp.mail.yahoo.com";
                        Port = 465;
                        return true;
                    case "hotmail.com":
                        Host = "smtp.live.com";
                        Port = 587;
                        return true;
                    case "hotmail.nl":
                        Host = "smtp.live.com";
                        Port = 587;
                        return true;
                    case "icloud.com":
                        Host = "smtp.mail.me.com";
                        Port = 587;
                        return true;
                    case "mail.ru":
                        Host = "smtp.mail.ru";
                        Port = 465;
                        return true;
                    default:
                        ErrorPopupCall("ERROR 30002" + "\n" + "Description: reached default in switch(splitFromAddres[1])");
                        return false;
                }
            }
            else
            {
                ErrorPopupCall("ERROR 30001" + "\n" + "Description: splitFromAddress[1] does not exist.");
                return false;
            }
        }

        /// <summary>
        /// Перевіряє, чи всі аргументи не є нульовими чи порожніми
        /// </summary>
        /// <param name="ToAddress"></param>
        /// <param name="FromAddress"></param>
        /// <param name="FromPass"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static bool CheckArguments(string ToAddress, string FromAddress, string FromPass, string subject, string body)
        {
            // Перевірте всі аргументи, якщо вони є нульовими або порожніми, з усіх вони не є нульовими або порожніми, поверніть true, інакше поверніть false і повідомлення про помилку
            if (!(string.IsNullOrEmpty(FromAddress)) && !(string.IsNullOrEmpty(ToAddress)) && !(string.IsNullOrEmpty(FromPass)) && !(string.IsNullOrEmpty(subject)) && !(string.IsNullOrEmpty(body)))
            {
                return true;
            }
            else
            {
                ErrorPopupCall("ERROR 30003" + "\n" + "Description: one of the given arguments is null or empty.");
                return false;
            }
        }

     

       
        public static List<MailMessage> GetAllMessages(string FromAddress, string FromPass)
        {
            // Перевірте хоста
            if (CheckEmailHostIMAP(FromAddress) == true && !(string.IsNullOrEmpty(FromPass)))
            {
                try
                {
                    // Створити ImapClient
                    ImapClient client = new ImapClient(Host, Port, FromAddress, FromPass, S22.Imap.AuthMethod.Login, true);

                    // отримати uids
                    IEnumerable<uint> uids = client.Search(S22.Imap.SearchCondition.All());

                    // Отримуйте повідомлення
                    IEnumerable<MailMessage> Messages = client.GetMessages(uids);

                    // Конвертувати в  list
                    List<MailMessage> MessagesList = Messages.ToList<MailMessage>();

                    //Поверніть їх
                    return MessagesList;
                }
                catch (Exception exception)
                {
                    // Створіть повідомлення про помилку
                    string ErrorMessage = "ERROR 60002" + "\n" + exception.ToString();

                    // Показати повідомлення про помилку
                    Program.ErrorPopupCall(ErrorMessage);

                    // Складіть порожній список для повернення
                    List<MailMessage> Stop = new List<MailMessage>();

                    // Припиніть виконувати цей метод
                    return Stop;
                }
            }
            else
            {
                // Створіть повідомлення про помилку
                string ErrorMessage = "ERROR 60003" + "\n" + "EmailHostIMAP(FromAddress) returned false";

                // Показати повідомлення про помилку
                Program.ErrorPopupCall(ErrorMessage);

                //Складіть порожній список для повернення
                List<MailMessage> Stop = new List<MailMessage>();

                // Припиніть виконувати цей метод
                return Stop;
            }
        }

       
        /// <param name="FromAddress"></param>
        private static bool CheckEmailHostIMAP(string FromAddress)
        {
            // Створіть масив splitFromAddress для зберігання розділеного FromAddress в
            string[] splitFromAddress;

            // Спочатку розділіть FromAddress між @
            try
            {
                splitFromAddress = FromAddress.Split('@');
            }
            catch (Exception exception)
            {
                // Створіть повідомлення про помилку
                string ErrorMessage = "ERROR 60006" + "\n" + "FromAddress is empty." + exception.ToString();

                //Показати повідомлення про помилку
                Program.ErrorPopupCall(ErrorMessage);

                return false;
            }
            
            // Потім перевірте, чи існує splitFromAddress [1]
            if (splitFromAddress.Length == 2)
            {
                // Цей комутатор перевіряє, який це хост, і призначає змінні Host і Port відповідним Host і Port
                switch (splitFromAddress[1])
                {
                    case "gmail.com":
                        Host = "imap.gmail.com";
                        Port = 993;
                        return true;
                    case "yahoo.com":
                        Host = "imap.mail.yahoo.com";
                        Port = 993;
                        return true;
                    case "hotmail.com":
                        Host = "imap-mail.outlook.com";
                        Port = 993;
                        return true;
                    case "hotmail.nl":
                        Host = "imap-mail.outlook.com";
                        Port = 993;
                        return true;
                    case "icloud.com":
                        Host = "imap.mail.me.com";
                        Port = 993;
                        return true;
                    case "mail.ru":
                        Host = "imap.mail.ru";
                        Port = 993;
                        return true;
                    default:
                        ErrorPopupCall("ERROR 60004" + "\n" + "Description: reached default in switch(splitFromAddres[1])");
                        return false;
                }
            }
            else
            {
                ErrorPopupCall("ERROR 60005" + "\n" + "Description: splitFromAddress[1] does not exists.");
                return false;
            }
        }



        /// <summary>
        /// Спочатку шифрує, а потім зберігає облікові дані для входу у файл
        /// </summary>
        /// <param name="FromAddress">The FromAddress to be encrypted</param>
        /// <param name="FromPass">The FromPass to be encrypted</param>
        /// <param name="Path">The path of the file</param>
        /// <param name="EncryptionPassword">The password used for the encryption</param>
        public static void WriteCredentialsToFile(string FromAddress, string FromPass, string Path, string EncryptionPassword)
        {
            // Declare variables
            string EncryptedFromAddress, EncryptedFromPass;

            // Створіть новий екземпляр класу FileStream
            FileStream fileStream = File.OpenWrite(Path);

            // Створіть новий екземпляр класу BinaryWriter
            BinaryWriter writer = new BinaryWriter(fileStream);

            // Зашифруйте FromAddress та Frompass
            EncryptedFromAddress = EncryptionClass.Encrypt(FromAddress, EncryptionPassword);
            EncryptedFromPass = EncryptionClass.Encrypt(FromPass, EncryptionPassword);

            // Запишіть облікові дані у файл
            writer.Write(EncryptedFromAddress);
            writer.Write(EncryptedFromPass);

            //Закриваємо BinaryWriter
            writer.Close();
        }

        /// <резюме>
        /// Читає та розшифровує FromAddress та Frompass з файлу, де вони збережені
        /// </резюме>
        /// <param name="Path"></param>
        /// <param name="EncryptionPassword">The password used for the encryption</param>
        public static void ReadCredentialsFromFile(string Path, string EncryptionPassword)
        {
            //Створіть новий екземпляр класу FileStream
            FileStream fileStream = File.OpenRead(Path);

            // Створіть новий екземпляр класу BinaryReader
            BinaryReader reader = new BinaryReader(fileStream);

            // Читаємо файл
            string EncryptedFromAddress = reader.ReadString();
            string EncryptedFromPass = reader.ReadString();

            //Створіть масив для зберігання розшифрованих даних

            string[] DecryptedData = new string[2];

            // Розшифруємо  FromAddress and FromPass
            DecryptedData[0] = EncryptionClass.Decrypt(EncryptedFromAddress, EncryptionPassword);
            DecryptedData[1] = EncryptionClass.Decrypt(EncryptedFromPass, EncryptionPassword);

            // Призначити зміни
            FromAddress = DecryptedData[0];
            FromPass = DecryptedData[1];
        }
    }
}//hello word
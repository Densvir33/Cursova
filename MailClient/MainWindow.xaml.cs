using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MailClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        // Button and Textbox methods

        private void ToAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SubjectTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BodyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        /
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

      
        private void AddAttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the AddAttachmentPopup
            AddAttachmentPopup popup = new AddAttachmentPopup();

            // Показаю спливаюче вікно
            popup.Show();
        }

       
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // If the credentials are saved, open those, else open the login popup
            if (File.Exists("Credentials") == true)
            {
                // Тут я  запитую пароль шифрування за допомогою EncryptionPasswordPopup
                EncryptionPasswordPopup popup = new EncryptionPasswordPopup();

                //Показаю спливаюче вікно
                popup.Show();
            }
            else
            {

                // відкриваю новий екземпляр LoginPopup class
                LoginPopup loginPopup = new LoginPopup();

                // ну тут все логічно (Показаю спливаюче вікно)
                loginPopup.Show();
            }
        }
        //Хто я ?
      
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(Program.FromAddress)))
            {
                string ToAddress, Subject, Body, CC, S_Attachment;

                // Тут призначаю зміни
                ToAddress = ToAddressTextBox.Text;
                Subject = SubjectTextBox.Text;
                Body = BodyTextBox.Text;
                CC = CCAddressTextBox.Text;
                S_Attachment = AddAttachmentPopup.AttachmentPath;

                // нада нажать SendEmail, щоб надіслати електронне повідомлення(тут даже не буду казати хто догається)
                Program.SendEmail(ToAddress, Program.FromAddress, Program.FromPass, Subject, Body, CC, S_Attachment);

                // закриваю вікно
                Close();
            }
            else
            {
                Program.ErrorPopupCall("Be sure to log in!");
            }
        }
    }
}

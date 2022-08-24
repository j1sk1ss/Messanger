using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatClient.ServiceChat; //Для ServiceChatClient

namespace ChatClient
{
    public partial class MainWindow : Window, IServiceChatCallback //Даём знать клиенту о методе Callback из сервиса
    {
        bool isConnected = false; //Статус подключения - отключён
        ServiceChatClient client; //Инициализация клиента
        int ID; //Переменная Айди
        public MainWindow()
        {

            InitializeComponent(); //Инциализация окна пользовательского интерфейса
            bConDiscon.Visibility = Visibility.Hidden;
            tbMessage.IsEnabled = false;
        }



        void ConnectUser()
        {
            if(!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this)); //инициалиация объекта client
                if (checkBox.IsChecked == true) ID = client.Connect(Crypt(tbUserName.Text)); else ID = client.Connect(tbUserName.Text); // айди возвращается из метода клиента, который вызывается с использованием имени юзера
                tbUserName.IsEnabled = false; //Выключение текстбокса с именем юзера
                tbMessage.IsEnabled = true; //Выключение текстбокса с именем юзера
                bConDiscon.Content = "Отключиться"; //Изменение надписи у кнопки на Disconnect
                isConnected = true; //Статус подключения - подключён
                checkBox.IsEnabled = false;
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID); //Отключение клиента по айди
                client = null; // деинициализация объекта client
                tbUserName.IsEnabled = true; //Включение текстбокса с именем юзера
                bConDiscon.Content = "Подключиться"; //Изменение надписи у кнопки на Connect
                isConnected = false; //Статус подключения - отключён
                checkBox.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) //Обработка нажатия клавиши
        {
            if (isConnected) //Если подключён
            {
                DisconnectUser(); //Отключить
            }
            else //Если отключён
            {
                ConnectUser(); //Подключить
            }
        }

        public void MsgCallback(string msg)
        {

            lbChat.Items.Add(Decrypt(msg)); //Добавить сообщение в листбокс
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count-1]); //Скроллинг до самого нового при появлении новых сообщений, что не помещаются
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) //Обработка закрытия приложения через крестик (добавлено через свойства объекта через интерфейс иде)
        {
            DisconnectUser(); //Вызов метода отключения юзера
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e) //Обработка нажатия клавиш в текстбоксе сообщения (добавлено через свойства объекта через интерфейс иде)
        {
            if (e.Key == Key.Enter) //Если нажат Enter
            {
                if (client != null) //Если клиент инициализирован (то есть юзер подключился)
                {
                    client.SendMsg(Crypt(tbMessage.Text), ID); //Вызов SendMsg из класса клиент - то есть отправить сообщение
                    tbMessage.Text = string.Empty; //Очистить строку в текстбоксе
                }
            }
        }

        private void tbUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            bConDiscon.Visibility = Visibility.Visible;
        }

        private void tbMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbMessage.Text.Length > 64) tbMessage.Height = 90; else tbMessage.Height = 24;
        }


        private string Crypt(string message)
        {
            char[] a = message.ToCharArray();
                int key = 0;
                char[] b = textBox.Text.ToCharArray();
                for (int i = 0; i < b.Length; i++) key += (int)b[i]; 
            if (checkBox.IsChecked == true) for (int i = 0; i < a.Length; i++) a[i] += (char)(DateTime.Now.Day + key);
            return new String(a);
        }
        private string Decrypt(string message)
        {
                int key = 0;
                char[] b = textBox.Text.ToCharArray();
                for (int i = 0; i < b.Length; i++) key += (int)b[i];
            char[] a = message.ToCharArray();
            if (checkBox.IsChecked == true) for (int i = 0; i < a.Length; i++) a[i] -= (char)(DateTime.Now.Day + key);
            return new String(a);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (client != null) client.SendMsg("Пользователь включил шифрование сообщений!", ID);

        }
    }
}// Всем привет, я с Оренбурга!

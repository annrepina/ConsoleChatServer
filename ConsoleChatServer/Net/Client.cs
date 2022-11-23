using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfChatServer.Net
{
    /// <summary>
    /// Клиент, который подключается к серверу
    /// </summary>
    public class Client
    {
        #region Константы

        /// <summary>
        /// Код добавления нового клиента
        /// </summary>
        public const byte AddingNewUserCode = 1;

        /// <summary>
        /// Код удаления клиента
        /// </summary>
        public const byte DisconnectingUserCode = 2;

        /// <summary>
        /// Код отравки сообщений
        /// </summary>
        public const byte SendingMessageCode = 3;

        #endregion Константы

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Предоставляет базовый поток данных для доступа к сети
        /// </summary>
        public NetworkStream NetworkStream { get; private set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// TCP клиент
        /// </summary>
        private TcpClient _tcpClient;

        /// <summary>
        /// Сервер
        /// </summary>
        private Server _server;

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="tcpClient">TCP клиент</param>
        /// <param name="server">Сервер</param>
        public Client(TcpClient tcpClient, Server server)
        {
            Id = Guid.NewGuid().ToString();
            Name = "";
            _tcpClient = tcpClient;
            _server = server;
            _server.AddClient(this);
            NetworkStream = _tcpClient.GetStream();
        }

        /// <summary>
        /// Обработать данные
        /// </summary>
        public void ProcessData()
        {
            try
            {
                GetUserName();

                string message = "";

                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = $"{Name}: {message}";
                        Console.WriteLine(message);

                        _server.BroadcastOperationCode(SendingMessageCode, Id);
                        _server.BroadcastMessage(message, Id);
                    }
                    catch (Exception)
                    {
                        message = $"{Name}: покинул/покинула чат";
                        Console.WriteLine(message);

                        _server.BroadcastOperationCode(DisconnectingUserCode, Id);
                        _server.BroadcastMessage(Name, Id);
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                _server.RemoveClient(Id);
                CloseConnection();
            }
        }

        /// <summary>
        /// Получить имя пользователя
        /// </summary>
        private void GetUserName()
        {
            string message = GetMessage();

            Name = message;

            //message = $"{Name} вошел/вошла в чат";

            // отправить всем сообщение о том, что добавляется новый человек
            _server.BroadcastOperationCode(AddingNewUserCode, Id);

            // Отправить всем имя этого человека
            _server.BroadcastMessage(Name, Id);

            Console.WriteLine($"{Name} вошел/вошла в чат");
            //Console.WriteLine(message);
        }

        /// <summary>
        /// Получить сообщение
        /// </summary>
        private string GetMessage()
        {
            string message = "";

            // Буфер для получения даты
            byte[] data = new byte[256];

            StringBuilder stringBuilder = new StringBuilder();

            int bytes = 0;

            do
            {
                bytes = NetworkStream.Read(data, 0, data.Length);

                stringBuilder.Append(Encoding.UTF8.GetString(data, 0, bytes));

            } while (NetworkStream.DataAvailable);

            message = stringBuilder.ToString();

            return message;
        }

        /// <summary>
        /// Закрытие подлючения
        /// </summary>
        public void CloseConnection()
        {
            if (NetworkStream != null)
                NetworkStream.Close();

            if (_tcpClient != null)
                _tcpClient.Close();
        }
    }
}

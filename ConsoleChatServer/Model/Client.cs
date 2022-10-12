using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfChatServer.Model
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="tcpClient">TCP клиент</param>
        /// <param name="server">Сервер</param>
        public Client(TcpClient tcpClient, Server server)
        {
            Id = Guid.NewGuid().ToString();
            _tcpClient = tcpClient;
            _server = server;
            _server.AddClient(this);
            NetworkStream = _tcpClient.GetStream();
        }

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

        public void Process()
        {
            try
            {
                // 


            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetUserName()
        {
            string message = GetMessage();

            Name = message;

            message = $"{Name} вошел/вошла в чат";


        }

        /// <summary>
        /// Получить сообщение
        /// </summary>
        private string GetMessage()
        {
            string message = "";

            /// Буфер для получения даты
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
            if(NetworkStream != null)
                NetworkStream.Close();

            if(_tcpClient != null)
                _tcpClient.Close();
        }

    }
}

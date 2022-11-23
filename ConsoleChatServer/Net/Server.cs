using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfChatServer.Net
{
    /// <summary>
    /// Сервер, который принимает подлючения клиентов
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Прослушиватель TCP подключений от клиентов
        /// </summary>
        private TcpListener _tcpListener;

        /// <summary>
        /// Порт
        /// </summary>
        private int _port;

        /// <summary>
        /// Список клиентов
        /// </summary>
        private List<Client> _clients;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Server()
        {
            _clients = new List<Client>();

            _port = 8888;

            _tcpListener = new TcpListener(IPAddress.Any, _port);
        }

        /// <summary>
        /// Добавить клиента
        /// </summary>
        /// <param name="client">Клиент</param>
        public void AddClient(Client client)
        {
            _clients.Add(client);
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="clientId">Id клиента</param>
        public void RemoveClient(string clientId)
        {
            if (_clients != null && _clients.Count > 0)
            {
                // получаем по id подключение
                Client client = _clients.FirstOrDefault(c => c.Id == clientId);

                if (client != null)
                    _clients.Remove(client);
            }
        }

        /// <summary>
        /// Прослушивание входящих подключений
        /// </summary>
        public void ListenForIncomingConnections()
        {
            try
            {
                _tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                    Client client = new Client(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(client.ProcessData));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                DisconnectClients();
            }
        }

        /// <summary>
        /// Трансляция сообщения подлюченным клиентам
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="id">Id клиента, который отправил данное сообщение</param>
        public void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var client in _clients)
            {
                // если id клиента не равно id отправляющего
                if (client.Id != id)
                {
                    // передача данных
                    client.NetworkStream.Write(data, 0, data.Length);
                }
            }
        }

        public void BroadcastOperationCode(byte operationCode, string id)
        {
            foreach (var client in _clients)
            {
                // если id клиента не равно id отправляющего
                if (client.Id != id)
                {
                    // передача данных
                    client.NetworkStream.WriteByte(operationCode);
                }
            }
        }

        /// <summary>
        /// Отключение всех клиентов
        /// </summary>
        public void DisconnectClients()
        {
            /// Остановка сервера
            _tcpListener.Stop();

            foreach (var client in _clients)
            {
                client.CloseConnection();
            }

            //завершение процесса
            Environment.Exit(0);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfChatServer.Model
{
    public class Server
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Server()
        {
            _clients = new List<Client>();
        }

        /// <summary>
        /// Прослушиватель TCP подключений от клиентов
        /// </summary>
        private static TcpListener _tcpListener = new TcpListener(IPAddress.Any, _port);

        /// <summary>
        /// Порт
        /// </summary>
        private static int _port = 8888;

        /// <summary>
        /// Список клиентов
        /// </summary>
        private List<Client> _clients;

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
            if(_clients != null && _clients.Count > 0)
            {
                // получаем по id подключение
                Client client = _clients.FirstOrDefault(c => c.Id == clientId);

                if(client != null)
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

                while(true)
                {
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                    Client client = new Client(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(client.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                DisconnectClients();
            }
        }

        //public void BroadcastMessage(string message, int id)

        /// <summary>
        /// Отключение всех клиентов
        /// </summary>
        private void DisconnectClients()
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
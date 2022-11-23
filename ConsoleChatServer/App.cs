using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfChatServer.Net;

namespace ConsoleChatServer
{
    public class App
    {
        private Server server; // сервер
        private Thread listenThread; // потока для прослушивания

        public void Launch()
        {
            try
            {
                server = new Server();
                listenThread = new Thread(new ThreadStart(server.ListenForIncomingConnections));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.DisconnectClients();
                Console.WriteLine(ex.Message);
            }
        }
    }
}


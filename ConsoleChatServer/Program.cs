using WpfChatServer.Net;

namespace ConsoleChatServer
{
    public class Program
    {
        //static Server server; // сервер
        //static Thread listenThread; // потока для прослушивания
        static void Main(string[] args)
        {
            //try
            //{
            //    server = new Server();
            //    listenThread = new Thread(new ThreadStart(server.ListenForIncomingConnections));
            //    listenThread.Start(); //старт потока
            //}
            //catch (Exception ex)
            //{
            //    server.DisconnectClients();
            //    Console.WriteLine(ex.Message);
            //}

            App app = new App();

            app.Launch();



        }
    }
}
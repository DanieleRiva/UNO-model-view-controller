using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Server.Models;
using Server.Controllers;
using Server.Views;

namespace Server
{
    public class Server
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 82;
            Console.SetBufferSize(82, 500);

            Console.ForegroundColor = ConsoleColor.Red;
            string title = @" █    ██  ███▄    █  ▒█████       ██████ ▓█████  ██▀███   ██▒   █▓▓█████  ██▀███  
 ██  ▓██▒ ██ ▀█   █ ▒██▒  ██▒   ▒██    ▒ ▓█   ▀ ▓██ ▒ ██▒▓██░   █▒▓█   ▀ ▓██ ▒ ██▒
▓██  ▒██░▓██  ▀█ ██▒▒██░  ██▒   ░ ▓██▄   ▒███   ▓██ ░▄█ ▒ ▓██  █▒░▒███   ▓██ ░▄█ ▒
▓▓█  ░██░▓██▒  ▐▌██▒▒██   ██░     ▒   ██▒▒▓█  ▄ ▒██▀▀█▄    ▒██ █░░▒▓█  ▄ ▒██▀▀█▄  
▒▒█████▓ ▒██░   ▓██░░ ████▓▒░   ▒██████▒▒░▒████▒░██▓ ▒██▒   ▒▀█░  ░▒████▒░██▓ ▒██▒
░▒▓▒ ▒ ▒ ░ ▒░   ▒ ▒ ░ ▒░▒░▒░    ▒ ▒▓▒ ▒ ░░░ ▒░ ░░ ▒▓ ░▒▓░   ░ ▐░  ░░ ▒░ ░░ ▒▓ ░▒▓░
░░▒░ ░ ░ ░ ░░   ░ ▒░  ░ ▒ ▒░    ░ ░▒  ░ ░ ░ ░  ░  ░▒ ░ ▒░   ░ ░░   ░ ░  ░  ░▒ ░ ▒░
 ░░░ ░ ░    ░   ░ ░ ░ ░ ░ ▒     ░  ░  ░     ░     ░░   ░      ░░     ░     ░░   ░ 
   ░              ░     ░ ░           ░     ░  ░   ░           ░     ░  ░   ░     
                                                              ░                   ";

            Console.WriteLine(title);
            Console.ResetColor();

            var model = new GameModel();
            var controller = new GameController(model);

            var ipe = new IPEndPoint(IPAddress.Any, 8080);
            var listener = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(ipe);
            listener.Listen(10);

            Console.WriteLine("Server listening on port 8080");
            model = new GameModel();

            var tasks = new Task[3];

            for (int i = 0; i < 3; i++)
            {
                var socket = listener.Accept();

                var reader = new StreamReader(new NetworkStream(socket));
                string username = reader.ReadLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\n{username}");
                Console.ResetColor();
                Console.Write($" si è connesso. [{i}]\n");

                var view = new VirtualView(socket, controller);
                model.AddView(view);
                controller.AddView(view);

                tasks[i] = Task.Run(view.Run);
                view.SendMessage(new Utils.Message());
                controller.Connected(socket, i, username);
            }

            controller.Start();

            Task.WaitAll(tasks);
        }
    }
}

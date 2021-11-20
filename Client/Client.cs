using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Server.Models;
using Server.Utils;
using Type = Server.Utils.Type;

namespace Client
{
    public class Client
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(94, 40);
            Console.SetBufferSize(500, 40);

            Console.ForegroundColor = ConsoleColor.Cyan;
            string title = @" █    ██  ███▄    █  ▒█████      ▄████▄   ▒█████   ███▄    █   ██████  ▒█████   ██▓    ▓█████ 
 ██  ▓██▒ ██ ▀█   █ ▒██▒  ██▒   ▒██▀ ▀█  ▒██▒  ██▒ ██ ▀█   █ ▒██    ▒ ▒██▒  ██▒▓██▒    ▓█   ▀ 
▓██  ▒██░▓██  ▀█ ██▒▒██░  ██▒   ▒▓█    ▄ ▒██░  ██▒▓██  ▀█ ██▒░ ▓██▄   ▒██░  ██▒▒██░    ▒███   
▓▓█  ░██░▓██▒  ▐▌██▒▒██   ██░   ▒▓▓▄ ▄██▒▒██   ██░▓██▒  ▐▌██▒  ▒   ██▒▒██   ██░▒██░    ▒▓█  ▄ 
▒▒█████▓ ▒██░   ▓██░░ ████▓▒░   ▒ ▓███▀ ░░ ████▓▒░▒██░   ▓██░▒██████▒▒░ ████▓▒░░██████▒░▒████▒
░▒▓▒ ▒ ▒ ░ ▒░   ▒ ▒ ░ ▒░▒░▒░    ░ ░▒ ▒  ░░ ▒░▒░▒░ ░ ▒░   ▒ ▒ ▒ ▒▓▒ ▒ ░░ ▒░▒░▒░ ░ ▒░▓  ░░░ ▒░ ░
░░▒░ ░ ░ ░ ░░   ░ ▒░  ░ ▒ ▒░      ░  ▒     ░ ▒ ▒░ ░ ░░   ░ ▒░░ ░▒  ░ ░  ░ ▒ ▒░ ░ ░ ▒  ░ ░ ░  ░
 ░░░ ░ ░    ░   ░ ░ ░ ░ ░ ▒     ░        ░ ░ ░ ▒     ░   ░ ░ ░  ░  ░  ░ ░ ░ ▒    ░ ░      ░   
   ░              ░     ░ ░     ░ ░          ░ ░           ░       ░      ░ ░      ░  ░   ░  ░
                                ░                                                             ";

            Console.WriteLine(title);
            Console.ResetColor();

            Console.WriteLine("Inserisci il tuo username:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string username = Console.ReadLine();
            Console.ResetColor();

            var view = new ConsoleView();

            var port = 8080;
            var ipe = new IPEndPoint(IPAddress.Loopback, port);
            var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(ipe);

            var reader = new StreamReader(new NetworkStream(socket));
            var writer = new StreamWriter(new NetworkStream(socket));
            writer.AutoFlush = true;

            writer.WriteLine(username);

            while (true)
            {
                var message = JsonSerializer.Deserialize<Message>(reader.ReadLine());

                switch (message.Type)
                {
                    // If the message from the server contains the START_GAME body, call start() from "view".
                    case Type.CONNECTED:
                        view.Connected(IPAddress.Loopback, port, message);
                        break;
                    case Type.START_GAME:                        
                        view.PrintCards(message.Cards);
                        view.PrintScrapCard(message.SingleCard);
                        break;
                    case Type.START_TURN:
                        view.PrintCards(message.Cards);
                        view.PrintScrapCard(message.SingleCard);
                        bool canProceed = false;

                        while (canProceed == false)
                        {
                            string pressedKey = view.ChooseMove();

                            if (pressedKey.ToLower() == "p")
                            {
                                canProceed = true;
                                writer.WriteLine(JsonSerializer.Serialize(new Message { Type = Type.DRAW_CARD}));
                            }
                            else
                            {
                                try
                                {
                                    if (int.Parse(pressedKey) >= 1 && int.Parse(pressedKey) <= message.Cards.Count)
                                    {
                                        canProceed = true;
                                        writer.WriteLine(JsonSerializer.Serialize(new Message { Type = Type.MOVE, CardNumber = int.Parse(pressedKey) - 1/*SingleCard = message.Cards[int.Parse(pressedKey) - 1]*/ }));
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }

                        break;
                }
            }
        }
    }
}

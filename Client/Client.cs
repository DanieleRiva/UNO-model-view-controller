using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
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
            Console.SetWindowSize(175, 40);
            Console.SetBufferSize(500, 40);

            Console.ForegroundColor = ConsoleColor.Red;
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

            Console.Title = $"UNO Console: {username}";

            var view = new ConsoleView();

            var port = 8080;
            var ipe = new IPEndPoint(IPAddress.Loopback, port);
            var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(ipe);

            var reader = new StreamReader(new NetworkStream(socket));
            var writer = new StreamWriter(new NetworkStream(socket));
            writer.AutoFlush = true;

            writer.WriteLine(username);

            bool canProceed = false;
            string pressedKey = string.Empty;
            char[] colors = new char[] { 'r', 'b', 'y', 'g' };

            while (true)
            {
                var message = JsonSerializer.Deserialize<Message>(reader.ReadLine());

                switch (message.Type)
                {
                    case Type.CONNECTED:
                        view.Connected(IPAddress.Loopback, port, message);
                        break;

                    case Type.STARTING:
                        view.Starting(message.Body);
                        break;

                    case Type.START_GAME:
                        view.PrintCards(message.Cards);
                        view.PrintScrapCard(message.SingleCard);
                        break;

                    case Type.START_TURN:
                        view.PrintCards(message.Cards);
                        view.PrintScrapCard(message.SingleCard);
                        canProceed = false;

                        while (canProceed == false)
                        {
                            pressedKey = view.ChooseMove();

                            if (pressedKey.ToLower() == "p")
                            {
                                canProceed = true;
                                writer.WriteLine(JsonSerializer.Serialize(new Message
                                {
                                    Type = Type.DRAW_CARD
                                }));
                            }
                            else
                            {
                                try
                                {
                                    if (int.Parse(pressedKey) >= 1 && int.Parse(pressedKey) <= message.Cards.Count)
                                    {
                                        canProceed = true;

                                        writer.WriteLine(JsonSerializer.Serialize(new Message
                                        {
                                            Type = Type.MOVE,
                                            CardNumber = int.Parse(pressedKey) - 1
                                        }));
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }

                        break;

                    case Type.CHANGE_COLOR:
                        canProceed = false;

                        while (canProceed == false)
                        {
                            pressedKey = view.ChangeColor();

                            if (pressedKey.Length == 1 && colors.Contains(char.Parse(pressedKey)))
                                canProceed= true;
                        }

                        writer.WriteLine(JsonSerializer.Serialize(new Message
                        {
                            Type = Type.CHANGE_COLOR,
                            Body = pressedKey
                        }));

                        break;

                    case Type.DRAW_CARD:
                        view.PrintCards(message.Cards);
                        view.PrintScrapCard(message.SingleCard);

                        canProceed = false;

                        while (canProceed == false)
                        {
                            pressedKey = view.ChooseMoveP(message.Cards);

                            if (pressedKey.ToLower() == "s")
                            {
                                canProceed = true;
                                writer.WriteLine(JsonSerializer.Serialize(new Message
                                {
                                    Type = Type.PASS_TURN
                                }));
                            }
                            else
                            {
                                try
                                {
                                    if (int.Parse(pressedKey) >= 1 && int.Parse(pressedKey) <= message.Cards.Count)
                                    {
                                        canProceed = true;

                                        writer.WriteLine(JsonSerializer.Serialize(new Message
                                        {
                                            Type = Type.MOVE,
                                            Body = "p_pressed",
                                            CardNumber = int.Parse(pressedKey) - 1
                                        }));
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                        break;

                    case Type.WIN:
                        view.Win();
                        break;

                    case Type.LOSE:
                        view.Lose();
                        break;
                }
            }
        }
    }
}

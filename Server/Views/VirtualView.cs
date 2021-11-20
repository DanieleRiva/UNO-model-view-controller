﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using Server.Controllers;
using Server.Utils;

namespace Server.Views
{
    public class VirtualView
    {
        public Socket _socket;
        private StreamReader _reader;
        private StreamWriter _writer;
        private GameController _controller;

        public VirtualView(Socket socket, GameController controller)
        {
            _controller = controller;
            _socket = socket;

            _reader = new StreamReader(new NetworkStream(_socket));
            _writer = new StreamWriter(new NetworkStream(_socket));
            _writer.AutoFlush = true;
        }

        public void Run()
        {
            while (true)
            {
                var data = _reader.ReadLine();
                var message = JsonSerializer.Deserialize<Message>(data);

                switch (message.Type)
                {
                    case Utils.Type.MOVE:
                        _controller.Move(this, message);
                        break;
                    case Utils.Type.DRAW_CARD:
                        _controller.DrawCard();
                        break;
                    default:
                        Console.WriteLine($"{message.Type} not supported");
                        break;
                }
            }
        }

        public void SendMessage(Message message)
        {
            Console.WriteLine(JsonSerializer.Serialize(message));
            _writer.WriteLine(JsonSerializer.Serialize(message));
        }
    }
}
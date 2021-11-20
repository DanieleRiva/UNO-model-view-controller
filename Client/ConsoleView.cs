using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Utils;
using Server.Models;
using System.Text.Json;
using System.Net;

namespace Client
{
    public class ConsoleView
    {
        private int[,] _board;

        public ConsoleView()
        {
            _board = new int[3, 3];
        }

        public void Connected(IPAddress serverIp, int port, Message message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (message.Body != null)
                Console.WriteLine(message.Body);

            Console.ResetColor();
        }

        public void Start()
        {
            PrintBoard();
        }

        public void Draw(Message message)
        {
            List<Server.Models.Card> li = new List<Card>();
            Console.WriteLine(li.Count);
        }

        private void Clear()
        {
            Console.Clear();
        }

        private void PrintBoard()
        {
            Clear();
        }

        public void PrintCards(List<Card> handCardList)
        {
            Clear();

            int positionX = 1;
            int positionXCounter = 3;
            int counter = 1;

            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", "TEXT"));

            foreach (Card drawnCard in handCardList)
            {
                int positionY = handCardList.Count + 20;

                Console.ForegroundColor = drawnCard.color;

                Console.SetCursorPosition(positionX, positionY);
                Console.WriteLine(" _____ ");

                Console.SetCursorPosition(positionX, positionY + 1);
                Console.WriteLine("|     |");

                Console.SetCursorPosition(positionX, positionY + 2);
                Console.WriteLine("|     |");

                Console.SetCursorPosition(positionX, positionY + 3);

                if (drawnCard.number == 10)
                    Console.WriteLine("|  X  |");

                else if (drawnCard.number == 11)
                    Console.WriteLine("| <-> |");

                else if (drawnCard.number == 12)
                    Console.WriteLine("| + 2 |");

                else if (drawnCard.number == 13)
                    Console.WriteLine("|COLOR|");

                else if (drawnCard.number == 14)
                    Console.WriteLine("| + 4 |");

                else
                    Console.WriteLine($"|  {drawnCard.number}  |");

                Console.SetCursorPosition(positionX, positionY + 4);
                Console.WriteLine("|     |");


                Console.SetCursorPosition(positionX, positionY + 5);
                Console.WriteLine("|_____|");

                Console.ResetColor();

                Console.SetCursorPosition(positionXCounter, positionY + 7);
                Console.WriteLine($"[{counter}]");

                Console.SetCursorPosition(positionXCounter, positionY + 9);

                positionX += 8;
                positionXCounter += 8;
                counter++;
            }
        }

        public void PrintScrapCard(Card scrapCard)
        {
            int positionX = 10;
            int positionXCounter = 3;
            int counter = 1;

            int positionY = 10;

            Console.ForegroundColor = scrapCard.color;

            Console.SetCursorPosition(positionX, positionY);
            Console.WriteLine(" _____ ");

            Console.SetCursorPosition(positionX, positionY + 1);
            Console.WriteLine("|     |");

            Console.SetCursorPosition(positionX, positionY + 2);
            Console.WriteLine("|     |");

            Console.SetCursorPosition(positionX, positionY + 3);

            if (scrapCard.number == 10)
                Console.WriteLine("|  X  |");

            else if (scrapCard.number == 11)
                Console.WriteLine("| <-> |");

            else if (scrapCard.number == 12)
                Console.WriteLine("| + 2 |");

            else if (scrapCard.number == 13)
                Console.WriteLine("|COLOR|");

            else if (scrapCard.number == 14)
                Console.WriteLine("| + 4 |");

            else
                Console.WriteLine($"|  {scrapCard.number}  |");

            Console.SetCursorPosition(positionX, positionY + 4);
            Console.WriteLine("|     |");


            Console.SetCursorPosition(positionX, positionY + 5);
            Console.WriteLine("|_____|");

            Console.ResetColor();


            counter++;
        }

        public string ChooseMove()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.WriteLine("Inserisci la carta che vuoi utilizzare, inserisci \"p\" per pescare. ");

            return Console.ReadLine();
        }
    }
}

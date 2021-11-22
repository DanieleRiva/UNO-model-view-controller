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
        public void Connected(IPAddress serverIp, int port, Message message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (message.Body != null)
                Console.WriteLine(message.Body);

            Console.ResetColor();
        }

        public void Starting(string countdown)
        {
            if (int.Parse(countdown) == 5)
                Console.WriteLine("\nLa lobby è piena!" +
                                  "\nLa partita comincia tra: ");

            Console.WriteLine($"{countdown} s");
        }

        public void Start()
        {
            PrintBoard();
        }

        private void Clear()
        {
            Console.Clear();
        }

        private void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
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

            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", "UNO"));
            Console.ResetColor();

            foreach (Card drawnCard in handCardList)
            {
                int positionY = 25;

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
            int positionX = 0;

            int positionY = 10;

            Console.ForegroundColor = scrapCard.color;

            Console.SetCursorPosition(positionX, positionY);
            Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "   _____ "));

            Console.SetCursorPosition(positionX, positionY + 1);
            Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  |     |"));

            Console.SetCursorPosition(positionX, positionY + 2);
            Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  |     |"));

            Console.SetCursorPosition(positionX, positionY + 3);

            if (scrapCard.number == 10)
                Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  |  X  |"));

            else if (scrapCard.number == 11)
                Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  | <-> |"));

            else if (scrapCard.number == 12)
                Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  | + 2 |"));

            else if (scrapCard.number == 13)
                Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  |COLOR|"));

            else if (scrapCard.number == 14)
                Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  | + 4 |"));

            else
                Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", $"  |  {scrapCard.number}  |"));

            Console.SetCursorPosition(positionX, positionY + 4);
            Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  |     |"));


            Console.SetCursorPosition(positionX, positionY + 5);
            Console.WriteLine(String.Format("{0," + (Console.WindowWidth / 2 + 2) + "}", "  |_____|"));

            Console.ResetColor();
        }

        public string ChooseMove()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 5);
            Console.WriteLine("Inserisci la carta che vuoi utilizzare, inserisci \"p\" per pescare. ");

            return Console.ReadLine();
        }

        public string ChooseMoveP(List<Card> handCardList)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 5);
            Console.WriteLine("Inserisci la carta numero " + handCardList.Count + " oppure inserisci \"s\" per passare il turno. ");

            return Console.ReadLine();
        }
        public string ChangeColor()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 5);

            // Clear previous text
            ClearLine();

            Console.WriteLine("Inserisci il colore: (r, b, y, g)");
            return Console.ReadLine();
        }

        public void Win()
        {
            ClearLine();

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("HAI VINTO LA PARTITA!");
            Console.ResetColor();
        }

        public void Lose()
        {
            ClearLine();

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Hai perso la partita...");
            Console.ResetColor();
        }
    }
}

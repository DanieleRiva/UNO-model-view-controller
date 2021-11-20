using System;
using System.Collections.Generic;
using System.Text;
using Server.Views;

namespace Server.Models
{
    public class Card
    {
        public int id { get; set; }
        public ConsoleColor color { get; set; }
        public int number { get; set; }

        public Card()
        {

        }

        public Card(int _id, ConsoleColor _color, int _number)
        {
            this.id = _id;
            this.color = _color;
            this.number = _number;
        }
    }

    public class GameModel
    {
        Random random = new Random();

        public List<Card> cardDatabase = new List<Card>();
        public List<Card> deck = new List<Card>();
        List<int> cardsUsed = new List<int>();

        private List<VirtualView> _views;

        public List<Card>[] hands = new List<Card>[3];

        public Card scrapCard;

        public GameModel()
        {
            _views = new List<VirtualView>();

            int numeroCarta = 0;

            for (int i = 0; i < 108; i++)
            {
                if (i < 25)
                {
                    cardDatabase.Add(new Card(i + 98, ConsoleColor.Red, numeroCarta));
                    numeroCarta++;

                    if (i == 12)
                        numeroCarta = 1;
                }
                else if (i >= 25 && i < 50)
                {
                    if (i == 25)
                        numeroCarta = 0;

                    cardDatabase.Add(new Card(i, ConsoleColor.Yellow, numeroCarta));
                    numeroCarta++;

                    if (i == 37)
                        numeroCarta = 1;
                }
                else if (i >= 50 && i < 75)
                {
                    if (i == 50)
                        numeroCarta = 0;

                    cardDatabase.Add(new Card(i, ConsoleColor.Green, numeroCarta));
                    numeroCarta++;

                    if (i == 62)
                        numeroCarta = 1;
                }
                else if (i >= 75 && i < 100)
                {
                    if (i == 75)
                        numeroCarta = 0;

                    cardDatabase.Add(new Card(i, ConsoleColor.Blue, numeroCarta));
                    numeroCarta++;

                    if (i == 87)
                        numeroCarta = 1;
                }
                else
                    if (i < 104)
                    cardDatabase.Add(new Card(i, ConsoleColor.DarkGray, 13));
                else
                    cardDatabase.Add(new Card(i, ConsoleColor.DarkGray, 14));
            }

            Shuffle();

            // 7 cards to each player
            for (int i = 0; i < 3; i++)
            {
                hands[i] = new List<Card>();

                for (int j = 0; j < 7; j++)
                {
                    hands[i].Add(deck[deck.Count - 1]);
                    deck.RemoveAt(deck.Count - 1);
                }
            }

            // Add card to scrapdeck
            scrapCard = deck[deck.Count - 1];
            int cont = 1;

            while (scrapCard.number == 14)
            {
                cont++;
                scrapCard = deck[deck.Count - cont];
            }

            deck.RemoveAt(deck.Count - cont);
        }

        public void Shuffle()
        {
            int x = 200; // 200 because of the while loop
            int deckSize = 108;

            for (int i = 0; i < deckSize; i++)
            {
                while (cardsUsed.Contains(x) || x > 107)
                    x = random.Next(0, 108);

                cardsUsed.Add(x);

                if (deck.Contains(cardDatabase[x])) // Duplicate check
                    Console.WriteLine("Found duplicate");
                else
                    deck.Add(cardDatabase[x]);
            }
        }

        public void AddView(VirtualView view)
        {
            _views.Add(view);
        }
    }
}

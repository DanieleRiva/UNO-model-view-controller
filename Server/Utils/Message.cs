using System;
using Server.Models;
using System.Collections.Generic;
using System.Text;

namespace Server.Utils
{
    public class Message
    {
        public Type Type { get; set; }
        public string Body { get; set; }
        public List<Card> Cards { get; set; }
        public Card SingleCard { get; set; }
        public int CardNumber { get; set; }
    }

    public enum Type
    {
        CONNECTED,
        STARTING,
        START_GAME,
        START_TURN,
        DRAW_CARD,
        PASS_TURN,
        MOVE,
        CHANGE_COLOR,
        WIN,
        LOSE
    }
}

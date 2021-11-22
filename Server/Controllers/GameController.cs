using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Server.Views;
using Server.Models;
using Server.Utils;
using System.Net.Sockets;

namespace Server.Controllers
{
    public class GameController
    {
        private GameModel _model;
        private VirtualView[] _views;
        private int _turn;
        private int _handTurn = 0;

        bool turnDirection = true;
        bool control = false;
        bool drawFour = false;

        public GameController(GameModel model)
        {
            _model = model;
            _views = new VirtualView[3];
            _turn = 0;
        }

        public void AddView(VirtualView view)
        {
            _views[_turn] = view;
            NextTurn();
        }

        private void NextTurn()
        {
            _turn = (_turn + 1) % 3;
        }

        private void NextHand()
        {
            if (turnDirection)
                _handTurn = (_handTurn + 1) % 3;
            else
            {
                if (_handTurn == 0)
                    _handTurn = 2;
                else
                    _handTurn -= 1;
            }
        }

        public void Connected(Socket socket, int players, string username)
        {
            foreach (var virtualView in _views)
            {
                if (virtualView != null)
                {
                    if (virtualView._socket == socket)
                    {
                        string connected = $"\nTi sei connesso al server. [{players + 1}/3]";
                        virtualView.SendMessage(new Message { Type = Utils.Type.CONNECTED, Body = connected });
                    }
                    else
                    {
                        string connected = $"\n{username} si è connesso. [{players + 1}/3]";
                        virtualView.SendMessage(new Message { Type = Utils.Type.CONNECTED, Body = connected });
                    }
                }
            }
        }

        public void Starting(int countdown)
        {
            foreach (var virtualView in _views)
                virtualView.SendMessage(new Message { Type = Utils.Type.STARTING, Body = countdown.ToString() });
        }

        public void Start()
        {
            foreach (var virtualView in _views)
                Console.WriteLine(virtualView);

            for (int i = 0; i < 3; i++)
            {
                StartGame();
                NextHand();
            }


            if (_model.scrapCard.number >= 10)
                PlacingRules(0, true, false);
            else
                StartTurn();
        }

        private void StartTurn()
        {
            _views[_handTurn].SendMessage(new Message { Type = Utils.Type.START_TURN, Cards = _model.hands[_handTurn], SingleCard = _model.scrapCard });
        }

        private void StartGame()
        {
            _views[_handTurn].SendMessage(new Message { Type = Utils.Type.START_GAME, Cards = _model.hands[_handTurn], SingleCard = _model.scrapCard });
        }

        private void DrawCardMessage()
        {
            _views[_handTurn].SendMessage(new Message { Type = Utils.Type.DRAW_CARD, Cards = _model.hands[_handTurn], SingleCard = _model.scrapCard });
        }

        private void PickColor()
        {
            _views[_handTurn].SendMessage(new Message { Type = Utils.Type.CHANGE_COLOR });
        }

        public void ChangeColor(string colorChar)
        {
            switch (colorChar)
            {
                case "r":
                    _model.scrapCard.color = ConsoleColor.Red;
                    break;

                case "b":
                    _model.scrapCard.color = ConsoleColor.Blue;
                    break;

                case "y":
                    _model.scrapCard.color = ConsoleColor.Yellow;
                    break;

                case "g":
                    _model.scrapCard.color = ConsoleColor.Green;
                    break;
            }

            if (drawFour == false)
            {
                StartGame();
                NextHand();
                StartTurn();
                NextHand();
                StartGame();
                NextHand();
                NextHand();
            }
            else
            {
                StartGame();
                NextHand();
                _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
                _model.deck.RemoveAt(_model.deck.Count - 1);
                _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
                _model.deck.RemoveAt(_model.deck.Count - 1);
                _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
                _model.deck.RemoveAt(_model.deck.Count - 1);
                _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
                _model.deck.RemoveAt(_model.deck.Count - 1);
                StartGame();
                NextHand();
                StartTurn();
                drawFour = false;
            }
            
        }

        public void Move(VirtualView currentView, Message message)
        {
            if (currentView != _views[_handTurn])
                throw new InvalidOperationException();

            if (message.Body == "p_pressed")
                PlacingRules(message.CardNumber, false, true);
            else
                PlacingRules(message.CardNumber, false, false);
        }

        public void DrawCard()
        {
            _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
            _model.deck.RemoveAt(_model.deck.Count - 1);
            DrawCardMessage();
        }

        public void PassTurn()
        {
            StartGame();
            NextHand();
            StartTurn();
            NextHand();
            StartGame();
            TurnDirection();
        }

        private void PlacingRules(int cardNumber, bool scrapCard, bool pPressed)
        {
            if (!scrapCard)
            {
                List<Card> handList = _model.hands[_handTurn];

                if (pPressed && cardNumber == handList.Count - 1 || !pPressed)
                {
                    Card userCard = handList[cardNumber];

                    if (userCard.color == _model.scrapCard.color && userCard.number < 10 || 
                        userCard.number == _model.scrapCard.number && userCard.number < 10)
                    {
                        _model.scrapCard = userCard;
                        handList.RemoveAt(cardNumber);
                        _model.hands[_handTurn] = handList;

                        if (EndGame())
                            return;

                        StartGame();
                        NextHand();
                        StartTurn();
                        NextHand();
                        StartGame();
                        TurnDirection();
                    }
                    else if (userCard.color == _model.scrapCard.color && userCard.number == 10 || 
                             userCard.number == _model.scrapCard.number && userCard.number == 10)
                    {
                        _model.scrapCard = userCard;
                        handList.RemoveAt(cardNumber);
                        _model.hands[_handTurn] = handList;

                        if (EndGame())
                            return;

                        StartGame();
                        NextHand();
                        StartGame();
                        NextHand();
                        StartTurn();
                    }
                    else if (userCard.color == _model.scrapCard.color && userCard.number == 11 || 
                             userCard.number == _model.scrapCard.number && userCard.number == 11)
                    {
                        if (turnDirection)
                            turnDirection = false;
                        else
                            turnDirection = true;

                        _model.scrapCard = userCard;
                        handList.RemoveAt(cardNumber);
                        _model.hands[_handTurn] = handList;

                        if (EndGame())
                            return;

                        StartGame();
                        NextHand();
                        StartTurn();
                        NextHand();
                        StartGame();
                        TurnDirection();
                    }
                    else if (userCard.color == _model.scrapCard.color && userCard.number == 12 || 
                             userCard.number == _model.scrapCard.number && userCard.number == 12)
                    {
                        _model.scrapCard = userCard;
                        handList.RemoveAt(cardNumber);
                        _model.hands[_handTurn] = handList;

                        if (EndGame())
                            return;

                        StartGame();

                        NextHand();
                        handList = _model.hands[_handTurn];
                        handList.Add(_model.deck[_model.deck.Count - 1]);
                        _model.deck.RemoveAt(_model.deck.Count - 1);
                        handList.Add(_model.deck[_model.deck.Count - 1]);
                        _model.deck.RemoveAt(_model.deck.Count - 1);
                        _model.hands[_handTurn] = handList;
                        StartGame();

                        NextHand();
                        StartTurn();
                    }
                    else if (userCard.number == 13)
                    {
                        _model.scrapCard = userCard;
                        handList.RemoveAt(cardNumber);
                        _model.hands[_handTurn] = handList;

                        if (EndGame())
                            return;

                        PickColor();
                    }
                    else if (userCard.number == 14)
                    {
                        foreach (Card usCard in handList)
                        {
                            control = DrawCheck(usCard, _model.scrapCard);

                            if (!control)
                            {
                                control = false;
                                break;
                            }
                        }

                        if (control)
                        {
                            drawFour = true;

                            _model.scrapCard = userCard;
                            handList.RemoveAt(cardNumber);
                            _model.hands[_handTurn] = handList;

                            if (EndGame())
                                return;

                            PickColor();
                        }
                        else
                            StartTurn();
                    }
                    else
                    {
                        if (!pPressed)
                            StartTurn();
                        else
                            DrawCardMessage();
                    }
                }
                else
                    DrawCardMessage();
            }
            else
            {
                if (_model.scrapCard.number == 11)
                {
                    turnDirection = false;
                    StartTurn();
                }
                else if (_model.scrapCard.number == 12)
                {
                    _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
                    _model.deck.RemoveAt(_model.deck.Count - 1);
                    _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
                    _model.deck.RemoveAt(_model.deck.Count - 1);

                    StartGame();
                    NextHand();
                    StartTurn();
                }
                else if (_model.scrapCard.number == 13)                
                    PickColor();
                else
                {
                    NextHand();
                    StartTurn();
                }                
            }
        }

        private bool DrawCheck(Card userCard, Card scrapCard)
        {
            if (userCard.color == scrapCard.color && userCard.number < 10 || 
                userCard.number == scrapCard.number && userCard.number < 10 || 
                userCard.number == 13)
                return false;

            else if (userCard.color == scrapCard.color && userCard.number == 10 || 
                     userCard.number == scrapCard.number && userCard.number == 10)
                return false;

            else if (userCard.color == scrapCard.color && userCard.number == 11 || 
                     userCard.number == scrapCard.number && userCard.number == 11)
                return false;

            else if (userCard.color == scrapCard.color && userCard.number == 12 || 
                     userCard.number == scrapCard.number && userCard.number == 12)
                return false;

            else
                return true;
        }

        private void TurnDirection()
        {
            if (_handTurn == 0 && turnDirection)
                _handTurn = 2;

            else if (_handTurn == 2 && !turnDirection)
                _handTurn = 0;

            else if (turnDirection)
                _handTurn -= 1;

            else
                _handTurn += 1;
        }

        private bool IsWinner()
        {
            if (_model.hands[_handTurn].Count == 0)
                return true;

            return false;
        }

        private bool EndGame()
        {
            if (IsWinner())
            {
                StartGame();
                NextHand();
                StartGame();
                NextHand();
                StartGame();

                NextHand();

                _views[_handTurn].SendMessage(new Message { Type = Utils.Type.WIN });
                NextHand();
                _views[_handTurn].SendMessage(new Message { Type = Utils.Type.LOSE });
                NextHand();
                _views[_handTurn].SendMessage(new Message { Type = Utils.Type.LOSE });

                return true;
            }

            return false;
        }
    }
}
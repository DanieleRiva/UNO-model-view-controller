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
        private int round;
        bool turnDirection = true;
        bool control = false;

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
                        string prova = $"Ti sei connesso al server. [{players + 1}/3]";
                        virtualView.SendMessage(new Message { Type = Utils.Type.CONNECTED, Body = prova });
                    }
                    else
                    {
                        string prova = $"{username} si è connesso. [{players + 1}/3]";
                        virtualView.SendMessage(new Message { Type = Utils.Type.CONNECTED, Body = prova });
                    }
                }
            }
        }

        public void Start()
        {
            foreach (var virtualView in _views)
            {
                Console.WriteLine(virtualView);
                //virtualView.SendMessage(new Message { Type = Utils.Type.START_GAME });
            }

            for (int i = 0; i < 3; i++)
            {
                StartGame();
                NextHand();
            }

            StartTurn();
        }

        public void StartTurn()
        {
            _views[_handTurn].SendMessage(new Message { Type = Utils.Type.START_TURN, Cards = _model.hands[_handTurn], SingleCard = _model.scrapCard });
        }

        public void StartGame()
        {
            _views[_handTurn].SendMessage(new Message { Type = Utils.Type.START_GAME, Cards = _model.hands[_handTurn], SingleCard = _model.scrapCard });
        }



        public void Move(VirtualView currentView, Message message)
        {
            if (currentView != _views[_handTurn])
                throw new InvalidOperationException();

            PlacingRules(message.CardNumber);
        }


        public void DrawCard()
        {
            _model.hands[_handTurn].Add(_model.deck[_model.deck.Count - 1]);
            _model.deck.RemoveAt(_model.deck.Count - 1);
            StartTurn();
        }


        public void PlacingRules(int cardNumber)
        {
            List<Card> handList = _model.hands[_handTurn];
            Card userCard = handList[cardNumber];
            
            if (userCard.color == _model.scrapCard.color && userCard.number < 10 || userCard.number == _model.scrapCard.number && userCard.number < 10)
            {
                _model.scrapCard = userCard;
                handList.RemoveAt(cardNumber);
                _model.hands[_handTurn] = handList;
                StartGame();
                NextHand();
                StartTurn();
                NextHand();
                StartGame();
                TurnDirection();

            }
            else if (userCard.color == _model.scrapCard.color && userCard.number == 10 || userCard.number == _model.scrapCard.number && userCard.number == 10)
            {
                _model.scrapCard = userCard;
                handList.RemoveAt(cardNumber);
                _model.hands[_handTurn] = handList;
                StartGame();
                NextHand();
                StartGame();
                NextHand();
                StartTurn();
            }
            else if (userCard.color == _model.scrapCard.color && userCard.number == 11 || userCard.number == _model.scrapCard.number && userCard.number == 11)
            {
                if (turnDirection)
                    turnDirection = false;
                else
                    turnDirection = true;


                _model.scrapCard = userCard;
                handList.RemoveAt(cardNumber);
                StartGame();
                NextHand();
                StartTurn();
                NextHand();
                StartGame();
                TurnDirection();
            }
            else if (userCard.color == _model.scrapCard.color && userCard.number == 12 || userCard.number == _model.scrapCard.number && userCard.number == 12)
            {
                _model.scrapCard = userCard;
                handList.RemoveAt(handList.Count - 1);
                //invia messaggio
                NextHand();
                //quello dopo deve pescare e salto tunro
                handList = _model.hands[_handTurn];
                handList.Add(_model.deck[_model.deck.Count - 1]);
                _model.deck.RemoveAt(_model.deck.Count - 1);
                handList.Add(_model.deck[_model.deck.Count - 1]);
                _model.deck.RemoveAt(_model.deck.Count - 1);
                //tocca quello dopo
                NextHand();
            }
            else if (userCard.number == 13)
            {
                _model.scrapCard = userCard;
                handList.RemoveAt(handList.Count - 1);
                //invia messaggio scelta colore
            }
            else if (userCard.number == 14)
            {
                foreach (Card usCard in handList)
                {
                    control = DrawCheck(usCard, _model.scrapCard);
                    if (!control)
                        control = false;
                    break;
                }

                if (control)
                {
                    _model.scrapCard = userCard;
                    handList.RemoveAt(handList.Count - 1);
                    //messaggio scelta colore
                    //messaggio cambio scrap
                    NextHand();
                    handList = _model.hands[_handTurn];
                    handList.Add(_model.deck[_model.deck.Count - 1]);
                    _model.deck.RemoveAt(_model.deck.Count - 1);
                    handList.Add(_model.deck[_model.deck.Count - 1]);
                    _model.deck.RemoveAt(_model.deck.Count - 1);
                    handList.Add(_model.deck[_model.deck.Count - 1]);
                    _model.deck.RemoveAt(_model.deck.Count - 1);
                    handList.Add(_model.deck[_model.deck.Count - 1]);
                    _model.deck.RemoveAt(_model.deck.Count - 1);
                    //messaggio aggiunta carte
                    NextHand();
                    //messaggio scelta carta
                }
                else
                {
                    //messaggio carta non puo essere inviata
                }
                
            }
            
        }


        public bool DrawCheck(Card userCard, Card scrapCard)
        {
            if (userCard.color == scrapCard.color && userCard.number < 10 || userCard.number == scrapCard.number && userCard.number < 10 || userCard.number == 13)
            {
                return false;
            }
            else if (userCard.color == scrapCard.color && userCard.number == 10 || userCard.number == scrapCard.number && userCard.number == 10)
            {
                return false;
            }
            else if (userCard.color == scrapCard.color && userCard.number == 11 || userCard.number == scrapCard.number && userCard.number == 11)
            {
                return false;
            }
            else if (userCard.color == scrapCard.color && userCard.number == 12 || userCard.number == scrapCard.number && userCard.number == 12)
            {
                return false;
            }
            return true;
        }


        public void TurnDirection()
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
    }
}
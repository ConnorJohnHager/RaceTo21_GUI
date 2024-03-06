using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21_GUI
{
    public class Deck
    {
        List<Card> cards = new List<Card>();

        public Deck()
        {
            string[] suits = { "S", "H", "C", "D" };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardLongName;

                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace of ";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack of ";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen of ";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King of ";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardLongName = cardVal.ToString() + " of ";
                            break;
                    }

                    if (cardSuit == "S")
                    {
                        cardLongName += "Spades";
                    }
                    else if (cardSuit == "H")
                    {
                        cardLongName += "Hearts";
                    }
                    else if (cardSuit == "C")
                    {
                        cardLongName += "Clubs";
                    }
                    else if (cardSuit == "D")
                    {
                        cardLongName += "Diamonds";
                    };

                    cards.Add(new Card { ID = cardName + cardSuit, name = cardLongName, source = @"file://C:\Users\DGM6308\source\repos\RaceTo21_GUI\RaceTo21_GUI\Card_Images\" + cardName + cardSuit + ".png"});
                }
            }
        }

        public void Shuffle()
        {
            Random rng = new Random();

            for (int i = 0; i < cards.Count; i++)
            {
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        public Card DealTopCard()
        {
            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }
    }
}

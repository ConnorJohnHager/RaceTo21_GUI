using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21_GUI
{
    public class Player
    {
        public string name;
        public List<Card> cards = new List<Card>();
        public PlayerStatus status = PlayerStatus.active;
        public int score;
        public int bank = 100;

        public Player(string n)
        {
            name = n;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace S10258240_PRG2Assignment
{
    class PointCard
    {
        private int points;
        private int punchCard;
        private string tier;
        public int Points {  get; set; }
        public int PunchCard { get; set; }
        public string Tier { get; set; }
        public PointCard() { }
        public PointCard(int points,int punchCard)
        {
            Points = points;
            Points = punchCard;
        }
        void AddPoints(int addedpoints)
        {
            Points += addedpoints;
        }
        void RedeemPoints(int points)
        {
            if (tier == "Gold"||tier == "Silver")
            {
                Points -= points;
            }
            else
            {
                Points = Points;
            }
        }
            void Punch()
        {
            int FinalPrice;
            if (punchCard == 10)
            {
                punchCard = 0;
                FinalPrice = 0;
            }
            else
            {
                punchCard += 1;
            }
        }
        public override string ToString()
        {
            return " Points: " + Points + " PunchCard " + punchCard;
        }
    }
    
}

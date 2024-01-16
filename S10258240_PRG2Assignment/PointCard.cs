using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

//==========================================================
// Student Number : S10255784
// Student Name :  Lucas Yeo
// Partner Name : Jovan Ong Yi Jie
// Partner Number : S10258240
//==========================================================

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
            PunchCard = punchCard;
        }
        void AddPoints(int addedpoints)
        {
            Points += addedpoints;
        }
        void RedeemPoints(int points)
        {
            if (Tier == "Gold"||Tier == "Silver")
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
            if (PunchCard == 10)
            {
                PunchCard = 0;
                FinalPrice = 0;
            }
            else
            {
                PunchCard += 1;
            }
        }
        public override string ToString()
        {
            return " Points: " + Points + " PunchCard " + PunchCard;
        }
    }
    
}

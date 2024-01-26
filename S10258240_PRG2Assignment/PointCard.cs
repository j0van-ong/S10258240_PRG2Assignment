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
        public void AddPoints(int addedpoints)
        {
            Points += addedpoints;
        }
        public void RedeemPoints(int pts)
        {
            if (pts <= Points)
            {
                Points -= pts;
            }
            else
            {
                throw new Exception($"Not enough points, Enter a point less than or equal to {Points}Pts");
            }
        }
        public void Punch() 
        {
            PunchCard = 0;
        }
        public override string ToString()
        {
            return " Points: " + Points + " PunchCard " + PunchCard;
        }
    }
    
}

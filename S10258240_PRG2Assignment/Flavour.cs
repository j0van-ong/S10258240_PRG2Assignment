using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

//==========================================================
// Student Number : S10258240
// Student Name : Jovan Ong Yi  Jie
// Partner Name : Lucas Yeo
// Partner Number : S10255784
//==========================================================

namespace S10258240_PRG2Assignment
{
    class Flavour
    {
		//Attributes
		private string type;
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		private bool premium;
		public bool Premium
		{
			get { return premium; }
			set { premium = value; }
		}

		private int quantity;
		public int Quantity
		{
			get { return quantity; }
            set
            {
                if (value <= 3) //Maximum scoop is 3
                {
                    quantity = value;
                }
                else
                {
                    throw new Exception($"The quantity selected exceeds your scoops, try again.");
                }
            }
        }

		//Constructors
		public Flavour() { }
		public Flavour(string t, bool p, int q)
		{
			Type = t;
			Premium = p;
			Quantity = q;
		}

        //Method
        public override string ToString()
        {
			return $"Type: {Type} Premium: {Premium}  Quantity: {Quantity}";
        }

    }
}

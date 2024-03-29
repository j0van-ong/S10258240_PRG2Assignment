﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10258240
// Student Name : Jovan Ong Yi  Jie
// Partner Name : Lucas Yeo
// Partner Number : S10255784
//==========================================================

namespace S10258240_PRG2Assignment
{
    class Cone: IceCream
    {
		//Attributes
		private bool dipped;
		public bool Dipped
		{
			get { return dipped; }
			set { dipped = value; }
		}

		//Constructors
		public Cone():base() { }
		public Cone(string o, int s, List<Flavour> fList, List<Topping> tList, bool d):base(o, s, fList, tList)
		{
			Dipped = d;
		}

        //Methods
        public override double CalculatePrice()
		{
            double cost = 0;
            double basePrice = 4;
            double doublePrice = 5.50;
            double triplePrice = 6.50;
            //Adding cost of scoops
            if (Scoops == 1)
            {
                cost += basePrice;
            }
            else if (Scoops == 2)
            {
                cost += doublePrice;
            }
            else if (Scoops == 3)
            {
                cost += triplePrice;
            }

            //Checking for premium flavours
            double premiumScoops = 2; //$2 for every premium
            foreach (Flavour f in Flavours)
            {
                if (f.Premium)
                {
                    cost += f.Quantity* premiumScoops;
                }
            }

            //Checking for dipped cones
            double dippedPrice = 2;
            if (Dipped)
            {
                cost += dippedPrice;
            }

            //Adding cost of toppings
            int toppingsPrice = 1;
            cost += (Toppings.Count * toppingsPrice);
            return cost;
        }

        public override string ToString()
        {
            return base.ToString() + $"\nChocolate-Dipped Cone: {Dipped}\nCost: ${CalculatePrice():0.00}";
        }

    }


}


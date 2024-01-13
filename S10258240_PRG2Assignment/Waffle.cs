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
    class Waffle: IceCream
    {
		//Attributes
		private string waffleFlavour;
		public string WaffleFlavour
		{
			get { return waffleFlavour; }
			set 
			{
				value = value.ToLower(); //to default lower
				bool correctOpt = false;
				string[] wFlavourList = { "red velvet", "charcoal", "pandan", "original" };
				for (int i = 0; i < wFlavourList.Length; i++)
				{
					if (value == wFlavourList[i])
					{
                        correctOpt = true;
                        waffleFlavour = value;
                        break;
                    }
                }
                if (!correctOpt)
                {
                    throw new Exception($"Incorrect flavour: {value}, check spelling or try again");
                }
            }
	
		}

		//Constructor
		public Waffle():base() { }
		public Waffle(string o, int s, List<Flavour> fList, List<Topping> tList, string wf):base(o, s, fList, tList )
		{
			WaffleFlavour = wf;
		}

        //Methods
        public override double CalculatePrice()
        {
            double cost = 0;
            double basePrice = 7;
            double doublePrice = 8.50;
            double triplePrice = 9.50;
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
            double premiumScoops = 2;
            foreach (Flavour f in Flavours)
            {
                if (f.Premium)
                {
                    cost += premiumScoops * f.Quantity;
                }
            }
            //Checking of special flavour waffles
            double addWFlavour = 3;
            if (waffleFlavour != "original")
            {
                cost += addWFlavour;
            }
            //Adding cost of toppings
            cost += (Toppings.Count * 1);
            return cost;
        }
        public override string ToString()
        {
            return base.ToString() + $"\nWaffle Flavour: {waffleFlavour}\nCost: {CalculatePrice()}";
        }




    }
}

using System;
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
    class Cup: IceCream
    {
        //Constructors
        public Cup() { }
        public Cup(string o, int s, List<Flavour> fList, List<Topping> tList):base(o, s, fList, tList) { }

        //Methods
        public override double CalculatePrice() //override abstract method
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
            double premiumScoops = 2; //price
            foreach(Flavour f in Flavours)
            {
                if (f.Premium)
                {
                    cost += premiumScoops * f.Quantity;
                }
            }

            //Adding cost of toppings
            int toppingsPrice = 1;
            cost += (Toppings.Count * toppingsPrice);
            return cost;
        }

        public override string ToString()
        {
            return base.ToString() + $"\nCost: ${CalculatePrice()}";
        }

    }
}

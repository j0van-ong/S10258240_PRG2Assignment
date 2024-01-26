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
     abstract class IceCream
    {
		//Attributes and Properties
		private string option;
		public string Option
		{
			get { return option; }
			set { option = value; }
		}

		private int scoops;
		public int Scoops
		{
			get { return scoops; }
			set { scoops = value; }
		}

		private List<Flavour> flavours;
		public List<Flavour> Flavours 
		{ 
			get { return flavours; }
			set { flavours = value; }
		}

		private List<Topping> toppings;
		public List<Topping> Toppings
		{
			get { return toppings; }
			set { toppings = value; }
		}

		//Constructors
		public IceCream() 
		{
			Flavours = new List<Flavour>();
			Toppings = new List<Topping>();
		}
		public IceCream(string o, int s, List<Flavour> fList, List<Topping> tList )
		{
			Option = o;
			Scoops = s;
			Flavours = fList;
			Toppings = tList;
		}

		//Methods
		public abstract double CalculatePrice();

        public override string ToString()
        {
			string information = $"\nOption: {Option}\nScoops: {Scoops}\n";
			information += "\nFlavours: ";
			foreach ( Flavour f in Flavours )
			{
				information += "\n" + f.ToString();
			}
			if (Toppings.Count != 0)
			{
                foreach (Topping t in Toppings)
                {
                    information += t.ToString();
                }
            }
			else 
			{ information += "\nType of Topping: NIL"; }

			return information;
        }

    }
}

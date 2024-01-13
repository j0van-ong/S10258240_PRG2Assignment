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
			set 
			{
				value = value.ToLower();
				bool correctOpt = false;
				string[] optionArray = { "cup", "cone", "waffle" };
				for (int i = 0; i < optionArray.Length; i++)
				{
					if (value == optionArray[i])
					{
                        option = value;
						correctOpt = true;
						break;
                    }
				}
				if (!correctOpt)
				{
					throw new Exception($"Invalid option: {value} , try again");
				}
				
			}
		}

		private int scoops;
		public int Scoops
		{
			get { return scoops; }
			set 
			{
				if (value <= 3)
				{
                    scoops = value;
                }
				else
				{
					throw new Exception("Please select between 1-3 scoops");
				}
            }
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
		public IceCream() { }
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
			string information = $"Option: {Option}   Scoops: {Scoops}\n";
			foreach ( Flavour f in Flavours )
			{
				
				information += "Flavours: \n" + f.ToString();
			}
			if (Toppings.Count != 0)
			{
                foreach (Topping t in Toppings)
                {
                    information += t.ToString();
                }
            }
			else 
			{ information += "Toppings: NIL"; }

			return information;
        }

    }
}

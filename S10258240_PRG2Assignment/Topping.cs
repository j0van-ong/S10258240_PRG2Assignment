using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    class Topping
    {
		//Attributes
		private string type;
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		//Constructors
		public Topping() { }
		public Topping(string t) 
		{
			Type = t;
		}

        //Method
        public override string ToString()
        {
			return $"Type of Topping(s): {Type}";
        }
    }
}

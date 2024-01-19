using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10255784
// Student Name :  Lucas Yeo
// Partner Name : Jovan Ong Yi Jie
// Partner Number : S10258240
//==========================================================

namespace S10258240_PRG2Assignment
{
    class Order
    {
        private int id;
        public int Id { get; set; }
        public DateTime TimeReceived { get;set; }
        public DateTime TimeFulfilled { get; set; }
        public List<IceCream>iceCreamList { get; set; } = new List<IceCream>();

        public Order() { }
        public Order(int id, DateTime r)
        {
            Id = id;
            TimeReceived = r;
        }
        public void AddIceCream(IceCream iceCream)
        {
            iceCreamList.Add(iceCream);
        }
        public double CalculateTotal()
        {
            double Price = 0;
            foreach (IceCream iceCream in iceCreamList)
            {
                Price = iceCream.CalculatePrice();
                Price += Price;
            }
            return Price;
        }
        public void ModifyIceCream(int icecreamchoice)
        {
            if (icecreamchoice >= 0 && icecreamchoice < iceCreamList.Count)
            {
                Console.WriteLine("You have chosen: "+ iceCreamList[icecreamchoice]);
                Console.Write("Enter new ice cream option: ");
                iceCreamList[icecreamchoice].Option = Console.ReadLine();
                Console.Write("Enter your new scoops: ");
                iceCreamList[icecreamchoice].Scoops = Convert.ToInt32(Console.ReadLine());
                modifyFlavour();
                modifyToppings();

                void modifyFlavour()
                {
                    if (iceCreamList[icecreamchoice].Flavours.Count < 3)
                    {
                        string NewFlavour = "";
                        bool IsPremium = false;
                        int quantity = 0;
                        Console.Write("Enter your new flavour: ");
                        NewFlavour = Console.ReadLine();
                        if(NewFlavour == "Durian"||NewFlavour == "Ube"||NewFlavour == "Sea Salt")
                        {
                            IsPremium = true;
                        }
                        Console.Write("Enter your quantity");
                        quantity = Convert.ToInt32(Console.ReadLine());
                        if(iceCreamList[icecreamchoice].Flavours.Count + quantity <= 3)
                        {
                            Flavour newflavour = new Flavour(NewFlavour,IsPremium,quantity);
                            iceCreamList[icecreamchoice].Flavours.Add(newflavour);
                        }
                        else
                        {
                            Console.WriteLine("Max quantity is 3");
                        }
                    } 

                }
                void modifyToppings() //
                {
                    if (iceCreamList[icecreamchoice].Toppings.Count < 4)
                    {
                        string newtopping = "";
                        Console.Write("Enter your new Topping: ");
                        newtopping = Console.ReadLine();
                        Topping topping = new Topping(newtopping);
                        iceCreamList[icecreamchoice].Toppings.Add(topping);
                    }
                    else
                    {
                        Console.WriteLine("Max toppings reached");
                    }
                }
                


            }
            else
            {
                Console.WriteLine("Index out of range");
            }

        }
        public override string ToString()
        {
            string information = "";
            foreach (IceCream icecream in iceCreamList)
            {
                information += "\nIce Cream Order: \n" + icecream.ToString();
            }
            return information + "\nId: " + Id + "\nTime received order: " + Convert.ToString(TimeReceived);


        }

    }
}

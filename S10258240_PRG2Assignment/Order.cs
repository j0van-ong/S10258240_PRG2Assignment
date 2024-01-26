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
        public DateTime TimeReceived { get; set; }
        public DateTime? TimeFulfilled { get; set; }
        public List<IceCream> iceCreamList { get; set; } = new List<IceCream>();

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
                Price += iceCream.CalculatePrice();
            }
            return Price;
        }

        public void ModifyIceCream(int icecreamchoice)
        {
            if (icecreamchoice >= 0 && icecreamchoice < iceCreamList.Count)
            {
                List<IceCream> icecreamOption = InitOptionList();

                //Intialise FlavourDict for reference as collection
                Dictionary<string, Flavour> flavourDict = InitFlavourDict();

                //Call to initialise ToppingDict for reference as collection
                Dictionary<string, Topping> toppingDict = InitToppingDict();
                Console.WriteLine("You have chosen: \n------------------------\n" + iceCreamList[icecreamchoice]);
                int DisplayMenu()
                {
                    Console.WriteLine("[MENU]\n[1] Modify ice cream choice\n[2] Modify scoops(Add/Delete scoops)\n[3] Modify flavours(Change flavour type and quantity)\n[4] Modify Toppings\n[0] Exit");
                    int option = Convert.ToInt32(Console.ReadLine());
                    return option;
                }
                while (true)
                {
                    int option = DisplayMenu(); //call the menu, and get back the int option
                    if (option == 0)
                    {
                        break; //ends the program for input 0
                    }
                    if (option == 1)
                    {
                        ModifyOption(iceCreamList[icecreamchoice]);
                        void ModifyOption(IceCream icecream)
                        {
                            Console.WriteLine("Current Option: {}", iceCreamList[icecreamchoice].Option);
                            Console.WriteLine("[1]Switch to Cup.");
                            Console.WriteLine("[2]Switch to Cone.");
                            Console.WriteLine("[3]Switch to Waffle.");
                            Console.WriteLine("[0]Exit.");
                            Console.Write("Enter your option: ");
                            int swap = Convert.ToInt32(Console.ReadLine());
                            while (true)
                            {
                                if (option == 0)
                                {
                                    break;
                                }
                                else if (swap == 1)
                                {
                                    iceCreamList.Remove(iceCreamList[icecreamchoice]);
                                    Cup cup1 = (Cup)icecream;
                                    AddScoops(flavourDict);
                                    AddToppings(toppingDict);
                                }
                                else if (swap == 2)
                                {
                                    bool isDipped = false;
                                    iceCreamList.Remove(iceCreamList[icecreamchoice]);
                                    Cone cone1 = (Cone)icecream;
                                    AddScoops(flavourDict);
                                    AddToppings(toppingDict);
                                    while (true) //prevents code from restarting to the top again
                                    {
                                        try
                                        {
                                            Console.Write("\nAdd on Chocolate Dipped Cone? [True or False]: ");
                                            isDipped = Convert.ToBoolean(Console.ReadLine().ToLower());
                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine("Invalid input, only true/false");
                                            continue;
                                        }
                                        break; //if try has no problem, code proceeds here
                                    }
                                    cone1.Dipped = isDipped;
                                }
                                else if (option == 3)
                                {
                                    iceCreamList.Remove(iceCreamList[icecreamchoice]);
                                    Waffle waffle = (Waffle)icecream;
                                    AddScoops(flavourDict);
                                    AddToppings(toppingDict);
                                    Console.Write("\nSelect Waffle Flavour [Original, Red Velvet, Charcoal, Pandan]: ");
                                    string WaffleF = Console.ReadLine().ToLower();
                                    waffle.Option = WaffleF;
                                }
                                else
                                {
                                    Console.WriteLine("Enter a valid input");
                                }
                            }
                        }
                    }
                    else if (option == 2)
                    {
                        Console.Write("[Modify Scoops]\n[1]Add new scoop\n[2]Delete scoop\nEnter option: ");
                        int scoopoption = Convert.ToInt32(Console.ReadLine());
                        if (scoopoption == 1)
                        {
                            AddScoops(flavourDict);
                            while (iceCreamList[icecreamchoice].Flavours.Count < 3)
                            {
                                Console.WriteLine("Modifed Ice Cream: \n-------------------\n" + iceCreamList[icecreamchoice]);
                                Console.WriteLine("Add another scoop?[Y or N]: ");
                                Console.Write("Enter Option: ");
                                string response = Console.ReadLine();
                                if (response != "N")
                                {
                                    if (response == "Y")
                                    {
                                        AddScoops(flavourDict);
                                    }
                                    else if (response == "N")
                                    {
                                        break;
                                    }
                                    else { Console.WriteLine("Incorrect input, only Y or N"); }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else if (option == 2)
                        {
                            Console.WriteLine("Scoop flavours in selected order: ");
                            foreach (Flavour flavour in iceCreamList[icecreamchoice].Flavours)
                            {
                                Console.WriteLine(flavour.Type);
                            }
                            Console.WriteLine("Choose which flavour to remove: ");
                            string removeFlavour = Console.ReadLine();
                            string loweredRemoveFlavour = removeFlavour.ToLower();
                            List<Flavour> flavorsToRemove = new List<Flavour>();
                            foreach (Flavour flavour in iceCreamList[icecreamchoice].Flavours)
                            {
                                if (flavour.Type.ToLower() == loweredRemoveFlavour)
                                {
                                    flavorsToRemove.Add(flavour);
                                }
                            }

                            foreach (Flavour flavourToRemove in flavorsToRemove)
                            {
                                iceCreamList[icecreamchoice].Flavours.Remove(flavourToRemove);
                                iceCreamList[icecreamchoice].Scoops -= 1;
                            }
                            Console.WriteLine("\nModifed Ice Cream: \n" + iceCreamList[icecreamchoice]);
                        }
                        else
                        {
                            Console.WriteLine("Please input a number");
                        }
                        //Adds scoops to selected ice cream
                    }
                    else if (option == 3)
                    {
                        modifyFlavour();
                    }
                    else if (option == 4)
                    {
                        modifyToppings();
                    }
                }
                void modifyFlavour()
                {
                    modifySpecificFlavour();
                    while (iceCreamList[icecreamchoice].Flavours.Count <= 3)
                    {
                        Console.WriteLine("Modifed Flavour: \n-------------------\n" + iceCreamList[icecreamchoice]);
                        Console.WriteLine("Modify another flavour?[Y or N]: ");
                        Console.Write("Enter Option: ");
                        string response = Console.ReadLine();
                        if (response != "N")
                        {
                            if (response == "Y")
                            {
                                modifySpecificFlavour();
                            }
                            else if (response == "N")
                            {
                                break;
                            }
                            else { Console.WriteLine("Incorrect input, only Y or N"); }
                        }
                        else
                        {
                            break;
                        }
                    }
                    void modifySpecificFlavour()
                    {
                        string NewFlavour = "";
                        string modifiedFlavour = "";
                        bool IsPremium = false;
                        int quantity = 0;
                        Console.WriteLine("Flavours in Ice Cream: ");
                        foreach (Flavour flavour in iceCreamList[icecreamchoice].Flavours)
                        {
                            Console.WriteLine(flavour.Type);
                        }
                        Console.Write("Choose which flavour to modify: ");
                        modifiedFlavour = Console.ReadLine();
                        foreach (Flavour f in iceCreamList[icecreamchoice].Flavours)
                        {
                            if (f.Type.ToLower() == modifiedFlavour.ToLower())
                            {
                                iceCreamList[icecreamchoice].Flavours.Remove(f);
                                Console.WriteLine("\nAvailable Flavours: \nNormal [Vanilla, Chocolate, Strawberry] \nPremium [Durian, Ube, Sea Salt]");
                                Console.Write("Enter your new flavour: ");
                                NewFlavour = Console.ReadLine();
                                string loweredNewFlavour = NewFlavour.ToLower();
                                if (loweredNewFlavour == "durian" || loweredNewFlavour == "ube" || loweredNewFlavour == "sea salt")
                                {
                                    IsPremium = true;
                                }
                                Console.Write("Enter your quantity: ");
                                quantity = Convert.ToInt32(Console.ReadLine());
                                Flavour newflavour = new Flavour(NewFlavour, IsPremium, quantity);
                                iceCreamList[icecreamchoice].Flavours.Add(newflavour);
                                break;
                            }
                        }
                    }

                }
                void modifyToppings() //
                {
                    Console.Write("[Modify Toppings]\n[1]Modify Existing Toppings\n[2]Add Toppings\n[3]Delete Topping\nEnter option: ");
                    int option = Convert.ToInt32(Console.ReadLine());
                    if (option == 1)
                    {
                        string modifiedTopping = "";
                        string NewTopping = "";
                        Console.WriteLine("Toppings in Ice Cream: ");
                        foreach (Topping topping in iceCreamList[icecreamchoice].Toppings)
                        {
                            Console.WriteLine(topping.Type);
                        }
                        Console.WriteLine("Choose which topping to modify: ");
                        modifiedTopping = Console.ReadLine();
                        foreach (Topping t in iceCreamList[icecreamchoice].Toppings)
                        {
                            if (t.Type.ToLower() == modifiedTopping.ToLower())
                            {
                                iceCreamList[icecreamchoice].Toppings.Remove(t);
                                Console.WriteLine("\nvailable Toppings: Oreos, Sprinkles, Mochi and Sago");
                                Console.Write("Enter your new Topping: ");
                                NewTopping = Console.ReadLine();
                                Topping newtopping = new Topping(NewTopping);
                                iceCreamList[icecreamchoice].Toppings.Add(newtopping);
                                break;
                            }
                        }
                    }
                    else if (option == 2)
                    {
                        AddToppings(toppingDict);
                    }
                    else if (option == 3)
                    {
                        Console.WriteLine("Toppings in selected order: ");
                        foreach (Topping topping in iceCreamList[icecreamchoice].Toppings)
                        {
                            Console.WriteLine(topping.Type);
                        }
                        Console.WriteLine("Choose which Topping to remove: ");
                        string removeTopping = Console.ReadLine();
                        string loweredRemoveTopping = removeTopping.ToLower();
                        List<Topping> ToppingToRemove = new List<Topping>();
                        foreach (Topping topping in iceCreamList[icecreamchoice].Toppings)
                        {
                            if (topping.Type.ToLower() == loweredRemoveTopping)
                            {
                                ToppingToRemove.Add(topping);
                            }
                        }

                        foreach (Topping toppingToRemove in ToppingToRemove)
                        {
                            iceCreamList[icecreamchoice].Toppings.Remove(toppingToRemove);
                        }
                    }
                }
                //Adds toppings
                void AddToppings(Dictionary<string, Topping> tList)
                {

                    string NewTopping = "";
                    Console.WriteLine("\nAvailable Toppings: Oreos, Sprinkles, Mochi and Sago");
                    Console.Write("Enter your new Topping: ");
                    NewTopping = Console.ReadLine();
                    string loweredNewTopping = NewTopping.ToLower();
                    if (tList.ContainsKey(loweredNewTopping))
                    {
                        if (iceCreamList[icecreamchoice].Toppings.Count + 1 <= 4)
                        {
                            Topping newtopping = new Topping(loweredNewTopping);
                            iceCreamList[icecreamchoice].Toppings.Add(newtopping);
                        }
                        else if (iceCreamList[icecreamchoice].Toppings.Count + 1 > 4)
                        {
                            Console.WriteLine("Max toppings is 4");
                        }
                    }

                }
                //Adds scoops
                void AddScoops(Dictionary<string, Flavour> fList)
                {

                    string NewFlavour = "";
                    bool IsPremium = false;
                    int quantity = 0;
                    Console.WriteLine("\nAvailable Flavours: \nNormal [Vanilla, Chocolate, Strawberry] \nPremium [Durian, Ube, Sea Salt]");
                    Console.Write("Enter your new flavour: ");
                    NewFlavour = Console.ReadLine();
                    string loweredNewFlavour = NewFlavour.ToLower();
                    if (fList.ContainsKey(loweredNewFlavour))
                    {
                        if (loweredNewFlavour == "durian" || loweredNewFlavour == "ube" || loweredNewFlavour == "sea salt")
                        {
                            IsPremium = true;
                        }
                    }
                    Console.Write("Enter your quantity: ");
                    quantity = Convert.ToInt32(Console.ReadLine());
                    if (iceCreamList[icecreamchoice].Flavours.Count + quantity <= 3)
                    {
                        Flavour newflavour = new Flavour(NewFlavour, IsPremium, quantity);
                        iceCreamList[icecreamchoice].Flavours.Add(newflavour);
                        iceCreamList[icecreamchoice].Scoops += 1;
                    }
                    else if (iceCreamList[icecreamchoice].Flavours.Count + 1 > 3)
                    {
                        Console.WriteLine("Max scoops is 3");
                    }
                }
                List<IceCream> InitOptionList() //Reads all possible combinations of valid icecream and stores them in a list
                {
                    bool dipped;
                    string[] optionArray = { "cup", "cone", "waffle" };
                    List<IceCream> icecreamOption = new List<IceCream>();
                    using (StreamReader sr = new StreamReader("options.csv"))
                    {
                        string? s = sr.ReadLine(); // header skip
                        while ((s = sr.ReadLine()) != null)
                        {
                            string[] data = s.Split(",");
                            string opt = data[0].ToLower();
                            int scoops = Convert.ToInt32(data[1]);
                            string tempBool = data[2];
                            if (!string.IsNullOrEmpty(tempBool)) //there are blanks in csv which can't be converted, hence the check
                            {
                                dipped = Convert.ToBoolean(data[2].ToLower()); //default small letter
                            }
                            else { dipped = false; } //no other icecream has this option, a dummy value
                            string waffleF = data[3].ToLower();
                            for (int i = 0; i < optionArray.Length; i++)
                            {
                                string optionData = optionArray[i];
                                if (optionData == opt && i == 0) //matches option type
                                {
                                    Cup ic1 = new Cup(); //empty class for further addition in
                                    ic1.Option = optionData;
                                    ic1.Scoops = scoops;
                                    icecreamOption.Add(ic1);
                                }
                                else if (optionData == opt && i == 1)
                                {
                                    Cone ic2 = new Cone();
                                    ic2.Option = optionData;
                                    ic2.Scoops = scoops;
                                    ic2.Dipped = dipped;
                                    icecreamOption.Add(ic2);
                                }
                                else if (optionData == opt && i == 2)
                                {
                                    Waffle ic3 = new Waffle();
                                    ic3.Option = optionData;
                                    ic3.Scoops = scoops;
                                    ic3.WaffleFlavour = waffleF;
                                    icecreamOption.Add(ic3);
                                }
                            }
                        }
                        return icecreamOption;
                    }
                }
                //Gets flavours from csv file
                Dictionary<string, Flavour> InitFlavourDict()
                {
                    string premiumPrice = "2";
                    Dictionary<string, Flavour> flavourDict = new Dictionary<string, Flavour>(); //Create New Dict
                    using (StreamReader sr = new StreamReader("flavours.csv"))
                    {
                        string? s = sr.ReadLine(); //header skip
                        while ((s = sr.ReadLine()) != null)
                        {
                            string[] data = s.Split(",");
                            string flavour = data[0].ToLower(); //standardize lower for comparison
                            if (data[1] == premiumPrice)
                            {
                                flavourDict.Add(flavour, new Flavour(flavour, true, 0)); //set default quantity to 0
                            }
                            else //if not is 0 cost
                            {
                                flavourDict.Add(flavour, new Flavour(flavour, false, 0));
                            }
                        }
                        return flavourDict;
                    }
                }
                //Gets all possible toppings
                Dictionary<string, Topping> InitToppingDict()
                {
                    Dictionary<string, Topping> toppingDict = new Dictionary<string, Topping>(); //Create New Dict
                    using (StreamReader sr = new StreamReader("flavours.csv"))
                    {
                        string? s = sr.ReadLine(); //header skip
                        while ((s = sr.ReadLine()) != null)
                        {
                            string[] data = s.Split(",");
                            string topping = data[0].ToLower();
                            toppingDict.Add(topping, new Topping(topping));
                        }
                        return toppingDict;
                    }
                }
                //Gets user flavour
                IceCream GetFlavour(int numScoops, IceCream iceCream, Dictionary<string, Flavour> fList)
                {
                    int flavourCount = 1; //first loop is counted as one time when first run, hence set to 1
                    while (flavourCount <= numScoops) //tally with corresponding scoops
                    {
                        //default 1 flavour at least, hence will repeat the amount of times as scoops
                        Console.WriteLine("\nAvailable Flavours: \nNormal [Vanilla, Chocolate, Strawberry] \nPremium [Durian, Ube, Sea Salt]");
                        Console.Write("\nEnter a flavour: ");
                        string flavour = Console.ReadLine().ToLower();

                        if (fList.ContainsKey(flavour)) //check if key exist in FlavourDict by user input
                        {
                            Flavour chkFlavour = fList[flavour]; //gets the corresponding flavour object from dict(value)
                            if (!iceCream.Flavours.Contains(chkFlavour)) //NOT(if it alr exist in icecream List)
                            {
                                chkFlavour.Quantity = 1; //increment
                                iceCream.Flavours.Add(chkFlavour);
                                flavourCount += 1; //increment

                            }
                            else //if already exist in icecream object flavour list, add the quantity
                            {
                                foreach (Flavour f in iceCream.Flavours)
                                {
                                    if (f.Type == flavour)
                                    {
                                        f.Quantity += 1;
                                        flavourCount += 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Flavour, try again");
                            continue;
                        }
                    }
                    return iceCream; //break
                }
                //Gets user topping
                IceCream GetTopping(IceCream iceCream, Dictionary<string, Topping> tList)
                {
                    int toppingCount = 1; //first loop is counted as one time when first run, hence set to 1
                    while (toppingCount <= tList.Count)
                    {
                        Console.WriteLine("\nAvailable Toppings: Oreos, Sprinkles, Mochi and Sago");
                        Console.Write("\nEnter Topping ([n] to exit): ");
                        string topping = Console.ReadLine();
                        if (topping == "n")
                        {
                            return iceCream;
                        }
                        if (tList.ContainsKey(topping)) //check if key exist in ToppingDict by user input
                        {
                            Topping topping1 = tList[topping]; //gets the corresponding topping object
                            if (!iceCream.Toppings.Contains(topping1)) //NOT(if it alr exist in icecream List)
                            {
                                iceCream.Toppings.Add(tList[topping]);
                                toppingCount += 1;
                            }
                            else
                            {
                                Console.WriteLine("Topping has been added, use a different one!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Toppings, try again");
                            continue;
                        }
                    }
                    return iceCream;
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
                information += icecream.ToString();
            }
            return information + "\n\nId: " + Id + "\nTime received order: " + Convert.ToString(TimeReceived) + $"\nTime fulfilled: {TimeFulfilled}";


        }

    }
}

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

        public void DeleteIceCream(int i)
        {
            iceCreamList.RemoveAt(i);
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
                    int option = 0;
                    Console.Write("[MENU]\n[1] Modify ice cream choice\n[2] Modify scoops(Add/Delete scoops)\n[3] Modify flavours(Change flavour type and quantity)\n[4] Modify Toppings(Modify existing topping, Add or Delete topping)\n[5] Modify dipped cone/waffle flavour(If applicable)\n[0] Exit\nEnter option: ");
                    try
                    {
                        option = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        DisplayMenu();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return option;
                }
                while (true)
                {
                    int option = DisplayMenu(); //call the menu, and get back the int option
                    if (option == 0)
                    {
                        break; //ends the program for input 0
                    }
                    else if (option == 1)
                    {
                        ModifyOption(iceCreamList[icecreamchoice]);
                        while (true)
                        {
                            try
                            {
                                Console.WriteLine("Modifed Ice Cream: \n-------------------\n" + iceCreamList[icecreamchoice]);
                                Console.WriteLine("Modify the option again?[Y or N]: ");
                                Console.Write("Enter Option: ");
                                string response = Console.ReadLine();
                                if (response != "N")
                                {
                                    if (response == "Y")
                                    {
                                        ModifyOption(iceCreamList[icecreamchoice]);
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
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                        }
                        void ModifyOption(IceCream icecream)
                        {
                            while (true)
                            {
                                //Console.WriteLine("Current Option: {}", iceCreamList[icecreamchoice].Option);
                                Console.WriteLine("\n[1]Switch to Cup.");
                                Console.WriteLine("[2]Switch to Cone.");
                                Console.WriteLine("[3]Switch to Waffle.");
                                Console.WriteLine("[0]Exit. ");
                                try
                                {
                                    Console.Write("Enter your option: ");
                                    int swap = Convert.ToInt32(Console.ReadLine());

                                    if (swap == 0)
                                    {
                                        break;
                                    }
                                    else if (swap == 1)
                                    {
                                        if (iceCreamList[icecreamchoice] is Cone)
                                        {
                                            Cone cone1 = (Cone)icecream;
                                            Cup cup = new Cup("Cup", cone1.Scoops, cone1.Flavours, cone1.Toppings);
                                            iceCreamList[icecreamchoice] = cup;
                                            break;
                                        }
                                        else if (iceCreamList[icecreamchoice] is Waffle)
                                        {
                                            Waffle waffle1 = (Waffle)icecream;
                                            Cup cup = new Cup("Cup", waffle1.Scoops, waffle1.Flavours, waffle1.Toppings);
                                            iceCreamList[icecreamchoice] = cup;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("IceCream option is already a cup");
                                            continue;
                                        }
                                    }
                                    else if (swap == 2)
                                    {
                                        bool isDipped = false;
                                        if (iceCreamList[icecreamchoice] is Cup)
                                        {
                                            Cup cup = (Cup)icecream;
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
                                            Cone cone = new Cone("Cone", cup.Scoops, cup.Flavours, cup.Toppings, isDipped);
                                            iceCreamList[icecreamchoice] = cone;
                                            break;
                                        }
                                        else if (iceCreamList[icecreamchoice] is Waffle)
                                        {
                                            Waffle waffle = (Waffle)icecream;
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
                                            Cone cone = new Cone("Cone", waffle.Scoops, waffle.Flavours, waffle.Toppings, isDipped);
                                            iceCreamList[icecreamchoice] = cone;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("IceCream option is already a cone");
                                            continue;
                                        }
                                    }
                                    else if (swap == 3)
                                    {
                                        string WaffleF = "";
                                        if (iceCreamList[icecreamchoice] is Cup)
                                        {
                                            Cup cup = (Cup)icecream;
                                            while (true)
                                            {
                                                try
                                                {
                                                    Console.Write("\nSelect Waffle Flavour [Original, Red Velvet, Charcoal, Pandan]: ");
                                                    WaffleF = Console.ReadLine().ToLower();
                                                    if (WaffleF == "original" || WaffleF == "red velvet" || WaffleF == "charcoal" || WaffleF == "pandan")
                                                    {
                                                        Waffle waffle = new Waffle("Waffle", cup.Scoops, cup.Flavours, cup.Toppings, WaffleF);
                                                        iceCreamList[icecreamchoice] = waffle;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Please Input one of the flavours available");
                                                    }

                                                }
                                                catch (FormatException ex)
                                                {
                                                    Console.WriteLine(ex.Message);
                                                    continue;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.Message);
                                                    continue;
                                                }
                                            }
                                            break;

                                        }
                                        else if (iceCreamList[icecreamchoice] is Cone)
                                        {
                                            Cone cone = (Cone)icecream;
                                            while (true)
                                            {
                                                try
                                                {
                                                    Console.Write("\nSelect Waffle Flavour [Original, Red Velvet, Charcoal, Pandan]: ");
                                                    WaffleF = Console.ReadLine().ToLower();
                                                    if (WaffleF == "original" || WaffleF == "red velvet" || WaffleF == "charcoal" || WaffleF == "pandan")
                                                    {
                                                        Waffle waffle = new Waffle("Waffle", cone.Scoops, cone.Flavours, cone.Toppings, WaffleF);
                                                        iceCreamList[icecreamchoice] = waffle;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Please Input one of the flavours available");
                                                    }
                                                }
                                                catch (FormatException ex)
                                                {
                                                    Console.WriteLine(ex.Message);
                                                    continue;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.Message);
                                                    continue;
                                                }
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("IceCream option is already a waffle");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Option selected was a invalid input");
                                        continue;
                                    }

                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }

                    }
                    else if (option == 2)
                    {
                        while (true)
                        {
                            try
                            {
                                Console.Write("[Modify Scoops]\n[1]Add new scoop\n[2]Delete scoop\n[0]Exit\nEnter option: ");
                                int scoopoption = Convert.ToInt32(Console.ReadLine());
                                if (scoopoption == 0)
                                {
                                    break;
                                }
                                else if (scoopoption == 1)
                                {
                                    AddScoops(flavourDict);
                                    while (iceCreamList[icecreamchoice].Flavours.Count < 3)
                                    {
                                        try
                                        {
                                            Console.WriteLine("\nModifed Ice Cream: \n-------------------\n" + iceCreamList[icecreamchoice]);
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
                                        catch (FormatException ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                            continue;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                            continue;
                                        }
                                    }
                                }
                                else if (scoopoption == 2)
                                {
                                    if (iceCreamList[icecreamchoice].Flavours.Count == 1)//Check if icecream has only one topping if so unable to delete and break
                                    {
                                        Console.WriteLine("Must have at least 1 Flavour on ice cream");
                                        break;
                                    }
                                    Console.WriteLine("Scoop flavours in selected order: ");
                                    foreach (Flavour flavour in iceCreamList[icecreamchoice].Flavours)
                                    {
                                        Console.WriteLine(flavour.Type);
                                    }
                                    try
                                    {
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
                                    catch (FormatException ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Input option is out of range");
                                }
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                        }

                    }
                    else if (option == 3)
                    {
                        modifyFlavour();
                    }
                    else if (option == 4)
                    {
                        modifyToppings();
                        while (iceCreamList[icecreamchoice].Toppings.Count <= 4)
                        {
                            try
                            {
                                Console.WriteLine("Modifed Toppings: \n-------------------\n" + iceCreamList[icecreamchoice]);
                                Console.WriteLine("Modify another topping?[Y or N]: ");
                                Console.Write("Enter Option: ");
                                string response = Console.ReadLine();
                                if (response != "N")
                                {
                                    if (response == "Y")
                                    {
                                        modifyToppings();
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
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                        }
                    }
                    else if (option == 5)
                    {
                        while (true)
                        {
                            string DippedOption = "";
                            string WaffleF = "";
                            if (iceCreamList[icecreamchoice] is Cone)
                            {
                                try
                                {
                                    Cone cone = (Cone)iceCreamList[icecreamchoice];
                                    Console.WriteLine("\nStatus of dipped: " + cone.Dipped);
                                    Console.Write("Change to  True / False: ");
                                    DippedOption = Console.ReadLine();
                                    if (DippedOption.ToLower() == Convert.ToString(cone.Dipped).ToLower())
                                    {
                                        Console.WriteLine("Status is the same");
                                        continue;
                                    }
                                    else if (DippedOption.ToLower() == "true")
                                    {
                                        cone.Dipped = true;
                                    }
                                    else if (DippedOption.ToLower() == "false")
                                    {
                                        cone.Dipped = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please input either True / False");
                                        continue;
                                    }
                                    Console.WriteLine("Updated Status: " + cone.Dipped);
                                    Console.WriteLine("Modified Ice Cream: \n" + iceCreamList[icecreamchoice]);
                                    break;
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else if (iceCreamList[icecreamchoice] is Waffle)
                            {
                                try
                                {
                                    Waffle waffle = (Waffle)iceCreamList[icecreamchoice];
                                    Console.WriteLine("\nCurrent flavour: " + waffle.WaffleFlavour);
                                    Console.Write("\nSelect Waffle Flavour [Original, Red Velvet, Charcoal, Pandan]: ");
                                    WaffleF = Console.ReadLine().ToLower();
                                    if (WaffleF == "original")
                                    {
                                        waffle.WaffleFlavour = "original";
                                    }
                                    else if (WaffleF == "red velvet")
                                    {
                                        waffle.WaffleFlavour = "red velvet";
                                    }
                                    else if (WaffleF == "charcoal")
                                    {
                                        waffle.WaffleFlavour = "charcoal";
                                    }
                                    else if (WaffleF == "pandan")
                                    {
                                        waffle.WaffleFlavour = "pandan";
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please Input one of the flavours available");
                                        continue;
                                    }
                                    Console.WriteLine("Updated Flavours: " + waffle.WaffleFlavour);
                                    Console.WriteLine(iceCreamList[icecreamchoice]);
                                    break;
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Not applicable");
                                break;
                            }

                        }

                    }
                    else
                    {
                        Console.WriteLine("Option out of range");
                        DisplayMenu();
                    }

                    void modifyFlavour()
                    {
                        modifySpecificFlavour();
                        while (iceCreamList[icecreamchoice].Flavours.Count <= 3)
                        {
                            try
                            {
                                Console.WriteLine("Modifed Flavour: \n-------------------" + iceCreamList[icecreamchoice]);
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
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }

                        }
                    }
                    void modifySpecificFlavour()
                    {

                        string NewFlavour = "";
                        string modifiedFlavour = "";
                        bool IsPremium = false;
                        int quantity = 0;
                        bool IsFound = false;
                        Console.WriteLine("Flavours in Ice Cream: ");
                        foreach (Flavour flavour in iceCreamList[icecreamchoice].Flavours)
                        {
                            Console.WriteLine(flavour.Type);
                        }
                        while (true)
                        {

                            try
                            {
                                Console.Write("\nChoose which flavour to modify: ");
                                modifiedFlavour = Console.ReadLine();
                                foreach (Flavour flavour1 in iceCreamList[icecreamchoice].Flavours)
                                {
                                    if (modifiedFlavour.ToLower() == flavour1.Type.ToLower())//checks if player input is in current order ice cream flavour list
                                    {
                                        while (true)
                                        {
                                            try
                                            {
                                                Console.WriteLine("\nAvailable Flavours: \nNormal [Vanilla, Chocolate, Strawberry] \nPremium [Durian, Ube, Sea Salt]");
                                                Console.Write("Enter your new flavour: ");
                                                NewFlavour = Console.ReadLine();
                                                string LoweredNewFlavour = NewFlavour.ToLower();
                                                if (flavourDict.ContainsKey(LoweredNewFlavour))
                                                {
                                                    if (LoweredNewFlavour == "durian" || LoweredNewFlavour == "ube" || LoweredNewFlavour == "sea salt")
                                                    {
                                                        IsPremium = true;
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Flavour not inside available flavours");
                                                    continue;

                                                }
                                            }
                                            catch (FormatException ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }

                                            Console.Write("\nEnter your quantity: ");
                                            try
                                            {
                                                quantity = Convert.ToInt32(Console.ReadLine());
                                                if (quantity <= 3)
                                                {
                                                    Flavour newflavour = new Flavour(NewFlavour, IsPremium, quantity);
                                                    iceCreamList[icecreamchoice].Flavours.Remove(flavour1);
                                                    iceCreamList[icecreamchoice].Flavours.Add(newflavour);
                                                    IsFound = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Quantity cannot exceed 3");
                                                }
                                            }
                                            catch (FormatException ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                                continue;
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                                continue;
                                            }
                                        }
                                        break;
                                    }


                                }
                                if (!IsFound)
                                {
                                    Console.WriteLine("Flavour is not in icecream flavour");
                                    continue;
                                }
                                break;


                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                        }
                    }
                }
                void modifyToppings() //
                {
                    while (true)
                    {
                        try
                        {
                            Console.Write("\n[Modify Toppings]\n[1]Modify Existing Toppings\n[2]Add Toppings\n[3]Delete Topping\n[0]Exit\nEnter option: ");
                            int option = Convert.ToInt32(Console.ReadLine());
                            if (option == 0)
                            {
                                break;
                            }
                            else if (option == 1)
                            {
                                if (iceCreamList[icecreamchoice].Toppings.Count == 0)
                                {
                                    Console.WriteLine("No toppings available to modify");
                                    break;
                                }
                                string modifiedTopping = "";
                                string NewTopping = "";
                                Console.WriteLine("Toppings in Ice Cream: ");
                                foreach (Topping topping in iceCreamList[icecreamchoice].Toppings)
                                {
                                    Console.WriteLine(topping.Type);
                                }
                                try
                                {
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
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else if (option == 2)
                            {
                                AddToppings(toppingDict);

                            }
                            else if (option == 3)
                            {
                                string removeTopping = "";
                                if (iceCreamList[icecreamchoice].Toppings.Count == 0)
                                {
                                    Console.WriteLine("You have no toppings");
                                    break;
                                }
                                Console.WriteLine("Toppings in selected order: ");
                                foreach (Topping topping in iceCreamList[icecreamchoice].Toppings)
                                {
                                    Console.WriteLine(topping.Type);
                                }
                                try
                                {
                                    Console.WriteLine("Choose which Topping to remove: ");
                                    removeTopping = Console.ReadLine();
                                    string loweredRemoveTopping = removeTopping.ToLower();
                                    List<Topping> ToppingToRemove = new List<Topping>();
                                    foreach (Topping topping in iceCreamList[icecreamchoice].Toppings)
                                    {
                                        if (topping.Type.ToLower() == loweredRemoveTopping)
                                        {
                                            ToppingToRemove.Add(topping);
                                            Console.WriteLine("--Topping successfully deleted--");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Topping not found");
                                        }
                                    }

                                    foreach (Topping toppingToRemove in ToppingToRemove)
                                    {
                                        iceCreamList[icecreamchoice].Toppings.Remove(toppingToRemove);
                                    }
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Option not in range");
                            }
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                }
                //Adds scoops to selected ice cream
                void AddScoops(Dictionary<string, Flavour> fList)
                {
                    while (true)
                    {
                        string NewFlavour = "";
                        bool IsPremium = false;
                        int quantity = 0;
                        Console.WriteLine("\nAvailable Flavours: \nNormal [Vanilla, Chocolate, Strawberry] \nPremium [Durian, Ube, Sea Salt]");
                        try
                        {
                            Console.Write("Enter your new flavour: ");
                            NewFlavour = Console.ReadLine();
                            string loweredNewFlavour = NewFlavour.ToLower();
                            if (fList.ContainsKey(loweredNewFlavour))
                            {
                                if (loweredNewFlavour == "durian" || loweredNewFlavour == "ube" || loweredNewFlavour == "sea salt")
                                {
                                    IsPremium = true;
                                }
                                Console.Write("Enter your quantity: ");
                                quantity = Convert.ToInt32(Console.ReadLine());
                                if (iceCreamList[icecreamchoice].Flavours.Count + quantity <= 3)
                                {
                                    Flavour newflavour = new Flavour(NewFlavour, IsPremium, quantity);
                                    iceCreamList[icecreamchoice].Flavours.Add(newflavour);
                                    iceCreamList[icecreamchoice].Scoops += quantity;
                                    break;
                                }
                                else if (iceCreamList[icecreamchoice].Flavours.Count + quantity > 3)
                                {
                                    Console.WriteLine("Max scoops is 3");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Flavour is not available");
                            }

                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                }
                //Adds toppings
                void AddToppings(Dictionary<string, Topping> tList)
                {
                    while (true)
                    {
                        string NewTopping = "";
                        Console.WriteLine("\nAvailable Toppings: Oreos, Sprinkles, Mochi and Sago");
                        try
                        {
                            Console.Write("Enter your new Topping: ");
                            NewTopping = Console.ReadLine();
                            string loweredNewTopping = NewTopping.ToLower();
                            if (tList.ContainsKey(loweredNewTopping))
                            {
                                if (iceCreamList[icecreamchoice].Toppings.Count + 1 <= 4)
                                {
                                    Topping newtopping = new Topping(loweredNewTopping);
                                    iceCreamList[icecreamchoice].Toppings.Add(newtopping);
                                    Console.WriteLine("--Toppings successfully added--");
                                    break;
                                }
                                else if (iceCreamList[icecreamchoice].Toppings.Count + 1 > 4)
                                {
                                    Console.WriteLine("Max toppings is 4");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Topping is not available");
                            }
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
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
                    using (StreamReader sr = new StreamReader("toppings.csv"))
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

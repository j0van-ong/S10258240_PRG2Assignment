// See https://aka.ms/new-console-template for more information

//==========================================================
// Student Number : S10258240
// Student Name : Jovan Ong Yi  Jie
// Partner Name : Lucas Yeo
// Partner Number : S10255784
//==========================================================


using S10258240_PRG2Assignment;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Transactions;
using System.Xml.Linq;

/*This function displays the menu for the overall application and returns a integer, 
 *the option for the application */
int DisplayMenu()
{
    while (true)
    { 
    int option = 0; //initialise
    Console.WriteLine(); //skip line
    Console.WriteLine("---------------MENU---------------");
    //store the menu options for referencing in a list
    string[] menuArray = { "List all customers", "List all current orders", "Register a new customer", "Create a customer's order", 
        "Display order details of a customer", "Modify order details", "Process an order and checkout", "Display monthly and yearly charged amounts" };
    for (int i = 0; i < menuArray.Length; i++)
    {
        Console.WriteLine($"[{i + 1}] {menuArray[i]}");
    }
    Console.WriteLine("[0] Exit");

    try
    {
        Console.Write("\nEnter option: ");
        option = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine(); //skip a line
        if (option > menuArray.Length || option < 0) //checking for the range of num
        {
            Console.WriteLine("Please enter a number in range");
            continue;
        }
    }
    catch (FormatException ex) //for input that cant be converted to int
    {
        Console.WriteLine(ex.Message);
        continue;
    }
    return option; //this automatically breaks the loop
    }
}

/*This method reads from the "customer.csv" file and creates a Dictionary to store their Info, and returns the Dictionary
 * MemberID will be the key*/ 
Dictionary<int, Customer> InitCustomer()
{
    Dictionary<int, Customer> customerDict = new Dictionary<int, Customer>(); //Create new Dict
    using (StreamReader sr = new StreamReader("customers.csv"))
    {
        string? s = sr.ReadLine(); //header skip
        while ((s = sr.ReadLine()) != null)
        {
            string[] data = s.Split(",");
            int memberID = Convert.ToInt32(data[1]);
            DateTime DOB = Convert.ToDateTime(data[2]);
            int mPoint = Convert.ToInt32(data[4]);
            int punchPoint = Convert.ToInt32(data[5]);
            Customer c1 = new Customer(data[0], memberID, DOB);
            c1.Rewards = new PointCard(mPoint, punchPoint);
            c1.Rewards.Tier = data[3];
            customerDict.Add(memberID, c1);
        }
        return customerDict;
    }
}

/*This method reads from "option.csv" and stores the information in a List, to be used for checking all possible options that a user can input*/
List<IceCream> InitOptionList()
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
                else if (optionData == opt && i == 1 ) 
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

int InitOrder(Queue<Order> RegularQueue, Queue<Order> GoldQueue, Dictionary<int, Customer> customerDict, List<Order> mainOrderList, List<Order> fulfilledOrders)
{
    int prevOrderId = -1;
    int largestId = 0; //dummy value to store
    string[] DataArray = File.ReadAllLines("orders.csv");
    for (int i = 1; i < DataArray.Length; i++)
    {
        List<IceCream> icecreamList = new List<IceCream>(); // Create new ice cream list
        bool premium = false;
        string[] data = DataArray[i].Split(",");
        int id = Convert.ToInt32(data[0]);

        //Check if current id is more than stored id
        if (id > largestId)
        {
            largestId = id;
        }

        int memberID = Convert.ToInt32(data[1]);

        DateTime timeReceived = Convert.ToDateTime(data[2]);
        DateTime timefulfilled = Convert.ToDateTime(data[3]);
        string option = data[4].ToLower();
        int scoops = Convert.ToInt32(data[5]);
        List<Flavour> flavourlist = new List<Flavour>();
        List<Topping> toppingList = new List<Topping>();


        // Populate flavour list
        for (int j = 8; j <= 10; j++)
        {
            string flavourData = data[j].ToLower();
            if (!string.IsNullOrEmpty(flavourData))
            {
                int quantity = 1;
                if (flavourData == "durian" || flavourData == "ube" || flavourData == "sea salt")
                {
                    premium = true;
                }
                Flavour newFlavour = new Flavour(flavourData, premium, quantity);
                flavourlist.Add(newFlavour);
            }
        }

        // Populate topping list
        for (int j = 11; j <= 14; j++)
        {
            string toppingData = data[j].ToLower();
            if (!string.IsNullOrEmpty(toppingData))
            {
                Topping newTopping = new Topping(toppingData);
                toppingList.Add(newTopping);
            }
        }
        if (id != prevOrderId)
        {
            Order newOrder = null;
            newOrder = new Order(id, timeReceived);
            if (option == "waffle")
            {
                string waffleFlavour = data[7].ToLower() ;
                Waffle waffle = new Waffle(option, scoops, flavourlist, toppingList, waffleFlavour);
                newOrder.iceCreamList.Add(waffle);
                //mainOrderList.Add(newOrder);
            }
            else if (option == "cone")
            {
                bool isDipped = Convert.ToBoolean(data[6]);
                Cone cone = new Cone(option, scoops, flavourlist, toppingList, isDipped);
                newOrder.iceCreamList.Add(cone);
                // mainOrderList.Add(newOrder);
            }
            else
            {
                Cup cup = new Cup(option, scoops, flavourlist, toppingList);
                newOrder.iceCreamList.Add(cup);
                //mainOrderList.Add(newOrder);
            }
            newOrder.TimeFulfilled = timefulfilled;
            prevOrderId = id; //for use ltr
            mainOrderList.Add(newOrder);

            foreach (KeyValuePair<int, Customer> kvp in customerDict)
            {
                if (memberID == kvp.Key)
                {
                    if (newOrder.TimeFulfilled == null)
                    {
                        kvp.Value.currentOrder = newOrder;
                        if (kvp.Value.Rewards.Tier == "Gold")
                        {
                            GoldQueue.Enqueue(newOrder);
                        }
                        else
                        {
                            RegularQueue.Enqueue(newOrder);
                        }
                    }
                    else
                    {
                        kvp.Value.orderHistory.Add(newOrder);
                        fulfilledOrders.Add(newOrder);
                    }
                }

            }
        }
        else //already exist the order id
        {
            foreach (KeyValuePair<int, Customer> kvp in customerDict)
            {
                if (memberID == kvp.Key)
                {
                    foreach (Order order in kvp.Value.orderHistory)
                    {
                        if (order.Id == id)
                        {
                            if (option == "waffle")
                            {
                                string waffleFlavour = data[7].ToLower();
                                Waffle waffle = new Waffle(option, scoops, flavourlist, toppingList, waffleFlavour);
                                order.iceCreamList.Add(waffle);
                                //mainOrderList.Add(newOrder);
                            }
                            else if (option == "cone")
                            {
                                bool isDipped = Convert.ToBoolean(data[6]);
                                Cone cone = new Cone(option, scoops, flavourlist, toppingList, isDipped);
                                order.iceCreamList.Add(cone);
                                // mainOrderList.Add(newOrder);
                            }
                            else
                            {
                                Cup cup = new Cup(option, scoops, flavourlist, toppingList);
                                order.iceCreamList.Add(cup);
                                //mainOrderList.Add(newOrder);
                            }
                        }
                    }
                }
            }

        }

    }
    return largestId;
}

/*This method reads from "flavour.csv" creates Flavour Dictionary to store the Flavours available, with the flavour name the key and
 * setting quanttiy to default 0 and returns the Dictionary*/
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

//This method reads from "topping.csv" and creates Toppings Dictionary to store the Toppings available(Flavour name as key), and returns Dicitonary.
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



//This method list all the customer information from the customerDict, Option 1
void ListAllCustomer(Dictionary<int, Customer> cList)
{
    Console.WriteLine($"{"Name",-15}{"MemberID",-15}{"DOB",-15}{"Membership Tier",-20}{"Memebership Pts",-20}{"Punchcard Pts",-15}");
    foreach(Customer c in cList.Values)
    {
        Console.WriteLine(c.ToString());
    }
    Console.WriteLine(); //skip line
}



void DisplayQueues(Queue<Order> GoldQueue, Queue<Order> RegularQueue)
{
    Console.WriteLine("-------------------------------\nGold Queue\n-------------------------------");
    foreach (Order order in GoldQueue)
    {
        Console.WriteLine($"Order Id: {order.Id}\tTime Received: {order.TimeReceived}");
    }
    Console.WriteLine("\n-------------------------------\nOrdinary Queue\n-------------------------------");
    foreach (Order order in RegularQueue)
    {
        Console.WriteLine($"Order Id: {order.Id}\tTime Received: {order.TimeReceived}");
    }
}

//This method registers a new customer via prompt, which then append to customer.csv and as well as to customerDict, Option 3
void RegisterNewCustomer(Dictionary<int, Customer> cList)
{
    while (true)
    {
        //Initialisation of data for it to access in this method
        string name;
        int id;
        string tempID;
        DateTime dob;
        Customer cus1;
        try
        {
            Console.WriteLine("\nREGISTER NEW CUSTOMER, TYPE 0 AT NAME TO EXIT");
            Console.Write("\nEnter your name: ");
            name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("No blanks allowed");
            }
            int intValue;
            if (name.Any(char.IsDigit)) //checks if has at least one character that contains 0-9 
            {
                if (name == "0") { break; } //end this method 
                throw new Exception($"The string contains an integer");
            }
            Console.Write("Enter your MemberID (6 digit integer): ");
            tempID = Console.ReadLine();
            if (tempID.Length == 6) //temp a string to check for length, which is 6 
            {
                id = Convert.ToInt32(tempID);
                if (!cList.ContainsKey(id)) //does not contain inside Dict, so is valid
                {
                    Console.Write("Enter your Date of Birth in this format (yyyy/MM/dd): ");
                    dob = Convert.ToDateTime(Console.ReadLine());
                    cus1 = new Customer(name, id, dob);
                    cus1.Rewards = new PointCard(0, 0);
                    cus1.Rewards.Tier = "Ordinary";
                }
                else
                {
                    throw new Exception("No duplicate User, MemberID already exists in our system, try a different one");
                }
            }
            else
            {
                throw new Exception("Incorrect input of MemberID, must be 6 Digit Integer");
            }
        }
        catch (FormatException ex) //any invalid input not specified will result in code to come here
        {
            Console.WriteLine(ex.Message);
            continue;
        }
        catch (Exception ex) //all general and edited errors are handled here
        {
            Console.WriteLine(ex.Message);
            continue;
        }
        //Code proceeds after validation above
        cList.Add(id, cus1); //add to dictionary
        Console.WriteLine("Registration completed!");
        break;
    }
}

//This method registers a new customer's order via prompt, which then creates order object and append to customer order, Option 4
int CreateCustomerOrder(Dictionary<int, Customer> cList, List<IceCream> icecreamOption, Dictionary<string, Flavour> fList, Dictionary<string, Topping> tList, Queue<Order>normalQ, Queue<Order> goldQ, int lastId, List<Order> mainOrderList)
{
    while (true)
    {
        //Initialisation of data for it to access in this method
        try
        {
            int id = 0; //dummy value
            Console.WriteLine("\nCREATE NEW CUSTOMER'S ORDER, TYPE 0 AT MEMBERID TO EXIT");
            Console.Write("\nEnter MemberID to continue: ");
            string tempID = Console.ReadLine();
            if (tempID == "0") { break; } //end this method 

            if (tempID.Length == 6) //temp a string to check for length, which is 6 
            {
                try
                {
                    id = Convert.ToInt32(tempID);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Must be integer.");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                //rest of the program
                if (cList.ContainsKey(id)) //contain inside Dict, so is valid
                {
                    Customer cus = cList[id]; //get its dictionary value, which is the customer object that we are dealing with

                    if (cus.currentOrder != null) //check for relationship of customer CurrentOrder, only 0..1 r/s
                    {
                        throw new Exception("You have a existing order. Please wait for it to dequeue.");
                    }
                    IceCream icecreamOrder = TakingOrders(icecreamOption, fList, tList); //method for creating ice cream process
                    Order newOrder = cus.MakeOrder(); //creation of new order tailored to the customer
                    newOrder.AddIceCream(icecreamOrder); 

                    while (true) //this loops checks for continue of order
                    {
                        Console.WriteLine("\n[Y] Would you like to add on more Ice Cream? \n[N] No, Next Step");
                        Console.Write("Enter Option: ");
                        string response = Console.ReadLine().ToLower();
                        if (response != "n")
                        {
                            if (response == "y")
                            {
                                IceCream icecreamOrder2 = TakingOrders(icecreamOption, fList, tList); //continues creating making of icecream
                                newOrder.AddIceCream(icecreamOrder2);
                            }
                            else { Console.WriteLine("Incorrect input, only Y or N");  }
                        }
                        else { break; }
                    }
                    
                    //C# hold references to objects, any changes made to the Customer object via the reference obtained from the dictionary will directly affect the object. 
                    string qType; //for use in displaying queue type
                    lastId++;//increment + 1
                    //Initialisation of attributes
                    newOrder.Id = lastId;
                    newOrder.TimeFulfilled = null;
                    cus.currentOrder = newOrder; //Set to current order of object
                    if (cus.Rewards.Tier == "Gold")
                    {
                        qType = "Gold";
                        goldQ.Enqueue(newOrder);
                    }
                    else
                    {
                        qType = "Normal";
                        normalQ.Enqueue(newOrder);
                    }
                    //Console.WriteLine(newOrder.ToString())Test Output
                    Console.WriteLine($"\nOrder Successful. Your OrderID is {newOrder.Id} in the {qType} Queue");
                    mainOrderList.Add(newOrder); //Main order list records   
                    break; //end the method
                }
                else //if it is not a key in the CustomerDict, invalid input
                {
                    throw new Exception("MemberID does not exist, try again. If new user, please proceed to [3] to register");
                }
            }
            else
            {
                throw new Exception("Incorrect input of MemberID, must be 6 Digit Integer");
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
    return lastId;
}
//This method is a sub method for Option 4 CreateOrders, which gets input to create Ice cream object
IceCream TakingOrders(List<IceCream> icecreamOption, Dictionary<string, Flavour>fList, Dictionary<string, Topping> tList)
{
    //Initialisation of data
    int tempScoop =0; //value
    bool IsoptionDone = false;
    bool IsscoopDone = false;
    bool isDipped = false;
    bool IsWaffleFDone = false;
    IceCream iceCream = null; 
    while (true)
    {
        try
        {
            Console.Write("Enter Ice Cream Option: ");
            string icOption = Console.ReadLine().ToLower();
            Console.Write("Enter Number of Scoops: ");
            int numScoops = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < icecreamOption.Count; i++)
            {
                iceCream = icecreamOption[i];
                if (iceCream.Option == icOption) //check if matches any
                {
                    IsoptionDone = true;
                    if (numScoops == iceCream.Scoops)
                    {
                        IsscoopDone = true;
                        tempScoop = iceCream.Scoops; //temp variable to store and pass in at GetFlavour()
                        break; // avoid looping once found for waste of memory
                    }
                    else { continue; } //continue looking for others
                }
            }
            if (!IsoptionDone)
            {
                throw new Exception("Invalid option, only Cone, Waffle and Cup.");
            }
            if (!IsscoopDone)
            {
                throw new Exception("Invalid scoops, choose between 1-3 scoops");
            }
            iceCream = GetFlavour(tempScoop, iceCream, fList); //mandatory for 1 flavours
            iceCream = GetTopping(iceCream, tList); 
            if (iceCream is Cone)
            {
                Cone cone1 = (Cone)iceCream;
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
                return cone1;
            }
            else if (iceCream is Waffle)
            {
                Waffle w1 = (Waffle)iceCream; //downcast to access its subclass property
                while (true)
                {
                    Console.Write("\nSelect Waffle Flavour [Original, Red Velvet, Charcoal, Pandan]: ");
                    string tempWaffleF = Console.ReadLine().ToLower();
                    for (int i = 0; i < icecreamOption.Count; i++) //This loop checks from the option list since we're required to use the data file;
                    {
                        IceCream tempIceCream = icecreamOption[i];
                        if (tempIceCream.Option == w1.Option)
                        {
                            Waffle waffle = (Waffle)tempIceCream; //downcast to access sub attributes
                            if (waffle.WaffleFlavour == tempWaffleF)
                            {
                                w1.WaffleFlavour = tempWaffleF; //edit info in the iceCream object
                                IsWaffleFDone = true;
                                return w1; //loop will end when returned
                            }
                        }
                    }
                    if (!IsWaffleFDone)
                    {
                        Console.WriteLine("Invalid waffle flavour, try again");
                    }
                }
            }
            return iceCream; //cup has no other adds on
        }
        catch (FormatException ex) 
        {
            Console.WriteLine("\n" + ex.Message);
        }
        catch (Exception ex) //all new exceptions thrown will be gone here.
        {
            Console.WriteLine("\n" + ex.Message);
            continue;
        }
    }
}

//This methods gets the icecream and adds flavour, and return corresponding edited Ice cream back, sub method of Option 4
IceCream GetFlavour(int numScoops, IceCream iceCream, Dictionary<string, Flavour> fList)
{
    int flavourCount = 1; //first loop is counted as one time when first run, hence set to 1
    while (flavourCount <= numScoops) //tally with corresponding scoops, counter must be less than or equal to the num of scoops
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

//This methods gets the icecream and adds topping, and return corresponding edited Ice cream back, sub method of Option 4
IceCream GetTopping(IceCream iceCream, Dictionary<string, Topping> tList)
{
    int toppingCount = 1; //first loop is counted as one time when first run, hence set to 1
    while (toppingCount <= tList.Count)
    {
        Console.WriteLine("\nAvailable Toppings: Oreos, Sprinkles, Mochi and Sago");
        Console.Write("\nEnter Topping ([n] to exit): ");
        string topping = Console.ReadLine().ToLower();
        if (topping == "n")
        {
            return iceCream; //no modification
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

//This method gets the custDict and the memberid to display the correct order, Option 5
List<Order> DisplayCustomerOrder(Dictionary<int, Customer> cList, int id)
{
    bool isOrderExist = false;
    if (cList.ContainsKey(id)) //contain inside Dict, so is valid
    {
        Customer cus = cList[id]; //get its dictionary value, which is the customer object\
        if (cus.currentOrder != null)
        {
            Console.WriteLine("\n--------------Current Order--------------");
            Console.WriteLine(cus.currentOrder.ToString());
            isOrderExist = true;
        }
        if (cus.orderHistory.Count >0)
        {
            Console.WriteLine("\n--------------Past Order--------------");
            foreach (Order order in cus.orderHistory)
            {
                Console.WriteLine(order.ToString());
            }
            isOrderExist = true;
        }
        if (!isOrderExist)
        {
            Console.WriteLine("Customer has no orders");
        }
        return cus.orderHistory;
    }
    else
    {
        return null;
    }
}

//Returns selected customer object in option 6
Customer GetSelectedCustomerObject(Dictionary<int, Customer> cList, int id)
{
    if (cList.ContainsKey(id)) //contain inside Dict, so is valid
    {
        Customer cus = cList[id];
        return cus;
    }
    return null;
}

//Option 6 method
void ModifyingCurrentOrder(Dictionary<int, Customer> customerDict, Dictionary<string, Flavour> flavourDict, Dictionary<string, Topping> toppingDict, List<IceCream> icecreamOption)
{
    //List all available customer to select
    ListAllCustomer(customerDict);
    while (true)
    {
        try
        {
            Console.Write("\nEnter MemberID to continue(Input integer pls): ");
            string tempID = Console.ReadLine();
            if (tempID.Length == 6)
            {
                int ID = 0;
                try
                {
                    ID = Convert.ToInt32(tempID);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Must be integer.");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;

                }
                if (customerDict.ContainsKey(ID)) //contain inside Dict, so is valid
                {
                    while (true)
                    {
                        int option = 0;
                        //Get customer class instance of customer use selected
                        Customer selectedCustomer = GetSelectedCustomerObject(customerDict, ID);
                        if (selectedCustomer.currentOrder == null)
                        {
                            Console.WriteLine("Customer you selected has no current orders");
                            break;
                        }
                        Console.WriteLine("\nCurrent order: \n" + selectedCustomer.currentOrder);
                        Console.WriteLine("\nCurrent order: \n" + selectedCustomer.currentOrder);
                        Console.WriteLine(); //skip line
                        Console.WriteLine("---------------MENU---------------");//Lists menu
                                                                                //store the menu options for referencing in a list
                        string[] menuArray = { "Modify existing ice cream", "Add ice cream to current order", "Delete ice cream from current order" };
                        for (int i = 0; i < menuArray.Length; i++)
                        {
                            Console.WriteLine($"[{i + 1}] {menuArray[i]}");
                        }
                        Console.WriteLine("[0] Exit");
                        try
                        {
                            Console.Write("\nEnter option: ");
                            option = Convert.ToInt32(Console.ReadLine());
                            if (option == 0)
                            {
                                break;
                            }
                            else if (option == 1)
                            {
                                for (int i = 0; i < selectedCustomer.currentOrder.iceCreamList.Count; i++)
                                {
                                    Console.WriteLine("\nIce cream index: " + (i + 1));
                                    Console.WriteLine(selectedCustomer.currentOrder.iceCreamList[i].ToString());
                                }
                                Console.Write("Choose the index of the ice cream you want to modify(input number pls): ");
                                int iceCreamIndex = Convert.ToInt32(Console.ReadLine());
                                iceCreamIndex = iceCreamIndex - 1;
                                selectedCustomer.currentOrder.ModifyIceCream(iceCreamIndex);
                            }
                            if (option > menuArray.Length || option < 0) //checking for the range of num
                            {
                                Console.WriteLine("Please enter a number in range");
                                continue;
                            }
                            else if (option == 2)
                            {
                                //Gives user option to make new ice cream and add it to currentorder icecreamList
                                AddIceCream(selectedCustomer, flavourDict, toppingDict, icecreamOption);
                                break;
                            }
                            else if (option == 3)
                            {
                                //Deletes ice cream of user choice will not delete if only 1 icecream inside 
                                DeleteIceCream(selectedCustomer);
                                break;
                            }

                        }


                        catch (FormatException ex) //for input that cant be converted to int
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                        break;
                    }
                }
                else
                {
                    throw new Exception("MemberID does not exist, try again. If new user, please proceed to [3] to register");
                }
            }
            else
            {
                throw new Exception("MemberID must be 6 digit integer");

            }
            break;
        }
        catch (FormatException ex) //for input that cant be converted to int
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

void AddIceCream(Customer customer, Dictionary<string, Flavour> fList, Dictionary<string, Topping> tList, List<IceCream> icecreamOption)
{
    Console.WriteLine("Current Ice Cream in order:\n-----------------\n");
    foreach (IceCream icecream in customer.currentOrder.iceCreamList)
    {
        Console.WriteLine(icecream.ToString());
    }
    IceCream icecreamOrder = TakingOrders(icecreamOption, fList, tList);
    customer.currentOrder.AddIceCream(icecreamOrder);
}

//Deletes icecream from currentOrder icecream list for option 6
void DeleteIceCream(Customer customer)
{
    while (true)
    {
        if (customer.currentOrder.iceCreamList.Count > 1)
        {
            Console.WriteLine("Current Ice Cream in order:\n-----------------");
            for (int i = 0; i < customer.currentOrder.iceCreamList.Count; i++)
            {
                Console.WriteLine("\nIce cream index: " + Convert.ToString(i + 1));
                Console.WriteLine(customer.currentOrder.iceCreamList[i].ToString());
            }
            try
            {
                Console.Write("\nChoose the index of the ice cream you want to delete(input number pls): ");
                int iceCreamIndex = Convert.ToInt32(Console.ReadLine());
                if (iceCreamIndex == 0 || iceCreamIndex > customer.currentOrder.iceCreamList.Count)
                {
                    Console.WriteLine("Index not in ice cream list");
                    continue;
                }
                customer.currentOrder.DeleteIceCream(iceCreamIndex);
                Console.WriteLine("Current Ice Cream in order:\n-----------------");
                foreach (IceCream icecream in customer.currentOrder.iceCreamList)
                {
                    Console.WriteLine(icecream.ToString());
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
        else
        {
            Console.WriteLine("Unable to delete. Cannot have zero icecream in a order");
            break;
        }
    }

}

void FillFinalPriceCSVData(Dictionary<int, double> finalPriceDictionary)
{
    int OrderID = 0;
    double finalPrice = 0;
    string header2 = "Id,FinalPrice\n";
    File.WriteAllText("finalprice.csv", header2); //This Write 'erases' everything from the exisiting file, restarting
    foreach (KeyValuePair<int, double> kvp in finalPriceDictionary)
    {
        OrderID = kvp.Key;
        finalPrice = kvp.Value;
        string info = Convert.ToString(kvp.Key + "," + Convert.ToString(kvp.Value) + "\n");
        File.AppendAllText("finalprice.csv", info);
    }
}

// Advanced B, display charges
Dictionary<string, double> DisplayMonthlyCharges(List<Order> fulfilledOrder, Dictionary<int, double> fpDict, int year)
{
    string[] Months = { "Buffer", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
Dictionary<string, double> MonthlyCharges = new Dictionary<string, double>();
double amount = 0;
foreach (Order order in fulfilledOrder)
{
    if (order.TimeFulfilled?.Year == year) //wont be null, since we already checked using fulfilled list
    {
        for (int i = 1; i < 13; i++)
        {
            if (order.TimeFulfilled?.Month == i)
            {
                double orderID = 0;
                double value = 0;
                foreach (KeyValuePair<int, double> kvp in fpDict)
                {
                    orderID = kvp.Key;
                    value = kvp.Value;
                    if (order.Id == orderID)
                    {
                        amount += value;
                        MonthlyCharges[Months[i]] = amount;
                    }
                }
            }
            else
            {
                MonthlyCharges[Months[i]] = 0;
            }
        }
    }
}
return MonthlyCharges;
}

//This method UPDATES THE Customer CSV FILE, before closing the application when 0 is click, to keep the information updated
void UpdateCustomerCSVData(Dictionary<int, Customer> cList)
{
    //Start off with CustomerDict, 'rewriting' into customer.csv
    string header = "Name,MemberId,DOB,MembershipStatus,MembershipPoints,PunchCard\n";
    File.WriteAllText("customers.csv", header); //This Write 'erases' everything from the exisiting file, restarting
    foreach (KeyValuePair<int, Customer> kvp in cList) //loop through whole list
    {
        Customer currentCus = kvp.Value;
        string data = currentCus.Name + "," + currentCus.MemberId.ToString() + "," + currentCus.DOB.ToString("dd/MM/yyyy") + "," + currentCus.Rewards.Tier + "," + currentCus.Rewards.Points + "," + currentCus.Rewards.PunchCard + "\n"; //append as string with comma
        File.AppendAllText("customers.csv", data); //no longer need to rewrite, to append
    }
}

//This method updates ordercsv by rewritting it and adding updated orders to order.csv, ONLY FOR FULFILLED ORDERS.
void UpdateOrderCSV(List<Order> fulfilledOrderList, Dictionary<int, Customer> customerDictionary)
{
    string file = "orders.csv";
    using StreamWriter writer = new StreamWriter(file, false);
    {
        writer.WriteLine("Id,MemberId,TimeReceived,TimeFulfilled,Option,Scoops,Dipped,WaffleFlavour,Flavour1,Flavour2,Flavour3,Topping1,Topping2,Topping3,Topping4");
        foreach (Order order in fulfilledOrderList)
        {
            string memberID = "";
            bool IsFound = false;
            string TimeFulfilled = "";
            string TimeReceived = "";
            foreach (Customer customer in customerDictionary.Values)
            {
                foreach (Order customerOrder in customer.orderHistory)
                {
                    if (customerOrder.Id == order.Id)
                    {
                        memberID = Convert.ToString(customer.MemberId);
                        IsFound = true;
                        break;
                    }
                }
                if (IsFound)
                {
                    break;
                }
            }
            if (!string.IsNullOrEmpty(Convert.ToString(order.TimeFulfilled)))//check if time fulfilled is null and makes it into a string if not
            {
                TimeFulfilled = order.TimeFulfilled?.ToString();
            }
            TimeReceived = order.TimeReceived.ToString();
            foreach (IceCream icecream in order.iceCreamList)
            {
                string Flavour = "";
                string Topping = "";
                string WaffleFlavour = "";
                string IsDipped = "";
                List<string> flavourList = new List<string>();
                foreach (Flavour flavour in icecream.Flavours)
                {
                    flavourList.Add(flavour.Type);
                }
                if (flavourList.Count <= 3)//checks if final flavourlist is less than 3
                {
                    while (flavourList.Count < 3)//adds blanks to make length equal to 3
                    {
                        flavourList.Add("");

                    }
                }
                List<string> toppingList = new List<string>();
                foreach (Topping topping in icecream.Toppings)
                {
                    toppingList.Add(topping.Type);
                }
                if (toppingList.Count <= 4)//check if final toppinglist is less than 4
                {
                    while (toppingList.Count < 4)//adds blanks until length is equal to 4
                    {
                        toppingList.Add("");
                    }
                }
                if (icecream is Waffle)//gets waffleflavour and assign value to string
                {
                    Waffle waffle = (Waffle)icecream;
                    WaffleFlavour = waffle.WaffleFlavour;
                }
                else if (icecream is Cone)//assign boolean of dipped to string
                {
                    Cone cone = (Cone)icecream;
                    if (cone.Dipped)
                    {
                        IsDipped = "True";
                    }
                    else
                    {
                        IsDipped = "False";
                    }
                }
                Flavour = String.Join(",", flavourList);//make entire flavourlist into a string
                Topping = String.Join(",", toppingList);//makes entire toppinglist into a string
                writer.Write("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\n", order.Id, memberID, TimeReceived, TimeFulfilled, icecream.Option, icecream.Scoops, IsDipped, WaffleFlavour, Flavour, Topping);
            }
        }
    }
}

//This method Process an order and checks it out, from the queue status , ADVANCED PART A
void ProcessAndCheckOut(Queue<Order> queue, Dictionary<int, Customer> cList, List<Order> mainOrderList, Dictionary<int, double> fpDict, List<Order> fulfilledOrders)
{
    //initialisation of data, dummy object and valu
    Customer settleCustomer = new Customer();
    Order orderToSettle = new Order();
    bool isOrderFound = false;
    string tier = "";
    int points = 0;
    //Real data
    int completePunch = 10;
    double pointsOffsetRate = 0.02;
    try
    {
        orderToSettle = queue.Dequeue(); //Gets the first order to work on
    }
    catch (Exception e) //queue empty/unable to dequeue
    {
        Console.WriteLine(e.Message);
        return; //end
    }
    Console.WriteLine($"Ice Cream Orders: {orderToSettle.ToString()}"); //list the icecream in the order object with tostring
    double totalcost = orderToSettle.CalculateTotal();
    Console.WriteLine($"\nTotal Bill: ${totalcost:0.00}");

    //Below checks for which customer the order belongs to
    foreach (Customer cus in cList.Values)
    {
        //test if order made by who, NOT NULL ONLY
        if (cus.currentOrder != null && cus.currentOrder.Id == orderToSettle.Id) //similar id num, check if curren order is not null first
        {
            settleCustomer = cus; //assign to this customer class for reference****
            int foundID = settleCustomer.MemberId; //get member id
            tier = settleCustomer.Rewards.Tier;
            points = settleCustomer.Rewards.Points;
            Console.WriteLine($"MemberID: {foundID}\nMembership Status: {tier}\nPoints: {points}");
            isOrderFound = true;
            break;
        }
    }
    if (!isOrderFound) //bool value to check for customer found
    {
        Console.WriteLine($"Cant find the corresponding customer for this order, Reference number is {orderToSettle.Id}");
        return; //break 
    }
    //Check for its birthday, get cost
    totalcost = DetermineCostAfterBday(orderToSettle, settleCustomer, totalcost);

    //Checks for completion of punchcard first, in case they have 10 from prev order
    if (settleCustomer.Rewards.PunchCard == completePunch)
    {
        settleCustomer.Rewards.Punch(); //this method will check if its 10 at the start and reset
        double deductfree = orderToSettle.iceCreamList[0].CalculatePrice(); //first ice cream in the list ordered
        try //testing if deducting after bday will clash w negative balance
        {
            totalcost = totalcost - deductfree; //minus the fee, eg if total cost($18) first is $14(most ex but bday), then left 4-14 <0
            if (totalcost >= 0)  //no problem w negative balance
            {
                Console.WriteLine("\nYour 1st Ice Cream is free.");
            }
            else if (totalcost < 0 && orderToSettle.iceCreamList.Count == 1) //In the case it is their birthday, and punchpoint = 10, should negative be balance
            {
                throw new Exception("Negative balance, Puunch points dont need to be used");
            }
            else if (totalcost < 0 && orderToSettle.iceCreamList.Count > 1)//have more than 1 ice cream inside, next ice cream is used
            {
                totalcost += deductfree; //add back cost
                Console.WriteLine("Deducting the next valid ice cream as first ice cream is already discounted");
                for (int i = 1; i < orderToSettle.iceCreamList.Count; i++)
                {
                    totalcost = totalcost - orderToSettle.iceCreamList[i].CalculatePrice();
                    if (totalcost>= 0)
                    {
                        break;
                    }
                    else //still negative
                    {
                        totalcost = totalcost + orderToSettle.iceCreamList[i].CalculatePrice(); //add back
                        continue;
                    }
                }
            }
            Console.WriteLine($"New Price: ${totalcost:0.00}"); //prints after confirmation it can be redeem.
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            settleCustomer.Rewards.PunchCard = 10; //CHANGE BACK TO 10
            totalcost += deductfree; //add back cost
        }
    }
    //reset & increment punch card below
    foreach (IceCream iceCream in orderToSettle.iceCreamList)
    {
        settleCustomer.Rewards.Punch(); //increment by 1, and check if reach 10 then stop
        if (settleCustomer.Rewards.PunchCard == completePunch) //reaches 10 by incrementing, then fix to 10 and break to prevent going in the method again and get resetted
        {
            break;
        }
    }

    //Redeeming of points, for selected GOLD OR SILVER
    if (tier == "Gold" || tier == "Silver")
    {
        while (true) //loop to check for correct input of points
        {
            if (totalcost > 0) 
            {
                Console.Write("\nWould you like to redeem your points? (y/n): ");
                string response = Console.ReadLine().ToLower();
                if (response != "n") //user didnt select n, check if its yes
                {
                    if (response == "y")
                    {
                        try
                        {
                            Console.Write($"Select amount of points to deduct : ");
                            int choosenPts = Convert.ToInt32(Console.ReadLine());
                            settleCustomer.Rewards.RedeemPoints(choosenPts);
                            //Deduction of totalcost further
                            totalcost -= (choosenPts * pointsOffsetRate); //deduction
                            if (totalcost < 0)
                            {
                                Console.WriteLine("Redeem amount worth is more than total price needed to paid!");
                                settleCustomer.Rewards.AddPoints(choosenPts); //give back the pts
                                totalcost += (choosenPts * pointsOffsetRate); //set back the total cost
                                int maxAmt = Convert.ToInt32(Math.Floor(totalcost / pointsOffsetRate));
                                Console.WriteLine($"Only a amount of up to {maxAmt} Pts can be used.");
                                continue;
                            }
                            Console.WriteLine($"Redeem Successful! Remaining Balance: {settleCustomer.Rewards.Points} ");
                            break;
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid option, (y/n) only");
                    }
                }
                else { break; } //user clicks 'no'
            }
            else { break; } //total cost <0, dont need to redeem pts
        }
    }
    Console.WriteLine($"\nFinal Nett Price: ${totalcost:0.00}"); //display of final charge
    Console.Write("\n----PRESS ANY KEY TO MAKE PAYMENT----");
    Console.ReadKey(); //To read in the random key, useless to store

    if (settleCustomer.Rewards.PunchCard == completePunch) //reaches the amt needed, prompt to let them know
    {
        Console.WriteLine("You have reached 10 PunchPts! Next first ice cream order is free.");
    }
    //Earn points!
    double rate = 0.72;
    int pointsEarn = Convert.ToInt32(Math.Floor(totalcost * rate));
    settleCustomer.Rewards.AddPoints(pointsEarn);

    //  Check for upgrade of Membership, only for members not yet in silver and gold for the first time
    points = settleCustomer.Rewards.Points; 
    if (points >= 100) //checks for 100 first, if user has used all his pts already but is a tier of gold, it will fulfuill first condition and not drop
    {
        settleCustomer.Rewards.Tier = "Gold";
    }
    else if (points >= 50)
    {
        settleCustomer.Rewards.Tier = "Silver";
    }

    //Marking DateTime for fulfilled in OrderHist, and also updating in the Main Order List
    orderToSettle.TimeFulfilled = DateTime.Now; //orderToSettle and settleCustomer contains the same object kind

    for (int i = 0; i < mainOrderList.Count; i++)
    {
        Order order = mainOrderList[i];
        if (order.Id == settleCustomer.currentOrder.Id) //finding the corresponding flavour to mark done
        {
            order.TimeFulfilled = DateTime.Now;
        }
    }
    settleCustomer.orderHistory.Add(orderToSettle); //add currentOrder to ORDER HIST;
    fpDict.Add(orderToSettle.Id, totalcost);
    settleCustomer.currentOrder = null; //empty now for customer to continue ordering
    fulfilledOrders.Add(orderToSettle);
    //Order done
    Console.WriteLine("\nOrder Completed!");

}

//Sub method for option 7/8, to determine cost after bday and return the total cost
double DetermineCostAfterBday(Order orderToSettle, Customer settleCustomer, double totalcost)
{
    if (settleCustomer.IsBirthday()) //true or fasle methods
    {
        double price = 0; //current price
        double mostexpensive = 0;
        foreach (IceCream ic in orderToSettle.iceCreamList) //check which icecream is free
        {
            price = ic.CalculatePrice(); //set price to current icecream
            if (mostexpensive > price) //if most ex is more, no changes needed
            {
                continue;
            }
            else
            {
                mostexpensive = price; //update most expensive price
            }
        }
        totalcost = orderToSettle.CalculateTotal() - mostexpensive;
        return totalcost;
    }
    return totalcost; //No change
}

//This intitialise the order and cost that were successfully fulfilled, for advanced B when subsequently run
Dictionary<int, double> InitFinalPrice()
{
    Dictionary<int, double> finalPriceDictionary = new Dictionary<int, double>();
    using (StreamReader sr = new StreamReader("finalprice.csv"))
    {
        string? s = sr.ReadLine(); //header skip
        while ((s = sr.ReadLine()) != null)
        {
            string[] data = s.Split(",");
            int orderid = Convert.ToInt32(data[0]);
            double totalcost = Convert.ToDouble(data[1]);
            finalPriceDictionary.Add(orderid, totalcost);
        }
    }
    return finalPriceDictionary;
}

//fills it up with the id and cost at the start of system
Dictionary<int, double> FillFinalPriceDict(List<Order> fulfilledOrders)
{
    string fileContent = File.ReadAllText("finalprice.csv");
    if (string.IsNullOrWhiteSpace(fileContent))//if file empty take data from order.csv and append data to finalprice dictionary, only for the first time running
    {
        Dictionary<int, double> finalPriceDictionary = new Dictionary<int, double>();
        foreach (Order order1 in fulfilledOrders)
        {
            double price = 0;
            foreach (IceCream icecream in order1.iceCreamList)
            {
                price += icecream.CalculatePrice();
            }
            finalPriceDictionary.Add(order1.Id, price);
        }
        return finalPriceDictionary;
    }
    else//if file not empty , intialise it and store data into finalprice dictionary
    {
        Dictionary<int, double> finalPriceDictionary = InitFinalPrice();
        return finalPriceDictionary;
    }
}

/******************Start of program*********************/

//Call to initialise CustomerDict for reference as collection
Dictionary<int, Customer> customerDict = InitCustomer();

//Call to initialise OptionList for reference as collection
List<IceCream> icecreamOption = InitOptionList();

//Call to initialise FlavourDict for reference as collection
Dictionary<string, Flavour> flavourDict = InitFlavourDict();

//Call to initialise ToppingDict for reference as collection
Dictionary<string, Topping> toppingDict = InitToppingDict();

//Creation of Order Queue
Queue<Order> normalQueue = new Queue<Order>();
Queue<Order> goldQueue = new Queue<Order>();

//Makes main orderList that stores every order both not fulfilled or  fulfilled
List<Order> orderList = new List<Order>();

//Makes list for only fulfilled orders for option 8
List<Order> fulfilledOrders = new List<Order>();

//Call to initialise data from "orders.csv", and gets the latest order id to be used for creating data 
int latestId = InitOrder(normalQueue, goldQueue, customerDict, orderList, fulfilledOrders);

//Makes final price dictionary to store orders and its respective cost
Dictionary<int, double> finalPriceDictionary = FillFinalPriceDict(fulfilledOrders);

while (true)
{
    int option = DisplayMenu(); //call the menu, and get back the int option
    if (option == 0)
    {
        UpdateCustomerCSVData(customerDict);
        UpdateOrderCSV(fulfilledOrders, customerDict);
        FillFinalPriceCSVData(finalPriceDictionary);
        break; //ends the program for input 0
    }
    else if (option == 1)
    {
        ListAllCustomer(customerDict); //call method
    }
    else if (option == 2)
    {
        DisplayQueues(goldQueue, normalQueue);
    }
    else if (option == 3)
    {
        RegisterNewCustomer(customerDict);
    }
    else if (option == 4)
    {
        ListAllCustomer(customerDict);

        latestId = CreateCustomerOrder(customerDict, icecreamOption, flavourDict, toppingDict, normalQueue, goldQueue, latestId, orderList); //updates latestId too
    }
    else if (option == 5)
    {
        ListAllCustomer(customerDict);
        while (true)
        {
            Console.Write("\nEnter MemberID to continue ([0] to exit): ");
            string tempID = Console.ReadLine();
            if (tempID == "0") { break; } //end this method 
            try
            {
                int ID = Convert.ToInt32(tempID);
                if (tempID.Length == 6)
                {
                    DisplayCustomerOrder(customerDict, ID);
                    break; //end
                }
                else { Console.WriteLine("Must be 6 digit integer"); }
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); continue; }

        }
    }
    else if (option == 6)
    {
        ModifyingCurrentOrder(customerDict, flavourDict, toppingDict, icecreamOption);
    }
    else if (option == 7)
    {
        if (goldQueue.Count > 0) //check for any VIP customers to process first
        {
            ProcessAndCheckOut(goldQueue, customerDict, orderList, finalPriceDictionary, fulfilledOrders); //pass in goldqueue first 
        }
        else
        {
            ProcessAndCheckOut(normalQueue, customerDict, orderList, finalPriceDictionary, fulfilledOrders); //pass normal queue if no one in golden
        }
    }
    else if (option == 8)
    {
        while (true)
        {
            try
            {
                Console.Write("\nEnter the year: ");
                int year = Convert.ToInt32(Console.ReadLine());
                double total = 0;
                Dictionary<string, double> MonthlyDictionary = DisplayMonthlyCharges(orderList, finalPriceDictionary, year);
                foreach (KeyValuePair<string, double> kvp in MonthlyDictionary)
                {
                    total += kvp.Value;
                    Console.WriteLine(kvp.Key + " " + Convert.ToString(year) + ":    {0,-15}", kvp.Value.ToString("$0.00"));
                }
                Console.WriteLine("Total:{0,13}", total.ToString("$0.00"));
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
    }
}

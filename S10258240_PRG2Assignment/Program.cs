// See https://aka.ms/new-console-template for more information

//==========================================================
// Student Number : S10258240
// Student Name : Jovan Ong Yi  Jie
// Partner Name : Lucas Yeo
// Partner Number : S10255784
//==========================================================


using S10258240_PRG2Assignment;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

//This methods initialise the "orders.csv" and makes order corresponding to each customer, then appending to queue. It returns the largest num order id for use later
int InitOrder(Queue<Order> RegularQueue, Queue<Order> GoldQueue, Dictionary<int, Customer> customerDict)
{
    int largestId = 0; //dummy value to store

    string[] DataArray = File.ReadAllLines("orders.csv");
    for (int i = 1; i < DataArray.Length; i++)
    {
        List<IceCream> icecreamList = new List<IceCream>(); // Create new ice cream list
        List<Order> orderHistory = new List<Order>();
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
        string option = data[4];
        int scoops = Convert.ToInt32(data[5]);
        Order neworder = new Order(id, timeReceived);
        List<Flavour> flavourlist = new List<Flavour>();
        List<Topping> toppingList = new List<Topping>();

        // Populate flavour list
        for (int j = 8; j <= 10; j++)
        {
            bool isInside = false;
            string flavourData = data[j];
            if (!string.IsNullOrEmpty(flavourData))
            {
                int quantity = 1;
                if (flavourData == "Durian" || flavourData == "Ube" || flavourData == "Sea Salt")
                {
                    premium = true;
                }
                Flavour newFlavour = new Flavour(flavourData, premium, quantity);
                flavourlist.Add(newFlavour);
            }
        }

        // Populate topping list
        for (int j = 11; j <= 13; j++)
        {
            string toppingData = data[j];
            if (!string.IsNullOrEmpty(toppingData))
            {
                Topping newTopping = new Topping(toppingData);
                toppingList.Add(newTopping);
            }
        }
        if (option == "Waffle")
        {
            string waffleFlavour = data[7];
            Waffle waffle = new Waffle(option, scoops, flavourlist, toppingList, waffleFlavour);
            neworder.iceCreamList.Add(waffle);
        }
        else if (option == "Cone")
        {
            bool isDipped = Convert.ToBoolean(data[6]);
            Cone cone = new Cone(option, scoops, flavourlist, toppingList, isDipped);
            neworder.iceCreamList.Add(cone);
        }
        else
        {
            Cup cup = new Cup(option, scoops, flavourlist, toppingList);
            neworder.iceCreamList.Add(cup);
        }
        foreach (KeyValuePair<int, Customer> kvp in customerDict)
        {
            if (memberID == kvp.Key)
            {
                kvp.Value.currentOrder = neworder;
                if (kvp.Value.Rewards.Tier == "Gold")
                {
                    kvp.Value.orderHistory.Add(neworder);
                    GoldQueue.Enqueue(neworder);
                }
                else
                {
                    kvp.Value.orderHistory.Add(neworder);
                    RegularQueue.Enqueue(neworder);
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

//This method registers a new customer via prompt, which then append to customer.csv and as well as to customerDict;
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

//This method registers a new customer's order via prompt, which then creates order object and append to customer order.
int CreateCustomerOrder(Dictionary<int, Customer> cList, List<IceCream> icecreamOption, Dictionary<string, Flavour> fList, Dictionary<string, Topping> tList, Queue<Order>normalQ, Queue<Order> goldQ, int lastId )
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
                    Customer cus = cList[id]; //get its dictionary value, which is the customer object
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
                        string response = Console.ReadLine();
                        if (response != "N")
                        {
                            if (response == "Y")
                            {
                                IceCream icecreamOrder2 = TakingOrders(icecreamOption, fList, tList); //continues creating making of icecream
                                newOrder.AddIceCream(icecreamOrder2);
                            }
                            else { Console.WriteLine("Incorrect input, only Y or N");  }
                        }
                        else { break; }
                    }
                    
                    cus.currentOrder = newOrder; //Set to current order of object
                    string qType; //for use in displaying queue type
                    lastId++;//increment + 1
                    newOrder.Id = lastId;
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
                    Console.WriteLine(newOrder.ToString() ); //Test Output
                    Console.WriteLine($"Order Successful. Your OrderID is {newOrder.Id} in the {qType} Queue");
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


//This method UPDATES THE CSV FILE, before closing the application when 0 is click, to keep the information updated
void UpdateCSVData(Dictionary<int, Customer> cList)
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

//This method Process an order and checks it out, from the queue status , ADVANCED PART A
void ProcessAndCheckOut(Queue<Order> queue, Dictionary<int, Customer> cList)
{
    //initialisation of data
    Customer settleCustomer = new Customer();
    bool isOrderFound = false;
    string tier;
    int points;
    int completePunch = 10;

    Order orderToSettle = queue.Dequeue(); //Gets the first order to work on
    Console.WriteLine($"Ice Cream Orders: {orderToSettle.ToString()}"); //list the icecream in the order object with tostring
    double totalcost = orderToSettle.CalculateTotal();
    Console.WriteLine($"\nTotal Bill: ${totalcost:0.00}");

    //Below checks for which customer the order belongs to
    foreach (Customer cus in cList.Values)
    {
        //test if order made by who
        if (cus.currentOrder != null && cus.currentOrder.Id == orderToSettle.Id) //similar id num, check if curren order is not null first
        {
            settleCustomer = cus; //assign to this customer class for reference
            int foundID = settleCustomer.MemberId;
            tier = settleCustomer.Rewards.Tier;
            points = settleCustomer.Rewards.Points;
            Console.WriteLine($"Membership Status: {tier}\nPoints: {points}");
            isOrderFound = true;
            break;
        }
    }
    if (isOrderFound) //bool value to check for customer found
    {
        Console.WriteLine("Cant find the corresponding customer for this order. So sorry.");
        return; //break 
    }
    //Check for its birthday
    if (settleCustomer.IsBirthday())
    {
        double price = 0;
        double mostexpensive = 0;
        IceCream freeIC = null;
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
                freeIC = ic; //update to its current icecream obj, free
            }
        }
        totalcost = orderToSettle.CalculateTotal() - mostexpensive; 
    }
    //No birthday, then no discount 
    //Checks for completion of punchcard
    if (settleCustomer.Rewards.PunchCard == completePunch)
    {
        settleCustomer.Rewards.PunchCard = 0; //reset
        double deductfree = orderToSettle.iceCreamList[0].CalculatePrice();
        totalcost = totalcost - deductfree;
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

//Call to initialise data from "orders.csv", and gets the latest order id 
int latestId = InitOrder(normalQueue, goldQueue, customerDict);

while (true)
{
    int option = DisplayMenu(); //call the menu, and get back the int option
    if (option == 0)
    {
        UpdateCSVData(customerDict);
        break; //ends the program for input 0
    }
    else if (option == 1)
    {
        ListAllCustomer(customerDict); //call method
    }
    else if (option == 2)
    {

    }
    else if (option == 3)
    {
        RegisterNewCustomer(customerDict);
    }
    else if (option == 4)
    {
        ListAllCustomer(customerDict); 
        latestId = CreateCustomerOrder(customerDict, icecreamOption, flavourDict, toppingDict, normalQueue, goldQueue, latestId); //updates latestId too
    }
    else if (option == 5)
    {

    }
    else if (option == 6)
    {

    }
    else if (option == 7)
    {
        if (goldQueue.Count > 0) //check for any VIP customers to process first
        {
            ProcessAndCheckOut(goldQueue, customerDict); //pass in goldqueue first 
        }
        else
        {
            ProcessAndCheckOut(normalQueue, customerDict); //pass normal queue if no one in golden
        }
    }
}

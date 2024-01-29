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
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
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
int InitOrder(Queue<Order> RegularQueue, Queue<Order> GoldQueue, Dictionary<int, Customer> customerDict, List<Order> mainOrderList)
{
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
        string option = data[4];
        int scoops = Convert.ToInt32(data[5]);
        Order neworder = new Order(id, timeReceived);

        neworder.TimeFulfilled = timefulfilled;

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
            mainOrderList.Add(neworder);
        }
        else if (option == "Cone")
        {
            bool isDipped = Convert.ToBoolean(data[6]);
            Cone cone = new Cone(option, scoops, flavourlist, toppingList, isDipped);
            neworder.iceCreamList.Add(cone);
            mainOrderList.Add(neworder);
        }
        else
        {
            Cup cup = new Cup(option, scoops, flavourlist, toppingList);
            neworder.iceCreamList.Add(cup);
            mainOrderList.Add(neworder);
        }
        foreach (KeyValuePair<int, Customer> kvp in customerDict)
        {
            if (memberID == kvp.Key)
            {
                if (neworder.TimeFulfilled == null) //currently not fulfilled
                {
                    kvp.Value.currentOrder = neworder;
                    if (kvp.Value.Rewards.Tier == "Gold")
                    {
                        GoldQueue.Enqueue(neworder);
                    }
                    else
                    {
                        RegularQueue.Enqueue(neworder);
                    }
                }
                else
                {
                    kvp.Value.orderHistory.Add(neworder);
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
    Console.WriteLine("-------------------------------\nOrdinary Queue\n-------------------------------");
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
    if (cList.ContainsKey(id)) //contain inside Dict, so is valid
    {
        Customer cus = cList[id]; //get its dictionary value, which is the customer object\
        if (cus.currentOrder != null)
        {
            Console.WriteLine("\n--------------Current Order--------------");
            Console.WriteLine(cus.currentOrder.ToString());
        }
        if (cus.orderHistory.Count >0)
        {
            Console.WriteLine("\n--------------Past Order--------------");
            foreach (Order order in cus.orderHistory)
            {
                Console.WriteLine(order.ToString());
            }    
        }
        else
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

//This method UPDATES THE CSV FILE, before closing the application when 0 is click, to keep the information updated
void UpdateCSVData(Dictionary<int, Customer> cList, Dictionary<int, double> fpDict)
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
    //Append and update finalprice
    string header2 = "Id,FinalPrice\n";
    File.WriteAllText("finalprice.csv", header2); //This Write 'erases' everything from the exisiting file, restarting
    foreach(KeyValuePair<int, double> keyValuePair in fpDict)
    {
        string info = Convert.ToString(keyValuePair.Key) + "," + Convert.ToString(keyValuePair.Value) + "\n";
        File.AppendAllText("finalprice.csv", info);
    }
}

//This method Process an order and checks it out, from the queue status , ADVANCED PART A
void ProcessAndCheckOut(Queue<Order> queue, Dictionary<int, Customer> cList, List<Order> mainOrderList, Dictionary<int, double> fpDict)
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

    //Checks for completion of punchcard
    int punchCard = settleCustomer.Rewards.PunchCard;
    if ( punchCard == completePunch )
    {
        settleCustomer.Rewards.Punch(); //reset to 0
        double deductfree = orderToSettle.iceCreamList[0].CalculatePrice(); //first ice cream in the list ordered
        Console.WriteLine("Your 1st Ice Cream is free.");
        try
        {
            totalcost = totalcost - deductfree; //minus the fee
            if (totalcost < 0) //In the case it is their birthday, and punchpoint = 10, should negative be balance
            {
                throw new Exception("Negative balance, Puunch points dont need to be used"); 
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            settleCustomer.Rewards.PunchCard = 10; //CHANGE BACK TO 10
            totalcost += deductfree; //add back cost
        }
    }

    //Redeeming of points, for selected GOLD OR SILVER
    if (tier == "Gold" || tier == "Silver")
    {
        while (true) //loop to check for correct input of points
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
            else { break; }
        }
    }
    Console.WriteLine($"\nFinal Nett Price: ${totalcost:0.00}"); //display of final charge
    Console.Write("\n----PRESS ANY KEY TO MAKE PAYMENT----");
    Console.ReadKey(); //To read in the random key, useless to store

    //Increment punch card below
    foreach(IceCream iceCream in orderToSettle.iceCreamList)
    {
        if (settleCustomer.Rewards.PunchCard < 10)
        {
            settleCustomer.Rewards.PunchCard += 1;
        } 
    }
    if (settleCustomer.Rewards.PunchCard == 10) //reaches the amt needed, prompt to let them know
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

//This is a method to read finalprice.csv, where it has a collection of order price that is successfully dequeue by us, diff from default existing order.csv orders
Dictionary<int, double> InitFinalPrice()
{
    Dictionary<int, double> fpDict = new Dictionary<int, double>();
    using (StreamReader sr = new StreamReader("finalprice.csv"))
    {
        string? s = sr.ReadLine(); // header skip
        while ((s = sr.ReadLine()) != null)
        {
            string[] data = s.Split(",");
            if (string.IsNullOrEmpty(data[0]) && string.IsNullOrEmpty(data[1])) //at the start, its empty
            {
                return fpDict;  //no orders yet
            }
            int id = Convert.ToInt32(data[0]);
            double finalprice = Convert.ToDouble(data[1]);
            fpDict.Add(id, finalprice);
        }
    }
    return fpDict;
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

//Call to initialise OrderLisst
List<Order> orderList = new List<Order>();
//Call to initialise data from "orders.csv", and gets the latest order id to be used for creating data 
int latestId = InitOrder(normalQueue, goldQueue, customerDict, orderList);

//Create a new Dictionary to store the respective order id and final price charged
Dictionary<int, double> finalPriceDict = InitFinalPrice();

while (true)
{
    int option = DisplayMenu(); //call the menu, and get back the int option
    if (option == 0)
    {
        UpdateCSVData(customerDict, finalPriceDict);
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

    }
    else if (option == 7)
    {
        if (goldQueue.Count > 0) //check for any VIP customers to process first
        {
            ProcessAndCheckOut(goldQueue, customerDict, orderList, finalPriceDict); //pass in goldqueue first 
        }
        else
        {
            ProcessAndCheckOut(normalQueue, customerDict, orderList, finalPriceDict); //pass normal queue if no one in golden
        }
    }
}

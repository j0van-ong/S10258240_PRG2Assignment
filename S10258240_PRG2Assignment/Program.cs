// See https://aka.ms/new-console-template for more information

//==========================================================
// Student Number : S10258240
// Student Name : Jovan Ong Yi  Jie
// Partner Name : Lucas Yeo
// Partner Number : S10255784
//==========================================================


using S10258240_PRG2Assignment;

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
        "Display order details of a customer", "Modify order details" };
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

            if (name == "0") { break; } //end this method

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
                    Console.WriteLine("No duplicate User, MemberID already exists in our system, try a different one");
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Incorrect input of MemberID, must be 6 Digit Integer");
                continue;
            }
        }
        catch (FormatException ex) //any invalid input not specified will result in code to come here
        {
            Console.WriteLine(ex.Message);
            continue;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            continue;
        }
        //Code proceeds after validation above
        cList.Add(id, cus1); //add to dictionary
        string data = name + "," + id.ToString() + "," + dob.ToString("dd/MM/yyyy") + "," + cus1.Rewards.Tier + "," + "0" + "," + "0"; //append as string with comma
        try
        {
            File.AppendAllText("customers.csv", data);
            Console.WriteLine("Registration completed!");
            break; //end method
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error in uploading: " + ex.Message); //if file not found
            Console.WriteLine("Retrying...");
            continue;
        }
    }
}

/******************Start of program*********************/

//Call to initialise CustomerDict for reference as collection
Dictionary<int, Customer> customerDict = InitCustomer();

//Call to initialise FlavourDict for reference as collection
Dictionary<string, Flavour> flavourDict = InitFlavourDict();

//Call to initialise ToppingDict for reference as collection
Dictionary<string, Topping> toppingDict = InitToppingDict();

while (true)
{
    int option = DisplayMenu(); //call the menu, and get back the int option
    if (option == 0)
    {
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

}
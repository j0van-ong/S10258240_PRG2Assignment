// See https://aka.ms/new-console-template for more information

//==========================================================
// Student Number : S10258240
// Student Name : Jovan Ong Yi  Jie
// Partner Name : Lucas Yeo
// Partner Number : S10255784
//==========================================================


/*This function displays the menu for the overall application and returns a integer, 
 *the option for the application */
using S10258240_PRG2Assignment;

int DisplayMenu()
{
    while (true)
    { 
    int option = 0; //initialise
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

//This method creates Flavour Dictionary to store the Flavours available, setting quanttiy to default 0 for references later
Dictionary<string, Flavour> InitFlavourDict()
{
    //Array for storing the available flavours
    string[] regularArray = { "vanilla", "chocolate", "strawberry" }; //premium bool No
    string[] premiumArray = { "durian", "ube", "sea salt" }; //premium bool Yes
    Dictionary<string, Flavour> flavourDict = new Dictionary<string, Flavour>();
    for (int i = 0; i<= regularArray.Length; i++)
    {
        string r = regularArray[i];
        flavourDict.Add(r, new Flavour(r, false, 0)); 
    }
    for (int i = 0; i<= premiumArray.Length; i++)
    {

    }
}

//Start of program
while (true)
{
    int option = DisplayMenu();
    if (option == 0)
    {
        break; //ends the program for input 0
    }
    else if (option == 1)
    {

    }
    else if (option == 2)
    {

    }
}
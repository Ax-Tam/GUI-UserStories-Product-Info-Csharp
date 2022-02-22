using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment_2
{
    public class ProductListing
    {
        private Menu mainMenu;
        private Menu functionMenu;
        private List<Customer> customers;
        private List<Item> items;
        private Customer loggedInUser;
        // LoggedIn Item tells whether there is a customer currently using the system
        private bool LoggedIn
        {
            get
            {
                return (loggedInUser != null);
            }
        }

        public ProductListing()
        {
            mainMenu = new Menu();
            functionMenu = new Menu();
            customers = new List<Customer>();
            items = new List<Item>();
        }

        //entry point of application - creates the realestate company and runs it.
        static void Main(string[] args)
        {
            ProductListing company = new ProductListing();
            company.Run();
        }
        private void Run()
        {
            PopulateMenus();
            DisplayMenu();
        }
        private void PopulateMenus()
        {
            //populate the main menu
            mainMenu.Add("Register as new Client", RegisterCustomer);
            mainMenu.Add("Login as existing Client", LogIn);
            mainMenu.Add("Exit", Exit);
            //populate the user's logged in option screen
            functionMenu.Add("Advertise Item", NewProduct);
            functionMenu.Add("List My Items", ListUsersItems);
            functionMenu.Add("Search for Items", SearchItems);
            functionMenu.Add("Bid on a Item", PlaceBid);
            functionMenu.Add("List bids received for a Item", ListBidsForItem);
            functionMenu.Add("Sell one of your items to the highest bidder", SellItem);
            functionMenu.Add("Logout", Logout);
        }

        private void DisplayMenu()
        {
            while (true)
            {
                if (LoggedIn)
                {
                    functionMenu.Display();
                }
                else
                {
                    mainMenu.Display();
                }
            }
        }

        // Actions - for menus
        // -- Main Menu
        private void RegisterCustomer()
        {
            string firstName = UserInterface.GetInput("First name");
            string lastName = UserInterface.GetInput("Last name");
            string address = UserInterface.GetInput("Address");
            string email;
            string pwd = "";
            // ensure email isn't already taken
            email = UserInterface.GetInput("Email address");
            if (customers.Find(cust => cust.Email == email) != null)
            {
                UserInterface.Error("Email taken");
                return;
            }
            bool pwd_confirmed = false;
            // Confirm password
            while (!pwd_confirmed)
            {
                pwd = UserInterface.GetPassword("Password");
                string pwd_ = UserInterface.GetPassword("Confirm Password");
                if (pwd == pwd_)
                {
                    pwd_confirmed = true;
                }
                else
                {
                    UserInterface.Error("Sorry, those passwords don't match");
                }
            }
            customers.Add(new Customer(firstName, lastName, email, pwd, address));
            UserInterface.Message(firstName + " " + lastName + " registered successfully");
        }

        private void LogIn()
        {
            string emailInp = UserInterface.GetInput("Email");
            string pwdInp = UserInterface.GetPassword("Password");
            Customer customer = customers.Find(r => r.Email == emailInp);
            if (customer == null) // error if customer with corresponding email doesn't exist
            {
                UserInterface.Error("Email not found");
            }
            else
            {
                //abstract logic of authentication - will return null if incorrect details 
                loggedInUser = customer.Authenticate(emailInp, pwdInp);
            }
        }

        private void Exit() { System.Environment.Exit(0); }


        // -- Actions for user logged in menu
        private void NewProduct()
        {
            Item newProduct = loggedInUser.ListNewItem(1);
            items.Add(newProduct);
            UserInterface.Message("Successfully advertised " + newProduct);
        }
        private void SearchItems()
        {

            string itemType = UserInterface.GetInput("Enter type of item for search");
            UserInterface.DisplayList($"item's found for {itemType}", Item.Filteritems(itemType, items), "No items found");
        }
        private void ListUsersItems()
        {
            List<Item> filtered_listings = Item.Filteritems(loggedInUser, items);
            UserInterface.DisplayList("All of your items", filtered_listings, "You do not currently have any items listed");
        }

        private void PlaceBid()
        {
            UserInterface.Message("Select a Item to bid on");
            // We don't need to show the customer their own items to bid on.
            // filtereditems is evaluated by taking all items except ones belonging to customer, then parsing it back as a list of items
            List<Item> filtereditems = items.Except(Item.Filteritems(loggedInUser, items)).ToList();
            if (filtereditems.Count == 0)
            {
                UserInterface.Error("No items available to bid on");
                return;
            }
            Item selected = UserInterface.ChooseFromList(filtereditems);
            // Inputs customer's bid amount, after displays the output
            UserInterface.Message(loggedInUser.PlaceBid(selected));
        }

        private void ListBidsForItem()
        {
            Item selected = UserInterface.ChooseFromList(Item.Filteritems(loggedInUser, items));
            if (selected == null) return;
            selected.ListBids();
        }

        private void SellItem()
        {
            //enquires about which Item the customer wishes to sell 
            UserInterface.Message("Which Item would you like to sell?");
            Item selected = UserInterface.ChooseFromList(Item.Filteritems(loggedInUser, items));
            if (selected != null)
            {
                //then the Item transfers its ownership
                selected.TransferToHighestBidder();
            }
        }

        private void Logout()
        {
            loggedInUser = null;
        }
    }
}
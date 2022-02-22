using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment_2
{
    public class Customer
    {
        // A unique email
        private string email;
        public string Email { get { return email; } }
        // Customer records/details
        private string firstName;
        private string lastName;
        private Password password;
        private string address;
        // Displays the names of their customers
        public string FullName { get { return firstName + " " + lastName; } }
        public Customer(string firstName, string lastName, string email, string password, string address)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.address = address;

            // we never want to store a user's plain text password in an application. Password class deals with all salting/hashing
            this.password = new Password(password);
        }
        // Authenticate returns the customer instance if authentication is successful else null is returned
        public Customer Authenticate(string emailInp, string pwdInp)
        {
            bool pwdMatch = password.checkPassword(pwdInp);
            bool emailMatch = (emailInp == email);
            if (emailMatch && pwdMatch)
            {
                UserInterface.Message($"Logged in as {this.FullName}");
                return this;
            }
            else
            {
                UserInterface.Error("Password incorrect");
                return null;
            }
        }
        public Item ListNewItem(int type)
        {
            string itemType = UserInterface.GetInput("Type");
            string description = UserInterface.GetInput("Description");
            Item.InitialPrice = UserInterface.GetInteger("Initial Bid");
            return new Product(this, itemType, description, Item.InitialPrice);
        }
        public string PlaceBid(Item Item)
        {
            int min = Item.InitialPrice;
            min = Item.GetHighestBidAmount();
            Item.bidAmount = UserInterface.GetInteger($"Enter bid amount (current price/bid: ${min})");
            if (Item.bidAmount <= min) return "Bid cannot be placed as it is less than the minimum amount";
            string answer = UserInterface.GetInput($"Home Delivery");
            //Home Delivery
            if (answer == "yes")
            {
                //Auction hosue takes 20 dollars plus a 5 dollar delivery fee
                Item.Delivery = 20;
                Item.DeliveryFee = 5;
            }
            //Click and collect
            else if (answer == "no")
            {
                //Auction hosue takes 10 dollars
                Item.Delivery = 10;
                Item.DeliveryFee = 0;
            }
            else
            {
                return "Please Enter a Valid Answer: yes or no";
            }
            Item.AddBid(this, Item.bidAmount);
            return $"Successfully placed ${Item.bidAmount} bid on {Item}";

        }
        public override string ToString()
        {
            return $"{FullName} ({email})";
        }
    }
}

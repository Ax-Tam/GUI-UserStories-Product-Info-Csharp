using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment_2
{
    public abstract class Item

    {
        // Fields and items
        protected Customer owner;
        protected string itemType;
        protected string description;
        protected List<Bid> Bids { get; set; }
        // Methods
        public Item(Customer owner, string itemType, string description)
        {
            this.owner = owner;
            this.itemType = itemType;
            this.description = description;
            Bids = new List<Bid>();
        }
        public static List<Item> Filteritems(Customer owner, List<Item> items)     // overloaded method with parameter customer
        {
            List<Item> _items = new List<Item>();

            foreach (Item prop in items)
            {
                if (prop.owner == owner) _items.Add(prop);
            }
            return _items;
        }
        public static List<Item> Filteritems(string itemType, List<Item> items)    // overloaded method with parameter description string
        {
            List<Item> _items = new List<Item>();

            foreach (Item prop in items)
            {
                if (prop.itemType == itemType) _items.Add(prop);
            }
            return _items;
        }
        public abstract void ListBids();
        public abstract int SalesTax(int salePrice);
        public abstract int AuctionHouseFee(int FeePrice);

        public static int Delivery;

        public static int DeliveryFee;

        public static int InitialPrice;

        public static int bidAmount;
        public int GetHighestBidAmount()
        {
            // ensure the Item has bids
            if (Bids.Count > 0)
            {
                return Bids.Max(r => r.BidAmount);
            }
            else
            {
                // no bids for current Item, starting bid is always zero despite the initial price
                return 0;
            }
        }

        public void AddBid(Customer customer, int bidAmount)
        {
            Bids.Add(new Bid(customer, bidAmount));
        }

        public void TransferToHighestBidder()
        {
            int salePrice = GetHighestBidAmount();
            int FeePrice = Delivery;
            // if the highest bid ammount is 0, there aren't any bids so can't sell the Item
            if (Bids.Count == 0)
            {
                UserInterface.Error("There are no bids for this Item at the current time");
                return;
            }
            //transfer item once there is a bid
            Bid highestBid = Bids.Find(x => x.BidAmount == salePrice);
            Customer newOwner = highestBid.Bidder;
            owner = newOwner;
            // bids is now reset, since the item has been transfered
            Bids = new List<Bid>();
            UserInterface.Message($"Successfully sold {this} to {this.owner}");
            UserInterface.Message($"Applicable sales tax: ${this.SalesTax(salePrice)}");
            UserInterface.Message($"Auction house fee: ${this.AuctionHouseFee(FeePrice)}");
        }
    }

class Product : Item
    {
        private int InitialCost;
        public Product(Customer owner, string itemType, string description, int InitialCost) : base(owner, itemType, description)
        {
            this.InitialCost = InitialCost;
        }

        public override void ListBids()
        {
            UserInterface.DisplayList($"All bids for {this.itemType}", Bids, "No bids for this item have been received");
        }

        public override string ToString()
        {
            return $"{itemType}, {description}, Initial Bid: ${InitialCost}, Current Highest Bid:${GetHighestBidAmount()}";
        }

        public override int SalesTax(int salePrice)
        {
            // sales tax is equal to 15% of the product plus additional $5 if it is home delievered
            return (int)(bidAmount * 0.15 + DeliveryFee);

        }
        public override int AuctionHouseFee(int FeePrice)
        {
            return (Delivery);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment_2
{
    public class Bid
    {
        public Customer Bidder { get; set; }
        public int BidAmount { get; set; }

        public Bid(Customer bidder, int bidAmount)
        {
            this.Bidder = bidder;
            this.BidAmount = bidAmount;
        }
        public override string ToString()
        {
            return $"{Bidder} : ${BidAmount}";
        }
    }
}

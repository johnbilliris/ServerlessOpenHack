using System;
using System.Collections.Generic;
using System.Collections;

namespace Team1.Models
{
    public class SalesOrder
    {
        public string id {get;set;}
        public header header {get;set;}
        public List<detail> details {get;set;}
    }

    public class header
    {
        public string salesNumber {get;set;}
        public string dateTime {get;set;}
        public string locationId {get;set;}
        public string locationName {get;set;}
        public string locationAddress {get;set;}
        public string locaiontPostcode {get;set;}
        public string totalCost {get;set;}
        public string totalTax {get;set;}
        public string receiptUrl {get;set;}
    }

    public class detail
    {
        public string productId {get;set;}
        public string quantity {get;set;}
        public string unitCost {get;set;}
        public string totalCost {get;set;}
        public string totalTax {get;set;}
        public string productName {get;set;}
        public string productDescription {get;set;}
    }
}
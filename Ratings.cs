using System;

namespace Team1
{
    public class Rating
    {
        public string Id {get;set;}
        public string UserId {get;set;}
        public string ProductId {get;set;}
        public string LocationName {get;set;}
        public int RatingValue {get;set;}
        public string UserNotes {get;set;}
        public DateTimeOffset TimeStamp {get;set;}
    }
}
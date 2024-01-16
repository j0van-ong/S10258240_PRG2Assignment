using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10258240_PRG2Assignment
{
    class Customer
    {
        private string name;
        private int memberId;
        private DateTime dob;
        public DateTime DOB {  get; set; }
        public string Name { get; set; }
        public int MemberId { get; set; }
        public Order currentOrder { get; set; }
        public PointCard rewards { get; set; }
        public List<Order> orderHistory { get; set; } = new List<Order>() { };
        public Customer() { }
        public Customer(string n,int id, DateTime dob)
        {
            Name = n;
            MemberId = id;
            DOB =dob;
        }
        public bool IsBirthday()
        {
            var date = DateTime.Now.Date;
            var birthday = DOB.Date;
            if (date.Month == birthday.Month && date.Day == birthday.Day)
            {
                Console.WriteLine("Birthday");
                return true;
            }
            Console.WriteLine("NOt birthday");
            return false;
            
        }
        public override string ToString()
        {
            return $"{Name,-15}{MemberId,-10}{DOB.ToString("dd/MM/yyyy"),-15}{rewards.Tier,-20}{rewards.Points,-20}{rewards.PunchCard,-15}";

            //"Name: " + Name + " MemberID: " + MemberId + " Date of Birth: " + Convert.ToString(DOB);
        }


    }
}

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
        public void IsBirthday()
        {
            var date = DateTime.Now.Date;
            var birthday = DOB.Date;
            if (date == birthday)
            {
                Console.WriteLine("Birthday");
            }
            else
            {
                Console.WriteLine("NOt birthday");
            }
        }
        public override string ToString()
        {
            foreach (Order order in orderHistory)
            {
                return order.ToString();
            }
            return "Name: " + Name + " MemberID: " + MemberId + " Date of Birth: " + Convert.ToString(DOB);
        }


    }
}

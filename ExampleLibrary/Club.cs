using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleLibrary
{
    public class Club
    {
        public bool MyBool { get; private set; }
        public List<Person> People;
        public Club()
        {
            People = new List<Person>();
            People.Add(new Person("Steve", 25));
            People.Add(new Person("Nancy", 80));
            MyBool = false;
        }

        public void ChangeMyBoolToTrue()
        {
            MyBool = true;
        }
       
    }
}

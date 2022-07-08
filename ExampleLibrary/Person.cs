using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleLibrary
{
    public class Person
    {
        public Person(string name, double age )
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }    
        public double Age { get; set; }
    }
}

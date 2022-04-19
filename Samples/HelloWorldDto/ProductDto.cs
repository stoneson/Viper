using System;

namespace HelloWorldDto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public double Price { get; set; }
        public double Amount { get { return Price * Number; } }
        public string CountryOfOrigin { get; set; }
    }

    public class PersonDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string CountryOfOrigin { get; set; }
    }
}

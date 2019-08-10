using System.Collections.Generic;

namespace RomansBuyerScraper
{
    public static class Http
    {
        public static void Post<T>(T data)
        {

        }

        public static List<Address> Get()
        {
            return new List<Address> {new Address { County=  "hampshire", PostCode = "GU14 6NH", Town ="Farnborough"}};
        }
    }
}

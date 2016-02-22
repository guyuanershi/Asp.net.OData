using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceUrl = "http://localhost:2029";
            var container = new Default.Container(new Uri(serviceUrl));

            var product = new ProductService.Models.Product()
            {
                Name = "Yo-yo",
                Category = "Toy",
                Price = 4.95M
            };

            //AddProduct(container, product);

            ListAllProducts(container);

            Console.ReadLine();
        }

        static void AddProduct(Default.Container contaner, ProductService.Models.Product product)
        {
            contaner.AddToProducts(product);
            try
            {
                var serviceResponse = contaner.SaveChanges();
                foreach (var operationRes in serviceResponse)
                {
                    Console.WriteLine("Response: {0}", operationRes.StatusCode);
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        static void ListAllProducts(Default.Container container)
        {
            foreach (var p in container.Products)
            {
                Console.WriteLine("{0} {1} {2}", p.Name, p.Category, p.Price);
            }
        }
    }
}

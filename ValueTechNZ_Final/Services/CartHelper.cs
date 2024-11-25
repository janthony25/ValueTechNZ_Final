using System.Text.Json;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Services
{
    public class CartHelper
    {
        public static Dictionary<int, int> GetCartDictionary(HttpRequest request, HttpResponse response)
        {
            string cookie_value = request.Cookies["shopping_cart"] ?? "";

            try
            {
                var cart = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookie_value));
                Console.WriteLine("[CartHelper] cart =" + cookie_value + " -> " + cart);
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
                
                if(dictionary != null)
                {
                    return dictionary;
                }
            }
            catch (Exception)
            {

            }

            if (cookie_value.Length > 0)
            {
                // This cookie is not valid => delete it
                response.Cookies.Delete("shopping_cart");
            }

            return new Dictionary<int, int>();
        }

        public static int GetCartSize(HttpRequest request, HttpResponse response)
        {
            int cartSize = 0;

            var cartDictionary = GetCartDictionary(request, response);
            foreach(var keyValuePair in cartDictionary)
            {
                cartSize += keyValuePair.Value;
            }

            return cartSize;
        }

        public static List<OrderItem> GetCartItems(HttpRequest request, HttpResponse response,
                                                   ApplicationDbContext context)
        {
            var cartITems = new List<OrderItem>();

            var cartDictionary = GetCartDictionary(request, response);

            foreach(var pair in cartDictionary)
            {
                var productId = pair.Key;
                int quantity = pair.Value;
                var product = context.Products.Find(productId);

                if (product == null) continue;

                var item = new OrderItem
                {
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    Product = product
                };

                cartITems.Add(item);
            }

            return cartITems;
        }

        public static decimal GetSubTotal(List<OrderItem> cartItems)
        {
            decimal subtotal = 0;

            foreach (var item in cartItems)
            {
                subtotal += item.Quantity + item.UnitPrice;
            }

            return subtotal;
        }
    }
}

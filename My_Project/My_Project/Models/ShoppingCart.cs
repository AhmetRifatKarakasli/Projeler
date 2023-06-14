using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Project.Models
{
    public class ShoppingCart
    {
        private List<ShopCart> _myshopingcart = new List<ShopCart>();

        public List<ShopCart> Myshopingcart { get => _myshopingcart; set => _myshopingcart = value; }

        public void add_cart(Product in_product, int qntty)
        {
            var my_product = _myshopingcart.FirstOrDefault(x => x.prdct.product_id == in_product.product_id);
            if (my_product == null)
            {
                /*ShopCart sc = new ShopCart();
                sc.product = incoming_product;
                sc.quantity = 1;
                _myshopingcart.Add(sc);*/
                _myshopingcart.Add(new ShopCart { prdct = in_product, quantity = 1 });
            }
            else if (qntty == 0) my_product.quantity += 1;// Linkle gelirsen
            else my_product.quantity = qntty;// Sepetin içinden güncellersen
        }

        public void delete_cart(Product deleted_item)
        {
            _myshopingcart.RemoveAll(x => x.prdct.product_id == deleted_item.product_id);
        }

        public void clear_cart()
        {
            _myshopingcart.Clear();
        }

        public double total_cart()
        {
            return (_myshopingcart.Sum(x => x.prdct.product_price * x.quantity));
        }
    }
}
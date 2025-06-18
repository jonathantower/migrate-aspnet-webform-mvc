using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Data;

namespace WingtipToys.ShopUI.Core.Logic
{
    public static class ShoppingCartActions
    {
        public static int GetCount(string cartId, ProductContext db)
        {
            // Get the count of each item in the cart and sum them up          
            int? count = (from cartItems in db.CartItems
                          where cartItems.CartId == cartId
                          select (int?)cartItems.Quantity).Sum();
            
            // Return 0 if all entries are null         
            return count ?? 0;
        }

    }
}
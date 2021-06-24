using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class SellerService
    {

        ShopifyContext _db;
        public SellerService(ShopifyContext db)
        {
            _db = db;
        }

        public void AddSellerId(string id , string storeName , List<string > documents)
        {
            _db.Sellers.Add(new Seller() { SellerId = id  ,StoreName = storeName, Contract= documents[0], NationalCard = documents[1], TaxCard=documents[2], CommercialRegistryCard = documents[3] });
           _db.SaveChanges();
        }



        // get all active Seller
        public List<Seller> GetAllActiveSellers()
        {

            List<Seller> sellers = _db.Sellers.Include(i => i.ApplicationUser).Where(t => t.ApplicationUser.AdminLocked == false).ToList();
            return sellers;

        }


      

        // get all waiting Seller
        public List<Seller> GetAllWaitingSellers()
        {
            List<Seller> sellers = _db.Sellers.Include(i => i.ApplicationUser ).Where(t=>t.ApplicationUser.AdminLocked==true).ToList();
            return sellers;
        }




        //edit seller
        public async Task<ApplicationUser> PutSeller(string id, [FromBody] ApplicationUser user)
        {
            var seller = await _db.Sellers.FirstOrDefaultAsync(s => s.SellerId == id);

            var userSeller =  _db.Users.FirstOrDefault(s => s.Id == seller.SellerId);
            userSeller.Fname = user.Fname;
            userSeller.Lname = user.Lname;
            userSeller.Gender = user.Gender;
            userSeller.Email = user.Email;
            userSeller.UserName = user.UserName;
            userSeller.Address = user.Address;
            userSeller.Age = user.Age;
            await _db.SaveChangesAsync();
            return userSeller;
        }

        public bool ApplySeller(string id)
        {
           Seller seller = _db.Sellers.Include(a=>a.ApplicationUser).FirstOrDefault(s => s.SellerId == id);
            if (seller != null)
            {
                seller.ApplicationUser.AdminLocked = false;
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }



        public bool BlockSeller(string id)
        {
            Seller seller = _db.Sellers.Include(a => a.ApplicationUser).FirstOrDefault(s => s.SellerId == id);
            if (seller != null)
            {
                seller.ApplicationUser.AdminLocked = true;
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }


        //// get seller orders
        //public object SellerOrders()
        //{
        //  var products = _db.Carts.Include(r => r.CartItems).ThenInclude(p => p.Product).Where(r => r.Approved == true && r.Isdeleted==false).Select(r=>r.CartItems.Where(r=>r.Isdeleted==false));
        //  return products;
        //}


    }
}



using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Services.Interfaces;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class SellerService
    {

        ShopifyContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public SellerService(ShopifyContext db , UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public void AddSellerId(string id , string storeName , List<string > documents)
        {
            _db.Sellers.Add(new Seller() { SellerId = id  ,StoreName = storeName, Contract= IPort.Port+documents[0], NationalCard = IPort.Port+documents[1], TaxCard= IPort.Port+documents[2], CommercialRegistryCard = IPort.Port+documents[3] });
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
        public async Task<Seller> PutSeller(IIdentity user , EditSellerViewModel editSellerView)
        {
            var seller = await _db.Sellers.Include(r=>r.ApplicationUser).FirstOrDefaultAsync(s => s.SellerId == HelperMethods.GetAuthnticatedUserId(user));
            seller.ApplicationUser.Fname = editSellerView.Fname;
            seller.ApplicationUser.Lname = editSellerView.Lname;
            seller.ApplicationUser.Address = editSellerView.Address;

            string token = await _userManager.GenerateChangeEmailTokenAsync(seller.ApplicationUser, editSellerView.Email);
            await _userManager.ChangeEmailAsync(seller.ApplicationUser, editSellerView.Email, token);

            await _db.SaveChangesAsync();
            return seller;
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


   


        // get seller orders
        public List<Product> SellerOrders(IIdentity seller)
        {
            string sellerId = HelperMethods.GetAuthnticatedUserId(seller);
             List<Product> sellerProducts = new List<Product>(); // get seller's products
             List<Inventory> inventories = _db.Inventories.Include(ip => ip.InventoryProducts).ThenInclude(r => r.Product).ThenInclude(p => p.ProductImages).Where(i => i.sellerId == sellerId && i.Isdeleted == false).ToList();
             List<List<Product>> products = inventories.Select(f => f.InventoryProducts.Where(f => f.Isdeleted == false).Select(f => f.Product).Where(r => r.IsdeletedBySeller == false).ToList()).ToList();
             foreach (var item in products)
             {
                sellerProducts.AddRange(item);
             }

            List<List<CartItem>> cartitems = _db.Carts.Include(r => r.Status).Include(r => r.CartItems.Where(r => r.Isdeleted == false)).Where(r => r.Isdeleted == false && r.Payed == true && r.Status.StatusName == StatusEnum.Approved.ToString()).Select(f=>f.CartItems).ToList();
            List<CartItem> prderProducts = new List<CartItem>(); // get order's products
            foreach (var item in cartitems)
            {
                prderProducts.AddRange(item);
            }


            List<Product> orders = new List<Product>(); //orders
            foreach (var item in sellerProducts)
            {
                prderProducts.ForEach(p => { 
                
                    if(p.ProductId == item.ProductId)
                    {
                        item.Quantity = p.Quantity;
                        orders.Add(item);
                    }
                });
            }


            return orders;


        }






        // get seller by Id
        public Seller GetSellerByID(string id)
        {
            return  _db.Sellers.Include(e => e.ApplicationUser).FirstOrDefault(f => f.Isdeleted == false && f.SellerId == id);
        }



        // get seller info

        public Seller GetSellerInfo(IIdentity seller)
        {
            return _db.Sellers.Include(e => e.ApplicationUser).FirstOrDefault(f => f.Isdeleted == false && f.SellerId == HelperMethods.GetAuthnticatedUserId(seller));
        }


    }
}



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class InventoryServices
    {
        ShopifyContext _db;
        public InventoryServices(ShopifyContext db)
        {
            _db = db;
        }
        //get all inventories
        public List<Inventory> GetAllInventories(IIdentity seller)
        {
              var sellerId  = HelperMethods.GetAuthnticatedUserId(seller);
              return _db.Sellers.Include(i=>i.Inventories).FirstOrDefault(s=>s.SellerId == sellerId && s.Isdeleted == false).Inventories;
        }

        // get inventory by id
        public Inventory GetInventoryById(int id , IIdentity seller)
        {
            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
           return _db.Inventories.SingleOrDefault(c => c.InventoryId == id && c.Isdeleted == false && c.sellerId==sellerId);  
        }
        // add   inventory
        public async Task<Inventory> AddInventory(Inventory inventory, IIdentity seller)
        {
            string sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            if (inventory != null)
            {
                inventory.sellerId = sellerId;
                _db.Inventories.Add(inventory);
                _db.SaveChanges();
            }
            return inventory;
        }


        //  Edit inventory
        public  Inventory EditInventoryAsync(Inventory inventory , IIdentity seller)
        {
            string sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            var inventoryDetails =  _db.Inventories.FirstOrDefault(f=>f.InventoryId== inventory.InventoryId && f.sellerId ==sellerId && f.Isdeleted == false);
            if (inventoryDetails != null)
            {
                inventoryDetails.City = inventory.City;
                inventoryDetails.Street = inventory.Street;
                inventoryDetails.BuildingNumber = inventory.BuildingNumber;
                _db.SaveChanges();
            }
            return inventoryDetails;
        }




       //// delete inventory
       // public Inventory DeleteInventory(int id ,IIdentity seller)
       // {
       //     Inventory inventory = GetInventoryById( id , seller );
       //     if (inventory != null)
       //     {
       //         inventory.Isdeleted = true;
              
       //         foreach (var inv in _db.InventoryProducts.Where(I => I.InventoryId == id).ToList())
       //         {
       //             inv.Isdeleted = true;
       //         }
       //         _db.SaveChanges();

       //     }
       //     return inventory;

       // }





        //  delete Inventory

        public Response DeleteInventory(int id , IIdentity seller)
        {
            
            Inventory inventory = GetInventoryById(id ,seller );
            if (inventory != null)
            {
                if (!IsInventoryInRelation(id))
                {
                    inventory.Isdeleted = true;
                    _db.SaveChanges();
                    return new Response { Status = "Success", Message = "inventory Deleted successully" };
                }
                return new Response { Status = "Error", Message = "Sorry this not allow" };
            }
            return new Response { Status = "Error2", Message = "inventory Not Found" };

        }





        // IS Inventory In Relation

        public bool IsInventoryInRelation(int id)
        {
            List<InventoryProduct> inventoryProducts = _db.Inventories.Include(p => p.InventoryProducts.Where(e => e.Isdeleted == false && e.InventoryId ==id)).FirstOrDefault(b => b.InventoryId == id && b.Isdeleted == false).InventoryProducts;
            if (inventoryProducts.Count > 0)
                return true;
            return false;

        }






    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System.IO;
using System.Security.Principal;

namespace Shopify.Repository.Interfaces
{
    public class PromotionService
    {
        ShopifyContext _db;
        public PromotionService(ShopifyContext db)
        {
            _db = db;
        }

        // get all promotions for seller
        public List<Promotions> GetAllPromotions(IIdentity seller)
        {
            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            return _db.Promotions.Where(c => c.SellerId==sellerId && c.Isdeleted == false).ToList();
        }

        // get promotions by id for seller
        public Promotions GetPromotion(int id, IIdentity seller)
        {

            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            Promotions promotion = _db.Promotions.Include("Seller").SingleOrDefault(c => c.PromotionsId == id &&c.SellerId == sellerId&& c.Isdeleted == false);
            return promotion;
        }

        // add Promotions by seller
        public async Task<Promotions> addPromotionAsync(Promotions promotion,IFormFile file, IIdentity seller)
        {
              var sellerId =  HelperMethods.GetAuthnticatedUserId(seller);
              promotion.SellerId = sellerId;
             _db.Promotions.Add(promotion);
            _db.SaveChanges();

            if (file != null)
             {
              string path= await  FileHelper.SaveImageAsync(promotion.PromotionsId, file, "Promotion");
              promotion.Image = path;
             }
             _db.SaveChanges();
             
            return promotion;
        }

        public Product AddPromotionToProduct(int PormotionId, int productId, IIdentity seller)
        {

            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            var promotion = _db.Promotions.FirstOrDefault(p => p.PromotionsId == PormotionId && p.SellerId ==sellerId && p.Isdeleted == false);

            if (promotion != null)
            {
                Product Product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
                Product.PromotionId = PormotionId;
                _db.SaveChanges();
                return Product;
            }
            return null;
        }

        public async Task<bool> EditPromotionAsync(Promotions promotion, IFormFile file, IIdentity seller)
        {
            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            Promotions promotion1  = _db.Promotions.FirstOrDefault(p => p.PromotionsId == promotion.PromotionsId && p.SellerId == sellerId && p.Isdeleted == false);

            if (promotion1 != null)
            {
                promotion1.Description = promotion.Description;
                promotion1.Discount = promotion.Discount;
                promotion1.StartDate = promotion.StartDate;
                promotion1.EndDate = promotion.EndDate;
                promotion1.Status = promotion.Status;
                promotion1.StatusControlled = promotion.StatusControlled;
              

                if (file != null)
                {
                    // delete old image
                    File.Delete(promotion1.Image);
                    // add new image
                    promotion1.Image = await FileHelper.SaveImageAsync(promotion.PromotionsId, file, "Promotion");
                  
                }
                _db.SaveChanges();
                return true;
            }
        
            return false;
        }

        //  Edit promotion
        //public async Task<Promotions> EditPromotionAsync(Promotions promotion, IFormFile file)
        //{
        //    Promotions promotionDetails = GetPromotion(promotion.PromotionsId);
        //    if (promotionDetails != null)
        //    {
        //        if (file != null)
        //        {

        //            // delete old image

        //            File.Delete(promotionDetails.Image);

        //            // create new image
        //            string imagepath = await FileHelper.SaveImageAsync(promotionDetails.PromotionsId, file, "Promotions");
        //            promotionDetails.Image = imagepath;
        //        }
        //        promotionDetails.Discount = promotion.Discount;
        //        promotionDetails.Description = promotion.Description;
        //        promotionDetails.SellerId = promotion.SellerId;

        //        _db.SaveChanges();
        //    }
        //    return promotionDetails;
        //}

        //  delete category
        public bool Deletepromotion(int id , IIdentity seller)
        {


            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            Promotions promotion1 = _db.Promotions.FirstOrDefault(p => p.PromotionsId == id && p.SellerId == sellerId && p.Isdeleted == false);

            if (promotion1 != null)
            {
                promotion1.Isdeleted = true;
                _db.SaveChanges();
                return true;
            }

            return false;

        }
    }
}

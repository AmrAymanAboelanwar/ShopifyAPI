using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Repository
{

    public class BrandService
    {

        ShopifyContext _db;
        public BrandService(ShopifyContext db)
        {
            _db = db;
        }


        // get brand by id
        public Brand GetBrand(int id)
        {
          return _db.Brands.Where(b => b.BrandId == id && b.Isdeleted == false).FirstOrDefault();
           
        }




        // add Brand 
        public  Brand AddBrand(Brand brand)
        {
              _db.Brands.Add(brand);
               _db.SaveChanges();
            return brand;
        }


        // get sub category's brands

        public  List<Brand> getBrandsForSubCategory(int id)
        {
            SubCategory subCategory = _db.SubCategories.Where(s => s.SubCategoryId == id && s.Isdeleted ==false).FirstOrDefault();
            if (subCategory != null)
            {
              return  _db.SubCategories.Include(i=>i.Brands.Where(r=>r.Isdeleted==false)).Where(s => s.SubCategoryId == id && s.Isdeleted == false).FirstOrDefault().Brands;
            }
            return null;
        }



        //  edit Brand
        public Brand EditBrandAsync(Brand brand)
        {
            Brand brandDetails = GetBrand(brand.BrandId);
            if(brandDetails != null)
            {
                brandDetails.BrandName = brand.BrandName;
                _db.SaveChanges();
                return brandDetails;
            }
            return null;

        }


        //  delete Brand
        public Response DeleteBrand(int id)
        {
            Brand brandDetails = GetBrand(id);
            if (brandDetails != null)
            {
                if (!IsBrandInRelation(id))
                {
                    brandDetails.Isdeleted = true;
                    _db.SaveChanges();
                    return new Response { Status = "Success", Message = "Brand Deleted successully" };
                }
                return new Response { Status = "Error", Message = "Sorry this not allow" };
            }
            return new Response { Status="Error2" , Message="Brand Not Found"};

        }



        // IS Brand In Relation

        public bool IsBrandInRelation(int id)
        {
            List<Product> brands = _db.Brands.Include(p => p.Products.Where(e => e.IsdeletedBySeller == false)).FirstOrDefault(b => b.BrandId == id && b.Isdeleted == false).Products;
            if (brands.Count > 0)
                return true;
            return false;

        }



    }

}

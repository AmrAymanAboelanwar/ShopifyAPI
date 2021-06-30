using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Repository.Interfaces
{
    public class CategoryService
    {
        ShopifyContext _db;
        public CategoryService(ShopifyContext db)
        {
            _db = db;
        }

        // get all categories
        public List<Category> GetAllCategories()
        {
            return _db.Categories.Include(s=>s.SubCategories.Where(d=>d.Isdeleted==false)).Where(c=>c.Isdeleted==false).ToList();
        }

        // get category by id
        public Category GetCategory(int id)
        {
            Category category = _db.Categories.Include(e=>e.Isdeleted==false).SingleOrDefault(c => c.CategoryId == id&&c.Isdeleted==false);
            return category;
        }






        // add   category
        public async Task<Category> AddCategory(Category category, IFormFile file)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            if (file != null)
            {
                string imagepath = await FileHelper.SaveImageAsync(category.CategoryId, file, "Categories");
                category.Image = "http://localhost:23873/"+imagepath;
                _db.SaveChanges();
            }
            return category;
        }

    


        //  Edit Category
        public async Task<Category> EditCategoryAsync(Category category , IFormFile file)
        {
           Category categoryDetails = GetCategory(category.CategoryId);
            if (categoryDetails != null)
            {
                if (file != null)
                {

                    // delete old image

                    File.Delete(categoryDetails.Image);

                    // create new image
                    string imagepath = await FileHelper.SaveImageAsync(categoryDetails.CategoryId, file, "Categories");
                    categoryDetails.Image = imagepath;
                }
                categoryDetails.Name = category.Name;
                
                _db.SaveChanges();
            }
            return categoryDetails;           
        }

      

        //  delete category
        public Response DeleteCategory(int id)
        {
            Category category = GetCategory(id);
            if (category != null)
            {
                if (!IsCategoryInRelation(id))
                {
                     category.Isdeleted = true;
                    _db.SaveChanges();
                    return new Response { Status = "Success", Message = "Category Deleted successully" };
                }
                return new Response { Status = "Error", Message = "Sorry this not allow" };
            }
            return new Response { Status = "Error2", Message = "Category Not Found" };

        }





        // IS Category In Relation

        public bool IsCategoryInRelation(int id)
        {
            List<SubCategory> subCategories = _db.Categories.Include(p => p.SubCategories.Where(e => e.Isdeleted == false)).FirstOrDefault(b => b.CategoryId == id && b.Isdeleted == false).SubCategories;
            if (subCategories.Count > 0)
                return true;
            return false;

        }

    }
}

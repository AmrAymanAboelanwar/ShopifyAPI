using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Services.Interfaces;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;


namespace Shopify.Repository.Interfaces
{
    public class ProductService
    {


        ShopifyContext _db;
        private readonly IUriService _uriService;
        public ProductService(ShopifyContext db , IUriService uriService)
        {
            _db = db;
            _uriService = uriService;

        }



        // add product 

        public async Task<Response> AddProdctAsync(Product product, int inventoryId, IFormFile[] files, IIdentity seller)
        {
            string sellerId = HelperMethods.GetAuthnticatedUserId(seller);

            var result = _db.Inventories.FirstOrDefault(i => i.InventoryId == inventoryId && i.sellerId == sellerId);
            if (result != null)
            {
                try
                {
                    _db.Products.Add(product);
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    var c = e;
                }



                // add images of product 
                if (files != null)
                {
                    var imagePaths = await FileHelper.SaveImagesAsync(product.ProductId, files, "Products");

                    for (int i = 0; i < imagePaths.Count; i++)
                    {
                        _db.ProductImages.Add(new ProductImages { Image = IPort.Port + imagePaths[i], ProductId = product.ProductId });
                    }


                }

                // add to inventory

                _db.InventoryProducts.Add(new InventoryProduct { InventoryId = inventoryId, ProductId = product.ProductId, Quantity = product.Quantity });
                _db.SaveChanges();
                return new Response { Status = "Success", Message = "product added successfully", data = product };
            }
            return new Response { Status = "Error", Message = "Inventory not found" };
        }

        public Product GetProductById(int id)
           {
             return _db.Products.Include(rr=>rr.Reviews).ThenInclude(r=>r.Customer).ThenInclude(f=>f.ApplicationUser).Include(i=>i.ProductImages).Include(b=>b.Brand).FirstOrDefault(p=>p.ProductId == id && p.IsdeletedBySeller==false && p.Active==true);
           }







   


        // get product details by id  for seller 
        public Response GetProductByIdForSeller(int id, IIdentity seller)
        {

            string sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            List<Inventory> inventories = _db.Inventories.Include(r=>r.InventoryProducts.Where(e=>e.Isdeleted==false)).Where(s => s.sellerId == sellerId && s.Isdeleted==false).ToList();
           
            Product product = null;
            foreach (var invent in inventories)
            {
                if (product == null)
                {
                   var InventoryProduct = invent.InventoryProducts.FirstOrDefault(t => t.ProductId == id && t.Isdeleted == false);
                    if (InventoryProduct != null)
                    {
                        product = _db.Products.Include(r=>r.ProductImages.Where(r=>r.Isdeleted==false)).SingleOrDefault(p => p.ProductId == id && p.IsdeletedBySeller==false);
                    }
                }
            }
            if (product == null)
            {
                return new Response{Status="Error" , Message="NotFound" };
            }
            return new Response { Status = "Success" ,data=product};
        }




        public Response EditProductDataAsync(Product product , IIdentity seller)
        {
            var sellerId =  HelperMethods.GetAuthnticatedUserId(seller);
            List<Inventory> inventories = _db.Inventories.Include(ip=>ip.InventoryProducts).Where(i => i.sellerId == sellerId && i.Isdeleted==false).ToList();
            if (inventories != null) { 
            Product ProductFound = null;
                foreach (var inventory in inventories)
                {
                    var InventoryProduct = inventory.InventoryProducts.Where(p => p.ProductId == product.ProductId && p.Isdeleted == false).FirstOrDefault();
                    ProductFound = _db.InventoryProducts.Include(p => p.Product).FirstOrDefault(p => p.ProductId == product.ProductId).Product;
                    if (ProductFound != null)
                    {
                        ProductFound.ProductName = product.ProductName;
                        ProductFound.Price = product.Price;
                        ProductFound.Description = product.Description;
                        ProductFound.Details = product.Details;
                        ProductFound.Discount = product.Discount;
                        ProductFound.Size = product.Size;
                        ProductFound.Color = product.Color;
                        ProductFound.Brand = product.Brand;
                        ProductFound.Manufacture = product.Manufacture;
                        ProductFound.SubCategotyId = product.SubCategotyId;
                        _db.SaveChanges();
                        return new Response { Status = "Success", Message = "Product update successfully", data = ProductFound };

                    }
                    return new Response { Status = "Error", Message = "Product Not Found", data = ProductFound };

                }
            }

            return new Response { Status = "Error", Message = "Inventory Not Found" };
           
        }



        public async Task<List<Product>> GetWaitingProduct()
        {
            Task<List<Product>> waitingProducts = _db.Products.Include(r => r.InventoryProducts.Where(r=>r.Isdeleted==false)).ThenInclude(r => r.Inventory).ThenInclude(p => p.Seller).ThenInclude(r => r.ApplicationUser).Where(r => r.Active == false && r.IsdeletedBySeller == false).ToListAsync();
            return await waitingProducts;
        }





        internal List<List<Product>> GetSellerProducts(IIdentity seller)
        {
            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            List<Inventory> inventories = _db.Inventories.Include(ip => ip.InventoryProducts).ThenInclude(r=>r.Product).ThenInclude(p=>p.ProductImages).Where(i => i.sellerId == sellerId && i.Isdeleted == false).ToList();
           
            List<List<Product>> products = inventories.Select(f => f.InventoryProducts.Where(f=>f.Isdeleted==false).Select(f => f.Product).Where(r=>r.IsdeletedBySeller==false).ToList()).ToList();
           
            return products;

        }
         

        //public async Task<Response> EditProductImagesAsync(int id,IFormFile [] files, IIdentity seller)
        //{
        //    var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
        //    List<Inventory> inventories = _db.Inventories.Include(ip => ip.InventoryProducts).Where(i => i.sellerId == sellerId && i.Isdeleted == false).ToList();
        //    if (inventories != null)
        //    {
        //        Product ProductFound = null;
        //        foreach (var inventory in inventories)
        //        {
        //            var InventoryProduct = inventory.InventoryProducts.Where(p => p.ProductId == id && p.Isdeleted == false).FirstOrDefault();
        //            ProductFound = _db.InventoryProducts.Include(p => p.Product).ThenInclude(i=>i.ProductImages).FirstOrDefault(p => p.ProductId ==id).Product;
        //            if (ProductFound != null)
        //            {
        //                // delete old image
        //                for (int i = 0; i < ProductFound.ProductImages.Count; i++)
        //                {
        //                    File.Delete(ProductFound.ProductImages[i].Image);
        //                }

        //                // create new image
        //                List<string> pathes = await FileHelper.SaveImagesAsync(id, files, "Products");
        //                for (int i = 0; i < pathes.Count; i++)
        //                {
        //                    ProductFound.ProductImages[i].Image = pathes[i];
        //                }
        //                _db.SaveChanges();
        //                return new Response { Status = "Success", Message = "Product images update successfully", data = ProductFound.ProductImages };

        //            }
        //            return new Response { Status = "Error", Message = "Product Not Found", data = ProductFound };

        //        }
        //    }

        //    return new Response { Status = "Error", Message = "Inventory Not Found" };
        //}







        public async Task<Response> EditProductImagesAsync(int id, IFormFile[] files, IIdentity seller)
        {
            var sellerId = HelperMethods.GetAuthnticatedUserId(seller);
            List<Inventory> inventories = _db.Inventories.Include(ip => ip.InventoryProducts).Where(i => i.sellerId == sellerId && i.Isdeleted == false).ToList();
            if (inventories != null)
            {
                Product ProductFound = null;
                foreach (var inventory in inventories)
                {
                    var InventoryProduct = inventory.InventoryProducts.Where(p => p.ProductId == id && p.Isdeleted == false).FirstOrDefault();
                    ProductFound = _db.InventoryProducts.Include(p => p.Product).ThenInclude(i => i.ProductImages).FirstOrDefault(p => p.ProductId == id).Product;
                    if (ProductFound != null)
                    {
                         await UpdateImages(id, files, ProductFound);
                        return new Response { Status = "Success", Message = "Product images update successfully", data = ProductFound.ProductImages };

                    }
                    return new Response { Status = "Error", Message = "Product Not Found", data = ProductFound };
                }
            }

            return new Response { Status = "Error", Message = "Inventory Not Found" };
        }



        private async Task UpdateImages(int id, IFormFile[] files, Product ProductFound)
        {
            for (int i = 0; i < ProductFound.ProductImages.Count; i++)
            {
                if (ProductFound.ProductImages[i].Image.Substring(45) != files[i].FileName)
                {
                    int imagePosition = i + 1;
                    File.Delete(ProductFound.ProductImages[i].Image.Substring(23));
                    string path = await FileHelper.SaveEditImageAsync(id, imagePosition, files[i], "Products");
                    ProductImages productImages = _db.ProductImages.FirstOrDefault(p => p.ProductId == ProductFound.ProductId && p.Image == ProductFound.ProductImages[i].Image);
                    _db.ProductImages.Remove(productImages);
                    //_db.SaveChanges();
                    _db.ProductImages.Add(new ProductImages () { ProductId=ProductFound.ProductId,Image= "http://localhost:23873/" + path , Isdeleted=false});
                    _db.SaveChanges();

                }
            }
        }



        //public List<Product> SearchProduct(string name)
        //{
        //    return _db.Products.Include(i => i.ProductImages).Where(p => p.ProductName.Contains(name) && p.IsdeletedBySeller == false && p.Active == true).ToList();
        //}

        public List<Product> SearchProduct(string name)
        {
            return _db.Products.Include(r=>r.Brand).Include(s=>s.subCategory).Include(i => i.ProductImages).Where(p =>( p.ProductName.Contains(name) || p.Brand.BrandName.Contains(name) || p.subCategory.Name.Contains(name))&&p.Brand.Isdeleted==false &&p.subCategory.Isdeleted==false && p.IsdeletedBySeller == false && p.Active == true).ToList();

        }



        public List<Product>  GetTopSeales()
        {
            return  _db.Products.Include(r=>r.ProductImages).Where(p=>p.IsdeletedBySeller==false && p.Active ==true).OrderByDescending(p => p.QuantitySealed).ToList();
         
        }



        public List<Product> GetTopDeals()
        {
            return _db.Products.Include(r => r.ProductImages).Where(p => p.IsdeletedBySeller == false && p.Active == true && p.Discount>0).OrderByDescending(p => p.Discount).ToList();

        }



        // Get Products Patination


        public async Task<PagedPagination<List<Product>>> GetProductsAsync(int subCategoryId, PaginationFilter filter, HttpRequest request)
        {
            var route = request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _db.Products.Include(i => i.ProductImages).Where(s => s.SubCategotyId == subCategoryId && s.IsdeletedBySeller==false && s.Active==true)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();
            var totalRecords = await _db.Products.Where(s => s.SubCategotyId == subCategoryId && s.IsdeletedBySeller==false && s.Active==true).CountAsync();
           return PaginationHelper.CreatePagedReponse<Product>(pagedData, validFilter, totalRecords, _uriService, route);
        }




        //approve product

        public Response aproveProduct(int id)
        {
            if (GetProductByIdwaitingtobeApproved(id) != null)
            {

                Product product = _db.Products.FirstOrDefault(a => a.ProductId == id && a.IsdeletedBySeller == false && a.Active == false);
                if (product != null)
                {
                    product.Active = true;
                    _db.SaveChanges();
                    return new Response { Status = "success" };
                }
                return new Response { Status = "Error", Message = "^^ Product already Active ^^" };
            }
            return new Response { Status = "Error2", Message = "Not Found" };

        }



        public Product GetProductByIdwaitingtobeApproved(int id)
        {
            return _db.Products.Include(rr => rr.Reviews).ThenInclude(r => r.Customer).ThenInclude(f => f.ApplicationUser).Include(i => i.ProductImages).Include(b => b.Brand).FirstOrDefault(p => p.ProductId == id && p.IsdeletedBySeller == false && p.Active == false);
        }




        //  delete Status

        public Response DeleteProduct(int id)
        {
            Product product = GetProductById(id);
            if (product != null)
            {
                if (!IsProductInRelation(id))
                {
                    product.IsdeletedBySeller = true;
                    _db.SaveChanges();
                    return new Response { Status = "Success", Message = "product Deleted successully" };
                }
                return new Response { Status = "Error", Message = "Sorry this not allow" };
            }
            return new Response { Status = "Error2", Message = "product Not Found" };

        }





        // IS Status In Relation

        public bool IsProductInRelation(int id)
        {
            List<View> carts = _db.Products.Include(p => p.Views).FirstOrDefault(b => b.ProductId == id && b.IsdeletedBySeller == false).Views;
            if (carts.Count > 0)
                return true;
            return false;

        }

    }
}

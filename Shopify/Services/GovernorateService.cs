using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class GovernorateService
    {
        ShopifyContext _db;
        public GovernorateService(ShopifyContext db)
        {
            _db = db;
        }
        //get all governorate
        public List<Governorate> GetAllGovernorate()
        {
            return _db.Governorates.Where(g=>g.Isdeleted==false).ToList();
            

        }
        // get Governorate by id
        public Governorate GetGovernorate(int id)
        {
            return _db.Governorates.Where(b => b.GovernorateId == id && b.Isdeleted == false).FirstOrDefault();

        }
        // add Governorate 
        public Governorate AddGovernorate(Governorate governorate)
        {
            _db.Governorates.Add(governorate);
            _db.SaveChanges();
            return governorate;
        }

        //  edit Governorate
        public Governorate EditStatus(Governorate governorate)
        {
            Governorate governorateDetails = GetGovernorate(governorate.GovernorateId);
            if (governorateDetails != null)
            {
                governorateDetails.GovernorateName = governorate.GovernorateName;
                governorateDetails.Duration = governorate.Duration;
                governorateDetails.ShippingValue = governorate.ShippingValue;
                _db.SaveChanges();
                return governorateDetails;
            }
            return null;

        }


        //  delete Governorate

        public Response DeleteGovernorate(int id)
        {
            Governorate governorate = GetGovernorate(id);
            if (governorate != null)
            {
                if (!IsGovernorateInRelation(id))
                {
                    governorate.Isdeleted = true;
                    _db.SaveChanges();
                    return new Response { Status = "Success", Message = "governorate Deleted successully" };
                }
                return new Response { Status = "Error", Message = "Sorry this not allow" };
            }
            return new Response { Status = "Error2", Message = "governorate Not Found" };

        }





        // IS Governorate In Relation

        public bool IsGovernorateInRelation(int id)
        {
            List<Cart> carts = _db.Governorates.Include(p => p.Carts.Where(e => e.Isdeleted == false )).FirstOrDefault(b => b.GovernorateId == id && b.Isdeleted == false).Carts;
            if (carts.Count > 0)
                return true;
            return false;

        }


    }
}

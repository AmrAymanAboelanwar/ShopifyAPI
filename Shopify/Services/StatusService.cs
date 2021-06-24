using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class StatusService
    {
        ShopifyContext _db;
        public StatusService(ShopifyContext db)
        {
            _db = db;
        }
        public List<Status> GetAllStatus()
        {
            return _db.Statuses.ToList();

        }
        // get Status by id
        public Status GetStatus(int id)
        {
            return _db.Statuses.Where(b => b.StatusId == id && b.Isdeleted == false).FirstOrDefault();

        }
        // add Status 
        public Status AddStatus(Status status)
        {
            _db.Statuses.Add(status);
            _db.SaveChanges();
            return status;
        }

        //  edit Status
        public Status EditStatus(Status status)
        {
            Status statusDetails = GetStatus(status.StatusId);
            if (status != null)
            {
                statusDetails.StatusName = status.StatusName;
                _db.SaveChanges();
                return statusDetails;
            }
            return null;

        }





        //  delete Status

        public Response DeleteStatus(int id)
        {
            Status status = GetStatus(id);
            if (status != null)
            {
                if (!IsStatusInRelation(id))
                {
                    status.Isdeleted = true;
                    _db.SaveChanges();
                    return new Response { Status = "Success", Message = "status Deleted successully" };
                }
                return new Response { Status = "Error", Message = "Sorry this not allow" };
            }
            return new Response { Status = "Error2", Message = "status Not Found" };

        }





        // IS Status In Relation

        public bool IsStatusInRelation(int id)
        {
            List<Cart> carts = _db.Statuses.Include(p => p.Carts.Where(e => e.Isdeleted == false)).FirstOrDefault(b => b.StatusId == id && b.Isdeleted == false).Carts;
            if (carts.Count > 0)
                return true;
            return false;

        }


        



    }
}

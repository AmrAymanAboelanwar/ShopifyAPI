using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Helper
{
    public class FileHelper
    {

       static public async Task<string> SaveImageAsync(int categoryId, IFormFile file, string path)
        {
            var fileExtension = Path.GetExtension(Path.GetFileName(file.FileName));
           
            var newFileName = String.Concat(Convert.ToString(categoryId), fileExtension);
            string filePath = Path.Combine("Files/Images/" + path, newFileName);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {

                await file.CopyToAsync(fileStream);
            }
            return filePath;
        }



        static public async Task<List<string>> SaveImagesAsync(int NameByid, IFormFile[] files, string path)
        {
            List<string > filePaths = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                var fileExtension = Path.GetExtension(Path.GetFileName(files[i].FileName));

                var newFileName = String.Concat(Convert.ToString(NameByid)+"."+(i+1), fileExtension);
                filePaths.Add(Path.Combine("Files/Images/" + path, newFileName));

                using (Stream fileStream = new FileStream(filePaths[i], FileMode.Create))
                {

                    await files[i].CopyToAsync(fileStream);
                }
               
            }
            return filePaths;

        }



       static List<string> pathes = new List<string>() { "contract" , "Id" , "TaxCard" , "CommercialCard" };


        static public async Task<List<string>> SaveFilesSellerDocumentsAsync(string NameByid, IFormFile[] files)
        {


            List<string> filePaths = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                var fileExtension = Path.GetExtension(Path.GetFileName(files[i].FileName));

                var newFileName = String.Concat(Convert.ToString(NameByid) + "." + (i + 1), fileExtension);
                filePaths.Add(Path.Combine("Files/sellers/" + pathes[i],newFileName));

                using (Stream fileStream = new FileStream(filePaths[i], FileMode.Create))
                {

                    await files[i].CopyToAsync(fileStream);
                }

            }
            return filePaths;

        }

        public static async Task<string> SaveEditImageAsync(int id, int imagePosition, IFormFile file, string path)
        {
            var fileExtension = Path.GetExtension(Path.GetFileName(file.FileName));
            var newFileName="";
            if (imagePosition == 0)
            {
                newFileName = String.Concat(Convert.ToString(id), fileExtension);
            }
            else
            {
                 newFileName =  id + "." + imagePosition + fileExtension;
            }
            string filePath = Path.Combine("Files/Images/" + path, newFileName);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {

                await file.CopyToAsync(fileStream);
            }
            return filePath;
        }
    }
}

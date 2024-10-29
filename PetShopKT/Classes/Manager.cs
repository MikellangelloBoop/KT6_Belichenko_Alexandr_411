using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PetShopKT.Classes
{
    internal class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Data.User CurrentUser { get; set; }

        public static void GetImageData()
        {
            try
            {
                var list = Data.ShopEntities.GetContext().Product.ToList();
                foreach (var item in list)
                {
                    string path = Directory.GetCurrentDirectory() + @"\img\" + item.PhotoName;
                    if (File.Exists(path))
                    {
                        item.ProductPhoto = File.ReadAllBytes(path);
                    }
                }
                Data.ShopEntities.GetContext().SaveChanges();
            }
            catch (Exception)
            {

            }

        }
    }
}

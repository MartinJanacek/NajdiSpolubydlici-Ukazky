using NajdiSpolubydlícíWeb.Models;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.Drawing;
using Org.BouncyCastle.Utilities.Encoders;
using System.Text.RegularExpressions;
using NuGet.Packaging.Signing;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NajdiSpolubydlícíWeb.Handlers
{
    public static class ImageHandler
    {
        private static string StripBase64Header(string base64)
        {
            return Regex.Replace(base64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
        }

        public static string GetFileExtension(string base64String)
        {
            return base64String.Substring(0, 5).ToUpper() switch
            {
                "IVBOR" => "png",
                "/9J/4" => "jpg",
                _ => string.Empty,
            };
        }
     
        private static string FotoFolder(string type)
        {
            string folder = @$"./wwwroot/UserImgs/{type}";
            return folder;
        }
        
        private static Image ResizeImage(Image img, int MaxDimension)
        {
            if (img.Width > img.Height && img.Width > MaxDimension)
            {
                int newHeight = (int)(MaxDimension * ((double)img.Height / (double)img.Width));
                img.Mutate(x => x.Resize(MaxDimension, newHeight));
            }
            else if (img.Height >= img.Width && img.Height > MaxDimension)
            {
                int newWidth = (int)(MaxDimension * ((double)img.Width / (double)img.Height));
                img.Mutate(x => x.Resize(newWidth, MaxDimension));
            }

            return img;
        }
        
        public static bool SaveTitleImage(string? titleBase64, int id, string typ)
        {
            if (string.IsNullOrEmpty(titleBase64)) return true;

            titleBase64 = StripBase64Header(titleBase64);
            string extension = GetFileExtension(titleBase64);

            if (string.IsNullOrEmpty(extension)) return false;

            string folder = FotoFolder(typ);

            byte[]? bytes = Convert.FromBase64String(titleBase64);
            Image image = Image.Load(bytes);

            string ObrPathSystem = folder + @"/" + id + @"/" + "img.jpeg";
            image = ResizeImage(image, 1440);
            image.Save(ObrPathSystem);

            ObrPathSystem = folder + @"/" + id + @"/" + "minimg.jpeg";
            image = ResizeImage(image, 300);
            image.Save(ObrPathSystem);

            image.Dispose();

            return true;
        }

        public static bool SaveImages(NabidkaViewModel objView, Nabidka obj)
        {
            if (string.IsNullOrEmpty(objView.Base64s)) return true;

            string[] base64 = objView.Base64s.Split("|", StringSplitOptions.RemoveEmptyEntries);
            string folder = FotoFolder("Nabidka");

            for (int i = 0; i < base64.Length; i++)
            {
                base64[i] = StripBase64Header(base64[i]);
                string extension = GetFileExtension(base64[i]);
                
                if (string.IsNullOrEmpty(extension)) return false;

                byte[] bytes = Convert.FromBase64String(base64[i]);
                Image image = Image.Load(bytes);

                string ObrPathSystem = folder + @"/" + obj.Id + @"/" + $"img{i}.jpeg";
                image = ResizeImage(image, 1440);
                image.Save(ObrPathSystem);
                
                image.Dispose();
            }

            return true;
        }

        public static void CreateDirectory(int id, string type)
        {
            string folder = FotoFolder(type);
            string path = $@"{folder}/{id}";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static void DeleteFiles(int id, string type)
        {
            string folder = FotoFolder(type);
            string path = $@"{folder}/{id}";
            DirectoryInfo directory = new($@"{folder}/{id}");

            if (Directory.Exists(path))
            {
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        public static void DeleteWholeDirectory(int id, string type)
        {
            string folder = FotoFolder(type);
            string path = $@"{folder}/{id}";

            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
    }
}

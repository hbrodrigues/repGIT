﻿using AgainArt.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AgainArt.Controllers
{
    public class ArtWorkController : Controller
    {
        // GET: ArtWork
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(HttpPostedFileBase file, string txtArtDescription)
        {
            if (ModelState.IsValid)
            {



                if (ModelState.IsValid)
                {

                    if (file != null)
                    {
                        string savedFileName = string.Empty;
                        string savedThumbFile = string.Empty;

                        try
                        {
                            ArtWork objArtWork = new ArtWork();
                            //objArtWork.FileURL = file.FileName;
                            objArtWork.OriginalName = file.FileName;
                            objArtWork.IdArtista = 1;// na mao o codigo do artista
                            objArtWork.GeneratedName = Path.GetFileNameWithoutExtension(objArtWork.OriginalName) + "_" + Guid.NewGuid() + Path.GetExtension(objArtWork.OriginalName);
                            objArtWork.ImageData = new byte[file.ContentLength];
                            objArtWork.Description = txtArtDescription;
                            objArtWork.ContentType = file.ContentType;


                            //attach the uploaded image to the object before saving to Database
                            //artwork.ImageMimeType = image.ContentLength;
                            //artwork.ImageData = new byte[image.ContentLength];
                            //image.InputStream.Read(artwork.ImageData, 0, image.ContentLength);

                            //Save image to file
                            //var filename = image.FileName;
                            var filePathOriginal = Server.MapPath("/Content/ArtWorkImages/Original");
                            var filePathThumbnail = Server.MapPath("/Content/ArtWorkImages/Thumbnail");
                            savedFileName = Path.Combine(filePathOriginal, objArtWork.GeneratedName);
                            savedThumbFile = Path.Combine(filePathThumbnail, objArtWork.GeneratedName);

                            file.SaveAs(savedFileName);
                            objArtWork.FileURL = Path.Combine("/Content/ArtWorkImages/Original", objArtWork.GeneratedName);

                            //Read image back from file and create thumbnail from it
                            var imageFile = savedFileName; //Path.Combine(Server.MapPath("~/Content/ArtWorkImages/Original"), objArtWork.GeneratedName);
                            using (var srcImage = Image.FromFile(imageFile))
                            using (var newImage = new Bitmap(100, 100))
                            using (var graphics = Graphics.FromImage(newImage))
                            using (var stream = new MemoryStream())
                            {
                                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                graphics.DrawImage(srcImage, new Rectangle(0, 0, 100, 100));
                                newImage.Save(stream, ImageFormat.Png);
                                objArtWork.ThumbNailURL = Path.Combine("/Content/ArtWorkImages/Thumbnail", objArtWork.GeneratedName);
                                //HttpPostedFileBase oi = newImage;
                                newImage.Save(savedThumbFile);
                                //var thumbNew = File(stream.ToArray(), "image/png");

                                //artwork.ArtworkThumbnail = thumbNew.FileContents;


                            }
                            MVCArtistContext db = new MVCArtistContext();
                            objArtWork.Artista = db.Artista.FirstOrDefault(a => a.Id == objArtWork.IdArtista);
                            db.ArtWork.Add(objArtWork);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            if (System.IO.File.Exists(savedFileName))
                            {
                                System.IO.File.Delete(savedFileName);
                            }

                            if (System.IO.File.Exists(savedThumbFile))
                            {
                                System.IO.File.Delete(savedThumbFile);
                            }

                        }
                    }
                }
            }
            return View("Index");

        }


        public ActionResult Add()
        {
            return View("Index");
        }

        public ActionResult List(ArtWork objSearch = null)
        {
            MVCArtistContext db = new MVCArtistContext();
            List<ArtWork> lstArtWork = null;

            if (objSearch == null || string.IsNullOrEmpty(objSearch.Description))
            {
                lstArtWork = db.ArtWork.ToList();
            }
            else
            {
                lstArtWork = db.ArtWork.Where(a => a.OriginalName.Contains(objSearch.OriginalName)).ToList();
            }

            return View("index", new Gallery() { LstArtWork = lstArtWork });
        }

        public List<ArtWork> List()
        {
            MVCArtistContext db = new MVCArtistContext();
            List<ArtWork> lstArtWork = null;

            lstArtWork = db.ArtWork.ToList();

            return lstArtWork;
        }

    }
}
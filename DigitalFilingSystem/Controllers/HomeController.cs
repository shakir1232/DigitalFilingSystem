using DigitalFilingSystem.BLL;
using DigitalFilingSystem.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DigitalFilingSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ImageIndexBLL _imageIndexBLL;

        public HomeController()
        {
            _imageIndexBLL = new ImageIndexBLL();
                        
        }

        [Authorize(Roles ="Admin,Dashboard")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDashboard()
        {
            return Json(_imageIndexBLL.GetDashboard(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetReport(string id)
        {
            return Json(_imageIndexBLL.GetDashboard(id), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Upload")]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(ImageUpload upload)
        {
            try
            {
                if (upload == null)
                {
                    throw new Exception("Data not found.");
                }

                var indexId = _imageIndexBLL.InsertImageIndex(upload, User.Identity.Name);
            
                return Json(new { type = "success", message = "Save successful", data = indexId.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "failed", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult UpdateDocument(ImageUpload upload)
        {
            if (upload == null)
            {
                throw new Exception("Data not found.");
            }

            var indexId = _imageIndexBLL.UpdateImageIndex(upload, User.Identity.Name);
            return Json(indexId.ToString());
        }

        public ActionResult GetRecentImage()
        {
            try
            {
                var searchResult = _imageIndexBLL.GetTodayImage();

                return Json(new { message = "success", data = searchResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult DownloadFile(string id)
        {
            try
            {
                var image = _imageIndexBLL.GetImageById(id);
                string fileName = string.Empty;
                string fileimage = string.Empty;
                if (string.IsNullOrEmpty(image.FileName))
                {

                    string fileExtension = image.Photo.Split(';')[0].Split('/')[1];
                    fileimage = image.Photo.Split(',')[1];
                    if (fileExtension.Contains("document"))
                    {
                        fileExtension = "docx";
                    }
                    fileName = "document." + fileExtension;
                }
                else
                {
                    fileName = image.FileName;
                    fileimage = image.Photo;
                }     

                byte[] fileBytes = Convert.FromBase64String(fileimage);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetFile(string id)
        {
            try
            {
                var image = _imageIndexBLL.GetImageById(id);
                string fileName = string.Empty;
                string fileimage = string.Empty;
                if (string.IsNullOrEmpty(image.FileName))
                {
                    fileimage = image.Photo.Split(',')[1];
                }
                else
                {
                    fileimage = image.Photo;
                }

                return Json(image.Photo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult DeleteFile(string id)
        {
            try
            {
                _imageIndexBLL.DeleteFileById(id);
                
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        [Authorize(Roles ="Admin,Search")]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchImage(ImageUpload search)
        {
            try
            {
                if (search == null)
                {
                    throw new Exception("Search parameter not found.");
                }
                var searchResult = _imageIndexBLL.SearchImage(search);

                return Json(new { message = "success", data = searchResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    var indexId = Request.Params["indexId"];
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string result = string.Empty;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        using (BinaryReader b = new BinaryReader(file.InputStream))
                        {
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            result = Convert.ToBase64String(binData);
                            _imageIndexBLL.InsertImage(result, fname, new Guid(indexId));
                        }
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpGet]
        public ActionResult UploadDocumentFiles()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    var fileName = Request.Params["fileName"];
                    HttpFileCollectionBase files = Request.Files;
                     
                        HttpPostedFileBase file = files[0];
                        
                        string filePath = Path.Combine(Server.MapPath(" "),"../Uploads", fileName);
                        file.SaveAs(filePath);
                  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult SearchInAdvance(ImageUpload search)
        {
            try
            {
                if (search == null)
                {
                    throw new Exception("Search parameter not found.");
                }
                var searchResult = _imageIndexBLL.SearchInAdnvace(search);

                return Json(new { type = "success", message = "", data = searchResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "failed", message = ex.Message, data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetImageIndexById(string id)
        {
            try
            {
                ImageUpload imageUpload = _imageIndexBLL.GetImageIndexById(id);
                return Json(new { message = "success", data = imageUpload }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetLastImageIndex()
        {
            try
            {
                var imageIndex = _imageIndexBLL.GetLastImageIndex();
                return Json(new { message = "success", data = imageIndex }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult GetAllUser()
        {
            var users = _imageIndexBLL.GetAllUser();
            return Json(users, JsonRequestBehavior.AllowGet);
        }




    }
}
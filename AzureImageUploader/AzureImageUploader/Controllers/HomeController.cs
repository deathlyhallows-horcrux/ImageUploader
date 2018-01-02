using AzureImageUploader.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
//using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using TinifyAPI;
//using System.Directory;
using TinyPng;
using UnifyConnectLibrary;
//using System.Drawing.Imaging;

namespace AzureImageUploader.Controllers
{
    public class HomeController : Controller
    {
        string message = "";
        string fileName1 = "";
        static string format;
        int size;

        private string StorageConnectionString;
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("Index")]
        [HttpPost]
        public ActionResult Index(string storage)
        {
            bool savedsuccess = true;
            string storageaccount = storage.ToLower();
            try
            {
                string fName = "";
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    fName = file.FileName.Substring(0, file.FileName.IndexOf('.'));
                    format = file.FileName.Substring(file.FileName.IndexOf('.'));
                    if (file != null && file.ContentLength > 0)
                    {
                        if (fName.Contains('_'))
                        {
                            fName = fName.Substring(0, fName.IndexOf('_'));
                        }
                        // Check if item exists
                        //    var company = (int)(EGenConType.Kretek_International);
                        try
                        {
                            //var itemType = UnifyConnect.GetItemType(fName, 2);
                            ////var itemExists = new UnifyItem(fName, true, 2);
                            ////var productLine = UnifyConnect.GetProductLines("x", 2).FirstOrDefault(x => x.ProductLineID == fName);
                            ////if (itemExists.BINNMBR != null)
                            //if (!string.IsNullOrWhiteSpace(itemType) && itemType != "2")
                            //{
                                //return Json(new { responseText = itemExists.BINNMBR.ToString() }, JsonRequestBehavior.AllowGet);
                                int width, height;
                                string pathString = Server.MapPath("~/Images");
                                string fileNameas = Path.GetFileName(file.FileName.Substring(0, file.FileName.IndexOf('.'))).ToString();
                                fileName1 = fileNameas.ToUpper();
                                bool isExists = Directory.Exists(pathString);
                                if (!isExists) Directory.CreateDirectory(pathString);
                                string uploadpath = Path.Combine(pathString, fileName1 + format);
                                file.SaveAs(uploadpath);

                                using (Image img = Image.FromFile(uploadpath))
                                {
                                    width = img.Width;
                                    height = img.Height;
                                }
                                if (width >= 1200 && height >= 1200)
                                {
                                    size = 1200;
                                    message = ResizeImages(uploadpath, fileName1, storage, width);
                                    if (message == "")
                                    {
                                        message = AzureUpload(fileName1, uploadpath, storageaccount);
                                       
                                    }
                                }
                                else if ((width >= 400 && width <= 1200))
                                {
                                    size = 400;
                                    message = ResizeImages(uploadpath, fileName1, storage, width);
                                    if (message == "")
                                    {
                                        message = AzureUpload(fileName1, uploadpath, storageaccount);
                                     
                                    }
                                }
                                else if ((width <= 400))
                                {
                                    size = 150;
                                    message = ResizeImages(uploadpath, fileName1, storage, width);
                                    if (message == "")
                                    {
                                        message = AzureUpload(fileName1, uploadpath, storageaccount);
                                       
                                    }
                                }
                                else
                                {
                                    savedsuccess = false;
                                }
                            //}
                            //else if (itemType == "2" || itemType == "")
                            //{
                            //    return Json(new { responseText = "The item cannot be uploaded as it is discontinued or does not exist." }, JsonRequestBehavior.AllowGet);
                            //}

                        }
                        catch (System.Exception ex)
                        {
                            return Json(new { responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                message = "The image " + fileName1 + " could not be uploaded to Azure account " + storage + ". " + ex.ToString();
                SendEmail("meghanasampelli@kretek.com", "AzureUploader@kretek.com", "", "", "Image Upload failed for" + fileName1, false, ex.ToString(), "");
            }
            if (savedsuccess)
            {
                return Json(new { responseText = message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { responseText = message }, JsonRequestBehavior.AllowGet);
            }

        }
    
        //public ActionResult Index(string storage)
        //{       
        //    bool savedsuccess = true;
        //    string storageaccount = storage.ToLower();
        //    //if (Request.Files != null)
        //    //{
        //    try
        //    {
        //        string fName = "";
        //        foreach (string fileName in Request.Files)
        //        {
        //            HttpPostedFileBase file = Request.Files[fileName];
        //            fName = file.FileName.Substring(0, file.FileName.Length - 4);
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                //Check if item exists   


        //                    //    var company = (int)(EGenConType.Kretek_International);

        //                    // try
        //                    // {
        //                    //     var binNumber = UnifyConnect.GetItemBinNumber(fName, 2);
        //                    //     //var itemExists = new UnifyItem(fName, true, 2);
        //                    //     //var productLine = UnifyConnect.GetProductLines("x", 2).FirstOrDefault(x => x.ProductLineID == fName);
        //                    //     //if (itemExists.BINNMBR != null)
        //                    //     if (!string.IsNullOrWhiteSpace(binNumber))
        //                    //     {
        //                    //         //return Json(new { responseText = itemExists.BINNMBR.ToString() }, JsonRequestBehavior.AllowGet);
        //                    //         return Json(new { responseText = binNumber }, JsonRequestBehavior.AllowGet);
        //                    //     }
        //                    // }
        //                    //catch(Exception ex)
        //                    // {
        //                    //     return Json(new { responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
        //                    // }


        //                    //if (itemExists.ITEMTYPE.Equals(1))
        //                    //{
        //                    int width, height;
        //                    string pathString = Server.MapPath("~/Images");
        //                    string fileNameas = Path.GetFileName(file.FileName).ToString();
        //                    fileName1 = fileNameas.Substring(0, fileNameas.Length - 4).ToUpper();
        //                    bool isExists = Directory.Exists(pathString);
        //                    if (!isExists) Directory.CreateDirectory(pathString);
        //                    string uploadpath = Path.Combine(pathString, fileName1 + ".png");
        //                    // var uploadpath = string.Format("{0}\\{1}", pathString, file.FileName);
        //                    //if (System.IO.File.Exists(uploadpath))
        //                    //{

        //                    //        System.IO.File.;

        //                    //    using (var stream = System.IO.File.Open(Path.Combine(pathString, fileName1 + "med.png"), FileMode.Open))
        //                    //    {
        //                    //        System.IO.File.Delete(uploadpath);
        //                    //    }
        //                    //    //try
        //                    //    //{
        //                    //    //    System.IO.File.Delete(uploadpath);

        //                    //    //    // System.IO.File.Delete(uploadpath.Substring(0, uploadpath.Length - 4) + "med.png");
        //                    //    //    // System.IO.File.Delete(Path.Combine(pathString, fileName1 + "thumb.png")); 
        //                    //    //}
        //                    //    //catch (System.IO.IOException ex)
        //                    //    //{
        //                    //    //    System.Diagnostics.Debug.WriteLine("Storing images failed", ex.Message);
        //                    //    //}
        //                    //}                       
        //                    file.SaveAs(uploadpath);

        //                    using (Image img = Image.FromFile(uploadpath))
        //                    {
        //                        width = img.Width;
        //                        height = img.Height;
        //                    }
        //                    if (width >= 1200 && height >= 1200)
        //                    {
        //                        size = 1200;
        //                        ////  Tiny PNG compression
        //                        //string key = "8PL-FrvU_A7C2Zd2Av3ybhPvj0q5JlF5";
        //                        //string input = uploadpath;
        //                        //string url = "https://api.tinify.com/shrink";
        //                        //WebClient client = new WebClient();
        //                        //string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
        //                        //client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);
        //                        //try
        //                        //{
        //                        //    client.UploadData(url, System.IO.File.ReadAllBytes(input));
        //                        //    /* Compression was successful, retrieve output from Location header. */
        //                        //    client.DownloadFile(client.ResponseHeaders["Location"], uploadpath);
        //                        //}
        //                        //catch (WebException ex)
        //                        //{
        //                        //    /* Something went wrong! You can parse the JSON body for details. */
        //                        //    System.Diagnostics.Debug.WriteLine("Compression failed.", ex.Message, ex.InnerException);
        //                        //}
        //                        //  var task = TinyPNG(uploadpath);
        //                        message = ResizeImages(uploadpath, fileName1, storage, width);
        //                        if (message != null)
        //                        {
        //                            message = AzureUpload(fileName1, uploadpath, storageaccount);
        //                        }
        //                    }
        //                    else if ((width >= 400 && width <= 1200))
        //                    {
        //                        size = 400;
        //                        message = ResizeImages(uploadpath, fileName1, storage, width);
        //                        if (message != null)
        //                        {
        //                            message = AzureUpload(fileName1, uploadpath, storageaccount);
        //                        }
        //                    }
        //                    else if ((width <= 400))
        //                    {
        //                        size = 150;
        //                        message = ResizeImages(uploadpath, fileName1, storage, width);
        //                        if (message != null)
        //                        {
        //                            message = AzureUpload(fileName1, uploadpath, storageaccount);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        savedsuccess = false;
        //                    }

        //                //catch (Exception ex)
        //                //{
        //                //    System.Diagnostics.Debug.WriteLine("Storing images failed", ex.Message, ex.InnerException);
        //                //    savedsuccess = false;
        //                //    ViewBag.imguploadazure = "not uploaded to azure";
        //                //    message = "The image " + fileName1 + " could not be uploaded to Azure account " + storage + ". " + ex.ToString();
        //                //}                     
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {                
        //        message = "The image " + fileName1 + " could not be uploaded to Azure account " + storage + ". "+ ex.ToString();
        //    }
        //    if (savedsuccess)
        //    {
        //        return Json(new { responseText = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { responseText = message }, JsonRequestBehavior.AllowGet);
        //    }

        //}


        public string AzureUpload(string fileName1, string uploadpath, string storageaccount)

        {
            //Uploading images to Azure
            //string StorageConnectionString;          
            try
            {
                if (storageaccount == "espimages")
                {
                    StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=espimages;AccountKey=7IapRi51a1IgHXyHZQ4kciMgMBfJmm43QfcQpVKWgXwgYaqsNYUAA3khWjIO9acjGPE/qbl0Hcp/zhZplJZ9JQ==";
                }
                else if (storageaccount == "pkimages")
                {
                    StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=FLgT0vRmlqc9EDvDwf6UENcueHJ/Co4yq2rMjb1wqSISM7boQxj+IlcN1I9cK+tSDf+qtf0CQJLIipgP31qeTA==";
                }
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(storageaccount);

                if (System.IO.File.Exists(uploadpath.Substring(0, uploadpath.IndexOf('.')) + "med" + format))
                {
                    CloudBlockBlob blockBlobmed = container.GetBlockBlobReference(fileName1 + "med.png");
                    blockBlobmed.Properties.ContentType = "image/png";
                    blockBlobmed.Properties.CacheControl = "public, max-age=604800";
                    using (var fileStream1 = System.IO.File.OpenRead(uploadpath.Substring(0, uploadpath.IndexOf('.')) + "med" + format))
                    {
                        //var filestream1 = System.IO.File.OpenRead(uploadpath.Substring(0, uploadpath.Length - 4) + "med.png");
                        blockBlobmed.UploadFromStream(fileStream1);
                    }
                }
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName1 + ".png");
                blockBlob.Properties.ContentType = "image/png";
                blockBlob.Properties.CacheControl = "public, max-age=604800";
                CloudBlockBlob blockBlobthumb = container.GetBlockBlobReference(fileName1 + "thumb.png");
                blockBlobthumb.Properties.ContentType = "image/png";
                blockBlobthumb.Properties.CacheControl = "public, max-age=604800";
                // Save blob contents to a file.
                using (var fileStream = System.IO.File.OpenRead(uploadpath))
                {
                    var fileStream2 = System.IO.File.OpenRead(uploadpath.Substring(0, uploadpath.IndexOf('.')) + "thumb" + format);
                    blockBlob.UploadFromStream(fileStream);
                    blockBlobthumb.UploadFromStream(fileStream2);
                }
                message = "The image " + fileName1 + " is uploaded to Azure account " + storageaccount;
                //azureLoad(storageaccount);
               
            }
            catch (System.Exception)
            {
                message = "The image " + fileName1 + " could not be uploaded to Azure account " + storageaccount;
            }
            return message;
        }

        //[HttpPost]
        //public ActionResult Search(string image, string account)
        //{
        //    // Azuredelete(filename, storageaccount);
        //    image = image.ToUpper();
        //    if (image != null && account != null)
        //    {
        //        ViewBag.linktoimage = "https://" + account.ToLower() + ".blob.core.windows.net/" + account.ToLower() + "/" + image + "";
        //        HttpWebResponse response = null;
        //        var request = (HttpWebRequest)WebRequest.Create(ViewBag.linktoimage);
        //        request.Method = "HEAD";
        //        try
        //        {
        //            response = (HttpWebResponse)request.GetResponse();
        //            ViewBag.imagestatus = "exists";
        //        }
        //        catch (WebException ex)
        //        {
        //            /* A WebException will be thrown if the status of the response is not `200 OK` */
        //            ViewBag.imagestatus = "Image <b>" + image + "</b>" + " does not exist in " + "<b>" + account + "</b>";
        //        }
        //        finally
        //        {
        //            // Don't forget to close your response.
        //            if (response != null)
        //            {
        //                response.Close();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //imagetodelete.Style.Add("display", "none"); 
        //        ViewBag.enterfilename = "Enter a filename to search";
        //    }
        //    return View("Index");
        //}
        [ActionName("AzureDelete")]
        [HttpPost]
        public ActionResult AzureDelete(string image, string account)
        {
             var url = ViewBag.linktoimage;

            var storageaccount = account.ToLower();
            if (storageaccount == "espimages")
            {
                StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=espimages;AccountKey=7IapRi51a1IgHXyHZQ4kciMgMBfJmm43QfcQpVKWgXwgYaqsNYUAA3khWjIO9acjGPE/qbl0Hcp/zhZplJZ9JQ==";
            }
            else
            {
                StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=FLgT0vRmlqc9EDvDwf6UENcueHJ/Co4yq2rMjb1wqSISM7boQxj+IlcN1I9cK+tSDf+qtf0CQJLIipgP31qeTA==";
            }
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(storageaccount);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(image);
                //CloudBlockBlob blockBlobMed = container.GetBlockBlobReference(image.Substring(0, image.Length - 4) + "med.png");
                //CloudBlockBlob blockBlobThumb = container.GetBlockBlobReference(image.Substring(0, image.Length - 4) + "thumb.png");

                blockBlob.Delete();
                //blockBlobMed.Delete();
                //blockBlobThumb.Delete();
                azurePurge(image, account);

                ViewBag.deletestatus = "yes";
                 ViewBag.deletestatus = image + " is deleted from " + storageAccount;
                return Json(new { success = true, responseText = "The image " + image + " is deleted from " + account }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                ViewBag.error = "Image could not be deleted due to an exception";
                return Json(new { success = false, responseText = "The image is not deleted due to an exception" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult AzureListBlobs(string image, string account, int page = 1, int pageSize = 25)
        {
           // AzureReport();
            image = image.ToUpper();
            DataTable tabe = new DataTable();
            tabe = UnifyConnect.GetAllItemsPK(4);
            //UnifyConnect.GetItems(image, 2);
            //var itemExists = new UnifyItem(image, true, 2);

            //if (itemExists.ITEMTYPE == 2 || itemExists.ITEMTYPE == 0)
            //{
            if (account != null)
                {
                    if (image.Length >= 3 && image != null)
                    {
                        var storageaccount = account.ToLower();
                        if (storageaccount == "espimages")
                        {
                            StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=espimages;AccountKey=7IapRi51a1IgHXyHZQ4kciMgMBfJmm43QfcQpVKWgXwgYaqsNYUAA3khWjIO9acjGPE/qbl0Hcp/zhZplJZ9JQ==";
                        }
                        else
                        {
                            StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=FLgT0vRmlqc9EDvDwf6UENcueHJ/Co4yq2rMjb1wqSISM7boQxj+IlcN1I9cK+tSDf+qtf0CQJLIipgP31qeTA==";
                        }
                        //  StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=espimages;AccountKey=7IapRi51a1IgHXyHZQ4kciMgMBfJmm43QfcQpVKWgXwgYaqsNYUAA3khWjIO9acjGPE/qbl0Hcp/zhZplJZ9JQ==";

                        try
                        {
                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                            CloudBlobContainer container = blobClient.GetContainerReference(storageaccount);
                            //   List<string> ListImages = new List<string>();
                            var model = new List<HomePageModel>();
                            var results = new List<HomePageModel>();
                            // HomePageModel hpm = new HomePageModel();
                            foreach (IListBlobItem item in container.ListBlobs(null, true))
                            {
                                CloudBlockBlob blob = (CloudBlockBlob)item;
                                model.Add(new HomePageModel { ImageList = blob.Uri.ToString() });
                                // hpm.Images.Add(blob.Uri.ToString());
                                // System.Diagnostics.Debug.WriteLine(blob.Uri);
                            }
                            // var resultList = new List<HomePageModel>();
                            // var resultList = new List<string>();
                            foreach (var item in model)
                            {
                                var index = (item.ImageList.ToString()).LastIndexOf('/');
                                var imagename = (item.ImageList.ToString()).Substring(index + 1);
                                if (imagename.StartsWith(image))
                                {
                                    results.Add(new HomePageModel { ImageFilter = imagename.ToString() });
                                    ViewBag.Url = item.ImageList;
                                }
                            }
                            ViewBag.account = storageaccount;
                            if (results.Count != 0)
                            {
                                ViewBag.sendMsg = "<b>" + results.Count + "</b>" + " image(s) starting with " + "<b>" + image + "</b>" + " are found";
                                return View("Index", results.ToPagedList(page, pageSize));
                            }
                            else
                            {
                                ViewBag.sendMsg = "No images were found for the search " + "<b>" + image + "</b>";
                                return View("Index");
                            }

                        }
                        catch (System.Exception)
                        {
                            ViewBag.sendMsg = "Unknown error occurred due to which the results could not be displayed.";
                            return View("Index");
                        }
                    }
                    else
                    {
                        ViewBag.sendMsg = "Type a minimum of 3 characters to search for images.";
                        return View("Index");
                    }
                }
                else
                {
                    ViewBag.sendMsg = "Select an account from the dropdown list.";
                    return View("Index");
                }
            //}
            //else
            //{
            //    ViewBag.sendMsg = "The Item no longer exists or is discontinued.";
            //    return View("Index");
            //}
        }       

        //CDN purge
        public static HttpWebResponse azurePurge(string image, string account)
        {

            //Data parameter Example
            var data = "/" + account + "/" + image;
            var body = JsonConvert.SerializeObject(data);
            string url = "https://portal.azure.com/#resource/subscriptions/c0670002-2f60-4ccb-adca-6009dd5fd90c/resourcegroups/pk/providers/Microsoft.Cdn/profiles/pkcdn/endpoints/pkcdn/purge?api-version=2016-10-02";
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;

            var streamWriter = new StreamWriter(httpRequest.GetRequestStream());
            streamWriter.Write(data);
            streamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            response.Close();
            return response;
        }
        //CDN update
 
        [ActionName("AzurePurgeAllPK")]
        [HttpPost]
        public ActionResult AzurePurgeAllPK(string purge)
        {
            var message = "";
            string clientId = "245485b9-1651-40df-bd5d-405b92b6450a";
            string clientSecret = "1bVycgLfKla4KWwNpFPPXf4iZ13/R8gWOSB4V0EcryE=";
            string uri = @"https://management.azure.com/subscriptions/c0670002-2f60-4ccb-adca-6009dd5fd90c/resourcegroups/pk/providers/Microsoft.Cdn/profiles/pkcdn/endpoints/pkcdn/purge?api-version=2015-06-01";

            var authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/helpdeskkretek.onmicrosoft.com");
            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
            Task<AuthenticationResult> resultstr = authenticationContext.AcquireTokenAsync("https://management.core.windows.net/", clientCredential);

            WebClient client = new WebClient();
            //authentication using the Azure AD application
            var token = resultstr.Result.AccessToken;
            client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " +token);
            client.Headers.Add("api-version:2015-06-01");
            client.Headers.Add("Content-Type", "application/json");

            var bodyText = string.Empty;

            //For individual files
            //dynamic content = new { ContentPaths = new List<string>() { “/1.jpg”, “/2.jpg” } };
            //bodyText = JsonConvert.SerializeObject(content);

            //For purge all (*.*)
            bodyText = "{\"ContentPaths\":[\"/*\"]}";
           // var body = JsonConvert.SerializeObject(bodyText);

            try
            {
                var result = client.UploadString(uri, bodyText);
            }
            catch (System.Exception ew)
            {
                //handle the exception here
                message = ew.ToString();
                ViewBag.purge = ew.ToString();
              //  return Json(new { responseText = message }, JsonRequestBehavior.AllowGet);
                return View("Index");
            }
            ViewBag.purge = "PK CDN links are updated!";
            message = "PK CDN links are updated!";
          //  return Json(new { responseText =  message }, JsonRequestBehavior.AllowGet);
            return View("Index");
        }

        [ActionName("AzurePurgeAllEsp")]
        [HttpPost]
        public ActionResult AzurePurgeAllESP(string purge)
        {
            var message = "";
            string clientId = "ba317958-d219-4c93-acf0-c4cc1ff728b7";
            string clientSecret = "C1h9nc17LTRUD0uGysK91ZpwPV5g8ZQIXuhesclo/Rk=";
            string uri = @"https://management.azure.com/subscriptions/c0670002-2f60-4ccb-adca-6009dd5fd90c/resourcegroups/ESP/providers/Microsoft.Cdn/profiles/espcdn/endpoints/espcdn/purge?api-version=2015-06-01";

            var authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/helpdeskkretek.onmicrosoft.com");
            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
            Task<AuthenticationResult> resultstr = authenticationContext.AcquireTokenAsync("https://management.core.windows.net/", clientCredential);

            WebClient client = new WebClient();
            //authentication using the Azure AD application
            var token = resultstr.Result.AccessToken;
            client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            client.Headers.Add("api-version:2015-06-01");
            client.Headers.Add("Content-Type", "application/json");

            var bodyText = string.Empty;

            //For individual files
            //dynamic content = new { ContentPaths = new List<string>() { “/1.jpg”, “/2.jpg” } };
            //bodyText = JsonConvert.SerializeObject(content);

            //For purge all (*.*)
            bodyText = "{\"ContentPaths\":[\"/*\"]}";
            // var body = JsonConvert.SerializeObject(bodyText);

            try
            {
                var result = client.UploadString(uri, bodyText);
            }
            catch (System.Exception ew)
            {
                //handle the exception here
                message = ew.ToString();
                ViewBag.purge = ew.ToString();
                //  return Json(new { responseText = message }, JsonRequestBehavior.AllowGet);
                return View("Index");
            }
            ViewBag.purge = "ESP CDN links are updated!";
            message = "ESP CDN links are updated!";
            //  return Json(new { responseText =  message }, JsonRequestBehavior.AllowGet);
            return View("Index");
        }

        public static HttpWebResponse azureLoad(string account)
        {

            //Data parameter Example
            var data = "/" + account + "/";
            var body = JsonConvert.SerializeObject(data);
            string url = "https://portal.azure.com/#resource/subscriptions/c0670002-2f60-4ccb-adca-6009dd5fd90c/resourcegroups/pk/providers/Microsoft.Cdn/profiles/pkcdn/endpoints/pkcdn?api-version=2016-10-02";
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "PATCH";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;

            var streamWriter = new StreamWriter(httpRequest.GetRequestStream());
            streamWriter.Write(data);
            streamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            response.Close();
            return response;
        }

        public static string ResizeImages(string uploadpath, string fileName1, string storage, int width)
        {
            string message = "";
            // Tinyfy
            //try
            //{
            //    Task resize = Task.Run(async () =>
            //    {
            //        Tinify.Key = "lm11M7X9ojtq_d1h_6m58hKu96nz5n3q";
            //        var source = Tinify.FromFile(uploadpath);
            //        if (width > 400)
            //        {
            //            var resizemed = source.Resize(new
            //            {
            //                method = "fit",
            //                width = 400,
            //                height = 400
            //            });
            //            await resizemed.ToFile(uploadpath.Substring(0, uploadpath.Length - 4) + "med.png");
            //        }
            //        var resizethumb = source.Resize(new
            //        {
            //            method = "fit",
            //            width = 150,
            //            height = 150
            //        });
            //        await resizethumb.ToFile(uploadpath.Substring(0, uploadpath.Length - 4) + "thumb.png");
            //    });
            //    resize.Wait();
            //}
            //catch (System.Exception ex)
            //{
            //    message = "Error occured while resizing images" + ex.ToString();
            //    SendEmail("meghanasampelli@kretek.com", "AzureUploader@kretek.com", "", "", "Resizing failed for image in TinyPNG" + fileName1, false, ex.ToString(), "");
            //}

            Task resize = Task.Run(async () =>
                                {
                                    try
                                    {
                                        using (var png = new TinyPngClient("lm11M7X9ojtq_d1h_6m58hKu96nz5n3q"))
                                        {
                                            //png.httpClient.Timeout = 100000000;
                                            //compress an image
                                            var result = await png.Compress(uploadpath);
                                            var compressedImage = await png.Download(result);
                                            //or just save to disk
                                            await compressedImage.SaveImageToDisk(uploadpath);
                                            if (width > 400)
                                            {
                                                var mediumSize = await png.Resize(result, new CoverResizeOperation(400, 400));
                                                await mediumSize.SaveImageToDisk(uploadpath.Substring(0, uploadpath.IndexOf('.')) + "med" + format);
                                            }
                                            var thumbnailSize = await png.Resize(result, new CoverResizeOperation(150, 150));
                                            await thumbnailSize.SaveImageToDisk(uploadpath.Substring(0, uploadpath.IndexOf('.')) + "thumb" + format);

                                        }

                                    }
                                    catch (System.Exception ex)
                                    {
                                        message = "Error occured while resizing image" + fileName1;
                                        SendEmail("meghanasampelli@kretek.com", "AzureUploader@kretek.com", "", "", "Resizing failed for image in TinyPNG" + fileName1, false, ex.ToString(), "");
                                    }
                                });
            resize.Wait();
            return message;
        }

        public ActionResult AzureReport()
        {
            DataTable tabe = new DataTable();
            DataTable finaltable = new DataTable();
            tabe = UnifyConnect.GetAllItemsPK(4);
            //StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=FLgT0vRmlqc9EDvDwf6UENcueHJ/Co4yq2rMjb1wqSISM7boQxj+IlcN1I9cK+tSDf+qtf0CQJLIipgP31qeTA==";
            StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=MEmq25OW6QWdqsfE2U0Dld7Ha7lfPftI7L5sH5Ng0//Xdh4MU+m+NbdW4Q1sn4ENjEv8blfLyTqkXV9d42lSjQ==;EndpointSuffix=core.windows.net";

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("pkimages");

                var model = new List<HomePageModel>();
                var results = new List<HomePageModel>();
                var imageList = new List<HomePageModel>();

                foreach (IListBlobItem item in container.ListBlobs(null, true))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    model.Add(new HomePageModel { ImageList = blob.Uri.ToString() });
                    // hpm.Images.Add(blob.Uri.ToString());
                    System.Diagnostics.Debug.WriteLine(blob.Uri);
                }
                foreach (var item in model)
                {
                    var index = (item.ImageList.ToString()).LastIndexOf('/');
                    var imagename = (item.ImageList.ToString()).Substring(index + 1);
                    imageList.Add(new HomePageModel { ImageFilter = imagename.ToString() });

                }
                //tabe.Columns.Contains
                var count = tabe.Rows.Count;
                // DataTable resultrow = new DataTable();
                
                var resultrows1 = from myRow in tabe.Rows.Cast<DataRow>()
                                 where (string)myRow["item"] == "100280"
                                 select myRow;
                foreach ( var itemimage in imageList)
                 {
                    string image = itemimage.ImageFilter.Substring(0,itemimage.ImageFilter.Length - 4);
                    var type = Type.GetType("System.String");
                    Convert.ChangeType(image, type);
                    if (image.Contains("thumb") || image.Contains("med"))
                    {
                        continue;
                    }
                    else
                    {

                        //var linqrow = tabe.AsEnumerable().Where(x => x.Field<string>("item").StartsWith(image));
                        var filter = string.Format("item = '{0}' ", image);
                        DataRow[] rows = tabe.Select(filter);
                        foreach (DataRow row in rows)
                        {
                            tabe.Rows.Remove(row);
                        }
                    }
                }
                //var linqrow = tabe.AsEnumerable().Where(x => x.Field<string>("item").StartsWith("200280"));
                //DataRow[] rows = tabe.Select("item = '200280'");
                //foreach( DataRow row in rows)
                //{
                //    tabe.Rows.Remove(row);
                //}
                
                finaltable  = tabe.Select("PRCLEVELITEM LIKE 'PK%' ").CopyToDataTable();
                
                for (int a = 1; a<=8 ; a++)
                {
                    finaltable.Columns.RemoveAt(3);
                }
                int x = 6;
                while(finaltable.Columns.Count > x)
                {
                    finaltable.Columns.RemoveAt(x);
                }
                ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                wbook.Worksheets.Add(finaltable, "tab1");
                // Prepare the response
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Provide you file name here
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Samplefile.xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();
            }
           
            catch (System.Exception ex)
            {

            }
            return View("Index", finaltable);
        }

        [WebMethod]
        public static void SendEmail(string to, string from, string cc, string bcc, string subject, bool htmlBody, string body, string attachmentPath)
        {
            if (to.Trim().Length <= 0)
                return;

            //System.Net.ServicePointManager.MaxServicePointIdleTime = 1;

            MailMessage mailMessage = new MailMessage();
            if (to.Trim().Length > 0)
                mailMessage.To.Add(to);
            //if (from.Trim().Length > 0)
            mailMessage.From = new MailAddress(from);
            if (cc.Trim().Length > 0)
                mailMessage.CC.Add(cc);
            if (bcc.Trim().Length > 0)
                mailMessage.Bcc.Add(bcc);
            mailMessage.Subject = subject;
            if (htmlBody)
                mailMessage.IsBodyHtml = htmlBody;

            mailMessage.Body = body;
            if (attachmentPath.Trim() != "")
            {
                Attachment attachment = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);
            }
            //
            SmtpClient emailServer = new SmtpClient("mail.kretek.com");
            //            SmtpClient emailServer = new SmtpClient("10.1.1.57");
            //SmtpClient emailServer = new SmtpClient("fexch2");
            //SmtpClient emailServer = new SmtpClient("10.1.1.180");
            //SmtpClient emailServer = new SmtpClient("fserverexch");
            emailServer.UseDefaultCredentials = true;
            emailServer.Port = 2526;
            //            emailServer.Credentials = new NetworkCredential("administrator", "kre.19cas", "kretek2");
            emailServer.Credentials = new NetworkCredential("Anonymous Logon", "", "NT AUTHORITY");

            emailServer.Send(mailMessage);
            //emailServer.SendAsync(mailMessage, "");
            mailMessage.Dispose();
        }

        //list of images with no medium size
        public ActionResult AzureReportMed()
        {
            DataTable tabe = new DataTable();
            DataTable finalTable = new DataTable();
          //  tabe = UnifyConnect.GetAllItemsPK(4);
            //StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=FLgT0vRmlqc9EDvDwf6UENcueHJ/Co4yq2rMjb1wqSISM7boQxj+IlcN1I9cK+tSDf+qtf0CQJLIipgP31qeTA==";
            StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pkimages;AccountKey=MEmq25OW6QWdqsfE2U0Dld7Ha7lfPftI7L5sH5Ng0//Xdh4MU+m+NbdW4Q1sn4ENjEv8blfLyTqkXV9d42lSjQ==;EndpointSuffix=core.windows.net";

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("pkimages");

                var model = new List<string>();
                //var results = new List<HomePageModel>();
                var imageList = new List<string>();
                var noMedList = new List<string>();

                foreach (IListBlobItem item in container.ListBlobs(null, true))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    model.Add(blob.Uri.ToString() );
                    // hpm.Images.Add(blob.Uri.ToString());
                    System.Diagnostics.Debug.WriteLine(blob.Uri);
                }
                foreach (var item in model)
                {
                    var index = (item.ToString()).LastIndexOf('/');
                    var imagename = (item.ToString()).Substring(index + 1);
                    imageList.Add(imagename.ToString());

                }
            
                foreach (var itemimage in imageList)
                {
                   
                    string image = itemimage.Substring(0, itemimage.Length - 4);
                    var type = Type.GetType("System.String");
                    var medimage = image.ToString() + "med.png";
                    Convert.ChangeType(image, type);
                    Convert.ChangeType(medimage, type);
                    if (image.Contains("thumb") || image.Contains("med"))
                    {
                        continue;
                    }
                    else if( imageList.Contains(medimage))
                    {
                        //if there is a medium sized image
                        continue;
                    }
                    else
                    {
                        noMedList.Add(image);
                      
                    }
                }
               

                //finaltable = tabe.Select("PRCLEVELITEM LIKE 'PK%' ").CopyToDataTable();

                //for (int a = 1; a <= 8; a++)
                //{
                //    finaltable.Columns.RemoveAt(3);
                //}
                //int x = 6;
                //while (finaltable.Columns.Count > x)
                //{
                //    finaltable.Columns.RemoveAt(x);
                //}
                ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                wbook.AddWorksheet("Imagesmissing_med");
                var ws = wbook.Worksheet("Imagesmissing_med");
                int row = 1;
                foreach (object item in noMedList)
                {
                    ws.Cell("A" + row.ToString()).Value = item.ToString();
                    row++;
                }

                wbook.SaveAs(@"C:\Users\msam\Downloads\MissingImages_med.xlsx");
               // wbook.Worksheets.Add(noMedList.ToString());
                // Prepare the response
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Provide you file name here
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Samplefile.xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();
            }

            catch (System.Exception ex)
            {

            }
            return View("Index");
        }
    }
}




using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using hcl_medicator.Filters;
using hcl_medicator.Models;

namespace hcl_medicator.Controllers
{
    [NoCache]
    public class PrescriptionController : Controller
    {
        PrescriptionRepo repo;
        ProductRepo productRepo;

        public PrescriptionController()
        {
            repo = new PrescriptionRepo();
            productRepo = new ProductRepo();
        }

        // GET: Prescription
        [CustomAuthorize]
        [NoCache]
        public ActionResult ViewAll()
        {
            int userId = (int)Session["UserID"];

            List<Prescription> prescriptionEntity = repo.getAllPrescriptions(userId);
            List<PrescriptionModel> prescriptionModel = new List<PrescriptionModel>();
            foreach(var item in prescriptionEntity)
            {
                PrescriptionModel pm = new PrescriptionModel();
                pm.PrescriptionID = item.PrescriptionID;
                pm.TextContent = item.TextContent;
                pm.FilePath = item.FilePath;
                pm.UserID = item.UserID;

                prescriptionModel.Add(pm);
            }
            return View(prescriptionModel);
        }

        // add new prescription to the database
        [CustomAuthorize]
        [NoCache]
        public ActionResult savePrescriptions()
        {
            return View();
        }


        [HttpPost]
        public ActionResult savePrescriptions(PrescriptionModel model)
        {
            Prescription pres = new Prescription();

            pres.PrescriptionID = model.PrescriptionID;
            pres.FilePath = "Added Using Text";
            pres.UserID = (int)Session["UserID"];
            pres.TextContent = model.TextContent;

            if(repo.savePrescription(pres))
            {
                ViewBag.message = "Prescription Added Successfully";
                return RedirectToAction("ViewAll");
            }
            ViewBag.message = "Failed to Add Prescription";
            return View();
        }

        // Delete Prescription
        [CustomAuthorize]
        public ActionResult DeletePrescription(int id)
        {
            var pe = repo.getPrescriptionById(id);

            var pModel = new PrescriptionModel
            {
                PrescriptionID = pe.PrescriptionID,
                FilePath = pe.FilePath,
                UserID = pe.UserID,
                TextContent = pe.TextContent,

            };
            return View(pModel);
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult DeletePrescription(int id, FormCollection fc)
        {
            try
            {
                if (repo.delete(id))
                {
                    ViewBag.message = "Prescription Deleted Successfully";
                }
                else
                {
                    ViewBag.message = "Failed to delete Prescription";
                }
                return RedirectToAction("ViewAll");
            }
            catch
            {
                return View();
            }

        }

        [CustomAuthorize]
        public ActionResult SearchByPrescription(int id)
        {
            var pe = repo.getPrescriptionById(id);


            List<string> medicines = pe.TextContent.ToLower().Split(',').ToList();

            List<Product> lpEntity = productRepo.ListProduct();
            List<ProductModel> productModel = new List<ProductModel>();

          
            List<Product> existingProducts = lpEntity.Where(p =>
                medicines.Any(med => p.Name.ToLower().Contains(med))
            ).ToList();

            List<string> NonexistingProducts = medicines.Except(
                lpEntity.Select(p => p.Name.ToLower())
            ).ToList();

            if (NonexistingProducts.Any())
            {
                ViewBag.message = NonexistingProducts;
            }
            else
            {
                ViewBag.message = null;
            }

            foreach (var item in existingProducts)
            {
                ProductModel pm = new ProductModel();
                pm.Name = item.Name;
                pm.Description = item.Description;
                pm.Price = item.Price;
                pm.ProductID = item.ProductID;
                pm.QuantityAvailable = item.QuantityAvailable;
                pm.SellerID = item.SellerID;

                productModel.Add(pm);
            }
            return View(productModel);
        }

        [CustomAuthorize]
        [NoCache]
        public ActionResult savePrescriptionImage()
        {
            return View();
        }


        [HttpPost]
        public ActionResult savePrescriptionImage(string text, string path, PrescriptionModel model)
        {
            Prescription pres = new Prescription();

            pres.PrescriptionID = model.PrescriptionID;
            pres.FilePath = path;
            pres.UserID = (int)Session["UserID"];
            pres.TextContent = text;

            if (repo.savePrescription(pres))
            {
                ViewBag.message = "Prescription Added Successfully";
                return RedirectToAction("ViewAll");
            }
            ViewBag.message = "Failed to Add Prescription";
            return View();
        }


    }
}
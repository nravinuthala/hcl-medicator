using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PrescriptionRepo
    {
        medicatordbContext db;

        public PrescriptionRepo()
        {
            db = new medicatordbContext();
        }

        public bool savePrescription (Prescription prescription)
        {
            db.Prescriptions.Add(prescription);

            if(db.SaveChanges() > 0)
            {
                return true;
            }
            else { return false; }
        }

        public Prescription getPrescriptionById(int Id)
        {
            return db.Prescriptions.FirstOrDefault(p => p.PrescriptionID == Id);
        }

        public bool delete(int id)
        {
            var precription = db.Prescriptions.FirstOrDefault(p => p.PrescriptionID == id);

            if(precription != null)
            {
                db.Prescriptions.Remove(precription);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Prescription> getAllPrescriptions(int id)
        {
            var prescriptions = db.Prescriptions.Where(p => p.UserID == id).ToList();
            if (prescriptions != null)
            {
                return prescriptions;
            }
            return null;
        }


    }
}

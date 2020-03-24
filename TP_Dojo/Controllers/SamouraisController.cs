using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BO;
using TP_Dojo.Data;
using TP_Dojo.Models;

//vm.Armes = db.Armes.Where(a => !db.Samourais.Any(s => s.Arme.Id == a.Id)).Select(a => new SelectListItem { Text = a.Nom, Value = a.Id.ToString() }).ToList();

namespace TP_Dojo.Controllers
{
    public class SamouraisController : Controller
    {
        private Context db = new Context();

        // GET: Samourais
        public ActionResult Index()
        {
            return View(db.Samourais.ToList());
        }

        // GET: Samourais/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }

            SamouraiVM vm = new SamouraiVM();
            vm.Samourai = samourai;
            
            vm.potentiel = calculPotentiel(samourai);
            return View(vm);
        }

        // GET: Samourais/Create
        public ActionResult Create()
        {
            SamouraiVM vm = new SamouraiVM();
            vm.Armes = db.Armes.ToList();
            vm.ArtMartials = db.ArtMartials.ToList();
            return View(vm);
        }

        // POST: Samourais/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SamouraiVM samVm)
        {
            if (ModelState.IsValid)
            {

                if (samVm.idArme != null)
                {

                    var armeTrouve = db.Samourais.Any(s => s.Arme.Id == samVm.idArme);
                    if (!armeTrouve)
                    {
                        samVm.Samourai.Arme = db.Armes.FirstOrDefault(a => a.Id == samVm.idArme);

                       
                    }
                    else {
                        samVm.Armes = db.Armes.ToList();
                        samVm.ArtMartials = db.ArtMartials.ToList();
                        return View(samVm);

                    }
                }

                if(samVm.idArtMartial.Count > 0)
                {
                    samVm.Samourai.ArtMartials= db.ArtMartials.Where(
                           x => samVm.idArtMartial.Contains(x.Id))
                           .ToList();
                }
                
                db.Samourais.Add(samVm.Samourai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            samVm.Armes = db.Armes.ToList();
            return View(samVm);
        }

        // GET: Samourais/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }

            SamouraiVM samouraiVM = new SamouraiVM();
            samouraiVM.Samourai = samourai;
            samouraiVM.Armes = db.Armes.ToList();
            samouraiVM.ArtMartials = db.ArtMartials.ToList();
            return View(samouraiVM);
        }

        // POST: Samourais/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SamouraiVM samvm)
        {
            if (ModelState.IsValid)
            {
                var Samourai = db.Samourais.Find(samvm.Samourai.Id);
               
                Samourai.Force = samvm.Samourai.Force;
                Samourai.Nom = samvm.Samourai.Nom;
                if (samvm.idArme != null)
                {
                    if (Samourai.Arme != null && Samourai.Arme.Id == samvm.idArme)
                    {

                        Samourai.Arme = samvm.Samourai.Arme;
                    }
                    else
                    {
                        var armeTrouve = db.Samourais.Any(s => s.Arme.Id == samvm.idArme);
                        if (!armeTrouve)
                        {
                            Samourai.Arme = db.Armes.FirstOrDefault(a => a.Id == samvm.idArme);
                        }
                        else
                        {
                            samvm.Armes = db.Armes.ToList();
                            samvm.ArtMartials = db.ArtMartials.ToList();
                            return View(samvm);
                        }

                    }
                }
                else
                {
                    Samourai.Arme = null;
                    db.Entry(Samourai).State = EntityState.Modified;
                    
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            samvm.Armes = db.Armes.ToList();
            return View(samvm);

        }

        // GET: Samourais/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);

           

            if (samourai == null)
            {
                return HttpNotFound();
            }
            SamouraiVM vm = new SamouraiVM();
            vm.Samourai = samourai;

            vm.potentiel = calculPotentiel(samourai);
            return View(vm);
        }

        // POST: Samourais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            Samourai samourai = db.Samourais.Find(id);
            
            if (samourai.Arme != null)
            {
                ModelState.AddModelError("", "Impossible de supprimer cette arme car elle est utilisée par des Samourais");
                return View(db.Samourais.Find(id));
            }

            db.Samourais.Remove(samourai);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private int calculPotentiel(Samourai samourai)
        {
            int force = samourai.Force;

            int degat = 0;
            if (samourai.Arme != null)
            {
                degat = samourai.Arme.Degats;
            }
            int nbArt = samourai.ArtMartials.Count + 1;
  
            return (force + degat) * nbArt;
        }
    }
}

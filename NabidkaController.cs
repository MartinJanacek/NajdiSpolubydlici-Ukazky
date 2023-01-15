using Microsoft.AspNetCore.Mvc;
using NajdiSpolubydlícíWeb.Data;
using NajdiSpolubydlícíWeb.Enums;
using NajdiSpolubydlícíWeb.Handlers;
using NajdiSpolubydlícíWeb.Models;
using PasswordGenerator;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace NajdiSpolubydlícíWeb.Controllers
{
    public class NabidkaController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IConfiguration _config;

        private readonly string typ = "Nabidka";

        public NabidkaController(ApplicationDbContext db) => _db = db;

        public ActionResult VyhledatNabidky()
        {
            return View();
        }

        [Route("Nabidky/{okres?}")]
        public IActionResult Nabidky(string? okres)
        {
            if (string.IsNullOrEmpty(okres)) return Redirect("~/Nabidka/VyhledatNabidky");

            IEnumerable<Nabidka> objNabidkaList = _db.Nabidky.Where(obj => obj.Okres == okres);
            return View(objNabidkaList);
        }

        public IActionResult Nabidka(int? id)
        {
            if (id is not null)
            {
                Nabidka? obj = (from Nabidka in _db.Nabidky
                                where Nabidka.Id == id
                                select Nabidka).FirstOrDefault();

                if (obj is null) return Redirect("~/Nabidka/VyhledatNabidky");

                NabidkaViewModel ObjView = new(obj);

                return View(ObjView);
            }

            return Redirect("Nabidky");
        }

        public ActionResult NovaNabidka()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NabidkaViewModel objView)
        {      
            if (ModelState.IsValid)
            {
                Nabidka obj = new(objView);

                _db.Nabidky.Add(obj);
                await _db.SaveChangesAsync();

                ImageHandler.DeleteWholeDirectory(obj.Id, typ);
                ImageHandler.CreateDirectory(obj.Id, typ);

                if (!ImageHandler.SaveTitleImage(objView.TitulObrBase64, obj.Id, typ))
                {
                    ImageHandler.DeleteWholeDirectory(obj.Id, typ);
                    _db.Nabidky.Remove(obj);
                    _db.SaveChangesAsync();

                    ViewBag.Message = "Obrázek není ve správném formátu";
                    return View("novanabidka");
                }
                if (!ImageHandler.SaveImages(objView, obj))
                {
                    ImageHandler.DeleteWholeDirectory(obj.Id, typ);
                    _db.Nabidky.Remove(obj);
                    _db.SaveChangesAsync();

                    ViewBag.Message = "Některý z obrázků není ve správném formátu";
                    return View("novanabidka");
                }

                var emailBody = "<h4 style='color:black'>Děkujeme Vám a přejeme hodně štěstí!</h4>" +
                                $"<h4>ID: <span style='color:red'>{obj.Id}</span></h4>" +
                                $"<h4>Heslo: <span style='color:red'>{obj.Password}</span></h4>" +
                                $"<h4>Vaši nabídku naleznete na: <a href='https://najdispolubydlici.azurewebsites.net/Nabidka/Nabidka/{obj.Id}'><br>https://najdispolubydlici.azurewebsites.net/Nabidka/Nabidka/{obj.Id}</a></h4>" +
                                "<p>Vaše nabídka bude po 30 dnech automaticky smazána.</p>" +
                                "<p>Pokud máte dotaz, či potřebujete pomoc, dotazujte se na: <br> info@najdispolubydlici.cz</p><br>" +
                                "<p>S pozdravem<p>" +
                                "<p>Tým Najdi Spolubydlící<p>";

                EmailHandler message = new(obj.Email, "Vaše nabídka byla vytvořena", emailBody);
                message.SendEmailAsync();

                return Redirect($"Nabidka/{obj.Id}");
            }

            return View("NovaNabidka");
        }

        public ActionResult UpravitNabidkuOvereni()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpravitNabidkuOvereni(Nabidka obj)
        {
            string? password = (from Nabidka in _db.Nabidky
                                where Nabidka.Id == obj.Id
                                select Nabidka.Password).FirstOrDefault();

            if (obj is not null  && obj.Password == password) return UpravitNabidku(obj.Id);

            ViewBag.Message = "Tuto nabídku jsme nenašli";
            return View("UpravitNabidkuOvereni");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpravitNabidku/{ObjView.Id?}")]
        public IActionResult UpravitNabidku(int? id)
        {
            if (id is null) return Redirect("UpravitNabidkuOvereni");

            Nabidka? obj = _db.Nabidky.FirstOrDefault(x => x.Id == id);

            if (obj is null) return Redirect("UpravitNabidkuOvereni");

            NabidkaViewModel objView = new(obj);

            ModelState.Clear();
            return UpravitNabidku(objView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpravitNabidku/{ObjView.Id?}")]
        public IActionResult UpravitNabidku(NabidkaViewModel ObjView)
        {
            return View("UpravitNabidku", ObjView);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpravitNabidkuDokoncit(NabidkaViewModel objView)
        {
            string? password = (from Nabidka in _db.Nabidky
                               where Nabidka.Id == objView.Id
                               select Nabidka.Password).FirstOrDefault();
            
            if (password is not null && password == objView.Password && ModelState.IsValid)
            {
                Nabidka? obj = _db.Nabidky.FirstOrDefault(x => x.Id == objView.Id);

                if (obj is null) return Redirect("UpravitNabidkuOvereni");

                ImageHandler.DeleteWholeDirectory(obj.Id, typ);
                ImageHandler.CreateDirectory(obj.Id, typ);

                if (!ImageHandler.SaveTitleImage(objView.TitulObrBase64, obj.Id, typ))
                {
                    ViewBag.Message = "VLožte titulní obrázek ve správném formátů";
                    return View("novanabidka");
                }
                if (!ImageHandler.SaveImages(objView, obj))
                {
                    ViewBag.Message = "Některý z obrázků není ve správném formátu";
                    return View("novanabidka");
                }

                obj.Update(objView);

                _db.Nabidky.Update(obj);
                await _db.SaveChangesAsync();

                var emailBody = "<h4 style='color:black'>Vaše nabídka byla změněna</h4>" +
                                $"<h4>ID: <span style='color:red'>{obj.Id}</span></h4>" +
                                $"<h4>Heslo: <span style='color:red'>{obj.Password}</span></h4>" +
                                $"<h4>Vaši nabídku naleznete na: <a href='https://najdispolubydlici.azurewebsites.net/Nabidka/Nabidka/{obj.Id}'><br>https://najdispolubydlici.azurewebsites.net/Nabidka/Nabidka/{obj.Id}</a></h4>" +
                                "<p>Vaše nabídka bude po 30 dnech automaticky smazána.</p>" +
                                "<p>Pokud máte dotaz, či potřebujete pomoc, dotazujte se na: <br> info@najdispolubydlici.cz</p><br>" +
                                "<p>S pozdravem<p>" +
                                "<p>Tým Najdi Spolubydlící<p>";

                EmailHandler message = new(obj.Email, "Vaše nabídka byla změněna", emailBody);
                message.SendEmailAsync();

                return Redirect($"Nabidka/{obj.Id}");
            }

            return View("NovaNabidka");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SmazatNabidku(Nabidka objView)
        {
            string? password = (from Nabidka in _db.Nabidky
                                where Nabidka.Id == objView.Id
                                select Nabidka.Password).FirstOrDefault();

            if (objView.Password == password && password is not null)
            {
                Nabidka? obj = _db.Nabidky.FirstOrDefault(x => x.Id == objView.Id);

                if (obj is null) return Redirect("UpravitNabidkuOvereni");

                ImageHandler.DeleteWholeDirectory(obj.Id, typ);

                var emailBody = $"<h4 style='color:black'>Nabídka ID {obj.Id} byla smazána</h4>" +
                                "<p>Pokud máte dotaz, či potřebujete pomoc, dotazujte se na: <br> info@najdispolubydlici.cz</p>" +
                                "<p>Doufáme, že jste byli spokojeni.<p><br>" +
                                "<p>S pozdravem<p>" +
                                "<p>Tým Najdi Spolubydlící<p>";

                EmailHandler message = new(obj.Email, "Vaše nabídka byla smazána", emailBody);
                message.SendEmailAsync();

                _db.Nabidky.Remove(obj);
                await _db.SaveChangesAsync();

                return Redirect($"~/");
            }

            return Redirect("UpravitNabidkuOvereni");
        }
    }
}

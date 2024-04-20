using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using PrepaEvaluation.Models;
using PrepaEvaluation.Models.Temporary;
using PrepaEvaluation.Models.View;
using PrepaEvaluation.Utils;
using Rotativa.AspNetCore;

namespace PrepaEvaluation.Controllers;

public class SceanceController : BaseController
{
    public SceanceController(Connection connection) : base(connection) { }
    [HttpGet]
    public IActionResult Index(int page = 1)
    {
        var totalStocksCount = Connection.sceancelib.Count();
        var totalPages = (int)Math.Ceiling((double)totalStocksCount / PageSize);
        var sceances = Connection.sceancelib
            .OrderBy(s => s.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        var pagedList = new StaticPagedList<SceanceLib>(sceances, page, PageSize, totalStocksCount);
        var dictionnary = new Dictionary<string, object>();
        dictionnary.Add("totalPages", totalPages);
        dictionnary.Add("currentPage", page);
        dictionnary.Add("sceances", pagedList);
        return View(dictionnary);
    }
    
    [HttpPost]
    public IActionResult Search(string search, int page = 1)
    {
        var totalStocksCount = Connection.sceancelib.Count(s => s.Film == search);
        var totalPages = (int)Math.Ceiling((double)totalStocksCount / PageSize);
        var sceances = Connection.sceancelib
            .Where(s => s.Film == search)
            .OrderBy(s => s.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        var pagedList = new StaticPagedList<SceanceLib>(sceances, page, PageSize, totalStocksCount);
        var dictionnary = new Dictionary<string, object>();
        dictionnary.Add("totalPages", totalPages);
        dictionnary.Add("currentPage", page);
        dictionnary.Add("sceances", pagedList);
        return View("Index", dictionnary);
    }
    [HttpPost]
    public IActionResult Add(int idfilm, DateOnly date, TimeOnly heure)
    {
        var sceance = new Sceance()
        {
            Film = new Film() {Id = idfilm},
            Date = date,
            Heure = heure
        };
        // TODO Notion Transaction
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                Connection.sceance.Add(sceance);
                Connection.SaveChanges();
                transaction.Commit();
                TempData["Success"] = "Insertion réussie";
            }
            catch (Exception e)
            {
                transaction.Rollback();
                TempData["Error"] = $"Une erreur s'est produite lors de l'insertion d'un nouveau item : {e.Message}";
            }
        }
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Delete(int id)
    {
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                var sceance = Connection.sceance.Find(id);
                if (sceance != null) Connection.sceance.Remove(sceance);
                Connection.SaveChanges();
                transaction.Commit();
                TempData["Success"] = "Suppression réussie";
            }
            catch (Exception e)
            {
                transaction.Rollback();
                TempData["Error"] = $"Une erreur s'est produite lors de la suppresion de l'élement : {e.Message}";
            }
        }
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Update(int id, string title, string categorie, DateOnly date, TimeOnly heure)
    {
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                var oldSceance = Connection.sceance.Find(id);
                if (oldSceance == null) return RedirectToAction("Index");
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(categorie)) TempData["Error"] = "Le titre et le catégorie ne peuvent pas être vides.";
                var film = new Film()
                {
                    Title = title,
                    Categorie = new Categorie() { Name = categorie }
                };
                oldSceance.Film = film;
                oldSceance.Date = date;
                oldSceance.Heure = heure;
                Connection.SaveChanges();
                transaction.Commit();
                TempData["Success"] = "Modification réussie";
            }
            catch (Exception e)
            {
                TempData["Error"] = $"Une erreur s'est produite lors de la mise à jour de l'article: {e.Message}";
                transaction.Rollback();
            }
        }
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [SuppressMessage("ReSharper.DPA", "DPA0010: ASP issues")]
    public IActionResult ImportFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Veuillez sélectionner un fichier à importer.";
            return RedirectToAction("Index");
        }
        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }
        var filePath = Path.Combine(uploadDirectory, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create)) file.CopyTo(stream);
        // TODO Notion Transaction
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                var sceances = FileManager.ImportFromCsv<SceanceTemp>(filePath);
                Connection.sceancetemp.AddRange(sceances);
                Connection.SaveChanges();
                const string newCategorie = @"INSERT INTO categorie (nom) SELECT DISTINCT categorie FROM sceancetemp WHERE NOT EXISTS (SELECT 1 FROM categorie WHERE categorie = categorie.nom);";
                Connection.Database.ExecuteSqlRaw(newCategorie);
                const string newSalle = @"INSERT INTO salle (nom) SELECT DISTINCT salle FROM sceancetemp WHERE NOT EXISTS (SELECT 1 FROM salle WHERE salle = salle.nom);";
                Connection.Database.ExecuteSqlRaw(newSalle);
                const string newFilm = @"INSERT INTO film (titre, idcategorie) SELECT DISTINCT st.film, c.id FROM sceancetemp st LEFT JOIN categorie c ON c.nom = st.categorie WHERE NOT EXISTS (SELECT 1  FROM film f WHERE f.titre = st.film AND f.idcategorie = c.id);";
                Connection.Database.ExecuteSqlRaw(newFilm);
                const string newSceance = @"INSERT INTO sceance (idfilm, idsalle, date, heure) SELECT DISTINCT f.id as idfilm, s.id as idsalle, st.date, st.heure FROM sceancetemp st LEFT JOIN salle s ON s.nom = st.salle LEFT JOIN film f ON f.titre = st.film WHERE NOT EXISTS (SELECT 1 FROM sceance WHERE date = st.date AND heure = st.heure AND idfilm = f.id);";
                Connection.Database.ExecuteSqlRaw(newSceance);
                transaction.Commit();
                System.IO.File.Delete(filePath);
                TempData["Success"] = "Importation réussie";
            }
            catch (Exception e)
            {
                transaction.Rollback();
                TempData["Error"] = $"Une erreur s'est produite lors de l'importation du fichier : {e.Message}";
            }   
        }
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult GeneratePdf()
    {
        var sceances = Connection.sceancelib.ToList();
        return new ViewAsPdf("~/Views/PDF/Sceance.cshtml", sceances);
    }
    [HttpGet]
    public IActionResult GenerateExcel()
    {
        try
        {
            var sceances = Connection.sceancelib.ToList();
            FileManager.ExportToExcel(sceances, "sceances.xlsx");
            TempData["Success"] = "Exportation réussie";
        }
        catch (Exception e)
        {
            TempData["Error"] = $"Une erreur s'est produite lors de l'exportation du fichier : {e.Message}";
        }
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult GenerateCsv()
    {
        try
        {
            var sceances = Connection.sceancelib.ToList();
            FileManager.ExportToCsv(sceances, "sceances.csv");
            TempData["Success"] = "Exportation réussie";
        }
        catch (Exception e)
        {
            TempData["Error"] = $"Une erreur s'est produite lors de l'exportation du fichier : {e.Message}";
        }
        return RedirectToAction("Index");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using PrepaEvaluation.Models;
using PrepaEvaluation.Utils;
using Rotativa.AspNetCore;

namespace PrepaEvaluation.Controllers;

public class HomeController : BaseController
{
    public HomeController (Connection connection) : base(connection) { }
    [SuppressMessage("ReSharper.DPA", "DPA0010: ASP issues")]
    public IActionResult Index(int page = 1)
    {
        var totalStocksCount = Connection.stock.Count();
        var totalPages = (int)Math.Ceiling((double)totalStocksCount / PageSize);
        var stocks = Connection.stock
            .OrderBy(s => s.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        var pagedList = new StaticPagedList<Stock>(stocks, page, PageSize, totalStocksCount);
        var dictionnary = new Dictionary<string, object>();
        dictionnary.Add("totalPages", totalPages);
        dictionnary.Add("currentPage", page);
        dictionnary.Add("stocks", pagedList);
        return View(dictionnary);
    }
    [HttpGet]
    [SuppressMessage("ReSharper.DPA", "DPA0010: ASP issues")]
    public IActionResult GetById(int id)
    {
        var stock = Connection.stock.Find(id);
        return Json(stock);
    }
    [HttpPost]
    [SuppressMessage("ReSharper.DPA", "DPA0010: ASP issues")]
    public IActionResult Add(string designation, string unity, double quantity, string? identification)
    {
        var stock = new Stock()
        {
            Designation = designation,
            Unity = unity,
            Quantity = quantity,
            Identification = identification
        };
        // TODO Notion Transaction
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                Connection.stock.Add(stock);
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
    [HttpPost]
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
                var stocks = FileManager.ImportFromCsv<Stock>(filePath);
                Connection.stock.AddRange(stocks);
                Connection.SaveChanges();
                transaction.Commit();
                System.IO.File.Delete(filePath);
                TempData["Success"] = "Importation réussie";
            }
            catch (Exception e)
            {
                transaction.Rollback();
                TempData["Error"] = $"Une erreur s'est produite lors de l'importation du fichier : {e.Message}";
                return RedirectToAction("Index");
            }   
        }
        return RedirectToAction("Index");
    }
    [HttpDelete]
    public IActionResult Delete(int id)
    {
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                var stock = Connection.stock.Find(id);
                if (stock != null) Connection.stock.Remove(stock);
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
        return Json(TempData);
    }
    [HttpPost]
    public IActionResult Update(int id, string designation, string unity, double quantity, string? identification)
    {
        using (var transaction = Connection.Database.BeginTransaction())
        {
            try
            {
                var oldStock = Connection.stock.Find(id);
                if (oldStock == null) return RedirectToAction("Index");
                if (string.IsNullOrEmpty(designation) || string.IsNullOrEmpty(unity)) ViewBag.Error = "La désignation et l'unité ne peuvent pas être vides.";
                oldStock.Designation = designation;
                oldStock.Unity = unity;
                oldStock.Quantity = quantity;
                oldStock.Identification = identification;
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
    public IActionResult GeneratePdf()
    {
        var stocks = Connection.stock.ToList();
        return new ViewAsPdf("~/Views/PDF/Stock.cshtml", stocks);
    }

    public IActionResult GenerateExcel()
    {
        try
        {
            var stocks = Connection.stock.ToList();
            FileManager.ExportToExcel(stocks, "stocks.xlsx");
            TempData["Success"] = "Exportation réussie";
        }
        catch (Exception e)
        {
            TempData["Error"] = $"Une erreur s'est produite lors de l'exportation du fichier : {e.Message}";
        }
        return RedirectToAction("Index");
    }

    public IActionResult GenerateCsv()
    {
        try
        {
            var stocks = Connection.stock.ToList();
            FileManager.ExportToCsv(stocks, "stocks.csv");
            TempData["Success"] = "Exportation réussie";
        }
        catch (Exception e)
        {
            TempData["Error"] = $"Une erreur s'est produite lors de l'exportation du fichier : {e.Message}";
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Search(string search, int page = 1)
    {
        var totalStocksCount = Connection.stock.Count(s => s.Designation == search);
        var totalPages = (int)Math.Ceiling((double)totalStocksCount / PageSize);
        var stocks = Connection.stock
            .Where(s => s.Designation == search)
            .OrderBy(s => s.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        var pagedList = new StaticPagedList<Stock>(stocks, page, PageSize, totalStocksCount);
        var dictionnary = new Dictionary<string, object>();
        dictionnary.Add("totalPages", totalPages);
        dictionnary.Add("currentPage", page);
        dictionnary.Add("stocks", pagedList);
        return Json(dictionnary);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
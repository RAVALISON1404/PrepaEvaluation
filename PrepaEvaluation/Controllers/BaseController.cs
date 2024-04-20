using Microsoft.AspNetCore.Mvc;
using PrepaEvaluation.Utils;

namespace PrepaEvaluation.Controllers;

public class BaseController : Controller
{
    protected Connection Connection { get; }
    protected const int PageSize = 5;
    public BaseController(Connection connection)
    {
        Connection = connection;
    }
    private IActionResult Index()
    {
        return View();
    }
}
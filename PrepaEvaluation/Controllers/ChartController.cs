using Microsoft.AspNetCore.Mvc;
using PrepaEvaluation.Utils;

namespace PrepaEvaluation.Controllers;

public class ChartController: BaseController
{
    public ChartController(Connection connection) : base(connection) { }

    public IActionResult Index()
    {
        return View();
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardCore.Elsa.Workflows.Controllers
{
    [Route("elsa/[controller]")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
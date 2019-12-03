using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;

namespace Elsa.OrchardCore.Controllers
{
    [Admin]
    [Route("admin/elsa/workflow-definitions")]
    public class WorkflowDefinitionsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
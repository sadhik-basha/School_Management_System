using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolTestProject.Controllers
{
    [Authorize(Roles = "Teacher,Guest1")]
    public class TeacherController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

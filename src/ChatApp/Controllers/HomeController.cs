using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    /// <summary>
    /// Auxiliary controller for redirecting to app's index page
    /// </summary>
    public class HomeController
    {
        /// <summary>
        /// Used for to redirect index.html file
        /// </summary>
        /// <returns>Redirect result</returns>
        public ActionResult Index()
        {
            return new RedirectResult("/index.html");
        }
    }
}

using ComicBookShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComicBookWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected Repository Repository { get; private set; }

        private Context _context = null;
        private bool _disposed = false;

        public BaseController()
        {
            _context = new Context();
            Repository = new Repository(_context);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
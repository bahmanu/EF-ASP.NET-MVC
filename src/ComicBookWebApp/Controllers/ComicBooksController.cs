﻿using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ComicBookLibraryManagerWebApp.ViewModels;
using System.Net;
using System.Data.Entity.Infrastructure;
using ComicBookShared.Data;
using ComicBookWebApp.Controllers;
using ComicBookWebApp.ViewModels;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Comic Books" section of the website.
    /// </summary>
    public class ComicBooksController : BaseController
    {
        private ComicBookRepository _comicBooksRepository = null;

        public ComicBooksController()
        {
            _comicBooksRepository = new ComicBookRepository(Context);
        }

        public ActionResult Index()
        {

            var comicBooks = _comicBooksRepository.GetList();

            return View(comicBooks);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            // Sort the artists.
            comicBook.Artists = comicBook.Artists.OrderBy(a => a.Role.Name).ToList();

            return View(comicBook);
        }

        public ActionResult Add()
        {
            var viewModel = new ComicBooksAddViewModel();
            

            viewModel.Init(Repository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBooksAddViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;
                comicBook.AddArtist(viewModel.ArtistId, viewModel.RoleId);

                _comicBooksRepository.Add(comicBook);

                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(Repository);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var comicBook = _comicBooksRepository.Get((int)id, includeRelated : false);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBooksEditViewModel()
            {
                ComicBook = comicBook
            };
            viewModel.Init(Repository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ComicBooksEditViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;

                try
                {
                    _comicBooksRepository.Update(comicBook);

                    TempData["Message"] = "Your comic book was successfully updated!";

                    return RedirectToAction("Detail", new { id = comicBook.Id });

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string message = null;
                    var entityPropertyValues = ex.Entries.Single().GetDatabaseValues();


                    if (entityPropertyValues == null)
                    {
                        message = "The comicbook being updated has been deleted by another user. click the 'Cancel' to return to home.";

                        viewModel.ComicBookHasBeenDeleted = true;
                    }
                    else
                    {
                        message = "The comicbook being updated has already been updated by another user. If you still want to make your change click the 'Save' button again. Otherwise click the 'Cancel' button to discard your changes.";

                        comicBook.RowVersion = ((ComicBook)entityPropertyValues.ToObject()).RowVersion;
                    }


                    ModelState.AddModelError(string.Empty, message);
                   
                }

                
            }

            viewModel.Init(Repository);

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBookDeleteViewModel()
            {
                ComicBook = comicBook
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(ComicBookDeleteViewModel viewModel)
        {

            try
            {
                _comicBooksRepository.Delete(viewModel.ComicBook.Id, viewModel.ComicBook.RowVersion);


                TempData["Message"] = "Your comic book was successfully deleted!";

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string message = null;
                var entityPropertyValues = ex.Entries.Single().GetDatabaseValues();


                if (entityPropertyValues == null)
                {
                    message = "The comicbook being deleted has been deleted by another user. click the 'Cancel' to return to home.";

                    viewModel.ComicBookHasBeenDeleted = true;
                }
                else
                {
                    message = "The comicbook being deleted has already been updated by another user. If you still want to delete the comicbook than click the 'Delete' button again. Otherwise click the 'Cancel' button to return to home.";

                    viewModel.ComicBook.RowVersion = ((ComicBook)entityPropertyValues.ToObject()).RowVersion;
                }
                ModelState.AddModelError(string.Empty, message);
                return View(viewModel);
            }

        }

        /// <summary>
        /// Validates a comic book on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="comicBook">The comic book to validate.</param>
        private void ValidateComicBook(ComicBook comicBook)
        {
            //// If there aren't any "SeriesId" and "IssueNumber" field validation errors...
            if (ModelState.IsValidField("ComicBook.SeriesId") &&
                ModelState.IsValidField("ComicBook.IssueNumber"))
            {
                // Then make sure that the provided issue number is unique for the provided series.
               // TODO Call method to check if the issue number is available for this comic book.
               if (_comicBooksRepository.ComicBookseriesHasIssueNumber(
                   comicBook.Id,comicBook.SeriesId,comicBook.IssueNumber))
                {
                    ModelState.AddModelError("ComicBook.IssueNumber",
                       "The provided Issue Number has already been entered for the selected Series.");
                }
            }
        }
    }
}
using ComicBookShared.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ComicBookRepository
    {
        private Context _context = null;

        public ComicBookRepository(Context context)
        {
            _context = context;
        }

        public IList<ComicBook> GetList()
        {
            return _context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
        }

        public ComicBook Get(int id, bool includeRelated = true)
        {
            var comicBook = _context.ComicBooks.AsQueryable();

            if (includeRelated)
            {
                comicBook = comicBook
                    .Include(cb => cb.Series)
                    .Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(a => a.Role));
            }

            return comicBook
                .Where(cb => cb.Id == id)
                .SingleOrDefault();
        }

        public void Add(ComicBook comicBook)
        {
            _context.ComicBooks.Add(comicBook);
            _context.SaveChanges();
        }

        public void Update(ComicBook comicBook)
        {
            _context.Entry(comicBook).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var comicBook = new ComicBook() { Id = id };
            _context.Entry(comicBook).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public bool ComicBookseriesHasIssueNumber(int comicBookId, int seriesId, int IssueNumber)
        {
            return _context.ComicBooks
                .Any(cb => cb.Id != comicBookId &&
                           cb.SeriesId == seriesId &&
                           cb.IssueNumber == IssueNumber);
        }

        public bool ComicBookHasArtistRoleCombiniation(int comicBookId, int artistId, int roleId)
        {
            return _context.ComicBookArtists
                        .Any(cba => cba.ComicBookId == comicBookId &&
                                    cba.RoleId == roleId &&
                                    cba.ArtistId == artistId);
        }
    }
}

using ComicBookShared.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ComicBookRepository : BaseRepository<ComicBook>
    {
        public ComicBookRepository(Context context) : base(context)
        {
        }

        public override IList<ComicBook> GetList()
        {
            return Context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
        }

        public override ComicBook Get(int id, bool includeRelated = true)
        {
            var comicBook = Context.ComicBooks.AsQueryable();

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

        public void Delete(int id, byte[] rowVersion)
        {
            var comicBook = new ComicBook()
            {
                Id = id,
                RowVersion = rowVersion
            };

            Context.Entry(comicBook).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        /*
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
        }*/

        public bool ComicBookseriesHasIssueNumber(int comicBookId, int seriesId, int IssueNumber)
        {
            return Context.ComicBooks
                .Any(cb => cb.Id != comicBookId &&
                           cb.SeriesId == seriesId &&
                           cb.IssueNumber == IssueNumber);
        }

        public bool ComicBookHasArtistRoleCombiniation(int comicBookId, int artistId, int roleId)
        {
            return Context.ComicBookArtists
                        .Any(cba => cba.ComicBookId == comicBookId &&
                                    cba.RoleId == roleId &&
                                    cba.ArtistId == artistId);
        }
    }
}

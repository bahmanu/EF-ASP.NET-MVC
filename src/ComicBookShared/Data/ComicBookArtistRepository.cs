using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicBookShared.Models;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class ComicBookArtistRepository : BaseRepository<ComicBookArtist>
    {

        public ComicBookArtistRepository(Context context) : base(context)
        {
        }

        /*
        public void Add(ComicBookArtist comicBookArtist)
        {
            _context.ComicBookArtists.Add(comicBookArtist);
            _context.SaveChanges();
        }
        */
        public override ComicBookArtist Get(int id, bool includeRelated = true)
        {
            var comicBookArtist = Context.ComicBookArtists.AsQueryable();

            if (includeRelated)
            {
                comicBookArtist = comicBookArtist
                    .Include(cba => cba.Artist)
                    .Include(cba => cba.Role)
                    .Include(cba => cba.ComicBook.Series);
            };

            return comicBookArtist
                .Where(cba => cba.Id == id)
                .SingleOrDefault();
        }

        public override IList<ComicBookArtist> GetList()
        {
            throw new NotImplementedException();
        }
        /*
        public void Delete(int id)
        {
            var comicBookArtist = new ComicBookArtist() { Id = id };
            _context.Entry(comicBookArtist).State = EntityState.Deleted;
            _context.SaveChanges();
        }*/
    }
}

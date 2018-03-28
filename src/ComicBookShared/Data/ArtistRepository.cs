using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class ArtistRepository : BaseRepository<Artist>
    {
        public ArtistRepository(Context context) : base(context)
        {
        }

        public override Artist Get(int id, bool includeRelated = true)
        {
            var artist = Context.Artists.AsQueryable();

            if (includeRelated)
            {
                artist = artist
                    .Include(a => a.ComicBooks.Select(cb => cb.Role))
                    .Include(a => a.ComicBooks.Select(cb => cb.ComicBook.Series));
            }

            return artist
                .Where(s => s.Id == id)
                .SingleOrDefault();
        }

        public override IList<Artist> GetList()
        {
            return Context.Artists.OrderBy(a => a.Name).ToList();
        }

        public bool IsArtistNameAvailable(int artistId, string name)
        {
            return Context.Artists
                .Any(s => s.Id != artistId &&
                           s.Name == name);
        }
    }
}

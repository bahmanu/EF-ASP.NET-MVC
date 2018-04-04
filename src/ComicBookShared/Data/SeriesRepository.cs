using ComicBookShared.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class SeriesRepository : BaseRepository<Series>
    {
        const string SeriesListKey = "SeriesList";

        public SeriesRepository(Context context) : base(context)
        {
        }

        public override IList<Series> GetList()
        {

            var seriesList = EntityCache.Get<List<Series>>(SeriesListKey);

            if (seriesList == null)
            {
                seriesList = Context
                    .Series.OrderBy(s => s.Title).ToList();

                EntityCache.Add(SeriesListKey, seriesList);
            }

            return seriesList;
        }

        public override Series Get(int id, bool includeRelated = true)
        {
            var series = Context.Series.AsQueryable();

            if (includeRelated)
            {
                series = series
                    .Include(s => s.ComicBooks);
            }

            return series
                .Where(s => s.Id == id)
                .SingleOrDefault();
        }

		public override void Add(Series tEntity)
		{
			base.Add(tEntity);

            EntityCache.Remove(SeriesListKey);
		}

		public override void Update(Series tEntity)
		{
			base.Update(tEntity);

            EntityCache.Remove(SeriesListKey);
		}

		public override void Delete(int id)
		{
			base.Delete(id);

            EntityCache.Remove(SeriesListKey);
		}

		public bool IsTitleAvailable(int seriesId, string title)
        {
            return Context.Series
                .Any(s => s.Id != seriesId &&
                           s.Title == title);
        }

    }
}

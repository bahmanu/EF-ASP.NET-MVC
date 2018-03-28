using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public abstract class BaseRepository<TEntity>
        where TEntity : class
    {
        protected Context Context { get; private set; }

        public BaseRepository(Context context)
        {
            Context = context;
        }

        public abstract TEntity Get(int id, bool includeRelated = true);
        public abstract IList<TEntity> GetList();
        public void Add(TEntity tEntity)
        {
            Context.Set<TEntity>().Add(tEntity);
            Context.SaveChanges();
        }
        public void Update(TEntity tEntity)
        {
            Context.Entry(tEntity).State = EntityState.Modified;
            Context.SaveChanges();
        }
        public void Delete(int id)
        {
            /*
            var entity = new TEntity()
            {
                Id = id
            };
            Context.Entry(entity).State = EntityState.Deleted;*/

            var set = Context.Set<TEntity>();
            var entity = set.Find(id);
            set.Remove(entity);

            Context.SaveChanges();
        }
    }


    public interface IEntity
    {
        int Id { get; set; }
    }
}

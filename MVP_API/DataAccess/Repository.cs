using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MVP_API
{
    public interface IRepository
    {
        IEnumerable<Note> QueryNotes(Expression<Func<Note, bool>> expression);
        void Save(Note note);
    }

    public class Repository : IRepository
    {
        private readonly IMongoDatabase _database = null;

        public Repository(IOptions<DbSetting> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
             _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IEnumerable<Note> QueryNotes(Expression<Func<Note, bool>> expression)
        {
            return _database.GetCollection<Note>(typeof(Note).Name).AsQueryable().Where(expression).ToList();
        }

        public void Save(Note note) 
        {
            var collection = _database.GetCollection<Note>(typeof(Note).Name);

            collection.ReplaceOne(doc => doc.Id.Equals(note.Id), note,
                new UpdateOptions { IsUpsert = true });
        }
    }
}
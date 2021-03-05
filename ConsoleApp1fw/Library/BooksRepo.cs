using ConsoleApp1fw.Database;

using Dapper;

using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1fw
{
    public class BooksRepo
    {
        private SqlDatabase _db;

        public BooksRepo(SqlDatabase db)
        {
            _db = db;
        }

        private string GetFilteredQuery(string sql, BooksQueeryFilter filter = null)
        {
            var sqlBuilder = new SqlBuilder();

            var template = sqlBuilder.AddTemplate(sql);

            if (filter != null)
            {
                if (filter.Name != null)
                    sqlBuilder.Where("Books.Title = @Title");
                if (filter.AuthorId.HasValue)
                    sqlBuilder.Where("Books.AuthorId = @AuthorId");
            }

            return template.RawSql;
        }

        public IEnumerable<BookWithAuthorDto> GetBooks(BooksQueeryFilter filter = null)
        {
            var sql = @"
                SELECT 
                    Books.Id,
                    Books.Title,
                    Authors.Name AS Author
                FROM
                    Books
                INNER JOIN
                    Authors ON Authors.Id = Books.AuthorId
                /**where**/
            ";

            sql = GetFilteredQuery(sql, filter);

            using (var conn = _db.OpenConnection())
            {
                return conn.Query<BookWithAuthorDto>(sql, filter);
            }
        }

        public Author FindAuthor(int id)
        {
            const string sql = @"
                SELECT 
                    * 
                FROM 
                    Authors 
                WHERE 
                    Id = @Id;

                SELECT 
                    * 
                FROM 
                    Books 
                WHERE 
                    AuthorId = @Id;
            ";

            using (var conn = _db.OpenConnection())
            {
                var result = conn.QueryMultiple(sql, new { Id = id });
                var author = result.ReadFirstOrDefault<Author>();

                if (author != null)
                {
                    author.Books = result.Read<Book>().ToList();
                }

                return author;
            }
        }

        public IEnumerable<Author> GetAuthors()
        {
            const string sql = @"
                SELECT 
                    * 
                FROM 
                    Authors
                INNER JOIN 
                    Books ON Books.AuthorId = Authors.Id
            ";

            var cache = new Dictionary<int, Author>();

            using (var conn = _db.OpenConnection())
            {
                return conn.Query<Author, Book, Author>(sql, (author, book) =>
                {
                    if (!cache.TryGetValue(author.Id, out var a))
                    {
                        cache.Add(author.Id, author);
                        a = author;
                    }
                    a.Books.Add(book);
                    return a;
                }).Distinct();
            }
        }

        public int DeleteAuthor(int id)
        {
            const string sql = @"
                DELETE FROM 
                    Authors 
                WHERE 
                    Id = @Id
            ";

            return _db.ExecuteInTransaction((conn, trans) =>
            {
                return conn.Execute(sql, new { Id = id }, trans);
            });
        }

        public int InsertBook(Book book)
        {
            const string sql = @"
                INSERT INTO Books
                    (Title, AuthorId) 
                VALUES
                    (@Title, @AuthorId)
            ";

            return _db.ExecuteInTransaction((conn, trans) =>
            {
                return conn.Execute(sql, book);
            });
        }

        public int InsertBooks(IEnumerable<Book> books)
        {
            const string sql = @"
                INSERT INTO Books
                    (Title, AuthorId) 
                VALUES 
                    (@Title, @AuthorId)
            ";

            return _db.ExecuteInTransaction((conn, trans) =>
            {
                return conn.Execute(sql, books);
            });
        }
    }
}

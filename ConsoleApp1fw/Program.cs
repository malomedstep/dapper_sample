using ConsoleApp1fw.Database;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1fw
{
    class Program
    {
        static void Main(string[] args)
        {
            var connStr = "Data Source=.;Initial Catalog=DapperDb2;Integrated Security=true";
            var db = new SqlDatabase(connStr);
            var repo = new BooksRepo(db);

            //repo.InsertBooks(new[]
            //{
            //    new Book{ Title = "Book1", AuthorId = 3},
            //    new Book{ Title = "Book2", AuthorId = 3},
            //    new Book{ Title = "Book3", AuthorId = 3},
            //    new Book{ Title = "Book4", AuthorId = 3},
            //    new Book{ Title = "Book5", AuthorId = 3},
            //    new Book{ Title = "Book6", AuthorId = 3},
            //    new Book{ Title = "Book7", AuthorId = 3},
            //    new Book{ Title = "Book8", AuthorId = 3},
            //    new Book{ Title = "Book9", AuthorId = 3},
            //});

            repo.GetBooks(new BooksQueeryFilter
            {
                AuthorId = 2
            }).ToList().ForEach(Console.WriteLine);

            //var authors = repo.GetAuthors();

            //var author = repo.FindAuthor(2);

            //Console.WriteLine(author.Name);
            //author.Books.ToList().ForEach(Console.WriteLine);
        }
    }
}

using Library.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Tests
{
    [TestClass]
    public class CrudServiceTests
    {
        private InMemoryCrudServiceAsync<Book> _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new InMemoryCrudServiceAsync<Book>();
        }

        [TestMethod]
        public async Task CreateAndReadAsync_ShouldWorkCorrectly()
        {
            var book = Book.CreateNew();
            var created = await _service.CreateAsync(book);
            Assert.IsTrue(created);

            var fetched = await _service.ReadAsync(book.Id);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(book.Title, fetched.Title);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldModifyElement()
        {
            var book = Book.CreateNew();
            await _service.CreateAsync(book);
            book.Title = "Updated Title";
            var updated = await _service.UpdateAsync(book);

            var result = await _service.ReadAsync(book.Id);
            Assert.IsTrue(updated);
            Assert.AreEqual("Updated Title", result.Title);
        }

        [TestMethod]
        public async Task RemoveAsync_ShouldDeleteElement()
        {
            var book = Book.CreateNew();
            await _service.CreateAsync(book);
            var removed = await _service.RemoveAsync(book);

            var result = await _service.ReadAsync(book.Id);
            Assert.IsTrue(removed);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ReadAllAsync_ShouldReturnPagedResults()
        {
            for (int i = 0; i < 10; i++)
                await _service.CreateAsync(Book.CreateNew());

            var page = await _service.ReadAllAsync(page: 2, amount: 3);
            Assert.AreEqual(3, page.Count());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TodoApp.Models;
using Microsoft.AspNet.Identity;

namespace TodoApp.Controllers
{
    [Authorize]
    public class TodoItemsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/TodoItems
        public IQueryable<TodoItem> GetTodoItems()
        {
            var user = this.User.Identity;
            string userId = user.GetUserId().ToString();
            return db.TodoItems.Where(x => x.ApplicationUserId == userId);
        }

        // GET: api/TodoItems/5
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> GetTodoItem(int id)
        {
            TodoItem todoItem = await db.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }

        // PUT: api/TodoItems/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            db.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TodoItems
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = this.User.Identity;
            string userId = user.GetUserId().ToString();
            todoItem.ApplicationUserId = userId;
            db.TodoItems.Add(todoItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> DeleteTodoItem(int id)
        {
            TodoItem todoItem = await db.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            var user = this.User.Identity;
            string userId = user.GetUserId().ToString();
            if (todoItem.ApplicationUserId != userId)
            {
                return BadRequest("Unauthorized access");
            }

            db.TodoItems.Remove(todoItem);
            await db.SaveChangesAsync();

            return Ok(todoItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoItemExists(int id)
        {
            return db.TodoItems.Count(e => e.Id == id) > 0;
        }
    }
}
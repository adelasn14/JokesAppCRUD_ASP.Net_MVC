using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JokesWebApp.Data;
using JokesWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace JokesWebApp.Controllers
{
    public class JokesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<JokesController> _logger;

        public JokesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<JokesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Jokes
        public async Task<IActionResult> Index()
        {
            var jokes = await _context.Joke.ToListAsync();

            return View(jokes);
        }

        // GET: Jokes/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // GET: Jokes/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            var matchingJokes = await _context.Joke
        .Where(j => j.JokeQuestion.ToUpper().Contains(SearchPhrase.ToUpper()))
        .ToListAsync();
            return View("Index", matchingJokes);
        }

        // GET: Jokes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.Id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // GET: Jokes/Create
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jokes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, JokeQuestion, JokeAnswer, Poster")] Joke joke)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (currentUser != null)
                {
                    // Set the IdPoster property of the Joke entity
                    joke.Poster = currentUser.Id;

                    // Add the joke to the context and save changes
                    _context.Add(joke);
                    await _context.SaveChangesAsync();


                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Current user is null.");
                }
            }
            else
            {
                _logger.LogError("Model state is invalid. ModelState: {@ModelState}", ModelState);
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }

            return View(joke);
        }

        // GET: Jokes/Edit/5
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }
            return View(joke);
        }

        // POST: Jokes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JokeQuestion,JokeAnswer,Poster")] Joke joke)
        {
            if (id != joke.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                joke.Poster = currentUser.Id;

                try
                {
                    _context.Update(joke);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JokeExists(joke.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        // GET: Jokes/Delete/5
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.Id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // POST: Jokes/Delete/5
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Joke == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Joke'  is null.");
            }
            var joke = await _context.Joke.FindAsync(id);
            if (joke != null)
            {
                _context.Joke.Remove(joke);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokeExists(int id)
        {
          return (_context.Joke?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

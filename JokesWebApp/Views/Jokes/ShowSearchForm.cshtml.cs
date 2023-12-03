using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using JokesWebApp.Data;
using JokesWebApp.Models;
using Microsoft.EntityFrameworkCore;
using JokesWebApp.Controllers;

namespace JokesWebApp.Views.Jokes
{
    public class ShowSearchFormModel : PageModel
    {
        private readonly JokesWebApp.Data.ApplicationDbContext _context;
        private readonly ILogger<JokesController> _logger;


        public ShowSearchFormModel(JokesWebApp.Data.ApplicationDbContext context, ILogger<JokesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Joke Joke { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Joke == null || Joke == null)
            {
                return Page();
            }

            _context.Joke.Add(Joke);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

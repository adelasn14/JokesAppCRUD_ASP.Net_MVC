using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static System.Net.Mime.MediaTypeNames;

namespace JokesWebApp.Models
{
	public class Joke
	{
		public int Id { get; set; }
		public string JokeQuestion { get; set; }
		public string JokeAnswer { get; set; }
        public string Poster { get; set; }

        public Joke()
        {

        }
    }

}


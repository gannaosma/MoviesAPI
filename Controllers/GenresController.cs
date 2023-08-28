using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenresController : ControllerBase
	{
		private readonly IGenresSrevice _genresSrevice;

        public GenresController(IGenresSrevice genresSrevice)
        {
			_genresSrevice = genresSrevice;
        }

        [HttpGet]
		public async Task<IActionResult> GetAllGenres()
		{
			var geners = await _genresSrevice.GetAll();
			return Ok(geners);
		}

		[HttpPost]
		public async Task<IActionResult> CreateGenre(GenreDTO dto)
		{
			var genre = new Genre
			{
				Name = dto.Name
			};

			await _genresSrevice.Add(genre);

			return Ok(genre);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateGenre(byte id,[FromBody]GenreDTO dto)
		{
			var genre = await _genresSrevice.GetById(id);

			if (genre == null)
				return NotFound($"No genre with id {id}");

			genre.Name = dto.Name;

			_genresSrevice.Update(genre);

			return Ok(genre);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteGenre(byte id)
		{
			var genre = await _genresSrevice.GetById(id);

			if (genre == null)
				return NotFound($"No genre with id {id}");

			_genresSrevice.Delete(genre);

			return Ok(genre);
		}
	}
}

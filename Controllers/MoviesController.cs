using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IMoviesService _moviesService;
		private readonly IGenresSrevice _genresSrevice;

		private new List<string> _allowedExtention = new List<string> { ".jpg", ".png" };
		private long _maxAllowedPosterSize = 1048576;
		public MoviesController(IMoviesService moviesService, IGenresSrevice genresSrevice, IMapper mapper)
		{
			_moviesService = moviesService;
			_genresSrevice = genresSrevice;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllMovie()
		{
			var movies = await _moviesService.GetAll();
			return Ok(movies);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound();

			return Ok(movie);
		}

		[HttpGet("GetByGenreId")]
		public async Task<IActionResult> GetByGenreId(byte genreId)
		{
			var movies = await _moviesService.GetAll(genreId);

			return Ok(movies);
		}

		[HttpPost]
		public async Task<IActionResult> CreateMovie([FromForm] MovieDTO dto)
		{
			if (dto.Poster == null)
				return BadRequest("Poster is required");

			if (!_allowedExtention.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
				return BadRequest("Only .png, .jpg are allowed");

			if (dto.Poster.Length > _maxAllowedPosterSize)
				return BadRequest("Max allowed size for Poster is 1 MB");

			var isValidGenre = await _genresSrevice.IsvalidGenre(dto.GenreId);

			if (!isValidGenre)
				return BadRequest("Invalid genre Id");

			using var datastream = new MemoryStream();

			await dto.Poster.CopyToAsync(datastream);

			var movie = _mapper.Map<Movie>(dto);
			movie.Poster = datastream.ToArray();

			await _moviesService.Add(movie);

			return Ok(movie);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateMovie(int id,[FromForm]MovieDTO dto)
		{
			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound($"No movie found with {id}");

			var isValidGenre = await _genresSrevice.IsvalidGenre(dto.GenreId);

			if (!isValidGenre)
				return BadRequest("Invalid genre Id");

			if(dto.Poster != null)
			{
				if (!_allowedExtention.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
					return BadRequest("Only .png, .jpg are allowed");

				if (dto.Poster.Length > _maxAllowedPosterSize)
					return BadRequest("Max allowed size for Poster is 1 MB");

				using var datastream = new MemoryStream();

				await dto.Poster.CopyToAsync(datastream);

				movie.Poster = datastream.ToArray();
			}

			movie.Title = dto.Title;
			movie.GenreId = dto.GenreId;
			movie.Year = dto.Year;
			movie.Storyline = dto.Storyline;
			movie.Rate = dto.Rate;

			_moviesService.Update(movie);

			return Ok(movie);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMovie(int id)
		{
			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound($"No movie found with {id}");

			_moviesService.Delete(movie);

			return Ok(movie);
		}
		
	}
}

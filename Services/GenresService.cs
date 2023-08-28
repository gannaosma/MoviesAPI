using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Services
{
	public class GenresService : IGenresSrevice
	{
		private readonly ApplicationDbContext _context;

		public GenresService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Genre>> GetAll()
		{
			return await _context.Genres.OrderBy(i => i.Name).ToListAsync();
		}

		public async Task<Genre> GetById(byte id)
		{
			return await _context.Genres.FirstOrDefaultAsync(i => i.Id == id);
		}

		public async Task<Genre> Add(Genre genre)
		{
			await _context.Genres.AddAsync(genre);
			_context.SaveChanges();

			return genre;
		}

		public Genre Update(Genre genre)
		{
			_context.Update(genre);
			_context.SaveChanges();

			return genre;
		}

		public Genre Delete(Genre genre)
		{
			_context.Remove(genre);
			_context.SaveChanges();

			return genre;
		}

		public Task<bool> IsvalidGenre(byte id)
		{
			return _context.Genres.AnyAsync(g => g.Id == id);
		}
	}
}

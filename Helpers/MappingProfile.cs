using AutoMapper;

namespace MoviesAPI.Helpers
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
            CreateMap<MovieDTO, Movie>()
                .ForMember(src => src.Poster, opt => opt.Ignore());
        }
    }
}

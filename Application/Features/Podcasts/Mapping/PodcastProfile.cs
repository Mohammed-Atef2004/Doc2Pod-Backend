using Application.Features.Podcasts.Query.GetAllPodcasts;
using AutoMapper;
using Domain.Entities;


namespace Application.Features.Podcasts.Mapping
{
    public class PodcastProfile : Profile
    {
        public PodcastProfile()
        {
            CreateMap<Podcast, GetAllPodcastsResponse>()
                .ForCtorParam("PodcastId", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Mode", opt => opt.MapFrom(src => src.Mode))
                .ForCtorParam("PodcastName", opt => opt.MapFrom(src => src.Document.FileName))
                .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
                .ForCtorParam("Topic", opt => opt.MapFrom(src => src.Topic))
                .ForCtorParam("StartPage", opt => opt.MapFrom(src => src.StartPage))
                .ForCtorParam("EndPage", opt => opt.MapFrom(src => src.EndPage));
        }
    }
}

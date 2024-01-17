using AutoMapper;

namespace Aspu.Template.Domain.Dto;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}

using AutoMapper;
using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;

public class FuncionarioProfile : Profile
{
    public FuncionarioProfile()
    {
        CreateMap<FuncionarioViewModel, Funcionario>()
            .ForMember(dest => dest.Cargo, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
            .ForMember(dest => dest.UsuarioFuncionario, opt => opt.MapFrom(src => src.UsuarioFuncionario));
    }
}

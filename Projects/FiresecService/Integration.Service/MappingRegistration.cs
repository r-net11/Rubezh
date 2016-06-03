using AutoMapper;
using Integration.Service.Converters;
using Integration.Service.Entities;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;

namespace Integration.Service
{
	internal static class MappingRegistration
	{
		public static void RegisterMapping()
		{
			Mapper.CreateMap<string, int>().ConvertUsing<StringToIntConverter>();
			Mapper.CreateMap<string, int?>().ConvertUsing<StringToIntNullableConverter>();
			Mapper.CreateMap<string, bool>().ConvertUsing<StringToBoolConverter>();
			Mapper.CreateMap<string, bool?>().ConvertUsing<StringToBoolNullableConverter>();
			Mapper.CreateMap<string, OPCZoneType?>().ConvertUsing<StringToOPCZoneTypeConverter>();
			Mapper.CreateMap<string, GuardZoneType?>().ConvertUsing<StringToGuardZoneTypeConverter>();

			Mapper.CreateMap<ScriptMessage, Script>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled));

			Mapper.CreateMap<OPCZoneMessage, OPCZone>()
				.ForMember(dest => dest.No, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.AutoSet, opt => opt.MapFrom(src => src.Autoset))
				.ForMember(dest => dest.Delay, opt => opt.MapFrom(src => src.Delay))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.GuardZoneType, opt => opt.MapFrom(src => src.GuardZoneType))
				.ForMember(dest => dest.IsSkippedTypeEnabled, opt => opt.MapFrom(src => src.IsSkippedTypeEnabled))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ZoneType));
		}
	}
}

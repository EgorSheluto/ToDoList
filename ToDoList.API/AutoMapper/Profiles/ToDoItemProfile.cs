using AutoMapper;
using System;
using ToDoList.Common.Enums;
using ToDoList.DAL.Models;
using ToDoList.DTO.ToDoItem;

namespace ToDoList.API.AutoMapper.Profiles
{
    public class ToDoItemProfile : Profile
    {
        public ToDoItemProfile()
        {
            CreateMap<ToDoItemModel, ToDoItemDto>()
                .ForMember(dest => dest.StatusId, source => source.MapFrom(s => (int)s.Status));
            CreateMap<ToDoItemNewDto, ToDoItemModel>()
                .ForMember(dest => dest.CreateDateUTC, source => source.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, source => source.MapFrom(_ => ToDoItemStatus.New));
            /*CreateMap<ToDoItemUpdateDto, ToDoItemModel>()
                .ForMember(dest => dest.UpdateDateUTC, );*/
        }
    }
}

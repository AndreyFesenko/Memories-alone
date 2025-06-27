// src/NotificationService/NotificationService.Application/Mappings/NotificationMappingProfile.cs
using AutoMapper;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<NotificationTemplate, NotificationTemplateDto>().ReverseMap();
        CreateMap<NotificationMessage, NotificationMessageDto>().ReverseMap();
    }
}

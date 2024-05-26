using AcmeCorp.Application.DTOs;
using AcmeCorp.Domain.Entities;
using AutoMapper;

namespace AcmeCorp.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<ContactInfo, ContactInfoDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
        }
    }
}

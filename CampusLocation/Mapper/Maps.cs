using AutoMapper;
using CampusLocation.Data;
using CampusLocation.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLocation.Mapper
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Location, LocationDTO>().ReverseMap();
            CreateMap<Location, LocationUpdateDTO>().ReverseMap();
        }
    }
}

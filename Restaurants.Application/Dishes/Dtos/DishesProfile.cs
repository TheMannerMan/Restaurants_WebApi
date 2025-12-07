using AutoMapper;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Dishes.Dtos
{
	public class DishesProfile : Profile
	{
		public DishesProfile()
		{
			CreateMap<Dish, DishDto>();
		}
		
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Domain.Exceptions;

public class NotFoundException(string resourceType, string resourceIdentifier) : 
	Exception($"{resourceType} with id: {resourceIdentifier} doesn't exist")
{
}

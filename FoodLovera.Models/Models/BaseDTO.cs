using System;
using System.Collections.Generic;
using System.Text;

namespace FoodLovera.Models.Models
{
    public class BaseDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}

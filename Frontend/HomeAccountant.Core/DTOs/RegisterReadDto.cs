using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.DTOs
{
    public class RegisterReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class Pagination
    {
        public int NumPagina { get; set; }
        public int RegistoPorPagina { get; set; }
        public int TotalRegistos { get; set; }

        public int TotalPaginas { get; set; }
    }
}

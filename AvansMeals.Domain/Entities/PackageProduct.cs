using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Domain.Entities
{
    public class PackageProduct
    {
        public int PackageId { get; set; }
        public Package Package { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}

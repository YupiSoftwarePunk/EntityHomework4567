using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public Company CompanyEntity { get; set; }
    }
}

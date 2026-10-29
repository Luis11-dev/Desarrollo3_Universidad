using Universidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Repositories.Sales
{
    public class SalesRepository
    {
        private readonly List<Sale> _sales = new List<Sale>();

        public void Add(Sale sale)
        {
            _sales.Add(sale);
        }

        public IEnumerable<Sale> GetAll()
        {
            return _sales.ToList();
        }

        public Sale? GetById(Guid id)
        {
            return _sales.FirstOrDefault(s => s.Id == id);
        }
    }
}

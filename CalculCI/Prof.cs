using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculCI
{
    class Prof
    {
        public string Nom { get; }
        public bool TempsPlein { get; set; }
        public ICollection<Allocation> AllocationsPreAlloues { get; } = new List<Allocation>();
        //public ICollection<Allocation> AllocationsForcees { get; }
        public IDictionary<string, Allocation> AllocationsDesirees { get; } = new Dictionary<string, Allocation>();

        public Prof(string nom, bool tempsPlein)
        {
            Nom = nom;
            TempsPlein = tempsPlein;
        }

        public void AjouteAllocation(Allocation allocation)
        {
            AllocationsPreAlloues.Add(allocation);
            allocation.Assigne = true;
        }

        public void EnleveAllocation(Allocation allocation)
        {
            AllocationsPreAlloues.Remove(allocation);
            allocation.Assigne = false;
        }

        public void AjouteAllocationDesirees(Allocation allocation)
        {
            AllocationsDesirees.Add(allocation.Nom, allocation);
            allocation.Assigne = true;
        }
    }
}

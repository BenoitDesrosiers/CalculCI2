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
        public int NbrCoursAssignes;

        public List<Allocation> AllocationActuelle { get; } = new List<Allocation>();
        //public ICollection<Allocation> AllocationsForcees { get; }
        public IDictionary<string, Allocation> AllocationsDesirees { get; } = new Dictionary<string, Allocation>();

        public Prof(string nom, bool tempsPlein)
        {
            Nom = nom;
            TempsPlein = tempsPlein;
        }

        public void AjouteAllocation(Allocation allocation)
        {
            AllocationActuelle.Add(allocation);
            allocation.Assigne = true;
            if(allocation.ComptePourNbrCours())
                { NbrCoursAssignes++; }
        }

        public void EnleveAllocation(Allocation allocation)
        {
            AllocationActuelle.Remove(allocation);
            allocation.Assigne = false;
            if(allocation.ComptePourNbrCours())
                { NbrCoursAssignes--; }
        }

        public void AjouteAllocationDesirees(Allocation allocation)
        {
            AllocationsDesirees.Add(allocation.Nom, allocation);
        }

        public double CiActuelle()
        {
            double ci = 0;
            foreach(Allocation alloc in AllocationActuelle)
                { ci += alloc.CalculCI(NbrCoursAssignes); }
            return ci;
        }

        public bool PeutPrendreAlloc(Allocation allocAtester)
        {
            double ci = 0;
            int coursDePlus = 0;
            coursDePlus = allocAtester.ComptePourNbrCours() ? 1 : 0;
            foreach (Allocation alloc in AllocationActuelle)
            { ci += alloc.CalculCI(NbrCoursAssignes+coursDePlus); }
            ci += allocAtester.CalculCI(NbrCoursAssignes + coursDePlus);
            return ci <= Constantes.CIMaxSession;
        }
    }
}

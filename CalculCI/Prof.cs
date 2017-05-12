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
        public double CiMinimun { get; set; }
        public int NbrCoursAssignes;

        public List<Allocation> AllocationPreAlloueA { get; } = new List<Allocation>();
        public List<Allocation> AllocationPreAlloueH { get; } = new List<Allocation>();

        public IDictionary<string, Allocation> AllocationsDesireesA { get; } = new Dictionary<string, Allocation>();
        public IDictionary<string, Allocation> AllocationsDesireesH { get; } = new Dictionary<string, Allocation>();

        public List<ulong> AllocationsPossibles { get; set; } = new List<ulong>();
        public ulong MaskPreAlloue { get; set; } = 0;
         
        public Prof(string nom, double ciMin)
        {
            Nom = nom;
            CiMinimun = ciMin;
        }

        public void AjoutePreAllocation(Allocation allocation)
        {
            if ( !estNouvelleAllocation(allocation) )
            {
                throw new IndexOutOfRangeException();
            }

            AllocationPreAlloueA.Add(allocation);
            MaskPreAlloue += allocation.BinId;
            allocation.Assigne = true;
            if(allocation.ComptePourNbrCours())
                { NbrCoursAssignes++; }
        }

        public void EnlevePreAllocation(Allocation allocation)
        {
            if (estNouvelleAllocation(allocation))
            {
                throw new IndexOutOfRangeException();
            }
            AllocationPreAlloueA.Remove(allocation);
            MaskPreAlloue -= allocation.BinId;
            allocation.Assigne = false;
            if(allocation.ComptePourNbrCours())
                { NbrCoursAssignes--; }
        }

        public void AjouteAllocationDesirees(Allocation allocation)
        {
            AllocationsDesireesA.Add(allocation.Nom, allocation);
        }

        public double CiActuelle()
        {
            double ci = 0;
            foreach(Allocation alloc in AllocationPreAlloueA)
                { ci += alloc.CalculCI(NbrCoursAssignes); }
            return ci;
        }

        public bool PeutPrendreAlloc(Allocation allocAtester)
        {
            double ci = 0;
            int coursDePlus = 0;
            coursDePlus = allocAtester.ComptePourNbrCours() ? 1 : 0;
            foreach (Allocation alloc in AllocationPreAlloueA)
            { ci += alloc.CalculCI(NbrCoursAssignes+coursDePlus); }
            ci += allocAtester.CalculCI(NbrCoursAssignes + coursDePlus);
            return ci <= Constantes.CIMaxSession;
        }
       
        /// </summary>
        /// <param name="allocsDePlus">l'allocation a ajouter par dessus la pre-allouée</param>
        /// <param name="nbrCoursDePlus">le nombre de cours de plus que la pre-allouée</param>
        /// <returns></returns>
        public bool PeutPrendreAllocList(List<Allocation> allocsDePlus, int nbrCoursDePlus)
        {
            return CiPourAllocListAdditionnel(allocsDePlus, nbrCoursDePlus) <= Constantes.CIMaxSession;
        }
       
        public double CiPourAllocListAdditionnel(List< Allocation> allocsDePlus, int nbrCoursDePlus)
        {
            double ci = 0;
            foreach (Allocation alloc in AllocationPreAlloueA)
                { ci += alloc.CalculCI(NbrCoursAssignes + nbrCoursDePlus); }

            foreach (Allocation alloc in allocsDePlus)
                { ci += alloc.CalculCI(NbrCoursAssignes + nbrCoursDePlus); }
            return ci;
        }


        private bool estNouvelleAllocation(Allocation alloc)
        {
            return (MaskPreAlloue ^ alloc.BinId) != MaskPreAlloue;
        }

        public void AllocationPossibleAjouteListe(List<Allocation> allocs)
        {
            ulong mask = 0;
            foreach(Allocation alloc in allocs)
            {
                mask += alloc.BinId;
            }
            AllocationsPossibles.Add(mask);
        }

        public void DumpAllocationsPossibles()
        {
            foreach (ulong alloc in AllocationsPossibles)
                { Console.Write("{0}, ", alloc); }
            Console.WriteLine();
        }
    }
}

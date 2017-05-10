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

        public List<Allocation> AllocationPreAlloue { get; } = new List<Allocation>();
        //public ICollection<Allocation> AllocationsForcees { get; }
        public IDictionary<string, Allocation> AllocationsDesirees { get; } = new Dictionary<string, Allocation>();

        public List<ulong> AllocationsPossibles { get; } = new List<ulong>();
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

            AllocationPreAlloue.Add(allocation);
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
            AllocationPreAlloue.Remove(allocation);
            MaskPreAlloue -= allocation.BinId;
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
            foreach(Allocation alloc in AllocationPreAlloue)
                { ci += alloc.CalculCI(NbrCoursAssignes); }
            return ci;
        }

        public bool PeutPrendreAlloc(Allocation allocAtester)
        {
            double ci = 0;
            int coursDePlus = 0;
            coursDePlus = allocAtester.ComptePourNbrCours() ? 1 : 0;
            foreach (Allocation alloc in AllocationPreAlloue)
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
            foreach (Allocation alloc in AllocationPreAlloue)
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

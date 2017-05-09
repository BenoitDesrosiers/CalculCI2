using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace CalculCI
{
    class Calculateur
    {
        static IList<Prof> Profs;
        static IList<Allocation> Allocations;

        public Calculateur(IList<Prof> profs, IList<Allocation> allocations )
        {
            Profs = profs;
            Allocations = allocations;
        }

        public void Calcul()
        {
            Debug.WriteLine("début des calculs");
            AjoutAlloc(Profs, 0, 0);
        }

        static void AjoutAlloc(IList<Prof> profs, int indexProf, int indexLibre)
        {
            Allocation nouvelle = null;

            if (indexProf == profs.Count )
            {
                Debug.WriteLine("profs empty");
                // on a fait le tour, on dépille
                AfficheResultat();
                return; 
            }
            else
            {
                Debug.WriteLine(string.Format("AjoutAlloc {0} ci {1}", indexProf, profs[indexProf].CiActuelle()));

                List<Allocation> alloc1 = AllocLibrePourProf(profs[indexProf]);
                if (alloc1.Count == 0)
                {
                    // on a fini un prof. On passe au suivant. 
                    AjoutAlloc(profs, indexProf+1, 0);

                }
                else
                {
                    List<Allocation> preAlloues = profs[indexProf].AllocationActuelle;
                    if(profs[indexProf].PeutPrendreAlloc(alloc1[indexLibre]) )
                    {
                        nouvelle = alloc1[indexLibre];
                        profs[indexProf].AjouteAllocation(nouvelle);
                        Debug.Write(nouvelle.Nom);
                    }
                    AjoutAlloc(profs, indexProf, indexLibre);

                }

                //On dépile
                indexProf--;
                indexLibre++;
                if(nouvelle != null )
                    { profs[indexProf].EnleveAllocation(nouvelle); }

                AjoutAlloc(profs, indexProf, indexLibre);
            }

        }

       /* obsolete, remplacé par prof.PeutPrendreAlloc()
       private static double CalculLaCI(IList<Allocation> allocations)
        {
            int nbrCours = 0;
            foreach (Allocation alloc in allocations)
            { nbrCours += alloc.ComptePourNbrCours() ? 1 : 0; }

            double ci = 0;
            foreach (Allocation alloc in allocations)
            {
                ci += alloc.CalculCI(nbrCours);
            }

            return ci;
        }
        */

        static void AfficheResultat()
        {
            Console.WriteLine("------------------------------------");
            foreach(Prof unprof in Profs)
            {
                Console.WriteLine("{0} ci {1}", unprof.Nom, unprof.CiActuelle());
            }
        }

        static List<Allocation> AllocLibrePourProf(Prof prof)
        {
            double ciActuelle = prof.CiActuelle();
            List<Allocation> libre = new List<Allocation>();
            if (ciActuelle < Constantes.CIMaxSession)
            {
                foreach(Allocation alloc in prof.AllocationsDesirees.Values)
                {
                    if(!alloc.estAssigne() & prof.PeutPrendreAlloc(alloc))
                    {
                        libre.Add(alloc);
                    }
                }

            }
            return libre;
        }
    }
}

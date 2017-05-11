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
        static int NbrAlloc;
        public Calculateur(IList<Prof> profs, IList<Allocation> allocations, int nbrAlloc )
        {
            Profs = profs;
            Allocations = allocations; // ca sert à quoi ca ??? 
            NbrAlloc = nbrAlloc;
        }

        public void Calcul()
        {
            Console.WriteLine("début des calculs");
            Console.WriteLine("Calcule toutes les possibilités par prof");
            foreach (Prof unProf in Profs)
            {
                CalculPossibilite(unProf, unProf.MaskPreAlloue, unProf.MaskPreAlloue, new List<Allocation>(), 0);
                //unProf.DumpAllocationsPossibles();
                Console.WriteLine("{0} {1}", unProf.Nom, unProf.AllocationsPossibles.Count);
            }

            Console.WriteLine("Calcule toutes les combinaisons gagnantes");

            ulong[] solutionVide = new ulong[Profs.Count];
            TrouveBonneCharge(0, new List<Array>() { solutionVide });


        }


        static void CalculPossibilite(Prof prof, ulong maskAlloc, ulong fait, List<Allocation> allocAdditionnelle, int nbrCoursAdditionnels)
        {
            foreach(Allocation alloc in prof.AllocationsDesirees.Values)
            {
                // si libre et pas encore fait
                if (alloc.estLibre() & ((fait | alloc.BinId) != fait )) 
                {
                    fait += alloc.BinId;
                    allocAdditionnelle.Add(alloc);
                    nbrCoursAdditionnels += alloc.ComptePourNbrCours() ? 1 : 0;
                    
                    if(prof.PeutPrendreAllocList(allocAdditionnelle, nbrCoursAdditionnels))
                    {
                        maskAlloc += alloc.BinId;

                        // c'est une bonne alloc, mais ca vaut pas la peine de l'ajouter si elle est pas assez grande
                        if (prof.CiPourAllocListAdditionnel(allocAdditionnelle, nbrCoursAdditionnels) >= prof.CiMinimun)
                        {
                            prof.AllocationsPossibles.Add(maskAlloc);
                        }
                        else
                        {

                        }
                        //Console.WriteLine("{0} {1} {2}", maskAlloc, ConvertToBinary(maskAlloc), prof.CiPourAllocListAdditionnel(allocAdditionnelle, nbrCoursAdditionnels));
                        CalculPossibilite(prof, maskAlloc, fait, allocAdditionnelle, nbrCoursAdditionnels);
                        maskAlloc -= alloc.BinId;

                    }
                    nbrCoursAdditionnels -= alloc.ComptePourNbrCours() ? 1 : 0;
                    allocAdditionnelle.Remove(alloc);

                }
            }
        }

        static void TrouveBonneCharge(int indexProf, List<Array> bonneAlloc)
        {
            List<Array> vraieBonneAlloc = new List<Array>(); // contiendra juste les allocs qui ont tous les cours
            if(indexProf==Profs.Count)
            {
                // on a finit, on affiche pis on sort d'ici
                Console.WriteLine("en Partant {0}", bonneAlloc.Count);
                

                // Élimine les allocation dans lequel certains cours sont non-assignés
                foreach (ulong[] uneBonneAlloc in bonneAlloc)
                {
                    ulong allocTotal = UlongSum(uneBonneAlloc);
                    
                    // Conserve uniquement les allocs ou tous les cours sont assignés.
                    if((allocTotal != ((ulong) Math.Pow(2,NbrAlloc )-1)))
                    {
                        vraieBonneAlloc.Add(uneBonneAlloc);
                    }
                   
                }
                Console.WriteLine("conservée {0}", vraieBonneAlloc.Count);
                foreach (ulong[] uneBonneAlloc in vraieBonneAlloc)
                {
                    int profid = 0;
                    foreach(ulong allocProf in uneBonneAlloc)
                    {
                        Console.WriteLine("{1} {0}", Profs[profid].Nom, ConvertToBinary(allocProf));
                        profid++;
                    }
                    Console.WriteLine();
                   

                }


                return;
            }
            List<Array> listDeNouvelleBonneAlloc = new List<Array>();

            // passe à travers toutes les allocations possibles pour ce profs
            foreach (ulong allocProf in Profs[indexProf].AllocationsPossibles)
            {
                //check si l'alloc du prof est en conflit avec toutes les allocs trouvées à date
                foreach (ulong[] uneBonneAlloc in bonneAlloc)
                {
                    ulong allocTotal = UlongSum(uneBonneAlloc);
                    if ((allocTotal & allocProf) == 0)
                    {
                        // c'est une bonne alloc (pas en conflit), on l'ajoute 
                        
                        ulong[] uneNouvelleBonneAlloc = new ulong[Profs.Count];
                        uneBonneAlloc.CopyTo(uneNouvelleBonneAlloc, 0);
                        uneNouvelleBonneAlloc[indexProf] = allocProf;
                        listDeNouvelleBonneAlloc.Add(uneNouvelleBonneAlloc);
                    }
                   
                }
                Console.Write(".");
                    
            }
            Console.WriteLine("-------------- {0}", indexProf);
            // on a fini ce prof, on passe au suivant avec toutes les allocs encore bonnes
            bonneAlloc = null; // flush la vieille liste car elle ne sert plus à rien. 
            TrouveBonneCharge(++indexProf, listDeNouvelleBonneAlloc);

        }

        // ulong.sum ca existe pas de base. 
        public static ulong UlongSum(ulong[] source)
        {
            ulong total = 0;

            foreach (var item in source)
                total += item;

            return total;
        }



        public static string ConvertToBinary(ulong value)
        {
            if (value == 0) return "".PadLeft(NbrAlloc,'0');
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            while (value != 0)
            {
                b.Insert(0, ((value & 1) == 1) ? '1' : '0');
                value >>= 1;
            }
            return b.ToString().PadLeft(NbrAlloc, '0');
        }


        //***********************************************************************
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
                    List<Allocation> preAlloues = profs[indexProf].AllocationPreAlloue;
                    if(profs[indexProf].PeutPrendreAlloc(alloc1[indexLibre]) )
                    {
                        nouvelle = alloc1[indexLibre];
                        profs[indexProf].AjoutePreAllocation(nouvelle);
                        Debug.Write(nouvelle.Nom);
                    }
                    AjoutAlloc(profs, indexProf, indexLibre);

                }

                //On dépile
                indexProf--;
                indexLibre++;
                if(nouvelle != null )
                    { profs[indexProf].EnlevePreAllocation(nouvelle); }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculCI
{
    class Cours : Allocation
    {
        public int NbrSemaine { get; }
        public int NbrEtudiant { get; }
        public int NumeroGroupe { get; }
        public int NbrPrestation { get; }

        public Cours(string nom, int nbrEtudiant, int nbrPrestation,  int numeroGroupe = 0, int nbrSemaine = 15) : base(nom)
        {
            NbrSemaine = nbrSemaine;
            NbrEtudiant = nbrEtudiant;
            NumeroGroupe = numeroGroupe;
            NbrPrestation = nbrPrestation;

            CiSimple = CalculCI(1);

        }

        public override bool ComptePourNbrCours()
        {
            return true;
        }


        public override double CalculCI(int nbrCours)
        {
            double multiplicateur =  nbrCours == 3 ? 1.1 : nbrCours == 4 ? 1.75 : 0.9;

            double hp = NbrPrestation * NbrSemaine / 15 * multiplicateur;
            double hc = NbrPrestation * NbrSemaine / 15 * 1.2;
            double Ni = NbrEtudiant * 0.04 * NbrPrestation;

            return hp + hc + Ni;
        }
    }
}

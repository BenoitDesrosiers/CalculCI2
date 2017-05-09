using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculCI
{
    abstract class Allocation
    {
        public string Nom { get; }
        public bool Assigne = false;
        public double CiSimple { get; set; } = 0;  // la CI pour ce cours. Précalculée pour accélerer les calculs



        public Allocation(string nom)
        {
            Nom = nom;

        }

        abstract public double CalculCI(int nbrCours);

        abstract public bool ComptePourNbrCours();

        public bool estAssigne()
        {
            return Assigne;
        }
    }
}

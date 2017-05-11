using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ksiegarnia.Models
{
    public class PozycjaKoszyka
    {
        public AutorzyKsiazki ksiazka { get; set; }
        public int ilosc { get; set; }
        public decimal wartosc { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ksiegarnia.Models;
namespace Ksiegarnia.Infrastrcture
{
    public class KoszykManager
    {
        private KsiegarniaEntities1 db;
        private ISessionManager session;

        public KoszykManager(ISessionManager session, KsiegarniaEntities1 db)
        {
            this.session = session;
            this.db = db;
        }

        public List<PozycjaKoszyka> pobierzKoszyk()
        {
            List<PozycjaKoszyka> koszyk;

            if (session.Get<List<PozycjaKoszyka>>(Consts.KoszykSessionKlucz) == null)
            {
                koszyk = new List<PozycjaKoszyka>();
            }
            else
            {
                koszyk = session.Get<List<PozycjaKoszyka>>(Consts.KoszykSessionKlucz) as List<PozycjaKoszyka>;
            }

            return koszyk;
        }

        public void dodajDoKoszyka(int ksiazkaId)
        {
            var koszyk = pobierzKoszyk();
            var pozycjaKoszyka = koszyk.Find(k => k.ksiazka.id_ksiazka == ksiazkaId);

            if (pozycjaKoszyka != null)
                pozycjaKoszyka.ilosc++;
            else
            {
                var ksiazkaDoDodania = db.AutorzyKsiazki.Where(k => k.id_ksiazka == ksiazkaId).SingleOrDefault();
                if (ksiazkaDoDodania != null)
                {
                    var nowaPozycjaKoszyka = new PozycjaKoszyka()
                    {
                        ksiazka = ksiazkaDoDodania,
                        ilosc = 1,
                        wartosc = (decimal)ksiazkaDoDodania.Ksiazki.cena_brutto_aktualna
                    };

                    koszyk.Add(nowaPozycjaKoszyka);
                }
            }
            session.Set(Consts.KoszykSessionKlucz, koszyk);
        }
        
        public int usunZKoszyka(int ksiazkaId)
        {
            var koszyk = pobierzKoszyk();
            var pozycjaKoszyka = koszyk.Find(k => k.ksiazka.id_ksiazka == ksiazkaId);

            if(pozycjaKoszyka != null)
            {
                if(pozycjaKoszyka.ilosc > 1)
                {
                    pozycjaKoszyka.ilosc--;
                    return pozycjaKoszyka.ilosc;
                }
                else
                {
                    koszyk.Remove(pozycjaKoszyka);
                }
            }
            return 0;
        }

        public decimal pobierzWartoscKoszyka()
        {
            var koszyk = pobierzKoszyk();
            int ilosc = koszyk.Sum(k => k.ilosc);

            return ilosc;
        }

        public Zamowienia utworzNoweZamowienie(Zamowienia noweZamowienie, int userId)
        {
            var koszyk = pobierzKoszyk();
            noweZamowienie.data = DateTime.Now;
            noweZamowienie.id_klient = userId;

            db.Zamowienia.Add(noweZamowienie);

            if(noweZamowienie.Zamowienia_ksiazki == null)
            {
                noweZamowienie.Zamowienia_ksiazki = new List<Zamowienia_ksiazki>();
            }

            decimal wartoscKoszyka = 0;

            foreach (var item in koszyk)
            {
                var nowaPozycjaZamowienia = new Zamowienia_ksiazki()
                {
                    id_ksiazki = item.ksiazka.id_ksiazka,
                    ilosc_ksiazek = item.ilosc,
                    cena_brutto_zakupu = item.ksiazka.Ksiazki.cena_brutto_aktualna,
                    cena_netto_zakupu = item.ksiazka.Ksiazki.cena_netto_aktualna,
                    proc_vat_zakupu = item.ksiazka.Ksiazki.proc_vat_aktualny,
                    tytul = item.ksiazka.Ksiazki.tytul,
                    id_zamowienia = noweZamowienie.id_zamowienia
                };

                wartoscKoszyka += (decimal)(item.ilosc * item.ksiazka.Ksiazki.cena_netto_aktualna);
                noweZamowienie.Zamowienia_ksiazki.Add(nowaPozycjaZamowienia);
            }
            noweZamowienie.suma = (double)wartoscKoszyka;
            db.SaveChanges();

            return noweZamowienie;
        }

        public void pustyKoszyk()
        {
            session.Set<List<PozycjaKoszyka>>(Consts.KoszykSessionKlucz, null);
        }

        public int pobierzIloscPozycjiKoszyka()
        {
            var koszyk = pobierzKoszyk();
            int ilosc = koszyk.Sum(k => k.ilosc);

            return ilosc;
        }
    }
}
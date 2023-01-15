using Microsoft.EntityFrameworkCore.Update.Internal;
using PasswordGenerator;
using System.ComponentModel.DataAnnotations;

namespace NajdiSpolubydlícíWeb.Models
{
    public class Nabidka
    {
        [Key]
        public int Id { get; set; }

        public string Password { get; set; }

        public bool Aktivni { get; set; }

        public DateTime DatumVytvoreni { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Toto pole je povinné")] 
        [EmailAddress(ErrorMessage = "Zadejte email ve správném formátu, př.: info@najdispolubydlici.cz")]
        public string Email { get; set; }

        [Required, RegularExpression("^\\+[1-9]{1}[0-9]{3,14}$", ErrorMessage = "Číslo zadejte ve vzoru +420 123 123 123 ")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné")]
        public string Cena { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné")]
        public string Okres { get; set; }

        [MaxLength(100, ErrorMessage = "Maximální počet znaků: 100")]
        public string? Lokalita { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^(?:Dům|Byt|Jiné)$", ErrorMessage = "Vyberte z možností")]
        public string Typ { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), Range(1, 100000, ErrorMessage = "Neplatná hodnota")]
        public int Mistnosti { get; set; }
        [Required(ErrorMessage = "Toto pole je povinné"), Range(1, 500, ErrorMessage = "Neplatná hodnota")]
        public int CelkoveMetry { get; set; }
        [Required(ErrorMessage = "Toto pole je povinné"), Range(1, 100000, ErrorMessage = "Neplatná hodnota")]
        public int MetryPokoje { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^(?:Ano|Ne)$", ErrorMessage = "Vyberte z možností")]
        public string Vybaveno { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^(?:Ano|Ne)$", ErrorMessage = "Vyberte z možností")]
        public string Internet { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^(?:Ano|Ne)$", ErrorMessage = "Vyberte z možností")]
        public string Wifi { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^(?:Ano|Ne)$", ErrorMessage = "Vyberte z možností")]
        public string ZvireByt { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^(?:Ano|Ne)$", ErrorMessage = "Vyberte z možností")]
        public string SvojeZvire { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), Range(-100, 500)]
        public int Podlazi { get; set; }

        [Required]
        public DateTime DatumNastehovani { get; set; }

        public DateTime DatumPosledniZmeny { get; set; }

        public Nabidka() { }

        public Nabidka(NabidkaViewModel objView)
        {
            Password = VytvoritHeslo();
            Aktivni = true;
            Telefon = objView.Telefon.Replace(" ", string.Empty);
            Email = objView.Email;
            Text = objView.Text;
            Okres = objView.Okres;
            Lokalita = objView.Lokalita;
            Typ = objView.Typ;
            Cena= objView.Cena;
            Mistnosti = objView.Mistnosti;
            CelkoveMetry = objView.CelkoveMetry;
            MetryPokoje = objView.MetryPokoje;
            Vybaveno = objView.Vybaveno;
            Internet = objView.Internet;
            Wifi = objView.Wifi;
            ZvireByt = objView.ZvireByt;
            SvojeZvire = objView.SvojeZvire;
            Podlazi = objView.Podlazi;
            DatumNastehovani = objView.DatumNastehovani;
            DatumPosledniZmeny = DateTime.Now;
        }

        public void Update(NabidkaViewModel objView)
        {
            Text = objView.Text;
            Okres = objView.Okres;
            Lokalita = objView.Lokalita;
            Typ = objView.Typ;
            Cena = objView.Cena;
            Mistnosti = objView.Mistnosti;
            CelkoveMetry = objView.CelkoveMetry;
            MetryPokoje = objView.MetryPokoje;
            Vybaveno = objView.Vybaveno;
            Internet = objView.Internet;
            Wifi = objView.Wifi;
            ZvireByt = objView.ZvireByt;
            SvojeZvire = objView.SvojeZvire;
            Podlazi = objView.Podlazi;
            DatumNastehovani = objView.DatumNastehovani;
        }
        private static string VytvoritHeslo()
        {
            Password pwd = new(16);
            return pwd.Next();
        }
    }
}


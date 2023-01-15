using Microsoft.AspNetCore.Hosting.Server;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace NajdiSpolubydlícíWeb.Models
{
    public class NabidkaViewModel
    {
        public int Id { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné")]
        [EmailAddress(ErrorMessage = "Zadejte email ve správném formátu, př.: info@najdispolubydlici.cz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), RegularExpression("^\\+[1-9]{1}[0-9](?:\\s*\\d){3,14}$", ErrorMessage = "Číslo zadejte dle př.: +420 123 123 123")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), MaxLength(40, ErrorMessage = "Maximální počet znaků: 40")/*, MinLength(2, ErrorMessage = "Minimální počet znaků: 2")*/]
        public string Okres { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), MaxLength(2000, ErrorMessage = "Maximální počet znaků: 2000")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), Range(0, 50000, ErrorMessage = "Rozmezí 0 až 50 000 Kč")]
        public string Cena { get; set; }

        public DateTime DatumVytvoreni { get; set; }

        public DateTime DatumPosledniZmeny { get; set; }

        public string? Base64s { get; set; }

        [Required(ErrorMessage = "Titulní obrázek je povinný")]
        public string TitulObrBase64 { get; set; }

        [MaxLength(100, ErrorMessage = "Maximální počet znaků: 100")]
        public string? Lokalita { get; set; }

        [Required, RegularExpression("^(?:Dům|Byt|Jiné)$", ErrorMessage = "Vyberte z možností")]
        public string Typ { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), Range(1, 500, ErrorMessage = "Neplatná hodnota")]
        public int Mistnosti { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné"), Range(1, 100000, ErrorMessage = "Neplatná hodnota")]
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

        [Required(ErrorMessage = "Toto pole je povinné"), Range(-10, 500, ErrorMessage = "Nesmyslná hodnota (musí být od -10 do 500)")]
        public int Podlazi { get; set; }

        [Required(ErrorMessage = "Toto pole je povinné")]
        public DateTime DatumNastehovani { get; set; }

        public List<string>? ImageURLs { get; set; }

        public NabidkaViewModel() { }

        public NabidkaViewModel(Nabidka obj)
        {
            Id = obj.Id;
            Password= obj.Password;
            Telefon = obj.Telefon;
            Email = obj.Email;
            Text = obj.Text;
            Cena = obj.Cena;
            Okres = obj.Okres;
            Lokalita = obj.Lokalita;
            Typ = obj.Typ;
            Mistnosti = obj.Mistnosti;
            CelkoveMetry = obj.CelkoveMetry;
            MetryPokoje = obj.MetryPokoje;
            Vybaveno= obj.Vybaveno;
            Internet = obj.Internet;
            Wifi = obj.Wifi;
            ZvireByt = obj.ZvireByt;
            SvojeZvire = obj.SvojeZvire;
            Podlazi = obj.Podlazi;
            DatumNastehovani = obj.DatumNastehovani;
            DatumVytvoreni = obj.DatumVytvoreni;
            DatumPosledniZmeny = obj.DatumPosledniZmeny;
            string folder = @$"./wwwroot/UserImgs/Nabidka/{obj.Id}";
            if (Directory.Exists(folder))
            {
                string[] filesInDirectory = Directory.GetFiles(folder);
                ImageURLs = new List<string>(filesInDirectory.Count());

                foreach (string file in filesInDirectory)
                {
                    ImageURLs.Add(string.Format(Path.GetFileName(file)));
                }
                ImageURLs.Remove("minimg.jpeg");
            }
        }
    }
}



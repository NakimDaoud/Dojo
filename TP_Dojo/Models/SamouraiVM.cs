using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TP_Dojo.Models
{
    public class SamouraiVM
    {
        public Samourai Samourai { get; set; }
        public List<Arme> Armes { get; set; }

        public List<ArtMartial> ArtMartials { get; set; } = new List<ArtMartial>();
        public int? idArme { get; set; }

        public List<int> idArtMartial { get; set; } = new List<int>();

        public int? potentiel { get; set; }
    }
}
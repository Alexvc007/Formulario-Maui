using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp5.Modelos
{
    public class Productos
    {
        [PrimaryKey, AutoIncrement]
        public int CodProd { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MauiApp5.Modelos;
using SQLite;

namespace MauiApp5.Datos
{
    public class DataBase
    {
        private readonly SQLiteAsyncConnection BaseDatos;

        public DataBase(string Path)
        {
            BaseDatos = new SQLiteAsyncConnection(Path);
            BaseDatos.CreateTableAsync<Productos>().Wait();
        }

        public Task<int> GuardarProducto(Productos Producto)
        {
            return BaseDatos.InsertAsync(Producto);
        }

        public Task<List<Productos>> ListaProductos()
        {
            return BaseDatos.Table<Productos>().ToListAsync();
        }
    }
}

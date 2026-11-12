using MauiApp5.Modelos;
using MauiApp5.Datos;
using SQLite;
namespace MauiApp5.Vistas
{
    public partial class MainPage : ContentPage
    {
        private string RutaImagen = string.Empty;
        private readonly DataBase BaseDatos;

        public MainPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "BdTienda.db");
            BaseDatos = new DataBase(dbPath);
        }


        private async void SeleccionarImagen(object sender, EventArgs e)
        {
            try
            {
                var Imagen = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleccionar imagen del producto",
                    FileTypes = FilePickerFileType.Images
                });
                if (Imagen != null)
                {
                    string NombreArchivo = $"{Guid.NewGuid()}.jpg";
                    string RutaDestino = Path.Combine(FileSystem.AppDataDirectory, NombreArchivo);
                    using (var StreamOrigen = await Imagen.OpenReadAsync())
                    using (var StreamDestino = File.OpenWrite(RutaDestino))
                    {
                        await StreamOrigen.CopyToAsync(StreamDestino);
                    }

                    RutaImagen = RutaDestino;
                    ImagenProducto.Source = ImageSource.FromFile(RutaDestino);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al seleccionar la imagen: {ex.Message}", "OK");
            }
        }



        private async void Procesar(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Precio.Text))

            {
                await DisplayAlert("Error", "Por favor, complete el precio.", "OK");
                Precio.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(Stock.Text))

            {
                await DisplayAlert("Error", "Por favor, complete el stock.", "OK");
                Stock.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(Descripcion.Text))

            {
                await DisplayAlert("Error", "Por favor, complete la descripción.", "OK");
                Descripcion.Focus();
                return;
            }
            Guardar();
        }

        private async void Guardar()
        {
            var DatosProducto = new Productos
            {
                Descripcion = Descripcion.Text,
                Precio = Convert.ToInt32(Precio.Text),
                Stock = Convert.ToInt32(Stock.Text),
                Imagen = RutaImagen
            };
            await BaseDatos.GuardarProducto(DatosProducto);
            Descripcion.Text = string.Empty;
            Precio.Text = string.Empty;
            Stock.Text = string.Empty;
            await DisplayAlert("Éxito", "Producto guardado correctamente.", "OK");

            Mostrar();
        }

        private async void Mostrar()
        {
            while (Dgv.RowDefinitions.Count > 1)
                Dgv.RowDefinitions.RemoveAt(Dgv.RowDefinitions.Count - 1);
            var HijosEliminar = Dgv.Children.Where(h => Grid.GetRow((BindableObject)h) > 0).ToList();
                foreach (var Hijo in HijosEliminar)
                    Dgv.Children.Remove(Hijo);
            
            var ListaProductos = await BaseDatos.ListaProductos();
            int V = 0;

            foreach(var Producto in ListaProductos)
            {
                V++;
                Dgv.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                string[] Datos =
                {
                    Producto.CodProd.ToString(),
                    Producto.Descripcion,
                    Producto.Precio.ToString(),
                    Producto.Stock.ToString()
                };
                for (int v2 = 0; v2 < Datos.Length; v2++)
                {
                    var Label = new Label
                    {
                        Text = Datos[v2],
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    Dgv.Add(Label, v2, V);

                }
                var Imagen = new Image
                {
                    HeightRequest = 100,
                    WidthRequest = 100,
                    Aspect = Aspect.AspectFill,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                if(!string.IsNullOrEmpty(Producto.Imagen) && File.Exists(Producto.Imagen))
                {
                    Imagen.Source = ImageSource.FromFile(Producto.Imagen);
                }
                else
                {
                    Imagen.Source = "no_image.png"; // Imagen por defecto si no hay imagen disponible
                }
                Dgv.Add(Imagen, 4, V);
            }

        }
    }
}

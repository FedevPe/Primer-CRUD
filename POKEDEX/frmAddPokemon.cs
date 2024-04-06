using Dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using System.Security.Policy;


namespace POKEDEX
{
    public partial class frmAddPokemon : Form
    {
        private Pokemon pokemon = null;
        //Constructor por defecto de la clase
        public frmAddPokemon()
        {
            InitializeComponent();

        }
        //
        //Sobrecarga del constructor
        //
        //El parametro que le doy es un objeto de la clase Pokemon, este me sirve para
        //poder asignarle valores a los controles, cuando quiero modificar un pokemon de la lista
        public frmAddPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
            btnAgregar.Text = "Modificar";
        }

        //Metodo que al cargar el formulario obtiene los datos de la DB, especificamente de la tabla Elementos
        //y los carga en los ComboBox Tipo y Debilidad
        private void frmAddPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementNegocio = new ElementoNegocio();
            try
            {
                cboTipo.DataSource = elementNegocio.Listar();
                //Esto me permite manejar los datos del cbo con una clave y un valor
                //a la propiedad ValueMember le indico cual es la clave y a la propiedad
                //DisplayMember le indico cual es el valor.
                //Entonces los Id de la tabla Elementos son la clave y la descripcion es el valor
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";

                cboDebilidad.DataSource = elementNegocio.Listar();
                cboTipo.ValueMember = "Id";
                cboDebilidad.DisplayMember = "Descripcion";

                //Validación de null, esto me permite comprobar si el campo pokemon es null o no,
                //Si no lo es, es porque el usuario desea modificar algun dato del pokemon seleccionado,
                //es por eso que el campo pokemon tiene los datos del mismo y esto me permite
                //asignar dichos datos en los controles, para que el usuario pueda verlos.

                if (pokemon != null)
                {
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    CargarImagen(txtUrlImagen.Text);
                    txtDescripcion.Text = pokemon.Descripcion;
                    cboTipo.SelectedItem = pokemon.Tipo.Id;
                    cboDebilidad.SelectedItem = pokemon.Debilidad.Id;                    
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            if (txtUrlImagen.Text == "")
            {
                //Al abrirse el formualrio se carga una imagen predeterminada
                CargarImagen("https://upload.wikimedia.org/wikipedia/commons/thumb/5/51/Pokebola-pokeball-png-0.png/640px-Pokebola-pokeball-png-0.png");
            }
        }
        
        //Este metodo me permite cargar la imagen en el pictureBox, si la url de la imagen no es correcta se carga una imagen predeterminada
        private void CargarImagen(string imagen)
        {
            try
            {
                imgPokemon.Load(imagen);
            }
            catch (Exception)
            {
                imgPokemon.Load("https://upload.wikimedia.org/wikipedia/commons/thumb/5/51/Pokebola-pokeball-png-0.png/640px-Pokebola-pokeball-png-0.png");
            }
        }

        //Metodo que al presionar el boton cancelar, cierra el formulario que sirve para cargar un nuevo pokemon
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
        //Este metodo configura el evento click del boton aceptar, al presionarlo se guardan los datos en un objeto Pokemon
        //y se carga en la DB.
        private void btnAgregar_Click(object sender, EventArgs e)
       {
            //Instancio un objeto de la clase negocio para poder utilizar sus metodos, conectarme a la DB y en este caso
            //agregar un nuevo registro en ella
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                //Si el pokemon hasta el momento que llega a esta instrucción y sigue siendo nulo,
                //es porque el usuario quiere agregar un nuevo registro a la DB. Entonces,
                //instancio un objeto de la clase Pokemon al cual le asigno los valores de los controles
                //a cada propiedad de la clase.
                if(pokemon == null)
                {
                    pokemon = new Pokemon(); //la variable pokemon dejar de ser null y ahora contiene un objeto.
                }

                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = CapitalizeLetra(txtNombre.Text);
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.UrlImagen = txtUrlImagen.Text;
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem;
                
                //Si el usuario quiere modificar un pokemon es porque el pokemon ya existe en la DB,
                //por lo tanto tiene un Id
                if(pokemon.Id != 0)
                {
                    negocio.ModificarPokemon(pokemon);
                    MessageBox.Show("El Pokemon se modifico correctamente. 👍");
                }
                else
                {
                    //Utilizo el metodo agregar pokemon del objeto negocio que me permite conectarme a la DB y cargar un nuevo registro
                    negocio.AgregarPokemon(pokemon);
                    MessageBox.Show("El Pokemon se agrego correctamente. 😀 ");
                }

                //Finalmente cierro el formulario
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //Con este metodo configuro el evento TextChanged del pictureBox, sirve para que cuando cambia el texto donde se indica la url
        //la imagen se muestre en el pictureBox
        private void txtUrlImagen_TextChanged(object sender, EventArgs e)
        {
            CargarImagen(txtUrlImagen.Text);
        }
        private string CapitalizeLetra(string str)
        {
            str = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);

            return str;
        }

    }
}

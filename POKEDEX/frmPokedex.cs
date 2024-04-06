using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;
using POKEDEX;

namespace WinForms
{
    public partial class formPokedex : Form
    {
        private List<Pokemon> pokemonList = new List<Pokemon>();

        public formPokedex()
        {
            InitializeComponent();
        }
        private void formPokedex_Load(object sender, EventArgs e)
        {
            ActualizarLista();

            cboCampo.Items.Add("Números");
            cboCampo.Items.Add("Nombres");
            cboCampo.Items.Add("Tipo");

        }

        private void ActualizarLista()
        {
            //Al cargarse el formulario, se intancia un objetos de la clase pokemon,
            //donde se guardan los datos provenientes de la DB al llamar a la funcion Listar().
            //Y asigno esos datos en el control DataGridView del formulario. Hago lo mismo para cargar los datos
            //al dgvElementos

            PokemonNegocio service = new PokemonNegocio();

            try
            {
                pokemonList = service.Listar();
                dgvPokemon.DataSource = pokemonList;
                dgvPokemon.Columns["UrlImagen"].Visible = false; //ocultar columna de urlImagen
                dgvPokemon.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //Esta instrucción sirve para poder cargar la imagen que contiene el DataGridView
            //utilizo una sobrecarga del metodo Load donde recibe como parámetro un String Url
            imgPokemon.Load();
        }

        private void dgvPokemon_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPokemon.CurrentRow != null)
            {
                //Cada fila del dgv es un objeto que tiene diferente propiedades, con esta instrucción
                //puedo obtener obtener los datos la fila seleccionada como un objeto y guardarlo en una 
                //variable para poder manipularlo.

                //DataBoundItem me devuelve un objeto con los datos enlazados en la fila,
                //por eso uso casteo explicito para especificar al sistema de que realmente
                //se trata de un objeto y poder guardarlo en una variable de la clase pokemon.

                Pokemon pokemonSelect = (Pokemon)dgvPokemon.CurrentRow.DataBoundItem;

                //llamo a la funcion CargarImagen y le doy como parametro la url de la imagen.
                CargarImagen(pokemonSelect.UrlImagen);
            }
        }
        
        //Metodo que configura el evento click del boton Agregar Pokemon, instacia un objeto de la clase frmAddPokemon (formulario)
        //y lo muestra.
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAddPokemon altaPokemon = new frmAddPokemon();
            //altaPokemon.Show(); //Muestra el nuevo formulario y me permite utilizar el formulario principal
            altaPokemon.ShowDialog(); //Muestra el nuevo formulario pero no me permite usar otro hasta que se cierre.
                                      //Toma el control y no lo devuelve hasta que el formulario se cierra.
            

            //El metodo me permite ver el último registro que agregue a la DB
            ActualizarLista();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            //Creo una variable del tipo pokemon y le asigno los valores de la fila seleccionada en la lista
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemon.CurrentRow.DataBoundItem;

            //Creo un formulario usando la sobrecarga del constructor
            //le doy como parametro el pokemon seleccionado y lo muestro
            frmAddPokemon modificarPokemon = new frmAddPokemon(seleccionado);
            modificarPokemon.ShowDialog();  

            //Actualizo la lista para ver los cambios modificados
            ActualizarLista();
        }

        private void bntEliminarFisico_Click(object sender, EventArgs e)
        {
            Eliminar();
        }

        //Esta función me permite cargar la imagen de los objetos de cada fila y en caso
        //de que ocurra una exception poder capturarla y que no se rompa la aplicación.
        //en caso de que la url de la imagen de algun pokemon no este cargada, le asigna una imagen
        //predeterminada.
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

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            Eliminar(true);
        }

        //Metodo para eliminar un pokemon de forma lógica o física, dependiendo del parametro
        //Escribir de esa forma el parametro del metodo, me da la posibilidad de que el parametro sea opcional
        //En caso de que no envíe un parametro va a tomar falso por defecto. Es un parametro bandera
        //que me sirve para hacer una comprobación.
        private void Eliminar (bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;

            try
            {
                seleccionado = (Pokemon)dgvPokemon.CurrentRow.DataBoundItem;

                //El metodo .Show retorna un resultado, este resultado lo puedo guardar en una variable del tipo
                //DialogResult luego puedo comprobar la respuesto y realizar determinadas acciones, en este caso
                //eliminar de forma física el pokemon seleccionado, es decir, que se elimina de la DB y se pierde para siempre.

                //Configuro el MessageBox para que se vea un Mensaje,                 TíTulo           , Botones                , Icono.
                DialogResult respuesta = MessageBox.Show($"Estas seguro de eliminar a {seleccionado.Nombre}", "Eliminar Pokemon", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                //Si el usuario presiona el boton Si se elimina el pokemon
                if (respuesta == DialogResult.Yes)
                {
                    if (logico)
                    {
                        negocio.EliminarPokemonLogico(seleccionado.Id);
                    }
                    else
                    {
                        negocio.EliminarPokemon(seleccionado.Id);  
                    }

                    MessageBox.Show($"Se ha eliminado a {seleccionado.Nombre} exitosamente 🗑");
                    ActualizarLista();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void FiltroBusqueda(string filtro) 
        {
            //No genero una instancia porque la voy a obtener del campo pokemonList
            List<Pokemon> listaFiltrada;

            try
            {
                if (txtFiltro.Text != "")
                {
                    //txtFiltro.Text = CapitalizeLetra(txtFiltro.Text);
                    //En pokemonList estan todos los objetos Pokemon que obtengo de la DB, es la que uso para mostrar en DGV
                    //Es una lista, por lo tanto, es una colección que tiene objetos del Tipo Pokemon.
                    listaFiltrada = pokemonList.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper() == txtFiltro.Text.ToUpper());
                    //El metodo FindAll requiere un parametro especial, porque se trata de una funcion lambda.
                    //El parametro funciona como un foreach que recorre la lista y devuelve el objeto que tiene el mismo nombre
                    //que esta escrito en la caja de texto.
                    //La funcion FindAll retorna un objeto que se almacena en la listaFiltrada.
                }
                else
                {
                    listaFiltrada = pokemonList;
                }

                dgvPokemon.DataSource = null; //Limpio la lista, no muestra nada
                dgvPokemon.DataSource = listaFiltrada; //asigno la lista filtrada al dgv y la muestra
                dgvPokemon.Columns["Id"].Visible = false;
                dgvPokemon.Columns["UrlImagen"].Visible = false;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        //Filtro de busqueda
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            FiltroBusqueda(txtFiltro.Text);
        }


        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();

            if (opcion == "Números")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a ");
                cboCriterio.Items.Add("Menor a ");
                cboCriterio.Items.Add("Igual a ");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con ");
                cboCriterio.Items.Add("Termina con ");
                cboCriterio.Items.Add("Contiene ");
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                //Validacion de campos, si el return es falso, se corta el evento
                if (ValidarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                dgvPokemon.DataSource = negocio.Filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            
        }
        private bool ValidarFiltro()
        {
            if (cboCampo.SelectedIndex >= 0)
            {
                MessageBox.Show("Seleccione una opción del campo para filtrar.");
                return true;
            }
            if (cboCriterio.SelectedIndex >= 0)
            {
                MessageBox.Show("Seleccione una ipcion del criterio para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes completar el filtro para buscar.");
                    return true;
                }
                if (!(SoloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo puedes escribir números.");
                    return true;
                }
            }

            return false;
        }
        private bool SoloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }

        //METODO PARA CONVERTIR EN MAYUSCULA LA PRIMER LETRA DE UNA PALABRA DE UNA CADENA DE CARACTERES
        //private string CapitalizeLetra(string str)
        //{
        //    if (str != "")
        //    {
        //        str = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        //    }

        //    return str;
        //}
    }
}

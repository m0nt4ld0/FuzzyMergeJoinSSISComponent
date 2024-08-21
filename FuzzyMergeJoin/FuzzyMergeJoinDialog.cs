//=====================================================================
//
//  Archivo:      FuzzyMergeJoinDialog.cs
//  Autor/a:      Mariela Montaldo <mmontaldo@live.com>
//  Descripción:  Ventana principal del componente FuzzyMergeJoin
//              
//  Fecha:        17/12/2020
//
//---------------------------------------------------------------------
//
//  Copyright (C) Mariela Montaldo.
//
//===================================================================== 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace FuzzyMergeJoin
{

    using Microsoft.SqlServer.Dts.Runtime;
    using Microsoft.SqlServer.Dts.Runtime.Design;

    public partial class FuzzyMergeJoinDialog : Form
    {
        #region Members
        private Dictionary<string, int> _entrada0Col;
        private Dictionary<string, int> _entrada1Col;

        //private Dictionary<string, int> _salida;

        public IServiceProvider ServiceProvider { get; set; }
        public IDTSComponentMetaData100 ComponentMetaData { get; set; }
        #endregion

        #region Events
        public FuzzyMergeJoinDialog()
        {
            InitializeComponent();

            // Inicialización de las propiedades de los componentes.
            prepararDisenio();
        }

        #region Window Events
        /// <summary>
        /// Se ejecuta al abrirse la ventana del Fuzzy Merge Join.
        /// </summary>
        private void FuzzyMergeJoinDialog_Load(object sender, EventArgs e)
        {
            // Por defecto, seleccionar el primer item del combo Tipo al abrir.
            comboBoxTipo.SelectedIndex = 0;

            // Cargo diccionarios con las columnas (metadata)
            _entrada0Col = cargarDicColumnas(0);
            _entrada1Col = cargarDicColumnas(1);

            // Completo los listboxes del form con los nombres de las columnas.
            cargarListBoxColumnas(listBoxEntrada0, _entrada0Col);
            cargarListBoxColumnas(listBoxEntrada1, _entrada1Col);
        }
        #endregion

        #region ListBox Events
        #region ListBox Double Click Events
        private void listBoxEntrada1_DoubleClick(object sender, EventArgs e)
        {
            string s = "Entrada1." + listBoxEntrada1.SelectedItem.ToString();
            if (!String.IsNullOrEmpty(s))
            {
                listBoxSalida.Items.Add(s);
                //_salida.Add(s, listBoxEntrada1.SelectedIndex);
            }
        }


        private void listBoxEntrada1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void listBoxEntrada0_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }


        private void listBoxEntrada0_MouseDoubleClick(object sender, EventArgs e)
        {
            string s = "Entrada0." + listBoxEntrada0.SelectedItem.ToString();
            if (!String.IsNullOrEmpty(s))
            {
                listBoxSalida.Items.Add(s);
                //_salida.Add(s, listBoxEntrada0.SelectedIndex);
            }
        }

        private void listBoxSalida_DoubleClick(object sender, EventArgs e)
        {
            int idx = listBoxSalida.SelectedIndex;
            if (idx >= 0 && idx < listBoxSalida.Items.Count)
            {
                listBoxSalida.Items.RemoveAt(idx);
            }
            
        }

        #endregion

        private void listBoxEntrada0_SelectedIndexChanged(object sender, EventArgs e)
        {
            //...To Do...
        }

        #endregion

        #region ComboBox Events
        private void comboBoxTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //...To Do...
        }
        #endregion

        #region Button Events
        /// <summary>
        /// Invocado cuando el usuario hace clic en el botón 'Ayuda'.
        /// </summary>
        private void buttonAyuda_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Seleccione la metadata a utilizar como clave del Join tanto en la Entrada 0 como " +
            "en la Entrada 1. Deben ser de tipo cadena de texto (DT_STR ó DT_WSTR). Luego, seleccione el tipo " +
            "de dato que contiene el campo, para optimizar la comparación.");
        }

        /// <summary>
        /// Invocado cuando el usuario hace clic en el botón 'Acerca de'.
        /// </summary>
        private void buttonAcercaDe_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(false);
            labelTextoAyuda.Text = "Para informar algún bug o enviar una sugerencia, por favor envíe un correo " +
            "electrónico a mmontaldo@live.com con asunto 'FuzzyMergeJoin'. Su contribución ayuda " +
            "enormemente a mejorar. Muchas gracias.";
            labelTituloAyuda.Text = "Acerca de";
            labelTituloAyuda.Visible = true;
            labelTextoAyuda.Visible = true;
        }

        /// <summary>
        /// Invocado cuando el usuario hace clic en el botón 'Guardar'.
        /// </summary>
        private void buttonGuardar_Click(object sender, EventArgs e)
        {
            // To Do.
        }

        /// <summary>
        /// Invocado cuando el usuario hace clic en el botón 'Cancelar'.
        /// </summary>
        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // To Do.
            panelAyudaSubmenu.Visible = !(panelAyudaSubmenu.Visible);
        }

        private void buttonAyudaNombre_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(false);
            labelTextoAyuda.Text = "Ayuda nombre";
            labelTituloAyuda.Text = "Comparar nombres de personas";
            labelTituloAyuda.Visible = true;
            labelTextoAyuda.Visible = true;
        }

        private void buttonAyudaLugar_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(false);
            labelTextoAyuda.Text = "Ayuda lugar";
            labelTituloAyuda.Text = "Comparar nombres de lugares geográficos";
            labelTituloAyuda.Visible = true;
            labelTextoAyuda.Visible = true;
        }

        private void buttonAyudaTel_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(false);
            labelTextoAyuda.Text = "Ayuda tel";
            labelTituloAyuda.Text = "Comparar números telefónicos";
            labelTituloAyuda.Visible = true;
            labelTextoAyuda.Visible = true;
        }

        private void buttonAyudaMail_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(false);
            labelTextoAyuda.Text = "Ayuda mail";
            labelTituloAyuda.Text = "Comparar correos electrónicos";
            labelTituloAyuda.Visible = true;
            labelTextoAyuda.Visible = true;
        }
        
        private void buttonAyudaDNI_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(false);
            labelTextoAyuda.Text = "Ayuda DNI";
            labelTituloAyuda.Text = "Comparar identificaciones (DNI, CUIT, CUIL, etc.)";
            labelTituloAyuda.Visible = true;
            labelTextoAyuda.Visible = true;
        }

        private void buttonConfigurar_Click(object sender, EventArgs e)
        {
            mostrarControlesPanelPpal(true);
            labelTextoAyuda.Visible = false;
            labelTituloAyuda.Visible = false;
        }

        #endregion

        #endregion

        #region Helpers

        #region cargarDicColumnas
        /// <summary>
        /// Devuelve diccionario con nombre de la columna del flujo de entrada y su ID de columna.
        /// <param name="input">ID de flujo de entrada del componente.</param>
        /// </summary>
        private Dictionary<string, int> cargarDicColumnas(int input)
        {
            Dictionary<string, int> dict = new Dictionary<string,int>();
            
            if (input < 0)
            {
                return null;
            }

            try
            {
                // Llamar al método GetVirtualInput para recuperar la lista de columnas disponibles de un componente 
                // de entrada.
                IDTSInput100 dtsInput = ComponentMetaData.InputCollection[input];
                IDTSVirtualInput100 vInput = dtsInput.GetVirtualInput();

                int cantCol = vInput.VirtualInputColumnCollection.Count;
                for (int i = 0; i < cantCol; i++)
                {
                    dict.Add(vInput.VirtualInputColumnCollection[i].Name, i);
                }
            }
            catch (COMException)
            {
                // Ignorar - Significa que el item no esta en la coleccion de la entrada
            }
            return dict;
        }
        #endregion

        #region Form Helpers

        #region cargarListBoxColumnas
        /// <summary>
        /// Devuelve diccionario con nombre de la columna del flujo de entrada y su ID de columna.
        /// <param name="lista">Objeto ListBox en el form.
        /// <param name="dict">Diccionario conteniendo los valores a mostrar en el ListBox.</param>
        /// </summary>
        public void cargarListBoxColumnas(ListBox lista, Dictionary<string, int> dict)
        {
            foreach(KeyValuePair<string, int> p in dict)
            {
                lista.Items.Add(p.Key);
            }
        }
        #endregion

        /// <summary>
        /// Inicializa la estética del formulario.
        /// </summary>
        public void prepararDisenio()
        {
            comboBoxTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            panelAyudaSubmenu.Visible = false;

            labelTituloAyuda.Visible = false;
            labelTextoAyuda.Visible = false;
        }

        /// <summary>
        /// Muestra u oculta los componentes del formulario principal. Utilizado para conmutar entre mostrar
        /// la ayuda del componente o los controles para configurarlo.
        /// <param name="mostrar"></param>
        /// </summary>
        public void mostrarControlesPanelPpal(bool mostrar)
        {
            labelEntrada0.Visible = mostrar;
            labelEntrada1.Visible = mostrar;
            labelTipo.Visible = mostrar;
            
            listBoxEntrada0.Visible = mostrar;
            listBoxEntrada1.Visible = mostrar;
            listBoxSalida.Visible = mostrar;

            buttonAgregar.Visible = mostrar;

            comboBoxTipo.Visible = mostrar;
        }

        #endregion

        #endregion

    }
}

//=====================================================================
//
//  Archivo:      FuzzyMergeJoinUI.cs
//  Autor/a:      Mariela Montaldo <mmontaldo@live.com>
//  Descripción:  Inicializa la interfaz gráfica del componente.
//  Fecha:        17/12/2020
//
//---------------------------------------------------------------------
//
//  Copyright (C) Mariela Montaldo.
//
//===================================================================== 

using System;
using System.Windows.Forms;

using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

namespace FuzzyMergeJoin
{
    public class FuzzyMergeJoinUI : IDtsComponentUI
    {
        private IDTSComponentMetaData100 _cmd = null;
        private IServiceProvider _sp = null;

        public void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            _cmd = dtsComponentMetadata;
            _sp = serviceProvider;
        }
        
        // Se invoca cuando se agrega el componente al canvas.
        public void New(IWin32Window parentWindow)
        {
            
        }

        public bool Edit(IWin32Window parentWindow, Variables variables, Connections connections)
        {
            var propertiesEditor = new FuzzyMergeJoinDialog();
            propertiesEditor.Text = "Fuzzy Merge Join";
            propertiesEditor.ComponentMetaData = _cmd;
            
            // Get the destination's default input and virtual input.  
            IDTSInput100 input = _cmd.InputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            propertiesEditor.ServiceProvider = _sp;
            //propertiesEditor.Variables = variables;

            return propertiesEditor.ShowDialog(parentWindow) == DialogResult.OK;
        }

        public void Delete(IWin32Window parentWindow)
        {

        }

        public void Help(IWin32Window parentWindow)
        {

        }
    }
}

//=====================================================================
//
//  Archivo:      FuzzyMergeJoin.cs
//  Autor/a:      Mariela Montaldo <montaldom@pseguros.com.ar>
//  Descripción:  Realiza una comparación difusa entre cadenas de texto
//                de dos flujos de entrada diferentes. La comparación
//                es realizada utilizando una optimización del
//                algoritmo de Jaro-Winkler.
//              
//  Fecha:        17/12/2020
//
//---------------------------------------------------------------------
//
//  Copyright (C) Provincia Seguros.
//
//===================================================================== 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Utilities;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.ServiceModel.Syndication;
using System.Data;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Collections;

namespace FuzzyMergeJoin
{
    [DtsPipelineComponent(
        DisplayName = "FuzzyMergeJoin", 
        ComponentType = ComponentType.Transform,
        Description = "Realiza un join con una comparación difusa de dos cadenas de texto.",
        CurrentVersion = 0,
        IconResource = "FuzzyMergeJoin.Bruja.ico",
        UITypeName = "FuzzyMergeJoin.FuzzyMergeJoinUI, FuzzyMergeJoin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cdd4fe7052ad3137"
        )]
    public class FuzzyMergeJoin : PipelineComponent
    {
        #region Members
        
        int[] inputColumnBufferIndexes;
        int[] outputColumnBufferIndexes;
            

        PipelineBuffer outputBuffer;
        
        #endregion
        #region Design Time

        #region ProvideComponentProperties
        /// <summary>
        /// Método de inicialización de las propiedades del componente.
        /// </summary>
        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            base.RemoveAllInputsOutputsAndCustomProperties();
            this.ComponentMetaData.UsesDispositions = true;
            ComponentMetaData.ContactInfo = "Ante errores o sugerencias enviar un e-mail a montaldom@pseguros.com.ar con asunto 'FuzzyMerjeJoin'.";

            // Agregar flujo de entrada 1
            var input0 = this.ComponentMetaData.InputCollection.New();
            input0.Name = "Entrada 0";
            input0.Description = "Entrada 0 para el FuzzyMergeJoin";
            input0.HasSideEffects = false; // Determines if component is left in data flow when run in OptimizedMode and output is not connected
            input0.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            input0.ErrorOrTruncationOperation = "ValidationFailure";

            // Agregar flujo de entrada 2
            var input1 = this.ComponentMetaData.InputCollection.New();
            input1.Name = "Entrada 1";
            input1.Description = "Entrada 0 para el FuzzyMergeJoin";
            input1.HasSideEffects = false; // Determines if component is left in data flow when run in OptimizedMode and output is not connected
            input1.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            input1.ErrorOrTruncationOperation = "ValidationFailure";

            // Agregar salida asincrónica
            var output1 = this.ComponentMetaData.OutputCollection.New();
            output1.Name = "Salida";
            output1.Description = "Salida de FuzzyMergeJoin";
            output1.HasSideEffects = false; // Determines if component is left in data flow when run in OptimizedMode and output is not connected
            output1.SynchronousInputID = 0;
            output1.ExclusionGroup = 0;
            
        }
        #endregion

        #region OnInputPathAttached
        /// <summary>
        /// Agrega una columna de salida a su colección de columnas de salida para cada columna disponible 
        /// del componente ascendente.
        /// </summary>
        /// <param name="inputID"></param>
        public override void OnInputPathAttached(int inputID)
        {
            IDTSInput100 input = ComponentMetaData.InputCollection.GetObjectByID(inputID);
            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            foreach (IDTSVirtualInputColumn100 vCol in vInput.VirtualInputColumnCollection)
            {
                IDTSOutputColumn100 outCol = output.OutputColumnCollection.New();
                outCol.Name = vCol.Name;
                outCol.SetDataTypeProperties(vCol.DataType, vCol.Length, vCol.Precision, vCol.Scale, vCol.CodePage);
            }
        }
        #endregion

        #region InsertOutput
        /// <summary>
        /// Previene que el usuario agregue manualmente una salida adicional a la predefinida
        /// por el componente.
        /// </summary>
        public override IDTSOutput100 InsertOutput(DTSInsertPlacement insertPlacement, int outputID)
        {
            throw new InvalidOperationException("Este componente no admite más de una salida.");
        }
        #endregion

        #region DeleteOutput
        /// <summary>
        /// Previene que el usuario elimine la salida predefinida.
        /// </summary>
        public override void DeleteOutput(int outputID)
        {
            throw new InvalidOperationException("Este componente debe tener una salida.");
        }
        #endregion

        #region DeleteInput
        public override void DeleteInput(int inputID)
        {
            throw new InvalidOperationException("Este componente debe tener dos entradas.");
        }
        #endregion

        #region SetComponentProperty
        public override IDTSCustomProperty100 SetComponentProperty(string propertyName, object propertyValue)
        {
            return base.SetComponentProperty(propertyName, propertyValue);
        }
        #endregion

        #endregion
        #region Runtime

        #region PreExecute
        /// <summary>
        /// Llamado antes de PrimeOutput y ProcessInput. 
        /// Crea el buffer interno y almacena en el buffer los indices de las columnas de entrada y salida.
        /// </summary>
        public override void PreExecute()
        {
            // Una única salida
            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];
            outputColumnBufferIndexes = new int[output.OutputColumnCollection.Count];
            
            // Múltiples entradas
            foreach (IDTSInput100 input in ComponentMetaData.InputCollection)
            {
                //IDTSInput100 input = ComponentMetaData.InputCollection[0];
                inputColumnBufferIndexes = new int[input.InputColumnCollection.Count];
                
                for (int col = 0; col < input.InputColumnCollection.Count; col++)
                {
                    inputColumnBufferIndexes[col] = BufferManager.FindColumnByLineageID(input.Buffer, input.InputColumnCollection[col].LineageID);
                }
            
                for (int col = 0; col < output.OutputColumnCollection.Count; col++)
                {
                    outputColumnBufferIndexes[col] = BufferManager.FindColumnByLineageID(output.Buffer, output.OutputColumnCollection[col].LineageID);
               
                }
            }
            
            
        }
        #endregion


        #region ProcessInput
        /// <summary>
        /// Este método es llamado reiteradas veces durante la ejecución del paquete. Es llamado cada vez que el
        /// Data Flow tiene el buffer lleno provisto por un componente de entrada.
        /// <param name="inputID">ID del objeto de entrada al componente.</param>
        /// <param name="buffer">Buffer de entrada.</param>
        /// </summary>
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            // Advance the buffer to the next row.  
            while (buffer.NextRow())
            {
                // Add a row to the output buffer.  
                outputBuffer.AddRow();
                for (int x = 0; x < inputColumnBufferIndexes.Length; x++)
                {
                    // Copy the data from the input buffer column to the output buffer column.  
                    outputBuffer[outputColumnBufferIndexes[x]] = buffer[inputColumnBufferIndexes[x]];
                }
            }
            if (buffer.EndOfRowset)
            {
                // EndOfRowset on the input buffer is true.  
                // Set EndOfRowset on the output buffer.  
                outputBuffer.SetEndOfRowset();
            }  
        }
        #endregion 


        #region PrimeOutput
        /// <summary>
        /// Se llama en tiempo de ejecución luego de PreExecute y antes de ProcessInput, para componentes de 
        /// origen y componentes de transformación con salidas asincrónicas para permitir que estos componentes 
        /// agreguen filas a los búferes de salida.
        /// </summary>
        /// <param name="outputs"></param>
        /// <param name="outputIDs">Array de IDs de flujos de salida del componente.</param>
        /// <param name="buffers">Array de PipelineBuffer que representa cada flujo de entrada al componente.</param>
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            //outputBuffer = buffers[0];
            foreach (PipelineBuffer pb in buffers)
            {
                outputBuffer = pb;
                while (pb.NextRow())
                {
                    outputBuffer.AddRow();
                }
            }
            
        }
        #endregion

        #endregion
        #region Helpers

        #endregion
    }
}

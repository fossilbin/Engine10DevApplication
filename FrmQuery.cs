using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace EngineApplication
{
    public partial class FrmQuery : Form
    {
        public IMapControl2 pMapControl;
        public IMap pMap;
        public int iLayerIndex;
        public int iFieldIndex;
     
        public FrmQuery(IMapControl2 pFMapControl)
        {
            InitializeComponent();
            pMapControl = pFMapControl;
            pMap = pFMapControl.Map;
            
          
           

        }

  
   
        private void FrmQuery_Load(object sender, EventArgs e)
        {                      
            ILayer pLayer;
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                pLayer = pMap.get_Layer(i);
                if(pLayer  is IFeatureLayer )
                cmbLayers.Items.Add(pLayer.Name);
            }
        

        }

        private void cmbLayers_TextChanged(object sender, EventArgs e)
        {
           // iLayerIndex = cmbLayers.Items.IndexOf(cmbLayers.Text);
            cmbFields.Items.Clear();
        }

        //获得图层的字段名称
        private void cmbFields_DropDown(object sender, EventArgs e)
        {
            cmbFields.Items.Clear();
            IFeatureLayer pFeatureLayer;
            ILayer pLayer;

            for (int i = 0; i < pMap.LayerCount; i++)
            {
                pLayer = pMap.get_Layer(i);
               //   iLayerIndex = 0;

                if (pLayer.Name.Equals (cmbLayers.SelectedItem))
                {
                    break;
                }
              iLayerIndex=i; 

            }
            pFeatureLayer = (IFeatureLayer)pMap.get_Layer(iLayerIndex);
   

            IFields pFields;

            pFields = pFeatureLayer.FeatureClass.Fields;


            for (int i = 0; i < pFields.FieldCount; i++)
            {
                string fieldName;
                fieldName = pFields.get_Field(i).Name;
                cmbFields.Items.Add(fieldName);
            }
        }

        private void btnShowAllValue_Click(object sender, EventArgs e)
        {
            if (cmbFields.Text=="")
            {
                MessageBox.Show("请选择字段名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            listBoxValue.Items.Clear();
            IFeatureLayer pFeatureLayer;
            pFeatureLayer = (IFeatureLayer)pMap.get_Layer(iLayerIndex);

            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFeatureLayer.FeatureClass.Search(null, false);

            IFeature pFeature;
            pFeature = pFeatureCursor.NextFeature();



            //对Table中当前字段进行排序,把结果赋给Cursor

            ITable pTable = pFeatureLayer as ITable;

            ITableSort pTableSort = new TableSortClass();

            pTableSort.Table = pTable;

            pTableSort.Fields = cmbFields.Text;

            pTableSort.set_Ascending(cmbFields.Text, true);

            pTableSort.set_CaseSensitive(cmbFields.Text, true);

            pTableSort.Sort(null);

            ICursor pCursor = pTableSort.Rows;


            //数值统计

            IDataStatistics pData = new DataStatisticsClass();

            pData.Cursor = pCursor;

            pData.Field = cmbFields.Text;



            System.Collections.IEnumerator pEnumeratorUniqueValues = pData.UniqueValues;//唯一值枚举
            int uniqueValueCount = pData.UniqueValueCount;//唯一值的个数

            this.listBoxValue.Items.Clear();

            string fldValue = null;

            pEnumeratorUniqueValues.Reset();

            ILayerFields pFields = pFeatureLayer as ILayerFields;

            if (pFields.get_Field(pFields.FindField(cmbFields.Text)).Type == esriFieldType.esriFieldTypeString)
            {
                while (pEnumeratorUniqueValues.MoveNext())
                {
                    fldValue = pEnumeratorUniqueValues.Current.ToString();
                    this.listBoxValue.Items.Add("'" + fldValue + "'");
                   
                }
            }
                 else if(cmbFields .Text =="shape" )
            {
                fldValue = Convert.ToString(pFeature.Shape.GeometryType);
                this.listBoxValue.Items.Add(fldValue);
            }
            else 
            {
                while (pEnumeratorUniqueValues.MoveNext())
                {
                    fldValue = pEnumeratorUniqueValues.Current.ToString();
                    this.listBoxValue.Items.Add(fldValue);
                }
            }


      
          

           /* while(pFeature!=null)
            {
                string fldValue;
                if (cmbFields.Text == "Shape")
                {
                    fldValue = Convert.ToString(pFeature.Shape.GeometryType);
                }
                else
                    fldValue = Convert.ToString(pFeature.get_Value(iFieldIndex));             
                
                listBoxValue.Items.Add(fldValue);
                pFeature = pFeatureCursor.NextFeature();
            } */
           
        }

        private void cmbFields_TextChanged(object sender, EventArgs e)
        {
            iFieldIndex = cmbFields.Items.IndexOf(cmbFields.Text);
        }

        private void listBoxValue_DoubleClick(object sender, EventArgs e)
        {
            txtValue.Text = Convert.ToString(listBoxValue.SelectedItem);
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
           // this.close();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            IActiveView pActiveView;
            pActiveView = (IActiveView)pMap;
            pMap.ClearSelection();
            pActiveView.Refresh();

            IQueryFilter pQueryFilter = new QueryFilterClass();         

            IFeatureLayer pFeatureLayer;
            pFeatureLayer = (IFeatureLayer)pMap.get_Layer(iLayerIndex);

            String x = pMap.get_Layer(iLayerIndex).Name;

            String N = pFeatureLayer.Name;
            IFields pFields;
            pFields = pFeatureLayer.FeatureClass.Fields;
            IField pField;
            pField=pFields.get_Field(iFieldIndex);

            switch(pField.Type)
            {
                case esriFieldType.esriFieldTypeString:
                    pQueryFilter.WhereClause=cmbFields.Text + " = '" + txtValue.Text + "'";
                    break;
                case esriFieldType.esriFieldTypeDouble:
                case esriFieldType.esriFieldTypeInteger:
                case esriFieldType.esriFieldTypeSingle:
                case esriFieldType.esriFieldTypeSmallInteger:
                    pQueryFilter.WhereClause = cmbFields.Text + " = " + txtValue.Text ;
                    break;
            }

            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFeatureLayer.FeatureClass.Search(pQueryFilter, true);
              IFeature pFeature;
             IFeature pFtr;
            pFeature = pFeatureCursor.NextFeature();
            while (pFeature!=null)
            {
                pMap.SelectFeature(pFeatureLayer, pFeature);
                pFtr = pFeatureCursor.NextFeature();
                if (pFtr == pFeature)
                {
                    string z ="hello";
                }

                pFeature = pFeatureCursor.NextFeature();
            }
            //  IFeatureSelection pSet = pFeatureLayer as IFeatureSelection;

            /*ISelectionSet pS = pFeatureLayer.FeatureClass.Select(pQueryFilter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, null);

            IFeatureSelection pSet = pFeatureLayer as IFeatureSelection;

            //pSet.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
            IRgbColor pColor = new ESRI.ArcGIS.Display.RgbColorClass();
            pColor.Red = 255;
            

            pSet.SelectionColor = pColor; */

            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            


        }

        private void btnTable_Click(object sender, EventArgs e)
        {
            FormTable fTable = new FormTable();
            fTable.Show();
        }

   
 
    }
}
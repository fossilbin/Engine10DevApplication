using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;


using ESRI.ArcGIS.Carto;

using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF.CATIDs;

namespace EngineApplication
{
    public partial class ParaSetting : Form
    {
        public ParaSetting(IFeatureClass pFtClass)
        {
            InitializeComponent();

            this.pFeatureClass = pFtClass;
        }

        IFeatureClass pFeatureClass;

        private void ParaSetting_Load(object sender, EventArgs e)
        {
            AddFdName(this.pFeatureClass);
        }

        private void AddFdName(IFeatureClass pFeatureClass)
        {
           

           pFieldNames.Items.Clear();
            for (int i = 0; i <= pFeatureClass.Fields.FieldCount - 1; i++)
            {
                if (pFeatureClass.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeDouble || pFeatureClass.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeInteger)
                {
                    pFieldNames.Items.Add(pFeatureClass.Fields.get_Field(i).Name);
                }
            }


        }

       

    }



}

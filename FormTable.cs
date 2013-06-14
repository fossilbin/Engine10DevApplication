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
    public partial class FormTable : Form
    {
        public FormTable()
        {
            
        }

        IFeatureLayer pFeatureLayer;

        public FormTable(IFeatureLayer _FeatureLayer)
        {
            InitializeComponent();
           
            this.pFeatureLayer =_FeatureLayer ;
        }

             
        public void Itable2Dtable()
        {
           
            IFields pFields;
            pFields = pFeatureLayer.FeatureClass.Fields;

            dtGridView.ColumnCount = pFields.FieldCount;
            for (int i = 0; i < pFields.FieldCount;i++ )
            {
               
                string  fldName = pFields.get_Field(i).Name;
                dtGridView.Columns[i].Name = fldName;

               dtGridView.Columns[i].ValueType = System.Type.GetType(ParseFieldType(pFields.get_Field(i).Type));
            }           

            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFeatureLayer.FeatureClass.Search(null, false);

            long lTotalRecords=0;
            IFeature pFeature;
            pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                string[] fldValue = new string[pFields.FieldCount]; 

                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    string fldName;
                    fldName = pFields.get_Field(i).Name;
                    if (fldName==pFeatureLayer .FeatureClass .ShapeFieldName)
                    {
                        fldValue[i] = Convert.ToString(pFeature.Shape.GeometryType);
                    }
                    else
                        fldValue[i] = Convert.ToString(pFeature.get_Value(i));
                }                
                dtGridView.Rows.Add(fldValue);
                pFeature = pFeatureCursor.NextFeature();
                lTotalRecords++;
            }
            tbarTotalRecords.Text = "共有" + Convert.ToString(lTotalRecords) + "条记录";
        }

       private  void FormTable_Load(object sender, EventArgs e)
        {
            Itable2Dtable();
        }

       public static string ParseFieldType(esriFieldType TableFieldType)
       {

           switch (TableFieldType)
           {

               case esriFieldType.esriFieldTypeBlob:

                   return "System.String";

               case esriFieldType.esriFieldTypeDate:

                   return "System.DateTime";

               case esriFieldType.esriFieldTypeDouble:

                   return "System.Double";

               case esriFieldType.esriFieldTypeGeometry:

                   return "System.String";

               case esriFieldType.esriFieldTypeGlobalID:

                   return "System.String";

               case esriFieldType.esriFieldTypeGUID:

                   return "System.String";

               case esriFieldType.esriFieldTypeInteger:

                   return "System.Int32";

               case esriFieldType.esriFieldTypeOID:

                   return "System.String";

               case esriFieldType.esriFieldTypeRaster:

                   return "System.String";

               case esriFieldType.esriFieldTypeSingle:

                   return "System.Single";

               case esriFieldType.esriFieldTypeSmallInteger:

                   return "System.Int32";

               case esriFieldType.esriFieldTypeString:

                   return "System.String";

               default:

                   return "System.String";

           }

       }
    }
}

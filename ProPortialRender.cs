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
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;


namespace EngineApplication
{
    
    public class ProPortialRender
    {
        public ProPortialRender(AxMapControl pMapcontrol, IFeatureLayer pFtLayer,  string pFieldName)
        {
            IGeoFeatureLayer pGeo = pFtLayer as IGeoFeatureLayer;

            IProportionalSymbolRenderer pProRender = new ProportionalSymbolRendererClass();

            pProRender.Field = pFieldName;

            pProRender.ValueUnit = esriUnits.esriUnknownUnits;


            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();

            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;

            pMarkerSymbol.Size = 2;

            pMarkerSymbol.Color = GetRGBColor(255, 0, 0);

            pProRender.MinSymbol = pMarkerSymbol as ISymbol;



           

            IDataStatistics pDataStat = new DataStatisticsClass();

            IFeatureCursor pFtCursor = pFtLayer.FeatureClass.Search(null, false);

            pDataStat.Cursor = pFtCursor as ICursor;

            pDataStat.Field = pFieldName;




            pProRender.MinDataValue = pDataStat.Statistics.Minimum;


            pProRender.MaxDataValue = pDataStat.Statistics.Maximum;

            IFillSymbol pFillS = new SimpleFillSymbolClass();

            pFillS.Color = GetRGBColor(239, 228, 190);

            ILineSymbol pLineS = new SimpleLineSymbolClass();

            pLineS.Width = 2;

            pFillS.Outline = pLineS;

           

            ISimpleFillSymbol pSFillS = pFillS as ISimpleFillSymbol;

            pSFillS.Color = GetRGBColor(100, 100, 253);

            pProRender.BackgroundSymbol = pFillS;

          

            pGeo.Renderer = pProRender as IFeatureRenderer;



            pMapcontrol.ActiveView.Refresh();






            
        }

        public IRgbColor GetRGBColor(int r, int g, int b)
        {
            IRgbColor pRGB;

            pRGB = new RgbColorClass();

            pRGB.Red = r;

            pRGB.Green = g;

            pRGB.Blue = b;

            return pRGB;


        }

    }
}

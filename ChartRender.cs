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

namespace EngineApplication
{
    class ChartRender
    {
        public ChartRender(AxMapControl pMapcontrol, IFeatureLayer pFtLayer, string pFieldName)
        {

            IGeoFeatureLayer pGeoFeaturelayer = pFtLayer as IGeoFeatureLayer;

            IChartRenderer pChart = new ChartRendererClass();



            IDataStatistics pDataStat = new DataStatisticsClass();

            IFeatureCursor pFtCursor = pFtLayer.FeatureClass.Search(null, false);

            pDataStat.Cursor = pFtCursor as ICursor;

            pDataStat.Field = pFieldName;

            double pMax = pDataStat.Statistics.Maximum;



            IRendererFields pRenderFields = pChart as IRendererFields;

            pRenderFields.AddField(pFieldName,pFieldName);

            IBarChartSymbol pBarChart = new BarChartSymbolClass();

            pBarChart.Width = 5;


            IMarkerSymbol pMarkerSymbol = pBarChart as IMarkerSymbol;


            pMarkerSymbol.Size = 12;


            IChartSymbol pChartSym = pBarChart as IChartSymbol;

            pChartSym.MaxValue = pMax;


            ISymbolArray pSymbolArr = pBarChart as ISymbolArray;


            IFillSymbol pFillSy = new SimpleFillSymbolClass();

            pFillSy.Color = GetRgb(220, 0, 0);

            pSymbolArr.AddSymbol(pFillSy as ISymbol);

            pGeoFeaturelayer.Renderer = pChart as IFeatureRenderer;


            pMapcontrol.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);


            
        }

        public IRgbColor GetRgb(int r, int g, int b)
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

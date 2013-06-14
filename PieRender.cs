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
    class PieRender
    {
        public PieRender(AxMapControl pMapcontrol, IFeatureLayer pFtLayer, string pFieldName1, string pFieldName2)
        {
           
            IGeoFeatureLayer pGeoFeaLayer = (IGeoFeatureLayer)pFtLayer;
            IChartRenderer pChartRenderer = new ChartRendererClass();
            // Set up the field to draw charts
            IRendererFields pRenderFields = (IRendererFields)pChartRenderer;
            pRenderFields.AddField(pFieldName1, pFieldName1);
            pRenderFields.AddField(pFieldName2, pFieldName2);
            IPieChartRenderer pPieChartRender = (IPieChartRenderer)pChartRenderer;

            //计算最大值部分有待补充////////////////////////////////////
            //Calculate the max value of the data field to scale the chart

            //ICursor pCursor = new CursorClass();
            IQueryFilter pQueryFilter = new QueryFilterClass();
            //IRowBuffer pRow = new RowBufferClass();
            ITable pTable = (ITable)pGeoFeaLayer;
            pQueryFilter.AddField(pFieldName1);
            ICursor pCursor = pTable.Search(pQueryFilter, true);

            IDataStatistics pDataStat = new DataStatisticsClass();

            IFeatureCursor pFtCursor = pFtLayer.FeatureClass.Search(null, false);

            pDataStat.Cursor = pFtCursor as ICursor;

            pDataStat.Field = pFieldName1;

            double pMax = pDataStat.Statistics.Maximum;



            IPieChartSymbol pPiechartSymbol = new PieChartSymbolClass();
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            IChartSymbol pChartSymbol = (IChartSymbol)pPiechartSymbol;
            pPiechartSymbol.Clockwise = true;
            pPiechartSymbol.UseOutline = true;
            ILineSymbol pOutLine = new SimpleLineSymbolClass();
            pOutLine.Color = GetRGBColor(255, 0, 255);
            pOutLine.Width = 1;
            pPiechartSymbol.Outline = pOutLine;

            IMarkerSymbol pMarkerSymbol = (IMarkerSymbol)pPiechartSymbol;
            //finally 
            pChartSymbol.MaxValue = pMax;
            pMarkerSymbol.Size = 16;
            //像符号数组中添加 添加符号
            ISymbolArray pSymbolArray = (ISymbolArray)pPiechartSymbol;
            pFillSymbol.Color = GetRGBColor(213, 212, 252);
            pFillSymbol.Outline = pOutLine;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);
            //////////////////////////
            pFillSymbol.Color = GetRGBColor(183, 242, 122);
            pFillSymbol.Outline = pOutLine;
            pSymbolArray.AddSymbol((ISymbol)pFillSymbol);
            //set up the background
            pFillSymbol.Color = GetRGBColor(239, 228, 190);
            pChartRenderer.BaseSymbol = (ISymbol)pFillSymbol;
            pChartRenderer.UseOverposter = false;
            pPieChartRender.MinSize = 1;
            pPieChartRender.MinValue = pDataStat.Statistics.Minimum;
            pPieChartRender.FlanneryCompensation = false;
            pPieChartRender.ProportionalBySum = true;
            pChartRenderer.ChartSymbol = (IChartSymbol)pPiechartSymbol;
            pChartRenderer.CreateLegend();
            pGeoFeaLayer.Renderer = (IFeatureRenderer)pChartRenderer;
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

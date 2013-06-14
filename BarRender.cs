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
    class BarRender
    {


        public BarRender(AxMapControl pMapcontrol, IFeatureLayer pFtLayer, String pFieldName1, string pFieldName2)
        {

   
          
            //IGeoFeatureLayer pGeoFeatureLayer = pFtLayer as IGeoFeatureLayer;

         
            //IFeatureClass pFeatureClass = pFtLayer.FeatureClass;

            ////定义柱状图渲染组建对象
            //IChartRenderer pChartRenderer = new ChartRendererClass();
            ////定义渲染字段对象并给字段对象实例化为pChartRenderer
            //IRendererFields pRendererFields;
            //pRendererFields = (IRendererFields)pChartRenderer;
            ////向渲染字段对象中添加字段--- 待补充自定义添加
            //pRendererFields.AddField(pFieldName1, pFieldName1);
            //pRendererFields.AddField(pFieldName2, pFieldName2);


            //ITable pTable;
            //pTable = pGeoFeatureLayer as ITable;
          

            //int[] pFieldIndecies = new int[2];


            //pFieldIndecies[0] = pTable.FindField(pFieldName1);
            //pFieldIndecies[1] = pTable.FindField(pFieldName2);
           


            //IDataStatistics pDataStat = new DataStatisticsClass();

            //IFeatureCursor pFtCursor = pFtLayer.FeatureClass.Search(null, false);

            //pDataStat.Cursor = pFtCursor as ICursor;

            //pDataStat.Field = pFieldName2;

            //double pMax = pDataStat.Statistics.Maximum;

            //// 定义并设置渲染时用的chart marker symbol
            //IBarChartSymbol pBarChartSymbol = new BarChartSymbolClass();
            //pBarChartSymbol.Width = 6;

            //IChartSymbol pChartSymbol;
            //pChartSymbol = pBarChartSymbol as IChartSymbol;

            //IMarkerSymbol pMarkerSymbol;
            //pMarkerSymbol = (IMarkerSymbol)pBarChartSymbol;



            //IFillSymbol pFillSymbol;
            ////设置pChartSymbol的最大值
            //pChartSymbol.MaxValue = pMax;
            //// 设置bars的最大高度 
            //pMarkerSymbol.Size = 80;
            ////下面给每一个bar设置符号

            ////定义符号数组
            //ISymbolArray pSymbolArray = (ISymbolArray)pBarChartSymbol;

           

            ////添加第一个符号
            //pFillSymbol = new SimpleFillSymbolClass();
            //pFillSymbol.Color = GetRGBColor(193, 252, 179) as IColor;
            //pSymbolArray.AddSymbol(pFillSymbol as ISymbol);
            ////添加第二个符号
            //pFillSymbol = new SimpleFillSymbolClass();
            // pFillSymbol.Color = GetRGBColor(145, 55, 251) as IColor;
            //pSymbolArray.AddSymbol(pFillSymbol as ISymbol);



           
            //pChartRenderer.ChartSymbol = pChartSymbol as IChartSymbol;
            ////pChartRenderer.Label = "AREA";
            //pChartRenderer.CreateLegend();

            //pChartRenderer.UseOverposter = false;
            ////将柱状图渲染对象与渲染图层挂钩
            //pGeoFeatureLayer.Renderer = (IFeatureRenderer)pChartRenderer;

            
            ////刷新地图和TOOCotrol
            //IActiveView pActiveView = pMapcontrol .ActiveView as IActiveView;
            //pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);




            //获取当前图层 ，并把它设置成IGeoFeatureLayer的实例 
            IMap pMap = pMapcontrol.Map;
            ILayer pLayer = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
            IGeoFeatureLayer pGeoFeatureLayer = pLayer as IGeoFeatureLayer;

            //获取图层上的feature 
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;

            //定义柱状图渲染组建对象 
            IChartRenderer pChartRenderer = new ChartRendererClass();
            //定义渲染字段对象并给字段对象实例化为pChartRenderer 
            IRendererFields pRendererFields;
            pRendererFields = (IRendererFields)pChartRenderer;
            //向渲染字段对象中添加字段--- 待补充自定义添加 
            pRendererFields.AddField(pFieldName1, pFieldName1);
            pRendererFields.AddField(pFieldName2, pFieldName2);

            // 通过查找features的所用字段的值，计算出数据字段的最大值，作为设置柱状图的比例大小的依据 
            ITable pTable;
            int fieldNumber;
            pTable = pGeoFeatureLayer as ITable;
            // 查找出geoFeatureLayer的属性表中的字段个数 
            fieldNumber = pTable.FindField(pFieldName1);
            if (fieldNumber == -1)
            {
                MessageBox.Show("Can't find field called ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            const int numFields = 2;// 设置bars的个数 
            int[] fieldIndecies = new int[2];
            //long fieldIndex; 
            double maxValue;
            bool firstValue;
            //double[] fieldValue=new double[5]; 
            double fieldValue;
            fieldIndecies[0] = pTable.FindField(pFieldName1);
            fieldIndecies[1] = pTable.FindField(pFieldName2);
            firstValue = true;
            maxValue = 0;
            int n = pFeatureClass.FeatureCount(null);
            for (int i = 0; i < numFields; i++)
            {
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                for (int j = 0; j < n; j++)
                {
                    IFeature pFeature = pFeatureCursor.NextFeature();
                    fieldValue = Convert.ToDouble(pFeature.get_Value(fieldIndecies[i]));

                    if (firstValue)
                    {
                        //给maxValue赋初值 
                        maxValue = fieldValue;
                        firstValue = false;
                    }
                    else if (fieldValue > maxValue)
                    {
                        maxValue = fieldValue;
                    }

                }
            }

            if (maxValue <= 0)
            {
                MessageBox.Show("Failed to calculate the maximum value or maxvalue is 0.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // 定义并设置渲染时用的chart marker symbol 
            IBarChartSymbol pBarChartSymbol = new BarChartSymbolClass();
            pBarChartSymbol.Width = 6;

            IChartSymbol pChartSymbol;
            pChartSymbol = pBarChartSymbol as IChartSymbol;

            IMarkerSymbol pMarkerSymbol;
            pMarkerSymbol = (IMarkerSymbol)pBarChartSymbol;



            IFillSymbol pFillSymbol;
            //设置pChartSymbol的最大值 
            pChartSymbol.MaxValue = maxValue;
            // 设置bars的最大高度 
            pMarkerSymbol.Size = 16;
            //下面给每一个bar设置符号 

            //定义符号数组 
            ISymbolArray pSymbolArray = (ISymbolArray)pBarChartSymbol;

            //克隆pFillSymbol用于符号操作 
            //IClone pSourceClone = pFillSymbol as IClone; 
            //ISimpleFillSymbol pSimpleFillSymbol = pSourceClone.Clone() as ISimpleFillSymbol; 

            // 向符号数组中添加设置后的符号 
            //pSimpleFillSymbol.Color = GetRGBColor(193, 252, 179); 
            //pSymbolArray.AddSymbol(pSimpleFillSymbol as ISymbol); 

            //pSimpleFillSymbol.Color = GetRGBColor(145, 55, 200); 
            //pSymbolArray.AddSymbol(pSimpleFillSymbol as ISymbol); 

            //添加第一个符号 
            pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = GetRGBColor(193, 252, 179) as IColor;
            pSymbolArray.AddSymbol(pFillSymbol as ISymbol);
            //添加第二个符号 
            pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = GetRGBColor(145, 55, 251) as IColor;
            pSymbolArray.AddSymbol(pFillSymbol as ISymbol);



            // 设置背景符号 
            //pSimpleFillSymbol.Color = GetRGBColor(239, 150, 190); 
            //pChartRenderer.BaseSymbol = pSimpleFillSymbol as ISymbol; 

            // Disable overpoaster 让符号处于图形中央 
            pChartRenderer.UseOverposter = false;


            //pChartRenderer.ChartSymbol = pSymbolArray as IChartSymbol; 
            pChartRenderer.ChartSymbol = pChartSymbol as IChartSymbol;
            //pChartRenderer.Label = "AREA"; 
            pChartRenderer.CreateLegend();

            //将柱状图渲染对象与渲染图层挂钩 
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pChartRenderer;
            pGeoFeatureLayer.DisplayField = pFieldName1;
            //刷新地图和TOOCotrol 
            IActiveView pActiveView = pMapcontrol.Map as IActiveView;
            pActiveView.Refresh();
          

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

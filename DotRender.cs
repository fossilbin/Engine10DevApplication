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
    
    public class DotRender
    {
        IGeoFeatureLayer pGeoLayer;

        IDotDensityRenderer pDotDensityRenderer;//渲染对象

        IDotDensityFillSymbol pDotDensityFill;//渲染填充符号对象，大对象分解小对象，独立的可看作对象。

        IRendererFields pRendFields;//用那个字段渲染。理解层次关系。

        ISymbolArray pSymbolArry;

        public DotRender(AxMapControl pMapControl, IFeatureLayer pFtLayer, double pValue,string pFieldName)
        {



            //IActiveView pActiveView;

            //this.pGeoLayer = pFtLayer as IGeoFeatureLayer;

            //pActiveView = pMapControl.ActiveView;

            //pDotDensityRenderer = new DotDensityRendererClass();

            //pRendFields = pDotDensityRenderer as IRendererFields;

            //pRendFields.AddField(pFieldName, pFieldName); //同一个对象的接口的切换，很方便的。

            //this.pDotDensityFill = new DotDensityFillSymbolClass();

            //pDotDensityFill.DotSize = 8;

            //pDotDensityFill.Color = GetRGBColor(10, 20, 0);

            //pDotDensityFill.BackgroundColor = GetRGBColor(100, 108, 190);

            //pSymbolArry = pDotDensityFill as ISymbolArray;//难道是密度。

            //ISimpleMarkerSymbol pSimpleMark;

            //pSimpleMark = new SimpleMarkerSymbolClass();

            //pSimpleMark.Style = esriSimpleMarkerStyle.esriSMSDiamond;

            //pSimpleMark.Size = 8;

            //pSimpleMark.Color = GetRGBColor(128, 128, 255);

            //pSymbolArry.AddSymbol(pSimpleMark as ISymbol);

            //pDotDensityRenderer.DotDensitySymbol = pDotDensityFill;

            //pDotDensityRenderer.DotValue = pValue;

            //pDotDensityRenderer.CreateLegend();
            //pGeoLayer.Renderer = (IFeatureRenderer)pDotDensityRenderer;

            //pActiveView.Refresh();




            //获取当前图层 ，并把它设置成IGeoFeatureLayer的实例 
            IMap pMap = pMapControl.Map;
            // ILayer pFtLayer = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureLayer pFeatureLayer = pFtLayer as IFeatureLayer;
            IGeoFeatureLayer pGeoFeatureLayer = pFtLayer as IGeoFeatureLayer;

            //获取图层上的feature
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            IFeature pFeature = pFeatureCursor.NextFeature();

            ///////////////////////

            ///////////////////////////////////////////////////////////////////
            //定义点密度图渲染组件
            IDotDensityRenderer DotDensityRenderer = new DotDensityRendererClass();

            //定义点密度图渲染组件对象的渲染字段对象
            IRendererFields flds = (IRendererFields)DotDensityRenderer;
            flds.AddField(pFieldName, pFieldName);
            //flds.AddField("Shape", "Shape");

            //定义点密度图渲染得符号对象
            IDotDensityFillSymbol ddSym = new DotDensityFillSymbolClass();
            IRgbColor BackColor = new RgbColorClass();
            BackColor.Red = 234;
            BackColor.Blue = 128;
            BackColor.Green = 220;
            IRgbColor SymbolColor = new RgbColorClass();
            SymbolColor.Red = 234;
            SymbolColor.Blue = 128;
            SymbolColor.Green = 220;
            ////点密度图渲染背景颜色
            //ddSym.BackgroundColor = BackColor;
            ddSym.DotSize = 8;
            ddSym.FixedPlacement = true;
            //ddSym.Color = SymbolColor;
            ILineSymbol pLineSymbol = new CartographicLineSymbolClass();
            ddSym.Outline = pLineSymbol;

            //定义符号数组 
            ISymbolArray symArray = (ISymbolArray)ddSym;
            //添加点密度图渲染的点符号到符号数组中去
            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            pMarkerSymbol.Size = 0.2;
            pMarkerSymbol.Color = SymbolColor; ;


            symArray.AddSymbol(pMarkerSymbol as ISymbol);

            //设置点密度图渲染的点符号
            //DotDensityRenderer.DotDensitySymbol =symArray;
            DotDensityRenderer.DotDensitySymbol = ddSym;
            //确定一个点代表多少值
            DotDensityRenderer.DotValue = pValue;
            //点密度渲染采用的颜色模式
            DotDensityRenderer.ColorScheme = "Custom";
            //创建点密度图渲染图例
            DotDensityRenderer.CreateLegend();
            //设置符号大小是否固定
            DotDensityRenderer.MaintainSize = true;
            //将点密度图渲染对象与渲染图层挂钩
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)DotDensityRenderer;
            //刷新地图和TOOCotrol
            IActiveView pActiveView = pMap as IActiveView;
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

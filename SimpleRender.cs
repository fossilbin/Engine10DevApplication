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
  
    public class SimpleRender
    {
        public SimpleRender(AxMapControl pMapcontrol, IFeatureLayer pFtLayer, String Field)
        {

            IGeoFeatureLayer pGeolayer;

            IActiveView pActiveView;

            pGeolayer = pFtLayer as IGeoFeatureLayer;

            pActiveView = pMapcontrol.ActiveView;

            IFillSymbol pFillSymbol;

            ILineSymbol pLineSymbol;

            pFillSymbol = new SimpleFillSymbolClass();

            pFillSymbol.Color = GetRGBColor(220, 110, 200);

            pLineSymbol = new SimpleLineSymbolClass();

            pLineSymbol.Color = GetRGBColor(255, 120, 105);

            pLineSymbol.Width = 2;

            pFillSymbol.Outline = pLineSymbol;

            ISimpleRenderer pSimpleRender;//用什么符号渲染

            pSimpleRender = new SimpleRendererClass();

            pSimpleRender.Symbol = pFillSymbol as ISymbol ;

            pSimpleRender.Description = "China";

            pSimpleRender.Label = "SimpleRender";


            ITransparencyRenderer pTrans;

            pTrans = pSimpleRender as ITransparencyRenderer;

            pTrans.TransparencyField = Field;

            pGeolayer.Renderer = pTrans as IFeatureRenderer;

            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);


            //地理图层的渲染对象是一个要素渲染对象，而这个对象是由一些相关对象组成的。
            //属性也是一个对象，说明大对象是由小对象组成的。


        }

        private IRgbColor  GetRGBColor(int R, int G, int B)//子类赋给父类
        {
            IRgbColor pRGB;

            pRGB = new RgbColorClass();

            pRGB.Red = R;

            pRGB.Green = G;

            pRGB.Green = B;

            return pRGB;


        }

       
    }
}

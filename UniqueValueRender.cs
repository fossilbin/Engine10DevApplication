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
    class UniqueValueRender
    {
        public  UniqueValueRender(AxMapControl pMapcontrol, IFeatureLayer pFtLayer,int pCount, string pFieldName)
        {

            IGeoFeatureLayer pGeoFeaturelayer = pFtLayer as IGeoFeatureLayer;

            IUniqueValueRenderer pUnique = new UniqueValueRendererClass();

            pUnique.FieldCount = 1;

            pUnique.set_Field(0, pFieldName);


            ISimpleFillSymbol pSimFill = new SimpleFillSymbolClass();

            

            //给颜色

            IFeatureCursor pFtCursor = pFtLayer.FeatureClass.Search(null, false);

            IFeature pFt = pFtCursor.NextFeature();


            IFillSymbol pFillSymbol1;


            ////添加第一个符号
            //pFillSymbol1 = new SimpleFillSymbolClass();
            //pFillSymbol1.Color = GetRGBColor(103, 252, 179) as IColor;
           
            ////添加第二个符号
            //IFillSymbol pFillSymbol2 = new SimpleFillSymbolClass();
            //pFillSymbol2.Color = GetRGBColor(125, 155, 251) as IColor;
         


            //创建并设置随机色谱从上面的的图可以看出我们要给每一个值定义一种颜色，我们可以创建色谱，但是色谱的这些参数
              //我调不好这个没，因此掠过则个步骤，我重新定义了两个符号。

            IRandomColorRamp pColorRamp = new RandomColorRampClass();

            pColorRamp.StartHue = 0;

            pColorRamp.MinValue = 20;

            pColorRamp.MinSaturation = 15;

            pColorRamp.EndHue = 360;

            pColorRamp.MaxValue = 100;

            pColorRamp.MaxSaturation = 30;

            pColorRamp.Size = pCount ;

            //pColorRamp.Size = pUniqueValueRenderer.ValueCount;

            bool ok = true;

            pColorRamp.CreateRamp(out ok);

            IEnumColors pEnumRamp = pColorRamp.Colors;

            //IColor pColor = pEnumRamp.Next();
            

            int pIndex =pFt.Fields.FindField(pFieldName);

           //因为我只有24条记录，所以改变这些，这些都不会超过255或者为负数.求余

            int i = 0;


            while (pFt != null)
            {
                IColor pColor = pEnumRamp.Next();
                if(pColor ==null)
                {
                    pEnumRamp.Reset();
                    pColor = pEnumRamp.Next();
                
                }

                   






                //if (i % 2 == 0)
                //{
                //    pUnique.AddValue(Convert.ToString(pFt.get_Value(pIndex)), pFieldName, pFillSymbol1 as ISymbol);

                //}
                //else
                //{
                //    pUnique.AddValue(Convert.ToString(pFt.get_Value(pIndex)), pFieldName, pFillSymbol2 as ISymbol);
                //}

                //i++;

                pFillSymbol1 = new SimpleFillSymbolClass();

                pFillSymbol1.Color = pColor;
                pUnique.AddValue(Convert.ToString(pFt.get_Value(pIndex)), pFieldName, pFillSymbol1 as ISymbol);


                pFt = pFtCursor.NextFeature();

              //  pColor = pEnumRamp.Next();

                
            }


            pGeoFeaturelayer.Renderer = pUnique as IFeatureRenderer;

            pMapcontrol.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }


        private IRgbColor GetRGBColor(int R, int G, int B)//子类赋给父类
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

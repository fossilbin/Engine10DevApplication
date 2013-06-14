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
    
    public class ClassRender
    {
        public ClassRender(AxMapControl pMapControl, IFeatureLayer pFtLayer, int ClassCount, string pFieldName)
        {
            IGeoFeatureLayer pGeolayer;


            IActiveView pActiveView;


            pGeolayer = pFtLayer as IGeoFeatureLayer;

            pActiveView = pMapControl.ActiveView;

            //以下是为了统计和分类所需要的对象

            ITable pTable;

            IClassifyGEN pClassify;//C#要用这个不同于VB中的，分类对象。

            ITableHistogram pTableHist;//相当于一个统计表

            IBasicHistogram pBasicHist;//这个对象有一个很重要的方法

            double[] ClassNum;

            int ClassCountResult;//返回分类个数。

            IHsvColor pFromColor;

            IHsvColor pToColor;//用于构建另外一个颜色带对象。

            IAlgorithmicColorRamp pAlgo;

            pTable = pGeolayer as ITable;

            IMap pMap;

            pMap = pMapControl.Map;

            pMap.ReferenceScale = 0;

            pBasicHist = new BasicTableHistogramClass();//也可以实例化 pTableHist。学会这个灵活的思维

            pTableHist = pBasicHist as ITableHistogram;

            pTableHist.Table = pTable;

            pTableHist.Field = pFieldName;

            object datavalus;

            object Frenquen;

            pBasicHist.GetHistogram(out datavalus,out  Frenquen);//获得数据和相应的频数。

            pClassify = new EqualIntervalClass();

            try
            {
                pClassify.Classify(datavalus, Frenquen, ref ClassCount);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            // 分类完成

            ClassNum  = (double[])pClassify.ClassBreaks;

            ClassCountResult = ClassNum.GetUpperBound(0);//返回分级个数。

            IClassBreaksRenderer pClassBreak;

            pClassBreak = new ClassBreaksRendererClass();

            pClassBreak.Field = pFieldName;

            pClassBreak.BreakCount = ClassCountResult;

            pClassBreak.SortClassesAscending = true;

            pAlgo = new AlgorithmicColorRampClass();

            pAlgo.Algorithm = esriColorRampAlgorithm.esriHSVAlgorithm;

            pFromColor = Hsv(60, 100, 96);

            pToColor = Hsv(0, 100, 96);

            pAlgo.FromColor = pFromColor;

            pAlgo.ToColor = pToColor;

            pAlgo.Size = ClassCountResult;

            bool ok;

            pAlgo.CreateRamp(out ok);

            IEnumColors pEnumColor;

            pEnumColor = pAlgo.Colors;

            pEnumColor.Reset();

            IColor pColor;

            ISimpleFillSymbol pSimFill;

           /* IRgbColor[] pRgbColor;//可以构造颜色

            pRgbColor = new IRgbColor[ClassCountResult];

            for (int j = 0; j < ClassCountResult; j++)
            {
                int R = 50;

                int G = 100;

                int B = 50;

                R = R + 50;

                if (R > 250)
                {
                    R = 50;
                }
                if (G > 250)
                {
                    G = 100;
                }
                if (B > 250)
                {
                    B = 50;
                }

                G = G + 100;

                B = B + 50;



                pRgbColor[j] = ColorRgb(R, G, B);

            }
            */


            for (int indexColor = 0; indexColor <= ClassCountResult - 1; indexColor++)
            {
                pColor = pEnumColor.Next();

                pSimFill = new SimpleFillSymbolClass();

                 pSimFill.Color = pColor;

               // pSimFill.Color = pRgbColor[indexColor ];

                pSimFill.Style = esriSimpleFillStyle.esriSFSSolid;

                //染色

                pClassBreak.set_Symbol(indexColor, pSimFill as ISymbol);

                pClassBreak.set_Break(indexColor, ClassNum[indexColor + 1]);



            }



            pGeolayer.Renderer = pClassBreak as IFeatureRenderer;

            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);


           



        }
        public IHsvColor Hsv(int hue, int saturation, int val)
        {
            IHsvColor pHsvC;

            pHsvC = new HsvColorClass();

            pHsvC.Hue = hue;

            pHsvC.Saturation = saturation;

            pHsvC.Value = val;

            return pHsvC;
        }
        public IRgbColor ColorRgb(int r, int g, int b)
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

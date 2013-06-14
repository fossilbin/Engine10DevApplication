using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace EngineApplication
{
    class GeometryTest
    {
        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private IPoint ConstructPoint(double x, double y)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(x, y);
            return pPoint;
        }

        private object pMissing = Type.Missing;

        public IGeometry GetMultipointGeometry()
        {
            const double MultipointPointCount = 25;

            IPointCollection pPointCollection = new MultipointClass();

            for (int i = 0; i < MultipointPointCount; i++)
            {
                pPointCollection.AddPoint(GetPoint(), ref pMissing, ref pMissing);
            }

            return pPointCollection as IGeometry;
        }
         private IPoint GetPoint()
        {
            const double Min = -10;
            const double Max = 10;

            Random pRandom = new Random();

            double x = Min + (Max - Min) * pRandom.NextDouble();
            double y = Min + (Max - Min) * pRandom.NextDouble();

            return ConstructPoint(x, y);
        }




         private IPolygon FlatBuffer(IPolyline pLline1, double pBufferDis)
         {
             object o = System.Type.Missing;
             //分别对输入的线平移两次（正方向和负方向）
             IConstructCurve pCurve1 = new PolylineClass();
             pCurve1.ConstructOffset(pLline1, pBufferDis, ref o, ref o);
             IPointCollection pCol = pCurve1 as IPointCollection;
             IConstructCurve pCurve2 = new PolylineClass();
             pCurve2.ConstructOffset(pLline1, -1 * pBufferDis, ref o, ref o);
             //把第二次平移的线的所有节点翻转
             IPolyline pline2 = pCurve2 as IPolyline;
             pline2.ReverseOrientation();
             //把第二条的所有节点放到第一条线的IPointCollection里面
             IPointCollection pCol2 = pline2 as IPointCollection;
             pCol.AddPointCollection(pCol2);
             //用面去初始化一个IPointCollection
             IPointCollection pPointCol = new PolygonClass();
             pPointCol.AddPointCollection(pCol);
             //把IPointCollection转换为面
             IPolygon pPolygon = pPointCol as IPolygon;
             //简化节点次序
             pPolygon.SimplifyPreserveFromTo();
             return pPolygon;
         }


       

       

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;



namespace EngineApplication
{
   
    public class CalculatContourTask : ESRI.ArcGIS.Controls.IEngineEditTask
    {
        #region Private Members
        IEngineEditor pEngineEditor;
        IEngineEditSketch pEditSketch;
        IEngineEditLayers pEditLayer;
        #endregion

   

        #region IEngineEditTask Implementations

        public void Activate(ESRI.ArcGIS.Controls.IEngineEditor pEditor, ESRI.ArcGIS.Controls.IEngineEditTask pOldTask)
        {
            if (pEditor == null)
                return;

            pEngineEditor = pEditor;
            pEditSketch = pEngineEditor as IEngineEditSketch;
            pEditSketch.GeometryType = esriGeometryType.esriGeometryPolyline;
            pEditLayer = pEditSketch as IEngineEditLayers;

            //Listen to engine editor events
            ((IEngineEditEvents_Event)pEditSketch).OnTargetLayerChanged += new IEngineEditEvents_OnTargetLayerChangedEventHandler(OnTargetLayerChanged);
           
            ((IEngineEditEvents_Event)pEditSketch).OnCurrentTaskChanged += new IEngineEditEvents_OnCurrentTaskChangedEventHandler(OnCurrentTaskChanged);
        }


        public void Deactivate()
        {
            pEditSketch.RefreshSketch();

            //Stop listening to engine editor events.
            ((IEngineEditEvents_Event)pEditSketch).OnTargetLayerChanged -= OnTargetLayerChanged;
       
            ((IEngineEditEvents_Event)pEditSketch).OnCurrentTaskChanged -= OnCurrentTaskChanged;

            //Release object references.
            pEngineEditor = null;
            pEditSketch = null;
            pEditLayer = null;
        }

        public string GroupName
        {
            get
            {
                //This property allows groups to be created/used in the EngineEditTaskToolControl treeview.
                //If an empty string is supplied the task will be appear in an "Other Tasks" group. 
                //In this example the Reshape Polyline_CSharp task will appear in the existing Modify Tasks group.
                return "Modify Tasks";
            }
        }

        public string Name
        {
            get
            {
                return "CalculateContourTask"; //unique edit task name
            }
        }

        public void OnDeleteSketch()
        {
        }

        public void OnFinishSketch()
        {
            //get reference to featurelayer being edited
            IFeatureLayer pFeatureLayer = pEditLayer.TargetLayer as IFeatureLayer;
            //get reference to the sketch geometry
            IGeometry pPolyline = pEditSketch.Geometry;

            if (pPolyline.IsEmpty == false)
            {
                ParaSetting pFormSetting = new ParaSetting(pFeatureLayer.FeatureClass);

                pFormSetting.ShowDialog();


                if (pFormSetting.DialogResult == DialogResult.OK)
                {
                    pHeightName = pFormSetting.pFieldNames.Text;

                    pHeight = Convert.ToDouble(pFormSetting.dHeight.Text);

                    pInterval = Convert.ToDouble(pFormSetting.dInterval.Text);

                    pFormSetting.Dispose();

                    pFormSetting = null;

                    IFeatureCursor pFeatureCursor = GetFeatureCursor(pPolyline, pFeatureLayer.FeatureClass);

                    CalculateIntersect(pFeatureCursor, pPolyline);

                    MessageBox.Show("计算完成");
                }
           

            }

            //refresh the display 
            IActiveView pActiveView = pEngineEditor.Map as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, (object)pFeatureLayer, pActiveView.Extent);


        }

        public string UniqueName
        {
            get
            {
                return "CalculateContourTask";
            }
        }

        #endregion

        #region Event Listeners
        public void OnTargetLayerChanged()
        {
            PerformSketchToolEnabledChecks();
        }



        void OnCurrentTaskChanged()
        {
            if (pEngineEditor.CurrentTask.Name == "CalculateContourTask")
            {
                PerformSketchToolEnabledChecks();
            }
        }
        #endregion


        private IFeatureCursor GetFeatureCursor(IGeometry pGeometry, IFeatureClass pFeatureClass)
        {

            //空间过虑器的创建
            ISpatialFilter pSpatialFilter = new SpatialFilter();
            pSpatialFilter.Geometry = pGeometry;
            //空间过虑器几何体实体
            //空间过虑器参照系

            //空间过虑器空间数据字段名
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
            //空间过虑器空间关系类型
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            //相交
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);

            return pFeatureCursor;


        }

        //起始等高线值
        private double pHeight;
        //等高线间距
        private double pInterval;
        //高程字段名
        private string pHeightName;



        private void CalculateIntersect(IFeatureCursor pFeatureCursor, IGeometry Geometry)
        {


            //要素游标
            IMultipoint pIntersectionPoints = null;
            //多点
            IPointCollection pPointColl = null;

            List<IFeature> pFeatureList = new List<IFeature>();
            //和直线相交的要素集合，未排序
            double[,] pIndex = null;
            //距离和初始索引


            if (pFeatureCursor == null)
            {
                return;
            }
            ITopologicalOperator pTopoOperator = Geometry as ITopologicalOperator;

            IPointCollection pSketchPointColl = Geometry as IPointCollection;
            //所画直线的起点
            IPoint P0 = pSketchPointColl.get_Point(0);
            IFeature pFeature = pFeatureCursor.NextFeature();
            double HValue = 0;
            int FldIndex = 0;
            pFeatureList.Clear();
            while ((pFeature != null))
            {
                //和直线相交的要素集合
                pFeatureList.Add(pFeature);
                //
                pFeature = pFeatureCursor.NextFeature();
            }
            //此时pFeatureL中的等值线并不是按顺序（空间）排列，需要排序
            //求出各交点到直线起点距离
            int pCount = pFeatureList.Count;
            pIndex = new double[2, pCount];
            for (int i = 0; i <= pCount - 1; i++)
            {
                try
                {
                    pFeature = pFeatureList[i];
                    //求交点:
                    pIntersectionPoints = pTopoOperator.Intersect(pFeature.Shape, esriGeometryDimension.esriGeometry0Dimension) as IMultipoint;

                    pPointColl = pIntersectionPoints as IPointCollection;
                    //QI
                    //原来序号
                    pIndex[0, i] = i;
                    //距离
                    pIndex[1, i] = GetDistace(P0, pPointColl.get_Point(0));
                    //下个要素
                    pFeature = pFeatureCursor.NextFeature();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            //排序:将和直线相交的等直线按与起点的距离排序，冒泡法
            for (int i = 0; i <= pCount - 1; i++)
            {
                for (int j = i + 1; j <= pCount - 1; j++)
                {
                    if (pIndex[1, j] < pIndex[1, i])
                    {
                        double pTempindex = pIndex[0, i];
                        pIndex[0, i] = pIndex[0, j];
                        pIndex[0, j] = pTempindex;
                        //交换索引
                        double pTemp = pIndex[1, i];

                        pIndex[1, i] = pIndex[1, j];

                        pIndex[1, j] = pTemp;
                        //交换距离
                    }
                }
            }
            //开始高程赋值
            HValue = pHeight;
            try
            {
                for (int i = 0; i <= pCount - 1; i++)
                {
                    pFeature = pFeatureList[i];
                    //获取高程字段的索引
                    FldIndex = pFeature.Fields.FindField(pHeightName);
                    //高程赋值
                    pFeature.set_Value(FldIndex, HValue as object);
                    //要素更新
                    pFeature.Store();
                    //Get the next feature and next H
                    HValue = HValue + pInterval;
                }

            }
            catch (Exception e)
            {


                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// 获取我们画的线和等高线之间的距离
        /// </summary>
        /// <param name="pPoint1"></param>
        /// <param name="pPoint2"></param>
        /// <returns></returns>
        private double GetDistace(IPoint pPoint1, IPoint pPoint2)
        {
            return (pPoint1.X - pPoint2.X) * (pPoint1.X - pPoint2.X) + (pPoint1.Y - pPoint2.Y) * (pPoint1.Y - pPoint2.Y);
        }

        #region private methods

        private void PerformSketchToolEnabledChecks()
        {
            if (pEditLayer == null)
                return;

            //Only enable the sketch tool if there is a polyline target layer.
            if (pEditLayer.TargetLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline)
            {
                pEditSketch.GeometryType = esriGeometryType.esriGeometryNull;
                return;
            }


            pEditSketch.GeometryType = esriGeometryType.esriGeometryPolyline;

        }

        #endregion

    }

}

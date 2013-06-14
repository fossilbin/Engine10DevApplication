using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.NetworkAnalyst;
using ESRI.ArcGIS.Location;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.Output;


namespace EngineApplication
{
    public partial class MainForm : Form
    {

        private ControlsSynchronizer pMapControlsSynchronizer = null;
        ILayer pGlobalFeatureLayer;

        int pClickedCount;

        INASolver pNASolveClass;
        
        //这个用来保存
        IMultipoint pMulPoint;

        //
        IGraphicsContainer pGC;

        IPointCollection pPointC;

        IMap pNetMap;

        public MainForm()
        {

         
            InitializeComponent();

            pMulPoint = new MultipointClass();

          

            pPointC = pMulPoint as IPointCollection;

            pClickedCount = 0;


        }

     
        public void IWorkspace__get_WorkspaceName(string workspacePath, IWorkspace workspace)
        {
            //Creates a new workspace name for a personal geodatabase.
            IWorkspaceName workspaceName = new WorkspaceNameClass();
            workspaceName.WorkspaceFactoryProgID = "esriDataSourcesGDB.AccessWorkspaceFactory";
            workspaceName.PathName = workspacePath;
            //Or Get a workspace name from an existing workspace.
            IDataset dataset = (IDataset)workspace; //Workspaces implement IDataset
            workspaceName = (IWorkspaceName)dataset.FullName;

        }


        public string OpenMxd()
        {
            string MxdPath = "";

            OpenFileDialog OpenMXD = new OpenFileDialog();

            OpenMXD.Title = "打开地图";

            OpenMXD.InitialDirectory = "E:";

            OpenMXD.Filter = "Map Documents (*.mxd)|*.mxd";
            

            if (OpenMXD.ShowDialog() == DialogResult.OK)
            {

                MxdPath = OpenMXD.FileName;

                
            }

            return MxdPath;
        }
        /// <summary>
        /// 返回一个长度为2的一维数组，string[0]表示Shape所在的文件夹，string[1]表示Shape文件名称
        /// </summary>
        /// <returns></returns>
        public string[] OpenShapeFile()
        {
            

            string[] ShapeFile = new string[2];

            OpenFileDialog OpenShpFile = new OpenFileDialog();

            OpenShpFile.Title = "打开Shape文件";

            OpenShpFile.InitialDirectory = "E:";

            OpenShpFile.Filter = "Shape文件(*.shp)|*.shp";


            if (OpenShpFile.ShowDialog() == DialogResult.OK)
            {

                string ShapPath = OpenShpFile.FileName;
                //利用"\\"将文件路径分成两部分

                int Position = ShapPath.LastIndexOf("\\");

                string FilePath = ShapPath.Substring(0,Position);

                string ShpName = ShapPath.Substring(Position+1);

                ShapeFile[0] = FilePath;

                ShapeFile[1] = ShpName;
               
            }

            return ShapeFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pMap"></param>
        /// <param name="LayerName"></param>
        /// <returns></returns>
        private ILayer GetLayer(IMap pMap, string LayerName)
        {
            IEnumLayer pEnunLayer;

            pEnunLayer = pMap.get_Layers(null, false);

            pEnunLayer.Reset();

            ILayer pRetureLayer;

            pRetureLayer = pEnunLayer.Next();

            while (pRetureLayer != null)
            {
                if (pRetureLayer.Name == LayerName)
                {
                    break;
                }

                pRetureLayer = pEnunLayer.Next();
            }
            return pRetureLayer;
        }



        private void axTOCControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseDownEvent e)
        {

            if (axMapControl1.LayerCount > 0)
            {
                esriTOCControlItem pItem = new esriTOCControlItem();

                 pGlobalFeatureLayer = new FeatureLayerClass();

                IBasicMap pBasicMap = new MapClass();

                object pOther = new object();

                object pIndex = new object();

                axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pBasicMap, ref pGlobalFeatureLayer, ref pOther, ref pIndex);
            }

            if (e.button == 2)
            {
                context.Show(axTOCControl1, e.x, e.y);
            }

            
        }

       

    

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (axMapControl1.LayerCount > 0)
            {

                if (pGlobalFeatureLayer != null)
                {
                    axMapControl1.Map.DeleteLayer(pGlobalFeatureLayer);
                }
            }

            
        }

        private void 打开属性表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTable Ft = new FormTable(pGlobalFeatureLayer as IFeatureLayer);

            Ft.Show();

        }

     

      
          public IWorkspace Get_Workspace(string _pWorkspacePath)
    {
        
        IWorkspaceName pWorkspaceName = new WorkspaceNameClass();

        pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesGDB.AccessWorkspaceFactory";

        pWorkspaceName.PathName = _pWorkspacePath;
        
        IName pName = pWorkspaceName as IName;

        IWorkspace pWorkspace = pName.Open() as IWorkspace;

        return pWorkspace;


    }

        /// <summary>
        ///打开要素类，以下几个函数为公用，暂时放在不放在一个公用类里面
        /// </summary>
        /// <returns></returns>
 
        /// <summary>
        /// 获取要素类
        /// </summary>
        /// <param name="p_GDBFileName"></param>
        /// <param name="p_FeatureClassName"></param>
        /// <returns></returns>
        private IFeatureClass OpenFeatureClass(string p_GDBFileName, string p_FeatureClassName)
        {
            IWorkspaceFactory li_WorkspaceFactory;

            IWorkspace li_Workspace;

            IFeatureWorkspace li_FeatureWorkspace;

            IFeatureClass li_FeatureClass;

            li_WorkspaceFactory = new AccessWorkspaceFactoryClass();

            li_Workspace = li_WorkspaceFactory.OpenFromFile(p_GDBFileName, 0);

            li_FeatureWorkspace = li_Workspace as IFeatureWorkspace;

            li_FeatureClass = li_FeatureWorkspace.OpenFeatureClass(p_FeatureClassName);

            return li_FeatureClass;

        }

        public  string  WsPath()
        {

            string WsFileName="";

            OpenFileDialog OpenFile = new OpenFileDialog();

            OpenFile.Filter = "个人数据库(MDB)|*.mdb";

            DialogResult DialogR = OpenFile.ShowDialog();

            if (DialogR == DialogResult.Cancel)
            {
               
            }
            else
            {

                WsFileName = OpenFile.FileName;
            }

            return WsFileName;
           
        }

    

        IRgbColor GetRGB(int R,int G,int B)
        {

            IRgbColor pColor = new RgbColorClass();

            pColor.Red = R;

            pColor.Green = G;

            pColor.Blue = B;

            return pColor;
        }

        IRasterDataset OpenGDBRasterDataset(IRasterWorkspaceEx pRasterWorkspaceEx, string pDatasetName)
        {
            //打开存放在数据库中的栅格数据
            return pRasterWorkspaceEx.OpenRasterDataset(pDatasetName);
        }



         IRasterDataset GetRasterCatalogItem(IRasterCatalog pCatalog, int pObjectID)
        {
            //栅格目录继承了IFeatureClass
            IFeatureClass pFeatureClass = (IFeatureClass)pCatalog;
            IRasterCatalogItem pRasterCatalogItem = (IRasterCatalogItem)pFeatureClass.GetFeature(pObjectID);
            return pRasterCatalogItem.RasterDataset;
        }

              IMosaicDataset GetMosaicDataset(string pFGDBPath,string pMDame)
              {
                  IWorkspaceFactory pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                  IWorkspace pFgdbWorkspace = pWorkspaceFactory.OpenFromFile(pFGDBPath, 0);
                  
                  IMosaicWorkspaceExtensionHelper pMosaicExentionHelper = new
                      MosaicWorkspaceExtensionHelperClass();
                  
                  IMosaicWorkspaceExtension pMosaicExtention = pMosaicExentionHelper.FindExtension(pFgdbWorkspace);
                  
                  return pMosaicExtention.OpenMosaicDataset(pMDame);

              }
         /// <summary>
        /// 创建镶嵌数据集
        /// </summary>
        /// <param name="pFGDBPath"></param>
        /// <param name="pMDame"></param>
        /// <param name="pSrs"></param>
        /// <returns></returns>
              IMosaicDataset CreateMosaicDataset(string pFGDBPath, string pMDame, ISpatialReference pSrs )
              {

                
                  IWorkspaceFactory pWorkspaceFactory = new FileGDBWorkspaceFactory();

                  IWorkspace pFgdbWorkspace = pWorkspaceFactory.OpenFromFile(pFGDBPath, 0);

                  ICreateMosaicDatasetParameters pCreationPars = new CreateMosaicDatasetParametersClass();
                 
                  pCreationPars.BandCount = 3;
                  
                  pCreationPars.PixelType = rstPixelType.PT_UCHAR;
                  
                  IMosaicWorkspaceExtensionHelper pMosaicExentionHelper = new  MosaicWorkspaceExtensionHelperClass();
                  
                  IMosaicWorkspaceExtension pMosaicExtention = pMosaicExentionHelper.FindExtension(pFgdbWorkspace);

                  return pMosaicExtention.CreateMosaicDataset(pMDame, pSrs, pCreationPars, "");


              }




              public IRasterDataset CreateRasterDataset(string pRasterFolderPath, string pFileName,string pRasterType,ISpatialReference pSpr )
              {
                  
                      IRasterWorkspace2 pRasterWs = GetRasterWorkspace(pRasterFolderPath) as IRasterWorkspace2;
                      
                      
                      IPoint pPoint = new PointClass();
                      pPoint.PutCoords(15.0, 15.0);
                     
                      int pWidth = 300; 
                      int pHeight = 300; 
                      double xCell = 30; 
                      double yCell = 30; 
                      int NumBand = 1;

                      IRasterDataset pRasterDataset = pRasterWs.CreateRasterDataset(pFileName, pRasterType,
                          pPoint, pWidth, pHeight, xCell, yCell, NumBand, rstPixelType.PT_UCHAR, pSpr,
                          true);

                      
                      IRasterBandCollection pRasterBands = (IRasterBandCollection)pRasterDataset;
                     
                       IRasterBand pRasterBand = pRasterBands.Item(0);
                      IRasterProps  pRasterProps = (IRasterProps)pRasterBand;
                     
                      pRasterProps.NoDataValue = 255;
                      
                      IRaster pRaster = pRasterDataset.CreateDefaultRaster();

                   
                      IPnt pPnt = new PntClass();
                      pPnt.SetCoords(30, 30);

                      IRaster2 pRaster2 = pRaster as IRaster2;
                      IRasterEdit pRasterEdit = (IRasterEdit)pRaster2;

                      IRasterCursor pRasterCursor = pRaster2.CreateCursorEx(pPnt);

                      do
                      {
                          IPixelBlock3 pPixelblock = pRasterCursor.PixelBlock as IPixelBlock3;

                          System.Array pixels = (System.Array)pPixelblock.get_PixelData(0);
                          for (int i = 0; i < pPixelblock.Width; i++)
                              for (int j = 0; j < pPixelblock.Height; j++)
                                  if (i == j)
                                      pixels.SetValue(Convert.ToByte(255), i, j);
                                  else
                                      pixels.SetValue(Convert.ToByte((i * j + 30) / 255), i, j);

                          pPixelblock.set_PixelData(0, (System.Array)pixels);

                          IPnt pUpperLeft = pRasterCursor.TopLeft;



                          pRasterEdit.Write(pUpperLeft, (IPixelBlock)pPixelblock);


                      } while (pRasterCursor.Next());
                      System.Runtime.InteropServices.Marshal.ReleaseComObject(pRasterEdit);
                      
                      return pRasterDataset;
 
              }

          
        void SearchHightlight(IMap _pMap,IFeatureLayer _pFeatureLayer, IQueryFilter _pQuery, bool _Bool)
        {
            IFeatureCursor pFtCursor = _pFeatureLayer.Search(_pQuery, _Bool);

            IFeature pFt = pFtCursor.NextFeature();

            while (pFt != null)
            {
                _pMap.SelectFeature(_pFeatureLayer as ILayer, pFt);

                pFt = pFtCursor.NextFeature();

            }
        }

       void  Search(IFeatureClass _pFeatureClass,bool _Bool)
        {

            IFeature pFt1, pFt2;

           IFeatureCursor pFtCursor;
           if (_Bool == false)
           {
               pFtCursor = _pFeatureClass.Search(null, _Bool);

               pFt1 = pFtCursor.NextFeature();

               while (pFt1 != null)
               {
                   pFt2 = pFtCursor.NextFeature();

                   if (pFt1 == pFt2)
                   {
                       MessageBox.Show("Recycling 参数是 false");
                   }
                   pFt1 = pFtCursor.NextFeature();

               }
           }
           else
           {
               pFtCursor = _pFeatureClass.Search(null, _Bool);

               pFt1 = pFtCursor.NextFeature();

               while (pFt1 != null)
               {
                   pFt2 = pFtCursor.NextFeature();

                   if (pFt1 == pFt2)
                   {
                       MessageBox.Show("Recycling 参数是true");
                   }
                   pFt1 = pFtCursor.NextFeature();

               }
           }

        }

    
       private IPolygon FlatBuffer(IPolyline _pLine, double _pBufferDis)
       {
           object o = System.Type.Missing;

           //分别对输入的线平移两次（正方向和负方向）

           IConstructCurve pConCurve = new PolylineClass();

           pConCurve.ConstructOffset(_pLine, _pBufferDis, ref o, ref o);

           IPointCollection pCol = pConCurve as IPointCollection;

           IConstructCurve pCurve = new PolylineClass();

           pCurve.ConstructOffset(_pLine, -1 * _pBufferDis, ref o, ref o);
           //把第二次平移的线的所有节点翻转
           IPolyline addline = pCurve as IPolyline;

           addline.ReverseOrientation();
           //把第二条的所有节点放到第一条线的IPointCollection里面
           IPointCollection pCol2 = addline as IPointCollection;

           pCol.AddPointCollection(pCol2);
           //用面去初始化一个IPointCollection
           IPointCollection myPCol = new PolygonClass();

           myPCol.AddPointCollection(pCol);
           //把IPointCollection转换为面
           IPolygon pPolygon = myPCol as IPolygon;
           //简化节点次序
           pPolygon.SimplifyPreserveFromTo();

           return pPolygon;
       }


       /// <summary>
       /// 通过线创建面
       /// </summary>
       /// <param name="pPolyline">线</param>
       /// <returns>面</returns>
       IPolygon ConstructPolygonFromPolyline(IPolyline _pPolyline)
       {
           IGeometryCollection pPolygonGeoCol = new PolygonClass();

           if ((_pPolyline != null) && (!_pPolyline.IsEmpty))
           {
               IGeometryCollection pPolylineGeoCol = _pPolyline as IGeometryCollection;

               ISegmentCollection pSegCol = new RingClass();

               ISegment pSegment = null;

               object missing = Type.Missing;

               for (int i = 0; i < pPolylineGeoCol.GeometryCount; i++)
               {
                   ISegmentCollection pPolylineSegCol = pPolylineGeoCol.get_Geometry(i) as ISegmentCollection;
                   for (int j = 0; j < pPolylineSegCol.SegmentCount; j++)
                   {
                       pSegment = pPolylineSegCol.get_Segment(j);

                       pSegCol.AddSegment(pSegment, ref missing, ref missing);
                   }
                   pPolygonGeoCol.AddGeometry(pSegCol as IGeometry, ref missing, ref missing);
               }
           }
           return pPolygonGeoCol as IPolygon;
       }


       void GPIntsect()
        {

            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();

            ESRI.ArcGIS.AnalysisTools.Intersect pIntsect = new ESRI.ArcGIS.AnalysisTools.Intersect();

            gp.OverwriteOutput = true;
            //设置工作空间 // IGpEnumList gpEnumEnv = gp.ListEnvironments("*");
            gp.SetEnvironmentValue("workspace", @".\data\分析用的空间数据");

            pIntsect.in_features = ".\data分析用的空间数据\\县界面.shp;.\data分析用的空间数据\\地级市.shp";

            pIntsect.out_feature_class = "Result.shp";

            pIntsect.cluster_tolerance = 0.5;

            pIntsect.join_attributes = "All";

            pIntsect.output_type = "INPUT";
        
            gp.Execute(pIntsect, null);

           
        }
       private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
       {
           
           if(pFlag==1)//缓冲区空间查询
           {
               IActiveView pActView = axMapControl1.Map as IActiveView;


               IPoint pPoint = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

               ITopologicalOperator pTopo = pPoint as ITopologicalOperator;

               IGeometry pGeo = pTopo.Buffer(500);


               ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
               rgbColor.Red = 255;

               ESRI.ArcGIS.Display.IColor color = rgbColor; // Implicit Cast
               ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
               simpleFillSymbol.Color = color;

               ESRI.ArcGIS.Display.ISymbol symbol = simpleFillSymbol as ESRI.ArcGIS.Display.ISymbol;

               pActView.ScreenDisplay.SetSymbol(symbol);

               pActView.ScreenDisplay.DrawPolygon(pGeo);

               axMapControl1.Map.SelectByShape(pGeo, null, false);

               axMapControl1.FlashShape(pGeo, 1000, 2, symbol);

               axMapControl1.ActiveView.Refresh();
           }
           if (pFlag == 2)
           {
               pNetMap = axMapControl1.Map;

               pGC = pNetMap as IGraphicsContainer;

               IActiveView pActView = pNetMap as IActiveView;

               IPoint pPoint = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

               object o = Type.Missing;
               object o1 = Type.Missing;

               pPointC.AddPoint(pPoint, ref o, ref o1);


               IElement Element;

               ITextElement Textelement = new TextElementClass();

               Element = Textelement as IElement;

               pClickedCount++;

               Textelement.Text = pClickedCount.ToString();

               Element.Geometry = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

               pGC.AddElement(Element, 0);

               pActView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);


               IFeatureClass pFeatureClass = pNaContext.NAClasses.get_ItemByName("Stops") as IFeatureClass;

               NASolve(pNaContext, pFeatureClass, pPointC, 5000);

               IGPMessages gpMessages = new GPMessagesClass();

               bool pBool = pNASolveClass.Solve(pNaContext, gpMessages, null);

           }

           if (pFlag == 3)//有向网络
           {
               IWorkspace pWs = GetMDBWorkspace(@".\data\Geometric.mdb");

               IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;

               IFeatureDataset pFtDataset = pFtWs.OpenFeatureDataset("work");

               double s = 0;

               IPolyline pPolyline = new PolylineClass();

               SolvePath(axMapControl1.Map, GetGeometricNetwork(pFtDataset, "TestGeometric"), "Weight", pPointC, 1000, ref pPolyline, ref s);

               IRgbColor pColor = new RgbColorClass();
               pColor.Red = 255;
               IElement pElement = new LineElementClass();
               ILineSymbol linesymbol = new SimpleLineSymbolClass();
               linesymbol.Color = pColor as IColor;
               linesymbol.Width = 100;

               pElement.Geometry = pPolyline;

               pGC.AddElement(pElement, 2);

               axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
           }
           if (pFlag == 4)
           {
               if(axMapControl1.Map.get_Layer(0)!=null)
               {
                   IRasterLayer pRasterLayer = axMapControl1.Map.get_Layer(0) as IRasterLayer;

                   IRasterSurface pRasterSurface = new RasterSurfaceClass();


                   pRasterSurface.PutRaster(pRasterLayer.Raster, 0);


                   ISurface pSurface = pRasterSurface as ISurface;

                  IPolyline pPolyline = axMapControl1.TrackLine() as  IPolyline;

                   IPoint pPoint =null ;

                    IPolyline pVPolyline =null;

                   IPolyline pInPolyline= null;

                   object pRef=0.13;

                   bool pBool =true;

                   double pZ1 = pSurface.GetElevation(pPolyline.FromPoint);

                   double pZ2= pSurface.GetElevation(pPolyline.ToPoint);



                   IPoint pPoint1 = new PointClass();

                   pPoint1.Z = pZ1;

                   pPoint1.X = pPolyline.FromPoint.X;

                   pPoint1.Y = pPolyline.FromPoint.Y;


                   IPoint pPoint2 = new PointClass();

                   pPoint2.Z = pZ2;

                   pPoint2.X = pPolyline.ToPoint.X;

                   pPoint2.Y = pPolyline.ToPoint.Y;


                   pSurface.GetLineOfSight(pPoint1, pPoint2, out pPoint, out pVPolyline,
                       out pInPolyline, out pBool, false, false, ref pRef);//大爷的，设置为true居然通不过bApplyCurvature和bApplyRefraction两项设置为true，surface必须定义成具有ZUnits的投影坐标

                   //This member should not be used in .NET. As a substitute, .NET developers must use IGeoDatabaseBridge2.GetLineOfSight.

                         //楼主，用IGeoDatabaseBridge2.GetLineOfSight.方法试试
                   if (pVPolyline != null)
                   {

                       IElement pLineElementV = new LineElementClass();

                       pLineElementV.Geometry = pVPolyline;

                       ILineSymbol pLinesymbolV = new SimpleLineSymbolClass();


                       pLinesymbolV.Width = 2;

                       IRgbColor pColorV = new RgbColorClass();

                       pColorV.Green =255;

                       pLinesymbolV.Color = pColorV;

                       ILineElement pLineV = pLineElementV as ILineElement;

                       pLineV.Symbol = pLinesymbolV;

                       axMapControl1.ActiveView.GraphicsContainer.AddElement(pLineElementV, 0);
                   }




                   if (pInPolyline != null)
                   {


                       IElement pLineElementIn = new LineElementClass();

                       pLineElementIn.Geometry = pInPolyline;

                       ILineSymbol pLinesymbolIn = new SimpleLineSymbolClass();


                       pLinesymbolIn.Width = 2;

                       IRgbColor pColorIn = new RgbColorClass();
                       pColorIn.Red = 255;

                       pLinesymbolIn.Color = pColorIn;
                       ILineElement pLineIn = pLineElementIn as ILineElement;

                       pLineIn.Symbol = pLinesymbolIn;



                       axMapControl1.ActiveView.GraphicsContainer.AddElement(pLineElementIn, 1);

                     
                   }

                   axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                   axMapControl1.TrackCancel.Cancel();
                   


               }
           }
                      
            
           
       }



       public ITinLayer GetTINLayer(string pPath)//打开TIN文件
       {
           ITinWorkspace pTinWorkspace;
           IWorkspace pWS;
           IWorkspaceFactory pWSFact = new TinWorkspaceFactoryClass();

           ITinLayer pTinLayer = new TinLayerClass();

           string pathToWorkspace = System.IO.Path.GetDirectoryName(pPath);
           string tinName = System.IO.Path.GetFileName(pPath);
           ITin pTin;

           pWS = pWSFact.OpenFromFile(pathToWorkspace, 0);
           pTinWorkspace = pWS as ITinWorkspace;

           if (pTinWorkspace.get_IsTin(tinName))
           {
               pTin = pTinWorkspace.OpenTin(tinName);

               pTinLayer.Dataset = pTin;

               pTinLayer.ClearRenderers();

               return pTinLayer;

           }
           else
           {
               MessageBox.Show("该目录不包含Tin文件");
               return null;

           }

       }
        /// <summary>
        /// 创建Tin
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="pField"></param>
        /// <param name="pPath"></param>
        /// <returns></returns>

       public  ITin   CreateTin(IFeatureClass pFeatureClass, IField pField,string pPath)
       {


           IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
           
           ITinEdit pTinEdit  = new TinClass();

           pTinEdit.InitNew(pGeoDataset.Extent);

            object pObj = Type.Missing;

            pTinEdit.AddFromFeatureClass(pFeatureClass, null, pField,null, esriTinSurfaceType.esriTinMassPoint, ref pObj);
           
        
            pTinEdit.SaveAs(pPath, ref pObj);
            pTinEdit.Refresh();

            return pTinEdit as ITin;
         

       }



       public IRaster DensityAnalyst(IFeatureClass pFeatureClass,string pFieldName, double pCellSize, double pRadius)
       {
           //辅助对象，设置密度分析时候的参数

           IFeatureClassDescriptor pFDescr = new FeatureClassDescriptorClass();
           pFDescr.Create(pFeatureClass, null, pFieldName);


           IDensityOp pDensityOp = new RasterDensityOpClass();

           //设置环境
           IRasterAnalysisEnvironment pEnv = pDensityOp as IRasterAnalysisEnvironment;

           object object_cellSize = (System.Object)pCellSize;

           pEnv.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ref object_cellSize);

          
           System.Double double_radio_dis = pRadius;
           object object_radio_dis = (System.Object)double_radio_dis;
           object Missing = Type.Missing;
           //核函数密度制图方法生成栅格数据
           IRaster pRaster = pDensityOp.KernelDensity(pFDescr as IGeoDataset, ref object_radio_dis, ref Missing) as IRaster;

           return pRaster;
         
       }
        /// <summary>
        /// 创建泰森多边形
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="pTin"></param>
      void  CreateVr(IFeatureClass pFeatureClass ,ITin pTin)
       {
           ITinNodeCollection pTinColl = pTin as ITinNodeCollection;


           pTinColl.ConvertToVoronoiRegions(pFeatureClass, null, null, "", "");


       }


       public ESRI.ArcGIS.Geodatabase.IFeatureCursor GetAllFeaturesFromPointSearchInGeoFeatureLayer(Double pSearchTolerance, IPoint pPoint, IFeatureClass pFeatureClass)
       {

           if (pSearchTolerance < 0 || pPoint == null || pFeatureClass == null)
           {
               return null;
           }


           ESRI.ArcGIS.Geometry.IEnvelope pEnvelope = pPoint.Envelope;
           pEnvelope.Expand(pSearchTolerance, pSearchTolerance, false);

           //ITopologicalOperator pTopo = pPoint as ITopologicalOperator;

           //IGeometry pGeo = pTopo.Buffer(pSearchTolerance);


           System.String pShapeFieldName = pFeatureClass.ShapeFieldName;
 
           ESRI.ArcGIS.Geodatabase.ISpatialFilter pSpatialFilter = new ESRI.ArcGIS.Geodatabase.SpatialFilterClass();
           pSpatialFilter.Geometry = pEnvelope;
           pSpatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelEnvelopeIntersects;
           pSpatialFilter.GeometryField = pShapeFieldName;

   
           ESRI.ArcGIS.Geodatabase.IFeatureCursor pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);

           return pFeatureCursor;
       }


        /// <summary>
        /// 要插值的要素类，插值的字段名，阈值，栅格大小，指数
        /// </summary>
        /// <param name="_pFeatureClass"></param>
        /// <param name="_pFieldName"></param>
        /// <param name="_pDistance"></param>
        /// <param name="_pCell"></param>
        /// <param name="_pPower"></param>
        /// <returns></returns>

       public IGeoDataset IDW(IFeatureClass _pFeatureClass, string _pFieldName, double _pDistance, double _pCell, int _pPower)
       {
           IGeoDataset Geo = _pFeatureClass as IGeoDataset;

           object pExtent = Geo.Extent;

           object o = Type.Missing;

           IFeatureClassDescriptor pFeatureClassDes = new FeatureClassDescriptorClass();

           pFeatureClassDes.Create(_pFeatureClass, null, _pFieldName);


           IInterpolationOp pInterOp = new RasterInterpolationOpClass();

           IRasterAnalysisEnvironment pRasterAEnv = pInterOp as IRasterAnalysisEnvironment;


          // pRasterAEnv.Mask = Geo;


           pRasterAEnv.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, ref pExtent, ref o);


           object pCellSize = _pCell;//可以根据不同的点图层进行设置


           pRasterAEnv.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ref pCellSize);


           IRasterRadius pRasterrad = new RasterRadiusClass();

           object obj = Type.Missing;

          // pRasterrad.SetFixed(_pDistance, ref obj);

           pRasterrad.SetVariable(12, ref obj);

           object pBar = Type.Missing;

           IGeoDataset pGeoIDW = pInterOp.IDW(pFeatureClassDes as IGeoDataset, _pPower, pRasterrad, ref pBar);

           return pGeoIDW;



       }

     
        /// <summary>
        /// 获取shpfile文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="LayerName"></param>
        /// <returns></returns>

       private IFeatureClass GetFeatureClass(string FilePath, string LayerName)
       {

           IWorkspaceFactory pWks = new ShapefileWorkspaceFactoryClass();

           IFeatureWorkspace pFwk = pWks.OpenFromFile(FilePath, 0) as IFeatureWorkspace;

           IFeatureClass pRtClass = pFwk.OpenFeatureClass(LayerName);

           return pRtClass;
       }

    

       public IFeatureClass  Intsect(IFeatureClass _pFtClass,IFeatureClass _pFtOverlay,string _FilePath,string _pFileName)
       {

           IFeatureClassName pOutPut = new FeatureClassNameClass();

           pOutPut.ShapeType = _pFtClass.ShapeType;

           pOutPut.ShapeFieldName = _pFtClass.ShapeFieldName;

           pOutPut.FeatureType = esriFeatureType.esriFTSimple;

           //set output location and feature class name

          IWorkspaceName pWsN = new WorkspaceNameClass();

          pWsN.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory";

          pWsN.PathName = _FilePath;

           //也可以用这种方法,IName 和IDataset的用法

          /* IWorkspaceFactory pWsFc = new ShapefileWorkspaceFactoryClass();

             IWorkspace pWs = pWsFc.OpenFromFile(_FilePath, 0);

             IDataset pDataset = pWs as IDataset;

             IWorkspaceName pWsN = pDataset.FullName as IWorkspaceName;
          */



           IDatasetName pDatasetName = pOutPut as IDatasetName;


           pDatasetName.Name = _pFileName;

           pDatasetName.WorkspaceName =pWsN;

           IBasicGeoprocessor pBasicGeo = new BasicGeoprocessorClass();

           IFeatureClass pFeatureClass = pBasicGeo.Intersect(_pFtClass as ITable , false, _pFtOverlay as ITable , false, 0.1, pOutPut);

           return pFeatureClass;


       }

        /// <summary>
        /// 打开个人数据库
        /// </summary>
        /// <param name="_pGDBName"></param>
        /// <returns></returns>
       public IWorkspace GetMDBWorkspace(String _pGDBName)
       {
           IWorkspaceFactory pWsFac = new AccessWorkspaceFactoryClass();

           IWorkspace pWs = pWsFac.OpenFromFile(_pGDBName,0);

           return pWs;
       }
       public void CreateGeometricNetwork(IWorkspace _pWorkspace, IFeatureDatasetName
   _pFeatureDatasetName,String _pGeometricName)
       {
           
           INetworkLoader2 pNetworkLoader = new NetworkLoaderClass();

           // 网络的名称
           pNetworkLoader.NetworkName = _pGeometricName;

           // 网络的类型
           pNetworkLoader.NetworkType = esriNetworkType.esriNTUtilityNetwork;

           // Set the containing feature dataset.
           pNetworkLoader.FeatureDatasetName = (IDatasetName)_pFeatureDatasetName;


           // 检查要建立几何网络的数据，每一个要素只能参与一个网络
           if (pNetworkLoader.CanUseFeatureClass("PrimaryLine") ==
               esriNetworkLoaderFeatureClassCheck.esriNLFCCValid)
           {
               pNetworkLoader.AddFeatureClass("PrimaryLine",
                   esriFeatureType.esriFTComplexEdge, null, false);
           }

           if (pNetworkLoader.CanUseFeatureClass("Feeder") ==
               esriNetworkLoaderFeatureClassCheck.esriNLFCCValid)
           {
               pNetworkLoader.AddFeatureClass("Feeder", esriFeatureType.esriFTSimpleJunction,
                   null, false);
           }

         // 我的数据中没有enable字段，所以，用了false，如果用true的话，就要进行相关的设置

          INetworkLoaderProps pNetworkLoaderProps = (INetworkLoaderProps)pNetworkLoader;
          
           pNetworkLoader.PreserveEnabledValues = false;


           // Set the ancillary role field for the Feeder class.
           String defaultAncillaryRoleFieldName =
               pNetworkLoaderProps.DefaultAncillaryRoleField;
           esriNetworkLoaderFieldCheck ancillaryRoleFieldCheck =
               pNetworkLoader.CheckAncillaryRoleField("Feeder",
               defaultAncillaryRoleFieldName);
           switch (ancillaryRoleFieldCheck)
           {
               case esriNetworkLoaderFieldCheck.esriNLFCValid:
               case esriNetworkLoaderFieldCheck.esriNLFCNotFound:
                   pNetworkLoader.PutAncillaryRole("Feeder",
                       esriNetworkClassAncillaryRole.esriNCARSourceSink,
                       defaultAncillaryRoleFieldName);
                   break;
               default:
                   Console.WriteLine(
                       "The field {0} could not be used as an ancillary role field.",
                       defaultAncillaryRoleFieldName);
                   break;
           }

          

           pNetworkLoader.SnapTolerance = 0.02;

           // 给几何网络添加权重
           pNetworkLoader.AddWeight("Weight", esriWeightType.esriWTDouble, 0);



           // 将权重和PrimaryLine数据中的SHAPE_Length字段关联
           pNetworkLoader.AddWeightAssociation("Weight", "PrimaryLine", "SHAPE_Length");

           // 构建网络
           pNetworkLoader.LoadNetwork();

          
       }
         /// <summary>
         /// 获得需要的几何网络
         /// </summary>
         /// <param name="_pFeatureDataset"></param>
         /// <param name="_pGeometricName"></param>
         /// <returns></returns>
       IGeometricNetwork GetGeometricNetwork(IFeatureDataset _pFeatureDataset,string _pGeometricName)

       {
           INetworkCollection pNetworkCollection = _pFeatureDataset as INetworkCollection;


           return pNetworkCollection.get_GeometricNetworkByName(_pGeometricName);
        
       }
        /// <summary>
        /// 将几何网络添加到地图上
        /// </summary>
        /// <param name="_pMap"></param>
        /// <param name="_pGeometricNetwork"></param>
        void LoadGeometric(IMap _pMap,IGeometricNetwork _pGeometricNetwork)
    {

        IFeatureClassContainer pFtContainer = _pGeometricNetwork as IFeatureClassContainer;

        int pCount = pFtContainer.ClassCount;

        for (int i = 0; i < pCount; i++)
        {
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();

            pFeatureLayer.FeatureClass = pFtContainer.get_Class(i);

            _pMap.AddLayer(pFeatureLayer as ILayer);
        }

    }
        /// <summary>
        /// 求解最短路径
        /// </summary>
        /// <param name="_pMap"></param>
        /// <param name="_pGeometricNetwork"></param>
        /// <param name="_pWeightName"></param>
        /// <param name="_pPoints"></param>
        /// <param name="_pDist"></param>
        /// <param name="_pPolyline"></param>
        /// <param name="_pPathCost"></param>
        public void SolvePath(IMap _pMap, IGeometricNetwork _pGeometricNetwork, string _pWeightName, IPointCollection _pPoints, double _pDist, ref IPolyline _pPolyline, ref double _pPathCost)
        {
            try
            { // 这4个参数其实就是一个定位Element的指标
                int intEdgeUserClassID;

                int intEdgeUserID;

                int intEdgeUserSubID;

                int intEdgeID;

                IPoint pFoundEdgePoint;

                double dblEdgePercent;

                ITraceFlowSolverGEN pTraceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;

                INetSolver pNetSolver = pTraceFlowSolver as INetSolver;

                //操作是针对逻辑网络的,INetwork是逻辑网络

                INetwork pNetwork = _pGeometricNetwork.Network;

                pNetSolver.SourceNetwork = pNetwork;

                INetElements pNetElements = pNetwork as INetElements;

                int pCount = _pPoints.PointCount;
                //定义一个边线旗数组
                IEdgeFlag[] pEdgeFlagList = new EdgeFlagClass[pCount];

                IPointToEID pPointToEID = new PointToEIDClass();

                pPointToEID.SourceMap = _pMap;

                pPointToEID.GeometricNetwork = _pGeometricNetwork;

                pPointToEID.SnapTolerance = _pDist;
                
                for (int i = 0; i < pCount; i++)
                {
                    INetFlag pNetFlag = new EdgeFlagClass() as INetFlag;

                    IPoint pEdgePoint = _pPoints.get_Point(i);
                    //查找输入点的最近的边线
                    pPointToEID.GetNearestEdge(pEdgePoint, out intEdgeID, out pFoundEdgePoint, out dblEdgePercent);

                    pNetElements.QueryIDs(intEdgeID, esriElementType.esriETEdge, out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);


                    pNetFlag.UserClassID = intEdgeUserClassID;

                    pNetFlag.UserID = intEdgeUserID;

                    pNetFlag.UserSubID = intEdgeUserSubID;

                    IEdgeFlag pTemp = (IEdgeFlag)(pNetFlag as IEdgeFlag);
                    pEdgeFlagList[i] = pTemp;
                }
                pTraceFlowSolver.PutEdgeOrigins(ref pEdgeFlagList);

                INetSchema pNetSchema = pNetwork as INetSchema;

                INetWeight pNetWeight = pNetSchema.get_WeightByName(_pWeightName);

                INetSolverWeightsGEN pNetSolverWeights = pTraceFlowSolver as INetSolverWeightsGEN;

                pNetSolverWeights.FromToEdgeWeight = pNetWeight;//开始边线的权重

                pNetSolverWeights.ToFromEdgeWeight = pNetWeight;//终止边线的权重

                object[] pRes = new object[pCount - 1];

                //通过FindPath得到边线和交汇点的集合
                IEnumNetEID  pEnumNetEID_Junctions;

                IEnumNetEID pEnumNetEID_Edges;

                pTraceFlowSolver.FindPath(esriFlowMethod.esriFMConnected,
                 esriShortestPathObjFn.esriSPObjFnMinSum,
                 out pEnumNetEID_Junctions, out pEnumNetEID_Edges, pCount - 1, ref pRes);
                //计算元素成本
                _pPathCost = 0;
                for (int i = 0; i < pRes.Length; i++)
                {
                    double m_Va = (double)pRes[i];

                    _pPathCost = _pPathCost + m_Va;
                }

                IGeometryCollection pNewGeometryColl = _pPolyline as IGeometryCollection;//QI

                ISpatialReference pSpatialReference = _pMap.SpatialReference;

                IEIDHelper pEIDHelper = new EIDHelperClass();

                pEIDHelper.GeometricNetwork = _pGeometricNetwork;

                pEIDHelper.OutputSpatialReference = pSpatialReference;

                pEIDHelper.ReturnGeometries = true;

                IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(pEnumNetEID_Edges);

                int Count = pEnumEIDInfo.Count;

                pEnumEIDInfo.Reset();

                for (int i = 0; i < Count; i++)
                {
                    IEIDInfo pEIDInfo = pEnumEIDInfo.Next();

                    IGeometry pGeometry = pEIDInfo.Geometry;

                    pNewGeometryColl.AddGeometryCollection(pGeometry as IGeometryCollection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

      

        int pFlag = 0;//这个参数用来标记点击对象的功能
    
        /// <summary>
        /// 获取网络数据集
        /// </summary>
        /// <param name="_pFeatureWs"></param>
        /// <param name="_pDatasetName"></param>
        /// <param name="_pNetDatasetName"></param>
        /// <returns></returns>
        INetworkDataset GetNetDataset(IFeatureWorkspace _pFeatureWs, string _pDatasetName,string  _pNetDatasetName)
        {

            ESRI.ArcGIS.Geodatabase.IDatasetContainer3 pDatasetContainer = null;

            ESRI.ArcGIS.Geodatabase.IFeatureDataset pFeatureDataset = _pFeatureWs.OpenFeatureDataset(_pDatasetName);
            ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = pFeatureDataset as ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtensionContainer; // Dynamic Cast
            ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtension pFeatureDatasetExtension = pFeatureDatasetExtensionContainer.FindExtension(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTNetworkDataset);
            pDatasetContainer = pFeatureDatasetExtension as ESRI.ArcGIS.Geodatabase.IDatasetContainer3; // Dynamic Cast

            ESRI.ArcGIS.Geodatabase.IDataset pNetWorkDataset = pDatasetContainer.get_DatasetByName(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTNetworkDataset, _pNetDatasetName);

            return pNetWorkDataset as ESRI.ArcGIS.Geodatabase.INetworkDataset; // Dynamic Cast
        }

        /// <summary>
        /// 加载NetworkDataset到Map中
        /// </summary>
        /// <param name="_pMap"></param>
        /// <param name="_pNetworkDataset"></param>
        void loadNet(IMap _pMap,INetworkDataset _pNetworkDataset)
       {
           INetworkLayer pNetLayer = new NetworkLayerClass();

           pNetLayer.NetworkDataset = _pNetworkDataset;

           _pMap.AddLayer(pNetLayer as ILayer);
       }
        /// <summary>
        /// 获取网络分析上下文，这个接口是网络分析中很重要的一个
        /// </summary>
        /// <param name="_pNaSolver"></param>
        /// <param name="_pNetworkDataset"></param>
        /// <returns></returns>
        public INAContext GetSolverContext(INASolver _pNaSolver, INetworkDataset _pNetworkDataset)
        {
            //Get the Data Element

            IDatasetComponent pDataComponent = _pNetworkDataset as IDatasetComponent;

            IDEDataset pDeDataset = pDataComponent.DataElement;     
      
            INAContextEdit pContextEdit = _pNaSolver.CreateContext(pDeDataset as IDENetworkDataset, _pNaSolver.Name) as INAContextEdit;

            //Prepare the context for analysis based upon the current network dataset schema.
            pContextEdit.Bind(_pNetworkDataset, new GPMessagesClass());
            return pContextEdit as INAContext;
        }
        /// <summary>
        /// 获取NALayer
        /// </summary>
        /// <param name="_pNaSover"></param>
        /// <param name="_pNaContext"></param>
        /// <returns></returns>

        INALayer GetNaLayer(INASolver _pNaSover,INAContext _pNaContext)
        {
            return _pNaSover.CreateLayer(_pNaContext);
        }
        /// <summary>
        /// _pFtClass参数为Stops的要素类，_pPointC是用鼠标点的点生成的点的集合，最后一个参数是捕捉距离
        /// </summary>
        /// <param name="_pNaContext"></param>
        /// <param name="_pFtClass"></param>
        /// <param name="_pPointC"></param>
        /// <param name="_pDist"></param>
        void NASolve(INAContext _pNaContext,IFeatureClass _pFtClass, IPointCollection _pPointC, double _pDist)
        {
            INALocator pNAlocator = _pNaContext.Locator;
            for (int i = 0; i < _pPointC.PointCount; i++)
            {
                IFeature pFt = _pFtClass.CreateFeature();

                IRowSubtypes pRowSubtypes = pFt as IRowSubtypes;

                pRowSubtypes.InitDefaultValues();

                pFt.Shape = _pPointC.get_Point(i) as IGeometry;

                IPoint pPoint = null; 

                 INALocation pNalocation = null;

                pNAlocator.QueryLocationByPoint(_pPointC .get_Point(i),ref pNalocation,ref pPoint,ref _pDist);

                INALocationObject pNAobject = pFt as INALocationObject;

                pNAobject.NALocation = pNalocation;

                int pNameFieldIndex = _pFtClass.FindField("Name");

                pFt.set_Value(pNameFieldIndex, pPoint.X.ToString() + "," + pPoint.Y.ToString());

                int pStatusFieldIndex = _pFtClass.FindField("Status");

                pFt.set_Value(pStatusFieldIndex, esriNAObjectStatus.esriNAObjectStatusOK);

                int pSequenceFieldIndex = _pFtClass.FindField("Sequence");

                pFt.set_Value(_pFtClass.FindField("Sequence"), ((ITable)_pFtClass).RowCount(null));

                pFt.Store();

            }
        }

        INAContext pNaContext;

        INALayer pNALayer;


        private void button11_Click(object sender, EventArgs e)
        {
           



           /* IPolyline ps = new PolylineClass();

            IRgbColor color = new RgbColorClass();
            color.Red = 255;
            IElement element = new LineElementClass();
            ILineSymbol linesymbol = new SimpleLineSymbolClass();
            linesymbol.Color = color as IColor;
            linesymbol.Width = 100;

            element.Geometry = ps;

            pGC.AddElement(element, 2);

            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            */
        }
        /// <summary>
        /// 个人数据库的路径,要素数据集的路径,建立网络的名称，参与网络的要素类
        /// </summary>
        /// <param name="_pWsName"></param>
        /// <param name="_pDatasetName"></param>
        /// <param name="_pNetName"></param>
        /// <param name="_pFtName"></param>

        void CreateNetworkDataset(string _pWsName, string _pDatasetName,string _pNetName, string _pFtName)
        {
            IDENetworkDataset2 pDENetworkDataset = new DENetworkDatasetClass();

            pDENetworkDataset.Buildable = true;

            IWorkspace pWs = GetMDBWorkspace(_pWsName);

            IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;

            IFeatureDataset pFtDataset = pFtWs.OpenFeatureDataset(_pDatasetName);

            // 定义空间参考,负责会出错
            IDEGeoDataset pDEGeoDataset = (IDEGeoDataset)pDENetworkDataset;

             IGeoDataset pGeoDataset = pFtDataset as IGeoDataset;

            pDEGeoDataset.Extent = pGeoDataset.Extent;

             pDEGeoDataset.SpatialReference = pGeoDataset.SpatialReference;



            // 网络数据集的名称
            IDataElement pDataElement = (IDataElement)pDENetworkDataset;

            pDataElement.Name = _pNetName;



            // 参加建立网络数据集的要素类
            INetworkSource pEdgeNetworkSource = new EdgeFeatureSourceClass();
            pEdgeNetworkSource.Name = _pFtName;

            
            pEdgeNetworkSource.ElementType = esriNetworkElementType.esriNETEdge;

            // 要素类的连通性
            IEdgeFeatureSource pEdgeFeatureSource = (IEdgeFeatureSource)pEdgeNetworkSource;
            pEdgeFeatureSource.UsesSubtypes = false;

           

            pEdgeFeatureSource.ClassConnectivityGroup = 1;

            pEdgeFeatureSource.ClassConnectivityPolicy =esriNetworkEdgeConnectivityPolicy.esriNECPEndVertex;


           
            //不用转弯数据
            pDENetworkDataset.SupportsTurns = false;

          
            IArray pSourceArray = new ArrayClass();
            pSourceArray.Add(pEdgeNetworkSource);


            pDENetworkDataset.Sources = pSourceArray;

            //网络数据集的属性设置

            IArray pAttributeArray = new ArrayClass();

            // Initialize variables reused when creating attributes:
            IEvaluatedNetworkAttribute pEvalNetAttr;
            INetworkAttribute2 pNetAttr2;
            INetworkFieldEvaluator pNetFieldEval;
            INetworkConstantEvaluator pNetConstEval;


      
            pEvalNetAttr = new EvaluatedNetworkAttributeClass();
            pNetAttr2 = (INetworkAttribute2)pEvalNetAttr;
            pNetAttr2.Name ="Meters";
            pNetAttr2.UsageType = esriNetworkAttributeUsageType.esriNAUTCost;
            pNetAttr2.DataType = esriNetworkAttributeDataType.esriNADTDouble;
            pNetAttr2.Units = esriNetworkAttributeUnits.esriNAUMeters;
            pNetAttr2.UseByDefault = false;


            pNetFieldEval = new NetworkFieldEvaluatorClass();
            pNetFieldEval.SetExpression("[METERS]", "");
            //方向设置
            pEvalNetAttr.set_Evaluator(pEdgeNetworkSource,
                esriNetworkEdgeDirection.esriNEDAlongDigitized, (INetworkEvaluator)pNetFieldEval);
            pEvalNetAttr.set_Evaluator(pEdgeNetworkSource,
                esriNetworkEdgeDirection.esriNEDAgainstDigitized, (INetworkEvaluator)
                pNetFieldEval);

            pNetConstEval = new NetworkConstantEvaluatorClass();
            pNetConstEval.ConstantValue = 0;
            pEvalNetAttr.set_DefaultEvaluator(esriNetworkElementType.esriNETEdge,
                (INetworkEvaluator)pNetConstEval);
            pEvalNetAttr.set_DefaultEvaluator(esriNetworkElementType.esriNETJunction,
                (INetworkEvaluator)pNetConstEval);
            pEvalNetAttr.set_DefaultEvaluator(esriNetworkElementType.esriNETTurn,
                (INetworkEvaluator)pNetConstEval);

            // 一个网络数据集可以有多个属性，我只添加了一个
            pAttributeArray.Add(pEvalNetAttr);

            pDENetworkDataset.Attributes = pAttributeArray;
          

            // 创建网络数据集，注意在创建几何网络的时候会锁定相应的要素类，因此不要用ArcMap或者catalog等打开参相应的数据

           



          INetworkDataset pNetworkDataset = Create(pFtDataset, pDENetworkDataset);

             
            //建立网络

            INetworkBuild pNetworkBuild = (INetworkBuild)pNetworkDataset;

            
            pNetworkBuild.BuildNetwork(pGeoDataset.Extent);

            
        }
        /// <summary>
        /// 创建无向网络
        /// </summary>
        /// <param name="_pFeatureDataset"></param>
        /// <param name="_pDENetDataset"></param>
        /// <returns></returns>

        public INetworkDataset Create(IFeatureDataset _pFeatureDataset, IDENetworkDataset2  _pDENetDataset)
        {
            IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = (IFeatureDatasetExtensionContainer) _pFeatureDataset;

            IFeatureDatasetExtension pFeatureDatasetExtension = pFeatureDatasetExtensionContainer.FindExtension
                (esriDatasetType.esriDTNetworkDataset);

            IDatasetContainer2 pDatasetContainer2 = (IDatasetContainer2)pFeatureDatasetExtension;

            IDEDataset pDENetDataset = (IDEDataset)_pDENetDataset;

            INetworkDataset pNetworkDataset = (INetworkDataset)pDatasetContainer2.CreateDataset
                (pDENetDataset);


            return pNetworkDataset;
        }



  
        /// <summary>
        /// 源工作空间，源工作空间名称，要转换的要素类名称，目标工作空间，目标工作空间名称，目标要素类名称
        /// </summary>
        /// <param name="_pSWorkspaceFactory"></param>
        /// <param name="_pSWs"></param>
        /// <param name="_pSName"></param>
        /// <param name="_pTWorkspaceFactory"></param>
        /// <param name="_pTWs"></param>
        /// <param name="_pTName"></param>
        public void ConvertFeatureClass(IWorkspaceFactory _pSWorkspaceFactory,String _pSWs,string _pSName, IWorkspaceFactory _pTWorkspaceFactory ,String _pTWs,string _pTName )
         {
         
            
            IWorkspace pSWorkspace = _pSWorkspaceFactory.OpenFromFile(_pSWs, 0);

            IWorkspace pTWorkspace = _pTWorkspaceFactory.OpenFromFile(_pTWs, 0);


            IFeatureWorkspace pFtWs = pSWorkspace as IFeatureWorkspace;

            IFeatureClass pSourceFeatureClass = pFtWs.OpenFeatureClass(_pSName);

            IDataset pSDataset = pSourceFeatureClass as IDataset;

            IFeatureClassName pSourceFeatureClassName = pSDataset.FullName as IFeatureClassName;
         
            IDataset pTDataset = (IDataset)pTWorkspace;
           
            IName pTDatasetName = pTDataset.FullName;

            IWorkspaceName pTargetWorkspaceName = (IWorkspaceName)pTDatasetName;    

            
            IFeatureClassName pTargetFeatureClassName = new FeatureClassNameClass();

            IDatasetName pTargetDatasetName = (IDatasetName)pTargetFeatureClassName;

            // 验证要素类的名称是否合法，但是并没有对这个名称是否存在做检查
            string pTableName = null;

            IFieldChecker pNameChecker = new FieldCheckerClass();

            pNameChecker.ValidateWorkspace = pTWorkspace;

            int pFlag = pNameChecker.ValidateTableName(_pTName, out pTableName);

            pTargetDatasetName.Name = pTableName;

            pTargetDatasetName.WorkspaceName = pTargetWorkspaceName;

            
            // 创建字段检查对象
            IFieldChecker pFieldChecker = new FieldCheckerClass();

            IFields sourceFields = pSourceFeatureClass.Fields;
            IFields pTargetFields = null;

            IEnumFieldError pEnumFieldError = null;
            
            pFieldChecker.InputWorkspace = pSWorkspace;

            pFieldChecker.ValidateWorkspace = pTWorkspace;

            // 验证字段
            pFieldChecker.Validate(sourceFields, out pEnumFieldError, out pTargetFields);
            if (pEnumFieldError != null)
            {
                // 报错提示
                Console.WriteLine("Errors were encountered during field validation.");
            }

 
            String pShapeFieldName = pSourceFeatureClass.ShapeFieldName; 
   
            int pFieldIndex = pSourceFeatureClass.FindField(pShapeFieldName);

            IField pShapeField = sourceFields.get_Field(pFieldIndex);
  
            IGeometryDef pTargetGeometryDef = pShapeField.GeometryDef;


            // 创建要素转换对象
            IFeatureDataConverter pFDConverter = new FeatureDataConverterClass();

            IEnumInvalidObject pEnumInvalidObject = pFDConverter.ConvertFeatureClass(pSourceFeatureClassName, null, null, pTargetFeatureClassName,
                pTargetGeometryDef, pTargetFields, "", 1000, 0);

            // 检查是否有错误
            IInvalidObjectInfo pInvalidInfo = null;

            pEnumInvalidObject.Reset();

            while ((pInvalidInfo = pEnumInvalidObject.Next()) != null)
            {
                // 报错提示.
                Console.WriteLine("Errors occurred for the following feature: {0}",
                    pInvalidInfo.InvalidObjectID);
            }
         }

        /// <summary>
        /// 打开文件数据库
        /// </summary>
        /// <param name="_pGDBName"></param>
        /// <returns></returns>
        public IWorkspace GetFGDBWorkspace(String _pGDBName)
        {
            IWorkspaceFactory pWsFac = new FileGDBWorkspaceFactoryClass();

            IWorkspace pWs = pWsFac.OpenFromFile(_pGDBName, 0);

            return pWs;
        }


        public void CreateRoutesUsing2Fields(string pAccessWS, string sLineFC, string sOutRouteFC, string sWhereClause, string sRouteIDField, string sFromMeasureField, string sToMeasureField)
        {
            try
            {
               
                IWorkspaceFactory wsf = new AccessWorkspaceFactoryClass();
                IWorkspace ws = wsf.OpenFromFile(pAccessWS, 0);
                IFeatureWorkspace fws = (IFeatureWorkspace)ws;
                IFeatureClass lineFC = fws.OpenFeatureClass(sLineFC);
                // Create an output feature class name object. We'll write to a stand alone feature class in the             
                // the same workspace as the inputs         
                IDataset ds = (IDataset)ws;
                IWorkspaceName outWSN = (IWorkspaceName)ds.FullName;
                IFeatureClassName outFCN = new FeatureClassNameClass();
                IDatasetName outDSN = (IDatasetName)outFCN;
                outDSN.WorkspaceName = outWSN;
                outDSN.Name = sOutRouteFC;
                //This name should not already exist               
                // Create a geometry definition for the new feature class. For the most part, we will copy the geometry              
                // definition from the input lines. We'll explicitly set the M Domain, however. You should always set an               
                // M Domain that is appropriate to your data. What is below is just a sample.            
                IFields flds = lineFC.Fields;
                Int32 i = flds.FindField(lineFC.ShapeFieldName);
                IField fld = flds.get_Field(i);
                IClone GDefclone = (IClone)fld.GeometryDef;
                IGeometryDef gDef = (IGeometryDef)GDefclone.Clone();
                ISpatialReference2 srRef = (ISpatialReference2)gDef.SpatialReference;
                srRef.SetMFalseOriginAndUnits(-1000, 10000);
                // Create a selection set to limit the number of lines that will be used to create routes   
                IQueryFilter qFilt = new QueryFilterClass();
                qFilt.WhereClause = sWhereClause;
                ISelectionSet2 selSet = (ISelectionSet2)lineFC.Select(qFilt, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, ws);
                // Create a new RouteMeasureCreator object. Note that below, we use the selection set and not the             
                // InputFeatureClass property              
                IRouteMeasureCreator routeCreator = new RouteMeasureCreatorClass();
                routeCreator.InputFeatureSelection = selSet;
                routeCreator.InputRouteIDFieldName = sRouteIDField;

                IEnumBSTR errors = routeCreator.CreateUsing2Fields(sFromMeasureField, sToMeasureField, outFCN, gDef, "", null);
                // The results of running CreatingUsing2Fields returns IEnumBSTR, which is a container              
                // for a list of error strings indicating why certain lines could not be used to create routes.            
                string sError = errors.Next();
                do
                {
                    System.Windows.Forms.MessageBox.Show(sError);
                    sError = errors.Next();
                } while (sError.Length != 0);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
       /// <summary>
       /// 线性参考代码
       /// </summary>
       /// <param name="_pRouteFC"></param>
       /// <param name="_pPKName"></param>
       /// <param name="_pID"></param>
       /// <param name="_pFrom"></param>
       /// <param name="_pTo"></param>
       /// <returns></returns>

        IPolyline FindRoutByMeasure(IFeatureClass _pRouteFC, string _pPKName,object _pID, double _pFrom, double _pTo)
        {
            IDataset pDataset = (IDataset)_pRouteFC; 
            IName pName = pDataset.FullName;
            IRouteLocatorName pRouteLocatorName = new RouteMeasureLocatorNameClass();
            pRouteLocatorName.RouteFeatureClassName = pName;
            pRouteLocatorName.RouteIDFieldName = _pPKName;
            pRouteLocatorName.RouteMeasureUnit = esriUnits.esriFeet;
            pName = (IName)pRouteLocatorName;
            IRouteLocator2 pRouteLocator = (IRouteLocator2)pName.Open();


            IRouteLocation pRouteLoc = new RouteMeasureLineLocationClass();

            pRouteLoc.MeasureUnit = esriUnits.esriFeet;
            pRouteLoc.RouteID = _pID;
            IRouteMeasureLineLocation rMLineLoc = (IRouteMeasureLineLocation)pRouteLoc;
            rMLineLoc.FromMeasure = _pFrom;
            rMLineLoc.ToMeasure = _pTo;

            IGeometry pGeo = null;

            esriLocatingError locError;
            pRouteLocator.Locate(pRouteLoc , out pGeo, out locError);

            return pGeo as IPolyline;


        }
        /// <summary>
        /// 动态分段，作者：刘宇
        /// </summary>
        /// <param name="_pRouteFC"></param>
        /// <param name="_pPKName"></param>
        /// <param name="_pEventTable"></param>
        /// <param name="_pFKName"></param>
        /// <param name="_pFrom"></param>
        /// <param name="_pTo"></param>
        /// <returns></returns>

        IFeatureClass EventTable2FeatureClass(IFeatureClass _pRouteFC, string _pPKName, ITable _pEventTable, string _pFKName, string _pFrom, string _pTo)
        {
            IDataset pDataset = (IDataset)_pRouteFC; 

            IName pName = pDataset.FullName;

            IRouteLocatorName pRouteLocatorName = new RouteMeasureLocatorNameClass();

            pRouteLocatorName.RouteFeatureClassName = pName;

            pRouteLocatorName.RouteIDFieldName = _pPKName;

            pRouteLocatorName.RouteMeasureUnit = esriUnits.esriFeet;
            pName = (IName)pRouteLocatorName;

            IRouteEventProperties2 pRouteProp = new RouteMeasureLinePropertiesClass();

            pRouteProp.AddErrorField = true;
            pRouteProp.EventMeasureUnit = esriUnits.esriFeet;
            pRouteProp.EventRouteIDFieldName = _pFKName;

            IRouteMeasureLineProperties rMLineProp = (IRouteMeasureLineProperties)pRouteProp;

            rMLineProp.FromMeasureFieldName = _pFrom;
            rMLineProp.ToMeasureFieldName = _pTo;


            IDataset pDs = (IDataset)_pEventTable;
            IName pNTableName = pDs.FullName;
            IRouteEventSourceName pRouteEventSourceName = new RouteEventSourceNameClass();
            pRouteEventSourceName.EventTableName = pNTableName;
            pRouteEventSourceName.EventProperties = (IRouteEventProperties)pRouteProp;
            pRouteEventSourceName.RouteLocatorName = pRouteLocatorName;

            pName = (IName)pRouteEventSourceName;

            IFeatureClass pFeatureClass = (IFeatureClass)pName.Open();

            return pFeatureClass;

        }
        /// <summary>
        /// 模拟Addxy
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="pSpatialReference"></param>
        /// <returns></returns>

        public IFeatureClass CreateXYEventSource(ITable pTable, ISpatialReference pSpatialReference)
        { 
            IXYEvent2FieldsProperties pEvent2FieldsProperties = new XYEvent2FieldsPropertiesClass(); 
            pEvent2FieldsProperties.XFieldName = "X";
            pEvent2FieldsProperties.YFieldName = "Y"; 
          
            IDataset pSourceDataset = (IDataset)pTable;
            IName sourceName = pSourceDataset.FullName;
       
            IXYEventSourceName pEventSourceName = new XYEventSourceNameClass();
            pEventSourceName.EventProperties = pEvent2FieldsProperties; pEventSourceName.EventTableName = sourceName; pEventSourceName.SpatialReference = pSpatialReference;
            
            IName pName = (IName)pEventSourceName;
            IXYEventSource pEventSource = (IXYEventSource)pName.Open();
           
            IFeatureClass pFeatureClass = (IFeatureClass)pEventSource; return pFeatureClass;
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {

            if (axMapControl1.LayerCount > 0)
            {
                axMapControl2.Map = new MapClass();

                for (int i = 0; i <= axMapControl1.Map.LayerCount - 1; i++)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.Extent;

                axMapControl2.Refresh();


            }
        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            // 得到新范围

            IEnvelope pEnvelope = (IEnvelope)e.newEnvelope;


            IGraphicsContainer pGraphicsContainer = axMapControl2.Map as IGraphicsContainer;

            IActiveView pActiveView = pGraphicsContainer as IActiveView;

            //在绘制前，清除axMapControl2中的任何图形元素

            pGraphicsContainer.DeleteAllElements();


            IRectangleElement pRectangleEle = new RectangleElementClass();

            IElement pElement = pRectangleEle as IElement;

            pElement.Geometry = pEnvelope;


            //设置鹰眼图中的红线框

            IRgbColor pColor = new RgbColorClass();

            pColor.Red = 255;

            pColor.Green = 0;

            pColor.Blue = 0;

            pColor.Transparency = 255;

            //产生一个线符号对象

            ILineSymbol pOutline = new SimpleLineSymbolClass();

            pOutline.Width = 3;

            pOutline.Color = pColor;


            //设置颜色属性

            pColor = new RgbColorClass();

            pColor.Red = 255;

            pColor.Green = 0;

            pColor.Blue = 0;

            pColor.Transparency = 0;

            //设置填充符号的属性

            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();

            pFillSymbol.Color = pColor;

            pFillSymbol.Outline = pOutline;


            IFillShapeElement pFillShapeEle = pElement as IFillShapeElement;

            pFillShapeEle.Symbol = pFillSymbol;

            pGraphicsContainer.AddElement((IElement)pFillShapeEle, 0);

            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (axMapControl2.Map.LayerCount > 0)
            {
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();

                    pPoint.PutCoords(e.mapX, e.mapY);

                    axMapControl1.CenterAt(pPoint);

                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                else if (e.button == 2)
                {
                    IEnvelope pEnv = axMapControl2.TrackRectangle();

                    axMapControl1.Extent = pEnv;

                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);




                }
            }
        }

        public IWorkspace GetSDEWorkspace(String _pServerIP, String _pInstance, String _pUser, String _pPassword, String _pDatabase, String _pVersion)
        {
            // Create and populate the property set         
            ESRI.ArcGIS.esriSystem.IPropertySet pPropertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
            pPropertySet.SetProperty("SERVER", _pServerIP);
            pPropertySet.SetProperty("INSTANCE", _pInstance);
            pPropertySet.SetProperty("DATABASE", _pDatabase);
            pPropertySet.SetProperty("USER", _pUser);
            pPropertySet.SetProperty("PASSWORD", _pPassword);
            pPropertySet.SetProperty("VERSION", _pVersion);
            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2 workspaceFactory;
            workspaceFactory = (ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2)new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactoryClass();
            return workspaceFactory.Open(pPropertySet, 0);
        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPoint = new PointClass();

                pPoint.PutCoords(e.mapX, e.mapY);

                axMapControl1.CenterAt(pPoint);
                

                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

            }
        }

      
      

        private void 空间分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pZoomIn = new ControlsMapZoomInToolClass();

            pZoomIn.OnCreate(axMapControl1.Object);

            axMapControl1.CurrentTool = pZoomIn as ITool;

        }

        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pZoomOut = new ControlsMapZoomOutToolClass();


            pZoomOut.OnCreate(axMapControl1.Object);

            axMapControl1.CurrentTool = pZoomOut as ITool;
        }

        private void 打开栅格数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IRasterWorkspace pRasterWs = GetRasterWorkspace(@".\data\IDW数据");

            IRasterDataset pRasterDataset = pRasterWs.OpenRasterDataset("MOD02HKM.A2010068.0310.005.2010069144441.GEO - 副本.tif");

            IRasterLayer pRasterLayer = new RasterLayerClass();

            pRasterLayer.CreateFromDataset(pRasterDataset);

            axMapControl1.Map.AddLayer(pRasterLayer as ILayer); ;




        }

         IRasterWorkspace GetRasterWorkspace(string pWsName)
        {
            
            try
            {
                IWorkspaceFactory pWorkFact = new RasterWorkspaceFactoryClass();
                return pWorkFact.OpenFromFile(pWsName, 0) as IRasterWorkspace;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


         IRasterDataset OpenFileRasterDataset(string pFolderName, string pFileName)
        {
            
            IRasterWorkspace pRasterWorkspace = GetRasterWorkspace(pFolderName);

            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pFileName);
            return pRasterDataset;
        }


         void GetRasterCursorDefault(IRaster pRaster)
         {


             IRasterCursor pRasterCursor = pRaster.CreateCursor();

             while (pRasterCursor.Next())
             {
                 IPixelBlock pPixBlock = pRasterCursor.PixelBlock;

                 int W = pPixBlock.Width;
                 //这个W也就是整个栅格数据记得宽度

                 int H = pPixBlock.Height;

             }
         }

         void GetRasterCursorCustom(IRaster pRaster)
         {


             IRaster2 pRaster2 = pRaster as IRaster2;

             IPnt pPnt = new PntClass();

             pPnt.X = 256;

             pPnt.Y = 256;

             IRasterCursor pRasterCursor2 = pRaster2.CreateCursorEx(pPnt);


             while (pRasterCursor2.Next())
             {
                 IPixelBlock pPixBlock = pRasterCursor2.PixelBlock;

                 int W = pPixBlock.Width;

                 int H = pPixBlock.Height;


                }
             

         }

         void GetRasterProps(IRaster pRaster)
         {
             IRasterProps pRasterPros = pRaster as IRasterProps;

             int pH = pRasterPros.Height;//3973

             int pW = pRasterPros.Width;//5629

         }

         private void 获取栅格游标ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IRasterLayer pRasterLayer = axMapControl1.Map.get_Layer(0) as IRasterLayer;

             IRaster pRaster = pRasterLayer.Raster;

            
         }

         public void StartEdit(IWorkspace pWorkspace, ITable pTable)
         {
            
             IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pWorkspace;
             //启动编辑会话
             pWorkspaceEdit.StartEditing(false);
             //启动编辑操作
             pWorkspaceEdit.StartEditOperation();

             IRow pRow = pTable.CreateRow();

             pRow.set_Value(2, "练习");
             pRow.Store();
             //结束编辑操作
             pWorkspaceEdit.StopEditOperation();
             //结束编辑会话
             pWorkspaceEdit.StopEditing(true);
         }

         public void ChangeNodataValue(IRasterDataset2 pRasterDatset)
         {
             
                 IRaster2 pRaster2 = pRasterDatset.CreateFullRaster() as IRaster2;


                 IPnt pPntBlock = new PntClass();

                 pPntBlock.X = 128;

                 pPntBlock.Y = 128;
                 IRasterCursor pRasterCursor = pRaster2.CreateCursorEx(pPntBlock);

                 IRasterEdit pRasterEdit = pRaster2 as IRasterEdit;

                 if (pRasterEdit.CanEdit())
                 {
                     IRasterBandCollection pBands = pRasterDatset as IRasterBandCollection;
                     IPixelBlock3 pPixelblock3 = null;
                     int pBlockwidth = 0;
                     int pBlockheight = 0;
                     System.Array pixels;
                     IPnt pPnt = null;
                     object pValue;
                     long pBandCount = pBands.Count;

                     //获取Nodata
                     //IRasterProps pRasterPro = pRaster2 as IRasterProps;

                     //object pNodata = pRasterPro.NoDataValue;


                     do
                     {
                         pPixelblock3 = pRasterCursor.PixelBlock as IPixelBlock3;
                         pBlockwidth = pPixelblock3.Width;
                         pBlockheight = pPixelblock3.Height;

                         for (int k = 0; k < pBandCount; k++)
                         {

                             pixels = (System.Array)pPixelblock3.get_PixelData(k);
                             for (int i = 0; i < pBlockwidth; i++)
                             {
                                 for (int j = 0; j < pBlockheight; j++)
                                 {

                                     pValue = pixels.GetValue(i, j);

                                     if (Convert.ToInt32(pValue) == 0)
                                     {
                                         pixels.SetValue(Convert.ToByte(50), i, j);
                                         

                                     }
                                 }
                             }
                             pPixelblock3.set_PixelData(k, pixels);
                         }

                         pPnt = pRasterCursor.TopLeft;
                         pRasterEdit.Write(pPnt, (IPixelBlock)pPixelblock3);

                        
                     }
                     while (pRasterCursor.Next());
                     System.Runtime.InteropServices.Marshal.ReleaseComObject(pRasterEdit);
                 }

                
             

         }


         public void kriging(IFeatureClass pFeatureClass)
         {
             
             //  FeatureClassDescriptor 对象用于控制和描述插值的参数

             IFeatureClassDescriptor pFDescr = new FeatureClassDescriptorClass();
             pFDescr.Create(pFeatureClass, null, "Ozone");

             // 栅格半径辅助对象用于控制插值的参数 
             IRasterRadius pRasRadius = new RasterRadiusClass();
             object object_Missing = System.Type.Missing;
          
             pRasRadius.SetVariable(12, ref object_Missing);
      
          
             IInterpolationOp pInterpOp = new RasterInterpolationOpClass();

             IRaster pRasterOut = (IRaster)pInterpOp.Krige((IGeoDataset)pFDescr,
                 esriGeoAnalysisSemiVariogramEnum.esriGeoAnalysisExponentialSemiVariogram,
                 pRasRadius, false, ref object_Missing);
         }


         double  GetElevation(IRaster pRaster, IPoint point)
         {
             
                 IRasterSurface pRasterSurface = new RasterSurfaceClass();
                 pRasterSurface.PutRaster(pRaster, 0);
                 ISurface pSurface = pRasterSurface as ISurface;
                 return pSurface.GetElevation(point);
                 
         }

         public void StartSDEVersion(IWorkspace pWorkspace)
         {
             
             IMultiuserWorkspaceEdit pMWorkspaceEdit = (IMultiuserWorkspaceEdit)pWorkspace;

             IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pWorkspace;

             //启动版本编辑会话
             pMWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
             //启动编辑操作
             pWorkspaceEdit.StartEditOperation();

             //编辑过程...

             //结束编辑操作
             pWorkspaceEdit.StopEditOperation();
             //结束编辑会话
             pWorkspaceEdit.StopEditing(true);
           
         }

         public void ChangeFieldAliasName(ITable pTable, string pOriFieldName, string pDesFieldName)
         {
             
             IClassSchemaEdit pClassSchemaEdit = (IClassSchemaEdit)pTable;

             //给对象加上锁
             ISchemaLock pSchemaLock = (ISchemaLock)pTable;

             pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

             if (pTable.FindField(pOriFieldName) != -1)
             {

                 pClassSchemaEdit.AlterFieldAliasName(pOriFieldName, pDesFieldName);

                 pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
             }
             else
             {
                 return;
             }
         }

      
         public void AssignDomainToFieldWithSubtypes(IFeatureClass pFeatureClass)
         {

             IDataset pDataset = (IDataset)pFeatureClass;


             IWorkspace pWorkspace = pDataset.Workspace;
             IWorkspaceDomains pWorkspaceDomains = (IWorkspaceDomains)pWorkspace;

      
             IDomain pDistributionDiamDomain = pWorkspaceDomains.get_DomainByName("DistDiam");
             

             ISubtypes pSubtypes = (ISubtypes)pFeatureClass;

             pSubtypes.set_Domain(1, "SIZE_ONE", pDistributionDiamDomain);

         }


         public void AddPipeSubtypes(IFeatureClass pFeatureClass)
         {
             
             ISubtypes pSubtypes = (ISubtypes)pFeatureClass;
            
             pSubtypes.SubtypeFieldName = "PipeType";

             pSubtypes.AddSubtype(1, "Primary");
             pSubtypes.AddSubtype(2, "Secondary");

             pSubtypes.DefaultSubtypeCode = 1;
         }

         void CreateDomain(IWorkspace pWorkspace)
         {
           
             IWorkspaceDomains pWorkspaceDomains = (IWorkspaceDomains)pWorkspace;

             ICodedValueDomain pCodedValueDomain = new CodedValueDomainClass();

             pCodedValueDomain.AddCode("RES", "Residential");
             pCodedValueDomain.AddCode("COM", "Commercial");
             pCodedValueDomain.AddCode("IND", "Industrial");

             IDomain pDomain = (IDomain)pCodedValueDomain;
             pDomain.Name = "Building Types";
             pDomain.FieldType = esriFieldType.esriFieldTypeString;
             pDomain.SplitPolicy = esriSplitPolicyType.esriSPTDuplicate;
             pDomain.MergePolicy = esriMergePolicyType.esriMPTDefaultValue;


             pWorkspaceDomains.AddDomain(pDomain);
         }

         private void 视域分析ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IRasterWorkspace pRasterWs = GetRasterWorkspace(@".\data\IDW数据");

             IRasterDataset pRasterDataset = pRasterWs.OpenRasterDataset("dem.img");

             IRasterLayer pRasterLayer = new RasterLayerClass();

             pRasterLayer.CreateFromDataset(pRasterDataset);

             axMapControl1.Map.AddLayer(pRasterLayer as ILayer);

             pFlag = 4;


         }

         private void 创建TinToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IFeatureClass pFeatureClass = GetFeatureClass(@".\data\IDW数据", "山东20100321");

            
            IField pField = pFeatureClass.Fields.get_Field(pFeatureClass.FindField("H"));

            ITin pTin = CreateTin(pFeatureClass, pField, @".\data\IDW数据\TinTest");

            ITinLayer pTinLayer = new TinLayerClass();

            pTinLayer.Dataset = pTin;

            axMapControl1.Map.AddLayer(pTinLayer as ILayer);
         }

         private void 密度分析ToolStripMenuItem_Click(object sender, EventArgs e)
         {

             IFeatureClass pFeatureClass = GetFeatureClass(@".\data\IDW数据", "山东20100321");

             IRaster pRaster = DensityAnalyst(pFeatureClass, "H", 0.005, 0.05);

             IRasterLayer pRasterLayer = new RasterLayerClass();

             pRasterLayer.CreateFromRaster(pRaster);

             axMapControl1.Map.AddLayer(pRasterLayer as IRasterLayer);


         }

         public IGeoDataset CreateExtractOp(IGeoDataset pGeoDataset, ESRI.ArcGIS.Geometry.IPolygon pPolygone)
         {

            
   
                 ESRI.ArcGIS.SpatialAnalyst.IExtractionOp pExtractionOp = new ESRI.ArcGIS.SpatialAnalyst.RasterExtractionOpClass();

              
                 ESRI.ArcGIS.Geodatabase.IGeoDataset pGeoOutput = pExtractionOp.Polygon(pGeoDataset, pPolygone, true);

                 return pGeoOutput;

            

         }

        /// <summary>
        /// 邻域分析
        /// </summary>
        /// <param name="GeoDataset"></param>
        /// <returns></returns>

         public IGeoDataset CreateNeighborhoodOpBlockStatisticsRaster(IGeoDataset GeoDataset)
         {

           
                 INeighborhoodOp pNeighborhoodOP = new RasterNeighborhoodOpClass();

                 IRasterNeighborhood pRasterNeighborhood = new RasterNeighborhoodClass();

               
                 pRasterNeighborhood.SetRectangle(3, 3, esriGeoAnalysisUnitsEnum.esriUnitsCells);


                 IGeoDataset pGeoOutput = pNeighborhoodOP.BlockStatistics(GeoDataset,esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMean, pRasterNeighborhood, true);

                 return pGeoOutput;

           

         }


        /// <summary>
        /// 分割栅格数据
        /// </summary>
        /// <param name="pRasterDataset"></param>
        /// <param name="pOutputWorkspace"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
         public void CreateTilesFromRasterDataset(IRasterDataset pRasterDataset, IWorkspace
             pOutputWorkspace, int pWidth, int pHeight)
         {
             
             IRasterProps pRasterProps = (IRasterProps)pRasterDataset.CreateDefaultRaster();
             double xTileSize = pRasterProps.MeanCellSize().X * pWidth;
             double yTileSize = pRasterProps.MeanCellSize().Y * pHeight;


             int xTileCount = (int)Math.Ceiling((double)pRasterProps.Width / pWidth);
             int yTileCount = (int)Math.Ceiling((double)pRasterProps.Height / pHeight);

             IEnvelope pExtent = pRasterProps.Extent;
             IEnvelope pTileExtent = new EnvelopeClass();
             ISaveAs pSaveAs = null;

             for (int i = 0; i < xTileCount; i++)
             {
                 for (int j = 0; j < yTileCount; j++)
                 {
                     pRasterProps = (IRasterProps)pRasterDataset.CreateDefaultRaster();

                     pTileExtent.XMin = pExtent.XMin + i * xTileSize;
                     pTileExtent.XMax = pTileExtent.XMin + xTileSize;
                     pTileExtent.YMin = pExtent.YMin + j * yTileSize;
                     pTileExtent.YMax = pTileExtent.YMin + yTileSize;

                     pRasterProps.Height = pHeight;
                     pRasterProps.Width = pWidth;
                     pRasterProps.Extent = pTileExtent;

                     pSaveAs = (ISaveAs)pRasterProps;
                     pSaveAs.SaveAs("tile_" + i + "_" + j + ".tif", pOutputWorkspace, "TIFF");
                 }
             }
         }


         public IGeoDataset CreateDistanceOpCostPathRaster(IGeoDataset pGeoDataset1, IGeoDataset pGeoDataset2, IGeoDataset pGeoDataset3)
         {


             IDistanceOp pDistanceOp = new RasterDistanceOpClass();


             IGeoDataset pGeoDatasetOutput = pDistanceOp.CostPath(pGeoDataset1, pGeoDataset2, pGeoDataset3, esriGeoAnalysisPathEnum.esriGeoAnalysisPathForEachZone);

             return pGeoDatasetOutput;
            

         }


         /// <summary>
         /// 栅格转矢量
         /// </summary>
         /// <param name="pRasterWs"></param>
         /// <param name="pRasterDatasetName"></param>
         /// <param name="pShapeFileName"></param>
         public void ConvertRaterToLineFeature(string pRasterWs,string pRasterDatasetName,string pShapeFileName)
         {

             IRasterDataset pRasterDataset = GetRasterWorkspace(pRasterWs).OpenRasterDataset(pRasterDatasetName);

          
             IConversionOp pConversionOp = new RasterConversionOpClass();
            
             IRasterAnalysisEnvironment pEnv = (IRasterAnalysisEnvironment)pConversionOp;
             IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactoryClass();
             IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pRasterWs, 0);
             pEnv.OutWorkspace = pWorkspace;
             IWorkspaceFactory pShapeFactory  = new ShapefileWorkspaceFactoryClass();
             IWorkspace pShapeWS = pShapeFactory.OpenFromFile(pRasterWs, 0);

             System.Object pDangle = (System.Object)1.0;
             IGeoDataset pFeatClassOutput = pConversionOp.RasterDataToLineFeatureData((IGeoDataset)pRasterDataset,
                 pShapeWS, pShapeFileName, false, false, ref pDangle);
         }




        /// <summary>
        /// 要素转成Grid
        /// </summary>
        /// <param name="pFeaureClass"></param>
        /// <param name="pRasterWorkspaceFolder"></param>
        /// <param name="pCellsize"></param>
        /// <param name="pGridName"></param>
        /// <returns></returns>
         public IGeoDataset CreateGridFromFeatureClass(IFeatureClass pFeaureClass, String pRasterWorkspaceFolder, double pCellsize ,string pGridName)
         {

             IGeoDataset pGeoDataset = (ESRI.ArcGIS.Geodatabase.IGeoDataset)pFeaureClass; // Explicit Cast

             ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;

             
            IConversionOp pConversionOp = new ESRI.ArcGIS.GeoAnalyst.RasterConversionOpClass();

             IWorkspaceFactory pWorkspaceFactory = new ESRI.ArcGIS.DataSourcesRaster.RasterWorkspaceFactoryClass();

             
             IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pRasterWorkspaceFolder, 0);

             IRasterAnalysisEnvironment pAnalysisEnvironment = (ESRI.ArcGIS.GeoAnalyst.IRasterAnalysisEnvironment)pConversionOp; // Explicit Cast
             pAnalysisEnvironment.OutWorkspace = pWorkspace;

             ESRI.ArcGIS.Geometry.IEnvelope pEnvelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
             pEnvelope = pGeoDataset.Extent;



             object pObjectCellSize = (System.Object)pCellsize;
             pAnalysisEnvironment.SetCellSize(ESRI.ArcGIS.GeoAnalyst.esriRasterEnvSettingEnum.esriRasterEnvValue, ref pObjectCellSize);

           
             object object_Envelope = (System.Object)pEnvelope; 
             object object_Missing = Type.Missing;
             pAnalysisEnvironment.SetExtent(ESRI.ArcGIS.GeoAnalyst.esriRasterEnvSettingEnum.esriRasterEnvValue, ref object_Envelope, ref object_Missing);

             
             pAnalysisEnvironment.OutSpatialReference = pSpatialReference;


             IRasterDataset pRasterDataset = new ESRI.ArcGIS.DataSourcesRaster.RasterDatasetClass();

          

             pRasterDataset = pConversionOp.ToRasterDataset(pGeoDataset, "GRID", pWorkspace, pGridName);

            IGeoDataset pGeoOutput = (ESRI.ArcGIS.Geodatabase.IGeoDataset)pRasterDataset;

             return pGeoOutput;

         }

         private void 改变像素的值ToolStripMenuItem_Click(object sender, EventArgs e)
         {
              IRasterWorkspace pRasterWs = GetRasterWorkspace(@".\data\IDW数据");

              IRasterDataset pRasterDataset = pRasterWs.OpenRasterDataset("MOD02HKM.A2010068.0310.005.2010069144441.GEO.tif");
              IRasterLayer pRasterLayer = new RasterLayerClass();

              pRasterLayer.CreateFromDataset(pRasterDataset);

              axMapControl1.Map.AddLayer(pRasterLayer as ILayer); ;

              ChangeNodataValue(pRasterDataset as IRasterDataset2);
              axMapControl1.Refresh();
         }

         private void CreateAttachTable(IFeatureClass pFeatureClass,int pID,string pFilePath,string pFileType)
         {
             //要素表是否有附件表,数据库只能是10版本的
             ITableAttachments pTableAtt = pFeatureClass as ITableAttachments;

             if (pTableAtt.HasAttachments == false)
             {
                 pTableAtt.AddAttachments();
             }

             //获取附件管理器
             IAttachmentManager pAttachmentManager = pTableAtt.AttachmentManager;

             //用二进制流读取数据
             IMemoryBlobStream pMemoryBlobStream = new MemoryBlobStreamClass();
             pMemoryBlobStream.LoadFromFile(pFilePath);

             //创建一个附件
             IAttachment pAttachment = new AttachmentClass();

             pAttachment.ContentType=pFileType;

             pAttachment.Name = System.IO.Path.GetFileName(pFilePath);

             pAttachment.Data = pMemoryBlobStream;

             //添加到表中
             pAttachmentManager.AddAttachment(pID, pAttachment);


         }

         private void 创建附件ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IFeatureWorkspace pFtWS = GetMDBWorkspace(@".\data\Attach.mdb") as IFeatureWorkspace;

             IFeatureClass pFtClass = pFtWS.OpenFeatureClass("H");

             CreateAttachTable(pFtClass, 2, @"E:\TJYX备份.rar", "RAR");
         }

         private void iDW插值ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IFeatureClass pFeatureClass = GetFeatureClass(@".\data\IDW数据", "山东20100321");

             IGeoDataset pGeoDataset = IDW(pFeatureClass, "H", 0.005, 0.05, 2);

             IRasterLayer pRasterlayer = new RasterLayerClass();

             //IRasterDataset pRs = pGeoDataset as IRasterDataset;

             pRasterlayer.CreateFromRaster(pGeoDataset as IRaster);


             axMapControl1.Map.AddLayer(pRasterlayer as ILayer);

             ISurfaceOp pSurface = new RasterSurfaceOpClass();
             object BaseHeight = Type.Missing;

             IGeoDataset p = pSurface.Contour(pGeoDataset, 2, ref BaseHeight);

             IFeatureLayer pFContourLayer = new FeatureLayerClass();

             pFContourLayer.FeatureClass = p as IFeatureClass;

             axMapControl1.AddLayer(pFContourLayer as ILayer);

             axMapControl1.Refresh();


             IWorkspaceFactory WF = new RasterWorkspaceFactoryClass();

             IWorkspace pRasterWS = WF.OpenFromFile(@".\data\IDW数据", 0);

             ISaveAs pSaveAs = (ISaveAs)pGeoDataset;

             pSaveAs.SaveAs("RasterTest.tif", pRasterWS, "TIFF");

             axMapControl1.ActiveView.Refresh();
         }

         private void 创建网络ToolStripMenuItem_Click(object sender, EventArgs e)
         {

             IWorkspace pWs = GetMDBWorkspace(@".\data\Geometric.mdb");

             IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;

             IFeatureDataset pFtDataset = pFtWs.OpenFeatureDataset("work");

             IDataset pDataset = pFtDataset as IDataset;

             IFeatureDatasetName pFtDatasetName = pDataset.FullName as IFeatureDatasetName;

             CreateGeometricNetwork(pWs, pFtDatasetName, "TestGeometric");

            
         }

         private void 最短路径ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             pFlag = 3;
         }

         private void 创建网络ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             


             CreateNetworkDataset(@".\data\Network.mdb", "network", "NetStreet", "Road");


         }

         private void 最短路径ToolStripMenuItem1_Click(object sender, EventArgs e)
         {

             pFlag = 2;//无向网络分析
         }

         private void 加载网络ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IWorkspace pWs = GetMDBWorkspace(@".\data\Geometric.mdb");

             IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;

             IFeatureDataset pFtDataset = pFtWs.OpenFeatureDataset("work");

             IDataset pDataset = pFtDataset as IDataset;

             IFeatureDatasetName pFtDatasetName = pDataset.FullName as IFeatureDatasetName;

             CreateGeometricNetwork(pWs, pFtDatasetName, "TestGeometric");

             LoadGeometric(axMapControl1.Map, GetGeometricNetwork(pFtDataset, "TestGeometric"));
         }

         private void 加载网络ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             IWorkspace pWs = GetMDBWorkspace(@".\data\Geometric.mdb");

             IFeatureWorkspace pFtWs = pWs as IFeatureWorkspace;

             INetworkDataset pNetWorkDataset = GetNetDataset(pFtWs, "network", "network_ND");

             pNASolveClass = new NARouteSolverClass();

             loadNet(axMapControl1.Map, pNetWorkDataset);

             pNaContext = GetSolverContext(pNASolveClass, pNetWorkDataset);

             pNALayer = GetNaLayer(pNASolveClass, pNaContext);

             axMapControl1.Map.AddLayer(pNALayer as ILayer);
         }

         private void 删除要素类ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             string WsName = WsPath();

             if (WsName != "")
             {


                 IWorkspaceFactory pWsFt = new AccessWorkspaceFactoryClass();

                 IWorkspace pWs = pWsFt.OpenFromFile(WsName, 0);

                 IFeatureWorkspace pFWs = pWs as IFeatureWorkspace;

                 IFeatureClass pFClass = pFWs.OpenFeatureClass("PointTest");

                 IDataset pDatset = pFClass as IDataset;

                 pDatset.Delete();

             }
         }

         private void 数据转换ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IWorkspaceFactory pSwf = new AccessWorkspaceFactoryClass();

             IWorkspaceFactory pDwf = new AccessWorkspaceFactoryClass();

             ConvertFeatureClass(pSwf, @".\datas.mdb", "s", pDwf, @".\datad.mdb", "H");
         }

         private void 打开Shp文件ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             string[] ShapeFile = OpenShapeFile();
             axMapControl1.AddShapeFile(ShapeFile[0], ShapeFile[1]);
         }

         private void 线性参考ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             IMap pMap = axMapControl1.Map;

             IFeatureWorkspace pFtWs = GetFGDBWorkspace(@".\data\Rout.gdb") as IFeatureWorkspace;

             IFeatureLayer pFeatureLayer = new FeatureLayerClass();

             pFeatureLayer.FeatureClass = pFtWs.OpenFeatureClass("routes");

             pFeatureLayer.Name = "路径";

             axMapControl1.Map.AddLayer(pFeatureLayer as ILayer);


             IPolyline pPolyline = FindRoutByMeasure(pFeatureLayer.FeatureClass, "ROUTE1", 20000013, 0, 25);

             IRgbColor pColor = new RgbColorClass();

             pColor.Red = 255;

             IElement pElement = new LineElementClass();

             ILineSymbol pLinesymbol = new SimpleLineSymbolClass();

             pLinesymbol.Color = pColor as IColor;

             pLinesymbol.Width = 100;

             pElement.Geometry = pPolyline as IGeometry;

             IGraphicsContainer pGrahicsC = pMap as IGraphicsContainer;

             pGrahicsC.AddElement(pElement, 0);


             axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
         }

         private void 动态分段ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IMap pMap = axMapControl1.Map;

             IFeatureWorkspace pFtWs = GetFGDBWorkspace(@".\data\Rout.gdb") as IFeatureWorkspace;

             IFeatureLayer pFeatureLayer = new FeatureLayerClass();

             pFeatureLayer.FeatureClass = pFtWs.OpenFeatureClass("routes");

             pFeatureLayer.Name = "路径";

             axMapControl1.Map.AddLayer(pFeatureLayer as ILayer);


             ITable pEventTable = pFtWs.OpenTable("RoutMeasure");

             IFeatureClass pEventTable2FeatureClass = EventTable2FeatureClass(pFeatureLayer.FeatureClass, "ROUTE1", pEventTable, "ROUTE1", "FROM_", "TO");

             IFeatureLayer pRouteLayer = new FeatureLayerClass();

             pRouteLayer.FeatureClass = pEventTable2FeatureClass;

             pRouteLayer.Name = "动态分段";


             // axMapControl1.Map.AddLayer(pRouteLayer as ILayer);
             //axMapControl1.ActiveView.GraphicsContainer.DeleteAllElements();

             axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
         }

         private void tureOrFalseToolStripMenuItem_Click(object sender, EventArgs e)
         {
             IMap pMap = axMapControl1.Map;

             IFeatureLayer pFeaturelayer = GetLayer(pMap, "Roads") as IFeatureLayer;

             Search(pFeaturelayer.FeatureClass, true);
         }

         private void IntesectToolStripMenuItem_Click(object sender, EventArgs e)
         {

             IFeatureClass pFeatureClass = GetFeatureClass(@"D:\IDW\JiAn", "BoundMask");

             IFeatureClass pFt = GetFeatureClass(@"D:\IDW\JiAn", "Bound");

             IFeatureClass pFt1 = Intsect(pFeatureClass, pFt, @"D:\IDW\JiAn", "intsetrest");

             IFeatureLayer pFeatureLayer = new FeatureLayerClass();

             pFeatureLayer.FeatureClass = pFt1;

             axMapControl1.Map.AddLayer(pFeatureLayer as ILayer);

             axMapControl1.ActiveView.Refresh();
         }

         private void GPToolStripMenuItem_Click(object sender, EventArgs e)
         {

             GPIntsect();

             Geoprocessor GP = new Geoprocessor();
             GP.OverwriteOutput = true;
             FeatureToPolygon fpg = new FeatureToPolygon(); ;
             fpg.in_features = @".\data\数据\regionR.shp";
             fpg.out_feature_class = @".\data\数据\FeatureToPolygon2.shp";



             GP.Execute(fpg, null);


             MessageBox.Show("OK");
         }

         private void 打开文档ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             string MxdFilePath = OpenMxd();

             if (MxdFilePath != "")
             {
                 axMapControl1.LoadMxFile(MxdFilePath);

             }

         }

         private void 获取所有要素类ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             string WsName = WsPath();

             if (WsName != "")
             {


                 IWorkspaceFactory pWsFt = new AccessWorkspaceFactoryClass();

                 IWorkspace pWs = pWsFt.OpenFromFile(WsName, 0);

                 IEnumDataset pEDataset = pWs.get_Datasets(esriDatasetType.esriDTAny);

                 IDataset pDataset = pEDataset.Next();

                 while (pDataset != null)
                 {
                     if (pDataset.Type == esriDatasetType.esriDTFeatureClass)
                     {
                         FeatureClassBox.Items.Add(pDataset.Name);
                     }
                     //如果是数据集
                     else if (pDataset.Type == esriDatasetType.esriDTFeatureDataset)
                     {
                         IEnumDataset pESubDataset = pDataset.Subsets;

                         IDataset pSubDataset = pESubDataset.Next();

                         while (pSubDataset != null)
                         {
                             FeatureClassBox.Items.Add(pSubDataset.Name);

                             pSubDataset = pESubDataset.Next();
                         }
                     }


                     pDataset = pEDataset.Next();
                 }

             }

             FeatureClassBox.Text = FeatureClassBox.Items[0].ToString();
         }

         private void 高亮显示ToolStripMenuItem_Click(object sender, EventArgs e)
         {





             IMap pMap = axMapControl1.Map;

             IFeatureLayer pFeaturelayer = GetLayer(pMap, "Roads") as IFeatureLayer;

             IFeatureSelection pFeatureSelection = pFeaturelayer as IFeatureSelection;

             IQueryFilter pQuery = new QueryFilterClass();

             pQuery.WhereClause = "TYPE=" + "'paved'";

             pFeatureSelection.SelectFeatures(pQuery, esriSelectionResultEnum.esriSelectionResultNew, false);

             pFeatureSelection.SelectionColor = GetRGB(225, 225, 225);

             SearchHightlight(pMap, pFeaturelayer, pQuery, true);

             axMapControl1.ActiveView.Refresh();

             ISelection pSelection = pMap.FeatureSelection;

             ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;


             ICursor pc;

             pSelectionSet.Search(null, false, out pc);

             IRow pr = pc.NextRow();

             while (pr != null)
             {
                 string s = pr.get_Value(0).ToString();

                 pr = pc.NextRow();
             }

         }

         private void 命令方式ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             ICommand pAddData = new ControlsAddDataCommandClass();

             pAddData.OnCreate(axMapControl1.Object);

             pAddData.OnClick();
         }

         private void 命令方式ToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             ICommand pMxd = new ControlsOpenDocCommandClass();

             pMxd.OnCreate(axMapControl1.Object);

             pMxd.OnClick();
             
         }


         /// <summary>
         /// EMF
         /// </summary>
         private void ExportEMF()
         {
             IActiveView pActiveView;
             pActiveView = axPageLayoutControl1.ActiveView;
             IExport pExport;
             pExport = new ExportEMFClass();
             pExport.ExportFileName = @".\data\ExportEMF.emf";
             pExport.Resolution = 300;
             tagRECT exportRECT;
             exportRECT = pActiveView.ExportFrame;
             IEnvelope pPixelBoundsEnv;
             pPixelBoundsEnv = new EnvelopeClass();
             pPixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.top,
             exportRECT.right, exportRECT.bottom);
             pExport.PixelBounds = pPixelBoundsEnv;
             int hDC;
             hDC = pExport.StartExporting();
             pActiveView.Output(hDC, (int)pExport.Resolution, ref exportRECT, null, null);
             pExport.FinishExporting();
             pExport.Cleanup();
         }
        /// <summary>
        /// 导出PDF
        /// </summary>
         private void ExportPDF()
         {

             IActiveView pActiveView;
             pActiveView = axPageLayoutControl1.ActiveView;
             IEnvelope pEnv;
             pEnv = pActiveView.Extent;
             IExport pExport;
             pExport = new ExportPDFClass();
             pExport.ExportFileName = @".\data\ExportPDF.pdf";
             pExport.Resolution = 30;
             tagRECT exportRECT;
             exportRECT.top = 0;
             exportRECT.left = 0;
             exportRECT.right = (int)pEnv.Width;


             exportRECT.bottom = (int)pEnv.Height;
             IEnvelope pPixelBoundsEnv;
             pPixelBoundsEnv = new EnvelopeClass();
             pPixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.bottom,
             exportRECT.right, exportRECT.top);
             pExport.PixelBounds = pPixelBoundsEnv;
             int hDC;
             hDC = pExport.StartExporting();
             pActiveView.Output(hDC, (int)pExport.Resolution, ref exportRECT, null, null);
             pExport.FinishExporting();
             pExport.Cleanup();
         }




         private void 打开TinToolStripMenuItem_Click(object sender, EventArgs e)
         {
            // ITinLayer pTinLayer = GetTINLayer(@".\data\IDW数据\dvtin");

             ITinAdvanced2 pTin = new TinClass();
             pTin.Init(@".\data\IDW数据\dvtin");

             ITinLayer pTinLayer = new TinLayerClass();

             pTinLayer.Dataset = pTin;
             axMapControl1.Map.AddLayer(pTinLayer as ILayer);
         }

         private void 泰森多边形ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             ITinLayer pTinLayer = GetTINLayer(@".\data\IDW数据\dvtin");

             IFeatureClass pFeatureClass = GetFeatureClass(@".\data\IDW数据", "Vr");

             CreateVr(pFeatureClass, pTinLayer.Dataset as ITin);

             IFeatureLayer pFeatLayer = new FeatureLayerClass();

             pFeatLayer.Name = "泰森多边形";

             pFeatLayer.FeatureClass = pFeatureClass;

             axMapControl1.Map.AddLayer(pFeatLayer as ILayer);
         }

         private void Tin2Contour(ITin pTin,IFeatureClass pFeatureClass)
         {
             ITinSurface pTinSurface = pTin as ITinSurface;

             //不要启动编辑，因为这个接口会在要素类中添加字段
             pTinSurface.Contour(0, 2, pFeatureClass, "Height", 0);
         }

         private void 等高线ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             ITinLayer pTinLayer = GetTINLayer(@".\data\IDW数据\dvtin");

             IFeatureClass pFeatureClass = GetFeatureClass(@".\data\IDW数据", "TinContour");

             Tin2Contour(pTinLayer.Dataset as ITin, pFeatureClass);

             IFeatureLayer pFeatLayer = new FeatureLayerClass();

             pFeatLayer.Name = "等高线";

             pFeatLayer.FeatureClass = pFeatureClass;

             axMapControl1.Map.AddLayer(pFeatLayer as ILayer);



         }

         private void 创建栅格数据集ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             ISpatialReference pSr = new UnknownCoordinateSystemClass();

          IRasterDataset  pRasterDataset= CreateRasterDataset(@".\data\IDW数据", "RTest","TIFF", pSr);

          IRasterLayer pRasterLayer = new RasterLayerClass();

          pRasterLayer.CreateFromDataset(pRasterDataset);

          axMapControl1.Map.AddLayer(pRasterLayer);
            

         }

        
              private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) //map view
            {
                //激活地图
                pMapControlsSynchronizer.ActivateMap();
            }
            else //layout view
            {
                //激活视图
                pMapControlsSynchronizer.ActivatePageLayout();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            
            pMapControlsSynchronizer = new ControlsSynchronizer((IMapControl3)axMapControl1.Object, (IPageLayoutControl2)axPageLayoutControl1.Object);

            
            pMapControlsSynchronizer.BindControls(true);

            //
            pMapControlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);

            pMapControlsSynchronizer.AddFrameworkControl(axTOCControl1.Object);

            
            
        }

        private void 同步关联ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenNewMapDocument pOpenMapDoc = new OpenNewMapDocument(pMapControlsSynchronizer);

            pOpenMapDoc.OnCreate(axMapControl1.Object);
            pOpenMapDoc.OnClick();

            //axToolbarControl1.AddItem(pOpenMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);
        }

        private void 取消工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (axMapControl1.CurrentTool != null)
            {
                axMapControl1.CurrentTool = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        /// 


        private ITable CreateTable(string FilePath, string TableName)
        {
            IWorkspaceFactory pWks = new ShapefileWorkspaceFactoryClass();

            IFeatureWorkspace pFwk = pWks.OpenFromFile(FilePath, 0) as IFeatureWorkspace;


            //用于记录面中的ID;

            IField pFieldID = new FieldClass();

            IFieldEdit pFieldIID = pFieldID as IFieldEdit;

            pFieldIID.Type_2 = esriFieldType.esriFieldTypeInteger;

            pFieldIID.Name_2 = "面ID";

            //用于记录个数的;
            IField pFieldCount = new FieldClass();

            IFieldEdit pFieldICount = pFieldCount as IFieldEdit;

            pFieldICount.Type_2 = esriFieldType.esriFieldTypeInteger;
            pFieldICount.Name_2 = "个数";

            //用于添加表中的必要字段
            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription = new ESRI.ArcGIS.Geodatabase.ObjectClassDescriptionClass();

            IFields pTableFields = objectClassDescription.RequiredFields;

            IFieldsEdit pTableFieldsEdit = pTableFields as IFieldsEdit;

            pTableFieldsEdit.AddField(pFieldID);

            pTableFieldsEdit.AddField(pFieldCount);

            ITable pTable = pFwk.CreateTable(TableName, pTableFields, null, null, "");

            return pTable;


        }


        private void 包含统计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeatureClass pPolygonFClass = GetFeatureClass(@".\data\空间数据", "三级成矿区带");

            IFeatureClass pPointFClass = GetFeatureClass(@".\data\空间数据", "探矿权点");

            ITable pTable = CreateTable(@".\data\空间数据", "Res3");


            IFeatureCursor pPolyCursor = pPolygonFClass.Search(null, false);

            IFeature pPolyFeature = pPolyCursor.NextFeature();

            while (pPolyFeature != null)
            {
                IGeometry pPolGeo = pPolyFeature.Shape;

                IRelationalOperator pRel = pPolGeo as IRelationalOperator;

                int Count = 0;

                IFeatureCursor pPointCur = pPointFClass.Search(null, false);

                IFeature pPointFeature = pPointCur.NextFeature();


                while (pPointFeature != null)
                {
                    IGeometry pPointGeo = pPointFeature.Shape;

                    if (pRel.Contains(pPointGeo))
                    {
                        Count++;
                    }
                    pPointFeature = pPointCur.NextFeature();

                }
                if (Count != 0)
                {
                    IRow pRow = pTable.CreateRow();
                    pRow.set_Value(1, pPolyFeature.get_Value(0));
                    pRow.set_Value(2, Count);
                    pRow.Store();

                    //

                }


                pPolyFeature = pPolyCursor.NextFeature();

            }

            MessageBox.Show("OK");
        }

        private void 空间查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search pSearch = new Search();


            IFeatureClass pPolygonFClass = pSearch.GetFeatureClass(@".\data\空间数据", "三级成矿区带");

            IFeatureClass pPointFClass = pSearch.GetFeatureClass(@".\data\空间数据", "探矿权点");

            ITable pTable = pSearch.CreateTable(@".\data\空间数据", "Result");

            pSearch.StatisticPointCount(pPolygonFClass, pPointFClass, pTable);

            // ICursor pTableCur = pTable.Insert(true);

            /* IFeatureCursor pPolyCursor = pPolygonFClass.Search(null, false);

             IFeature pPolyFeature = pPolyCursor.NextFeature();


             while (pPolyFeature != null)
             {
                 IGeometry pPolGeo = pPolyFeature.Shape;


                 int Count = 0;

                 ISpatialFilter spatialFilter = new SpatialFilterClass();

                 spatialFilter.Geometry = pPolGeo;

                 spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                 IFeatureCursor pPointCur = pPointFClass.Search(spatialFilter, false);

                 if (pPointCur != null)
                 {
                     IFeature pPointFeature = pPointCur.NextFeature();

                     while (pPointFeature != null)
                     {
                         pPointFeature = pPointCur.NextFeature();
                         Count++;
                     }

                 }

                 if (Count != 0)
                 {

                     IRow pRow = pTable.CreateRow();
                     pRow.set_Value(1, pPolyFeature.get_Value(0));
                     pRow.set_Value(2, Count);
                     pRow.Store();
                 }
                 pPolyFeature = pPolyCursor.NextFeature();




             }
             MessageBox.Show("hello");
             */
        }

        /// <summary>
        /// 这个字段要是唯一的
        /// </summary>
        /// <param name="_FilePath"></param>
        /// <param name="_TableName"></param>
        /// <param name="_pFeatureClass"></param>
        /// <param name="_FieldName"></param>
        /// <returns></returns>

        private ITable CreateWeightTable(string _FilePath, string _TableName, IFeatureClass _pFeatureClass, string _FieldName)
        {
            IWorkspaceFactory pWks = new ShapefileWorkspaceFactoryClass();

            IFeatureWorkspace pFwk = pWks.OpenFromFile(_FilePath, 0) as IFeatureWorkspace;


            //用于添加表中的必要字段
            ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription = new ESRI.ArcGIS.Geodatabase.ObjectClassDescriptionClass();



            IFields pTableFields = objectClassDescription.RequiredFields;

            IFieldsEdit pTableFieldsEdit = pTableFields as IFieldsEdit;


            int index = _pFeatureClass.FindField(_FieldName);

            IField pField = new FieldClass();

            IFieldEdit pFieldEdit = pField as IFieldEdit;

            pFieldEdit.Name_2 = "Name";
            pTableFieldsEdit.AddField(pFieldEdit);

            pFieldEdit.Type_2 = _pFeatureClass.Fields.get_Field(index).Type;
            //ArrayList arrValues = GetUniqueFieldValue(pPolygonFClass, "NAME");


            IFeatureCursor pFtCursor = _pFeatureClass.Search(null, false);

            IFeature pFt = pFtCursor.NextFeature();



            while (pFt != null)
            {
                IField pFieldv = new FieldClass();

                IFieldEdit pFieldEditv = pFieldv as IFieldEdit;

                pFieldEditv.Name_2 = pFt.get_Value(index).ToString();
                pFieldEditv.Type_2 = esriFieldType.esriFieldTypeInteger;
                pTableFieldsEdit.AddField(pFieldEditv);
                pFt = pFtCursor.NextFeature();
            }


            ITable pTable = pFwk.CreateTable(_TableName, pTableFields, null, null, "");


            IFeatureCursor pFtCursor1 = _pFeatureClass.Search(null, false);

            IFeature pFt1 = pFtCursor1.NextFeature();

            //  ICursor  pcr = pTable .Insert(true);

            while (pFt1 != null)
            {


                IRow pRow = pTable.CreateRow();

                pRow.set_Value(1, pFt1.get_Value(index));

                pRow.Store();

                pFt1 = pFtCursor1.NextFeature();
            }

            return pTable;


        }

        private void 邻接矩阵ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeatureClass pPolygonFClass = GetFeatureClass(@".\data\空间数据", "行政区");

            ITable pTable = CreateWeightTable(@".\data\空间数据", "Weight", pPolygonFClass, "NAME");

            IFeature pFt1, pFt2;

            IFeatureCursor pFtCur1, pFtCur2;

            pFtCur1 = pPolygonFClass.Search(null, false);

            pFt1 = pFtCur1.NextFeature();

            ICursor pCursor = pTable.Update(null, false);

            IRow pRow = pCursor.NextRow();

            int j = 0;
            ///这里是关键，在这里进行计算
            while (pFt1 != null)
            {
                IProximityOperator pProx = pFt1.Shape as IProximityOperator;

                pFtCur2 = pPolygonFClass.Search(null, false);

                pFt2 = pFtCur2.NextFeature();

                while (pFt2 != null)
                {
                    double dis = pProx.ReturnDistance(pFt2.Shape);

                    if (dis == 0)
                    {
                        pRow.set_Value(j + 2, 1);

                        pRow.Store();
                    }
                    pFt2 = pFtCur2.NextFeature();
                    j++;
                }
                j = 0;
                pRow = pCursor.NextRow();
                pFt1 = pFtCur1.NextFeature();
            }
            MessageBox.Show("0k");
            
        }

        private void 查询图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryLayerTest pQueryLayerTest = new QueryLayerTest();

            IFeatureLayer pFeaturelayer = pQueryLayerTest.OracleQueryLayer();

            pFeaturelayer.Name = "查询图层";

            axMapControl1.AddLayer(pFeaturelayer as ILayer);

            axMapControl1.Refresh();


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSourceFeatureClass"></param>
        /// <param name="_pTWorkspaceFactory"></param>
        /// <param name="_pTWs"></param>
        /// <param name="_pTName"></param>
        public void ConvertQuery2FeatureClass( IFeatureClass pSourceFeatureClass , IWorkspaceFactory _pTWorkspaceFactory, String _pTWs, string _pTName)
        {

            IWorkspace pTWorkspace = _pTWorkspaceFactory.OpenFromFile(_pTWs, 0);


            IDataset pSDataset = pSourceFeatureClass as IDataset;

            IFeatureClassName pSourceFeatureClassName = pSDataset.FullName as IFeatureClassName;

            IDataset pTDataset = (IDataset)pTWorkspace;

            IName pTDatasetName = pTDataset.FullName;

            IWorkspaceName pTargetWorkspaceName = (IWorkspaceName)pTDatasetName;


            IFeatureClassName pTargetFeatureClassName = new FeatureClassNameClass();

            IDatasetName pTargetDatasetName = (IDatasetName)pTargetFeatureClassName;

            // 验证要素类的名称是否合法，但是并没有对这个名称是否存在做检查
            string pTableName = null;

            IFieldChecker pNameChecker = new FieldCheckerClass();

            pNameChecker.ValidateWorkspace = pTWorkspace;

            int pFlag = pNameChecker.ValidateTableName(_pTName, out pTableName);

            pTargetDatasetName.Name = pTableName;

            pTargetDatasetName.WorkspaceName = pTargetWorkspaceName;


            // 创建字段检查对象
            IFieldChecker pFieldChecker = new FieldCheckerClass();

            IFields sourceFields = pSourceFeatureClass.Fields;
            IFields pTargetFields = null;

            IEnumFieldError pEnumFieldError = null;



            pFieldChecker.InputWorkspace = ((IDataset)pSourceFeatureClass).Workspace;

            pFieldChecker.ValidateWorkspace = pTWorkspace;

            // 验证字段
            pFieldChecker.Validate(sourceFields, out pEnumFieldError, out pTargetFields);
            if (pEnumFieldError != null)
            {
                // 报错提示
                Console.WriteLine("Errors were encountered during field validation.");
            }


            String pShapeFieldName = pSourceFeatureClass.ShapeFieldName;

            int pFieldIndex = pSourceFeatureClass.FindField(pShapeFieldName);

            IField pShapeField = sourceFields.get_Field(pFieldIndex);

            IGeometryDef pTargetGeometryDef = pShapeField.GeometryDef;


            // 创建要素转换对象
            IFeatureDataConverter pFDConverter = new FeatureDataConverterClass();

            IEnumInvalidObject pEnumInvalidObject = pFDConverter.ConvertFeatureClass(pSourceFeatureClassName, null, null, pTargetFeatureClassName,
                pTargetGeometryDef, pTargetFields, "", 1000, 0);

            // 检查是否有错误
            IInvalidObjectInfo pInvalidInfo = null;

            pEnumInvalidObject.Reset();

            while ((pInvalidInfo = pEnumInvalidObject.Next()) != null)
            {
                // 报错提示.
                Console.WriteLine("Errors occurred for the following feature: {0}",
                    pInvalidInfo.InvalidObjectID);
            }
        }

        private void 查询图层转成要素类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeatureLayer pFeaturelayer = axMapControl1.Map.get_Layer(0) as IFeatureLayer;

            IWorkspaceFactory pDwf = new AccessWorkspaceFactoryClass();

            ConvertQuery2FeatureClass(pFeaturelayer.FeatureClass, pDwf, ".\datad.mdb", "QueryLayer");


        }

        private void 饼状图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PieRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, "PERIMETER", "AREA");
        }

        private void zhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BarRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, "AREA", "PERIMETER");
           //  new ChartRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, "GEO_CODE");
        }

        private void 唯一值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UniqueValueRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, 24, "PERIMETER");

            axTOCControl1.Refresh();
        }

        private void 点状图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
              new DotRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer,100000, "GEO_CODE");

              axTOCControl1.Refresh();
        }

        private void 比例图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ProPortialRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, "PERIMETER");

            axTOCControl1.Refresh();
        }

        private void 简单渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleRender pSimple = new SimpleRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, "PERIMETER");
        }


        public IRasterRenderer StretchRenderer(ESRI.ArcGIS.Geodatabase.IRasterDataset pRasterDataset)
        {
            try
            {
                //Define the from and to colors for the color ramp.
                IRgbColor pFromColor = new RgbColorClass();
                pFromColor.Red = 255;
                pFromColor.Green = 0;
                pFromColor.Blue = 0;
                IRgbColor pToColor = new RgbColorClass();
                pToColor.Red = 0;
                pToColor.Green = 255;
                pToColor.Blue = 0;
                //Create the color ramp.
                IAlgorithmicColorRamp pRamp = new AlgorithmicColorRampClass();
                pRamp.Size = 255;
                pRamp.FromColor = pFromColor;
                pRamp.ToColor = pToColor;
                bool createColorRamp;
                pRamp.CreateRamp(out createColorRamp);
                //Create a stretch renderer.
                IRasterStretchColorRampRenderer pStretchRenderer = new
                    RasterStretchColorRampRendererClass();
                IRasterRenderer pRasterRenderer = (IRasterRenderer)pStretchRenderer;
                //Set the renderer properties.
                IRaster pRaster = pRasterDataset.CreateDefaultRaster();
                pRasterRenderer.Raster = pRaster;
                pRasterRenderer.Update();
                pStretchRenderer.BandIndex = 0;
                pStretchRenderer.ColorRamp = pRamp;
                //Set the stretch type.
                IRasterStretch pStretchType = (IRasterStretch)pRasterRenderer;
                pStretchType.StretchType =
                    esriRasterStretchTypesEnum.esriRasterStretch_StandardDeviations;
                pStretchType.StandardDeviationsParam = 2;
                return pRasterRenderer;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private void 分类渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          ClassRender pClassRender=  new ClassRender (axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer,5, "PERIMETER");
        }

        private void pdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportPDF();
        }

        private void eMFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportEMF();
        }

    
       
      }
    }



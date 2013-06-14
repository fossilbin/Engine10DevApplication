using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;


namespace EngineApplication
{
    class QueryLayerTest
    {

        public  IFeatureLayer  OracleQueryLayer()
        {
            // 创建SqlWorkspaceFactory的对象
            Type pFactoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SqlWorkspaceFactory");

            IWorkspaceFactory pWorkspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(pFactoryType);
           

            // 构造连接数据库的参数
            IPropertySet pConnectionProps = new PropertySetClass();
            pConnectionProps.SetProperty("dbclient", "Oracle11g");
            pConnectionProps.SetProperty("serverinstance", "esri");
            pConnectionProps.SetProperty("authentication_mode", "DBMS");
            pConnectionProps.SetProperty("user", "scott");
            pConnectionProps.SetProperty("password", "arcgis");

            // 打开工作空间
            IWorkspace workspace = pWorkspaceFactory.Open(pConnectionProps, 0);

            ISqlWorkspace pSQLWorkspace = workspace as ISqlWorkspace;


       
            //获取数据库中的所有表的名称

           //IStringArray pStringArray= pSQLWorkspace.GetTables();

           //for (int i = 0; i < pStringArray.Count; i++)
           //{
           //    MessageBox.Show(pStringArray.get_Element(i));
                   
           //}

           // 构造过滤条件 SELECT * FROM PointQueryLayer

           IQueryDescription queryDescription = pSQLWorkspace.GetQueryDescription("SELECT * FROM TEST");

           ITable pTable = pSQLWorkspace.OpenQueryClass("QueryLayerTest", queryDescription);

           IFeatureLayer pFeatureLayer = new FeatureLayerClass();

           pFeatureLayer.FeatureClass = pTable as IFeatureClass;

           return pFeatureLayer;


            }
        }
    }


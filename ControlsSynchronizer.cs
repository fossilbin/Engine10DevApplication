// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at &lt;your ArcGIS install location&gt;/DeveloperKit10.0/userestrictions.txt.
// 

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace EngineApplication
{
  /// <summary>
  /// This class is used to synchronize a given PageLayoutControl and a MapControl.
  /// When initialized, the user must pass the reference of these control to the class, bind
  /// the control together by calling 'BindControls' which in turn sets a joined Map referenced
  /// by both control; and set all the buddy controls joined between these two controls.
  /// When alternating between the MapControl and PageLayoutControl, you should activate the visible control 
  /// and deactivate the other by calling ActivateXXX.
  /// This class is limited to a situation where the controls are not simultaneously visible. 
  /// </summary>
  public class ControlsSynchronizer
  {
    #region class members
    private IMapControl3 pMapControl = null;
    private IPageLayoutControl2 pPageLayoutControl = null;
    private ITool pMapActiveTool = null;
    private ITool pPageLayoutActiveTool = null;
    private bool pIsMapControlactive = true;

    private ArrayList pFrameworkControls = null;
    #endregion

    #region constructor

    /// <summary>
    /// 默认的构造函数
    /// </summary>
    public ControlsSynchronizer()
    {
      //initialize the underlying ArrayList
      pFrameworkControls = new ArrayList();
    }

    /// <summary>
    /// 在构造函数中传入地图控件和布局控件
    /// </summary>
    /// <param name="_MapControl"></param>
    /// <param name="_PageLayoutControl"></param>
    public ControlsSynchronizer(IMapControl3 _MapControl, IPageLayoutControl2 _PageLayoutControl)
      : this()
    {
      //assign the class members
      pMapControl = _MapControl;

      pPageLayoutControl = _PageLayoutControl;
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets or sets the MapControl
    /// </summary>
    public IMapControl3 MapControl
    {
      get { return pMapControl; }
      set { pMapControl = value; }
    }

    /// <summary>
    /// Gets or sets the PageLayoutControl
    /// </summary>
    public IPageLayoutControl2 PageLayoutControl
    {
      get { return pPageLayoutControl; }
      set { pPageLayoutControl = value; }
    }

    /// <summary>
    /// Get an indication of the type of the currently active view
    /// </summary>
    public string ActiveViewType
    {
      get
      {
        if (pIsMapControlactive)
          return "MapControl";
        else
          return "PageLayoutControl";
      }
    }

    /// <summary>
    /// get the active control
    /// </summary>
    public object ActiveControl
    {
      get
      {
        if (pMapControl == null || pPageLayoutControl == null)
          throw new Exception("ControlsSynchronizer::ActiveControl:\r\nEither MapControl or PageLayoutControl are not initialized!");

        if (pIsMapControlactive)
          return pMapControl.Object;
        else
          return pPageLayoutControl.Object;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// 激活地图控件并销毁布局控件
    /// </summary>
    public void ActivateMap()
    {
      try
      {
        if (pPageLayoutControl == null || pMapControl == null)
          throw new Exception("ControlsSynchronizer::ActivateMap:\r\nEither MapControl or PageLayoutControl are not initialized!");

        //cache the current tool of the PageLayoutControl
        if (pPageLayoutControl.CurrentTool != null) pPageLayoutActiveTool = pPageLayoutControl.CurrentTool;

        //deactivate the PagleLayout
        pPageLayoutControl.ActiveView.Deactivate();

        //activate the MapControl
        pMapControl.ActiveView.Activate(pMapControl.hWnd);

        //assign the last active tool that has been used on the MapControl back as the active tool
        if (pMapActiveTool != null) pMapControl.CurrentTool = pMapActiveTool;

        pIsMapControlactive = true;

        //on each of the framework controls, set the Buddy control to the MapControl
        this.SetBuddies(pMapControl.Object);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("ControlsSynchronizer::ActivateMap:\r\n{0}", ex.Message));
      }
    }

    /// <summary>
    /// 激活布局控件并销毁地图控件
    /// </summary>
    public void ActivatePageLayout()
    {
      try
      {
        if (pPageLayoutControl == null || pMapControl == null)
          throw new Exception("ControlsSynchronizer::ActivatePageLayout:\r\nEither MapControl or PageLayoutControl are not initialized!");

        //cache the current tool of the MapControl
        if (pMapControl.CurrentTool != null) pMapActiveTool = pMapControl.CurrentTool;

        //deactivate the MapControl
        pMapControl.ActiveView.Deactivate();

        //activate the PageLayoutControl
              pPageLayoutControl.ActiveView.Activate(pPageLayoutControl.hWnd);

        //assign the last active tool that has been used on the PageLayoutControl back as the active tool
        if (pPageLayoutActiveTool != null) pPageLayoutControl.CurrentTool = pPageLayoutActiveTool;

        pIsMapControlactive = false;

        //on each of the framework controls, set the Buddy control to the PageLayoutControl
        this.SetBuddies(pPageLayoutControl.Object);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("ControlsSynchronizer::ActivatePageLayout:\r\n{0}", ex.Message));
      }
    }

    /// <summary>
    /// 如果地图发生了变化，那么地图控件和布局控件的地图也应发生变化
    /// </summary>
    /// <param name="_NewMap"></param>
    public void ReplaceMap(IMap _NewMap)
    {
      if (_NewMap == null)
        throw new Exception("ControlsSynchronizer::ReplaceMap:\r\nNew map for replacement is not initialized!");

      if (pPageLayoutControl == null || pMapControl == null)
        throw new Exception("ControlsSynchronizer::ReplaceMap:\r\nEither MapControl or PageLayoutControl are not initialized!");

      //create a new instance of IMaps collection which is needed by the PageLayout
      IMaps pMaps = new Maps();
      //add the new map to the Maps collection
      pMaps.Add(_NewMap);

      bool bIsMapActive = pIsMapControlactive;

      //call replace map on the PageLayout in order to replace the focus map
      //we must call ActivatePageLayout, since it is the control we call 'ReplaceMaps'
      this.ActivatePageLayout();
      pPageLayoutControl.PageLayout.ReplaceMaps(pMaps);


      //assign the new map to the MapControl
      pMapControl.Map = _NewMap;

      //reset the active tools
      pPageLayoutActiveTool = null;
      pMapActiveTool = null;

      //make sure that the last active control is activated
      if (bIsMapActive)
      {
        this.ActivateMap();
        pMapControl.ActiveView.Refresh();
      }
      else
      {
        this.ActivatePageLayout();
        pPageLayoutControl.ActiveView.Refresh();
      }
    }

    /// <summary>
    /// 绑定地图控件和布局控件
    /// </summary>
    /// <param name="_MapControl"></param>
    /// <param name="_PageLayoutControl"></param>
    /// <param name="_ActivateMapFirst"></param>
    public void BindControls(IMapControl3 _MapControl, IPageLayoutControl2 _PageLayoutControl, bool _ActivateMapFirst)
    {
      if (_MapControl == null || _PageLayoutControl == null)
        throw new Exception("ControlsSynchronizer::BindControls:\r\nEither MapControl or PageLayoutControl are not initialized!");

      pMapControl = MapControl;

      pPageLayoutControl = _PageLayoutControl;

      this.BindControls(_ActivateMapFirst);
    }

    /// <summary>
    /// 当运行应用程序的时候，即便没有加载地图，则创建一个空的地图，让这两个控件和这个地图绑定在一起，这样就能保持一致
    /// </summary>
    /// <param name="activateMapFirst">true if the MapControl supposed to be activated first</param>
    public void BindControls(bool _ActivateMapFirst)
    {
      if (pPageLayoutControl == null || pMapControl == null)
        throw new Exception("ControlsSynchronizer::BindControls:\r\nEither MapControl or PageLayoutControl are not initialized!");

      //创建一个地图实例
      IMap pNewMap = new MapClass();

      pNewMap.Name = "Map";

      //其中Maps为我们创建的一个类，表示的是地图的集合
      IMaps pMaps = new Maps();
      //add the new Map instance to the Maps collection
      pMaps.Add(pNewMap);

      //call replace map on the PageLayout in order to replace the focus map
      pPageLayoutControl.PageLayout.ReplaceMaps(pMaps);
      //assign the new map to the MapControl
      pMapControl.Map = pNewMap;

      //reset the active tools
      pPageLayoutActiveTool = null;

      pMapActiveTool = null;

      //make sure that the last active control is activated
      if (_ActivateMapFirst)
        this.ActivateMap();
      else
        this.ActivatePageLayout();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_Control"></param>
    public void AddFrameworkControl(object _Control)
    {
      if (_Control == null)
        throw new Exception("ControlsSynchronizer::AddFrameworkControl:\r\nAdded control is not initialized!");

      pFrameworkControls.Add(_Control);
    }

    /// <summary>
    /// Remove a framework control from the managed list of controls
    /// </summary>
    /// <param name="control"></param>
    public void RemoveFrameworkControl(object _Control)
    {
      if (_Control == null)
        throw new Exception("ControlsSynchronizer::RemoveFrameworkControl:\r\nControl to be removed is not initialized!");

      pFrameworkControls.Remove(_Control);
    }

    /// <summary>
    /// Remove a framework control from the managed list of controls by specifying its index in the list
    /// </summary>
    /// <param name="index"></param>
    public void RemoveFrameworkControlAt(int _Index)
    {
      if (pFrameworkControls.Count < _Index)
        throw new Exception("ControlsSynchronizer::RemoveFrameworkControlAt:\r\nIndex is out of range!");

      pFrameworkControls.RemoveAt(_Index);
    }

    /// <summary>
    /// 当激活的控件发生变化时，IToolbarControl控件和ITOCControl控件的伙伴控件也应发生变化
    /// </summary>
    /// <param name="buddy">the active control</param>
    private void SetBuddies(object _buddy)
    {
      try
      {
        if (_buddy == null)
          throw new Exception("ControlsSynchronizer::SetBuddies:\r\nTarget Buddy Control is not initialized!");

        foreach (object obj in pFrameworkControls)
        {
          if (obj is IToolbarControl)
          {
            ((IToolbarControl)obj).SetBuddyControl(_buddy);
          }
          else if (obj is ITOCControl)
          {
            ((ITOCControl)obj).SetBuddyControl(_buddy);
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("ControlsSynchronizer::SetBuddies:\r\n{0}", ex.Message));
      }
    }
    #endregion
  }
}

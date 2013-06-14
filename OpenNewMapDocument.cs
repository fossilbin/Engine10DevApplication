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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;


namespace EngineApplication
{
  /// <summary>
  /// Summary description for OpenNewMapDocument.
  /// </summary>
  [Guid("5bf50443-f852-47cd-9c96-984184b6cca6")]
  [ClassInterface(ClassInterfaceType.None)]
  [ProgId("MapAndPageLayoutSynchApp.OpenNewMapDocument")]
  public sealed class OpenNewMapDocument : BaseCommand
  {
    #region COM Registration Function(s)
    [ComRegisterFunction()]
    [ComVisible(false)]
    static void RegisterFunction(Type registerType)
    {
      // Required for ArcGIS Component Category Registrar support
      ArcGISCategoryRegistration(registerType);

      //
      // TODO: Add any COM registration code here
      //
    }

    [ComUnregisterFunction()]
    [ComVisible(false)]
    static void UnregisterFunction(Type registerType)
    {
      // Required for ArcGIS Component Category Registrar support
      ArcGISCategoryUnregistration(registerType);

      //
      // TODO: Add any COM unregistration code here
      //
    }

    #region ArcGIS Component Category Registrar generated code
    /// <summary>
    /// Required method for ArcGIS Component Category registration -
    /// Do not modify the contents of this method with the code editor.
    /// </summary>
    private static void ArcGISCategoryRegistration(Type registerType)
    {
      string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
      ControlsCommands.Register(regKey);

    }
    /// <summary>
    /// Required method for ArcGIS Component Category unregistration -
    /// Do not modify the contents of this method with the code editor.
    /// </summary>
    private static void ArcGISCategoryUnregistration(Type registerType)
    {
      string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
      ControlsCommands.Unregister(regKey);

    }

    #endregion
    #endregion

    private IHookHelper m_hookHelper;
    private ControlsSynchronizer pControlsSynchronizer = null;
    private string m_sDocumentPath = string.Empty;

    public OpenNewMapDocument(ControlsSynchronizer controlsSynchronizer)
    {
      base.m_category = ".NET Samples";
      base.m_caption = "Open Map Document";
      base.m_message = "Open Map Document";
      base.m_toolTip = "Open Map Document";
      base.m_name = "DotNetSamplesOpenMapDocument";

      pControlsSynchronizer = controlsSynchronizer;

      try
      {
        string bitmapResourceName = GetType().Name + ".bmp";
        base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
      }
    }

    #region Overriden Class Methods

    /// <summary>
    /// Occurs when this command is created
    /// </summary>
    /// <param name="hook">Instance of the application</param>
    public override void OnCreate(object hook)
    {
      if (hook == null)
        return;

      if (m_hookHelper == null)
        m_hookHelper = new HookHelperClass();

      m_hookHelper.Hook = hook;

    }

    /// <summary>
    /// Occurs when this command is clicked
    /// </summary>
    public override void OnClick()
    {
      //launch a new OpenFile dialog
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = "Map Documents (*.mxd)|*.mxd";
      dlg.Multiselect = false;
      dlg.Title = "Open Map Document";
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        string docName = dlg.FileName;

        IMapDocument pMapDoc = new MapDocumentClass();

        if (pMapDoc.get_IsPresent(docName) && !pMapDoc.get_IsPasswordProtected(docName))
        {
          pMapDoc.Open(docName, string.Empty);

          // set the first map as the active view
          IMap map = pMapDoc.get_Map(0);
          pMapDoc.SetActiveView((IActiveView)map);

          pControlsSynchronizer.PageLayoutControl.PageLayout = pMapDoc.PageLayout;

          pControlsSynchronizer.ReplaceMap(map);

          pMapDoc.Close();

          m_sDocumentPath = docName;
        }
      }
    }

    #endregion

    public string DocumentFileName
    {
      get { return m_sDocumentPath;  }
    }
  }
}

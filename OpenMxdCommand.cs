using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;

namespace EngineApplication
{
    /// <summary>
    /// Summary description for OpenMxdCommand.
    /// </summary>
    [Guid("c142fea5-2e8e-4f68-95e1-9cad4a6a290e")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("EngineApplication.OpenMxdCommand")]
    public sealed class OpenMxdCommand : BaseCommand
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


        IMapControl2 pMapControl;

        public OpenMxdCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "打开地图文档"; //localizable text
            base.m_caption = "打开地图文档";  //localizable text
            base.m_message = "打开地图文档";  //localizable text 
            base.m_toolTip = "打开地图文档";  //localizable text 
            base.m_name = "打开地图文档";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;
            //在这里对hook进行判断

            if (hook is IToolbarControl)

            {
                IToolbarControl pToolBar= hook as IToolbarControl ;
                pMapControl = pToolBar.Buddy as IMapControl2;
            }
            else if(hook is IMapControl2 )
            {
                pMapControl = hook as IMapControl2;
            }
               


         
            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add OpenMxdCommand.OnClick implementation

            //launch a new OpenFile dialog
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.Filter = "Map Documents (*.mxd)|*.mxd";
            pOpenFileDialog.Multiselect = false;
            pOpenFileDialog.Title = "Open Map Document";
            if (pOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string docName = pOpenFileDialog.FileName;

                IMapDocument pMapDoc = new MapDocumentClass();
                if (pMapDoc.get_IsPresent(docName) && !pMapDoc.get_IsPasswordProtected(docName))
                {
                  

                    pMapControl.LoadMxFile(pOpenFileDialog.FileName, null, null);

                    // set the first map as the active view

                    pMapControl.ActiveView.Refresh();

                    pMapDoc.Close();

                    
                }
            }
        }

        #endregion
    }
}

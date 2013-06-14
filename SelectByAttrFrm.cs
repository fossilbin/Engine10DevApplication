using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace EngineApplication
{
    public partial class SelectByAttrFrm : DevComponents.DotNetBar.Office2007Form
    {
        public SelectByAttrFrm(MainForm mainFrm)
        {
            InitializeComponent();
            tempMainFrm = mainFrm;
           
        }

        MainForm tempMainFrm;
        IMap pMap;
        IFeatureLayer pFeatureLayer;
        ILayer pLayer;
        ILayerFields pLayerFields;
        IEnumLayer pEnumLayer;

        #region 鼠标单击/双击生成Whereclause的部分
        private void listBoxValues_DoubleClick(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " " + listBoxValues.SelectedItem.ToString();
        }

        private void buttonEqual_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " = ";
        }

        private void buttonNotEqual_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " <> ";
        }

        private void buttonBig_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " > ";
        }

        private void buttonBigEqual_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " >= ";
        }

        private void buttonSmall_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " < ";
        }

        private void buttonSmallEqual_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " <= ";
        }

        private void buttonChars_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = "%";
        }

        private void buttonChar_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = "_";
        }

        private void buttonLike_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " Like ";
        }

        private void buttonAnd_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " And ";
        }

        private void buttonOr_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " Or ";
        }

        private void buttonNot_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " Not ";
        }

        private void buttonIs_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = " Is ";
        }


        private void buttonBrace_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = "(  )";
            //让输入的位置恰好处在（）里面，就同arcmap的效果一样
            textBoxWhereClause.SelectionStart = textBoxWhereClause.Text.Length - 2;
        }
        #endregion
        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectByAttrFrm_Load(object sender, EventArgs e)
        {


            this.pMap = this.tempMainFrm.axMapControl1.Map;
            if (this.pMap.LayerCount == 0) return;
            this.pEnumLayer = this.pMap.get_Layers(null, true);
            if (pEnumLayer == null)
            {
                return;
            }
            pEnumLayer.Reset();
            for (this.pLayer = pEnumLayer.Next(); this.pLayer != null; this.pLayer = pEnumLayer.Next())
            {
                if (this.pLayer is IFeatureLayer)
                {
                    this.comboBoxLayers.Items.Add(this.pLayer.Name);
                }
            }
            if (this.comboBoxLayers.Items.Count == 0)
            {
                MessageBox.Show("没有可供选择的图层！");
                this.Close();
                return;
            }
            this.comboBoxLayers.SelectedIndex = 0;
            this.comboBoxMethod.SelectedIndex = 0;
        }
        /// <summary>
        /// 选择了别的图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.pEnumLayer == null) return;
            
            IField pField;
            int currentFieldType;
            this.pEnumLayer.Reset();
            this.listBoxFields.Items.Clear();
            for (this.pLayer = this.pEnumLayer.Next(); this.pLayer != null; this.pLayer = this.pEnumLayer.Next())
            {
                if (this.pLayer.Name != this.comboBoxLayers.Text) continue;
                this.pLayerFields = this.pLayer as ILayerFields;//强制转化，似乎有什么规则。Java中的东西很像。
                for (int i = 0; i < this.pLayerFields.FieldCount; i++)
                {
                    pField = this.pLayerFields.get_Field(i);
                    currentFieldType = (int)pField.Type;
                    if (currentFieldType > 5) continue;//不是可以查询的字段类型
                    this.listBoxFields.Items.Add(pField.Name);
                }
                break;
            }
            this.pFeatureLayer = this.pLayer as IFeatureLayer;
            this.labelLayer.Text = this.comboBoxLayers.Text;
        }

        /// <summary>
        /// 双击字段名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxFields_DoubleClick(object sender, EventArgs e)
        {
            textBoxWhereClause.SelectedText = listBoxFields.SelectedItem.ToString() + " ";
        }
        /// <summary>
        /// 获得当前字段的唯一值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetValue_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listBoxFields.SelectedIndex == -1) return;

                string currentFieldName = this.listBoxFields.Text;//当前字段名
                string currentLayerName=this.comboBoxLayers.Text;
                this.pEnumLayer.Reset();
                for (this.pLayer = this.pEnumLayer.Next(); this.pLayer != null; this.pLayer = this.pEnumLayer.Next())
                {
                    if (this.pLayer.Name == currentLayerName) break;
                }
                this.pLayerFields = this.pLayer as ILayerFields;
                IField pField = this.pLayerFields.get_Field(this.pLayerFields.FindField(currentFieldName));
                esriFieldType pFieldType = pField.Type;

                //对Table中当前字段进行排序,把结果赋给Cursor
                ITable pTable = this.pLayer as ITable;
                ITableSort pTableSort = new TableSortClass();
                pTableSort.Table = pTable;
                pTableSort.Fields = currentFieldName;
                pTableSort.set_Ascending(currentFieldName, true);
                pTableSort.set_CaseSensitive(currentFieldName, true);//这两句的意思我还不懂，照猫画虎写的。
                pTableSort.Sort(null);//排序
                ICursor pCursor = pTableSort.Rows;

                //字段统计
                IDataStatistics pDataStatistics = new DataStatisticsClass();
                pDataStatistics.Cursor = pCursor;
                pDataStatistics.Field = currentFieldName;
                System.Collections.IEnumerator pEnumeratorUniqueValues = pDataStatistics.UniqueValues;//唯一值枚举
                int uniqueValueCount = pDataStatistics.UniqueValueCount;//唯一值的个数

                this.listBoxValues.Items.Clear();
                string currentValue = null;
                pEnumeratorUniqueValues.Reset();

                //别人的答案

                if (pFieldType == esriFieldType.esriFieldTypeString)
                {
                    while (pEnumeratorUniqueValues.MoveNext())
                    {
                        currentValue = pEnumeratorUniqueValues.Current.ToString();
                        this.listBoxValues.Items.Add("'" + currentValue + "'");
                    }
                }
                else
                {
                    while (pEnumeratorUniqueValues.MoveNext())
                    {
                        currentValue = pEnumeratorUniqueValues.Current.ToString();
                        this.listBoxValues.Items.Add(currentValue);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxWhereClause.Clear();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (textBoxWhereClause.Text == "")
            {
                MessageBox.Show("请生成查询语句！");
                return;
            }
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();

                pQueryFilter.WhereClause = textBoxWhereClause.Text;
                
                IFeatureSelection pFeatureSelection = this.pFeatureLayer as IFeatureSelection;

             //   int iSelectedFeaturesCount = pFeatureSelection.SelectionSet.Count;
                esriSelectionResultEnum selectMethod;
                switch (comboBoxMethod.SelectedIndex)
                {
                    case 0: selectMethod = esriSelectionResultEnum.esriSelectionResultNew; break;
                    case 1: selectMethod = esriSelectionResultEnum.esriSelectionResultAdd; break;
                    case 2: selectMethod = esriSelectionResultEnum.esriSelectionResultSubtract; break;
                    case 3: selectMethod = esriSelectionResultEnum.esriSelectionResultAnd; break;
                    default: selectMethod = esriSelectionResultEnum.esriSelectionResultNew; break;
                }
                pFeatureSelection.SelectFeatures(pQueryFilter, selectMethod, false);//执行查询

                //如果本次查询后，查询的结果数目没有改变，则认为本次查询没有产生新的结果
                if (pFeatureSelection.SelectionSet.Count == 0 )//|| pFeatureSelection.SelectionSet.Count == 0)
                {
                    MessageBox.Show("没有符合本次查询条件的结果！");
                    return;
                }

                ////如果复选框被选中，则定位到选择结果
                //if (checkBoxZoomtoSelected.Checked == true)
                //{
                //    IEnumFeature pEnumFeature = MainAxMapControl.Map.FeatureSelection as IEnumFeature;
                //    IFeature pFeature = pEnumFeature.Next();
                //    IEnvelope pEnvelope = new EnvelopeClass();
                //    while (pFeature != null)
                //    {
                //        pEnvelope.Union(pFeature.Extent);
                //        pFeature = pEnumFeature.Next();
                //    }
                //    MainAxMapControl.ActiveView.Extent = pEnvelope;
                //    MainAxMapControl.ActiveView.Refresh();//如果不这样刷新，只要查询前地图已经被放大所效果的话，定位后
                //    //底图没有刷新，选择集倒是定位和刷新了
                //}
                //else MainAxMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                this.tempMainFrm.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);

                //double i = MainAxMapControl.Map.SelectionCount;
                //i = Math.Round(i, 0);//小数点后指定为０位数字
                //pMainform.toolStripStatusLabel1.Text = "当前共有" + i.ToString() + "个查询结果";
            }
            catch (Exception ex)
            {
                MessageBox.Show("您的查询语句可能有误,请检查 | " + ex.Message);
                return;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.buttonAnd_Click(sender, e);
            this.Dispose();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void listBoxFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBoxValues.Items.Clear();
        }

        
    }
}
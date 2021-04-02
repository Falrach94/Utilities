using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogUtils.UI
{
    public partial class LogSelector : Form
    {
        private Dictionary<string, CategoryConfiguration> _categoryDic;

        CategoryConfiguration _category;

        public LogSelector()
        {
            InitializeComponent();
            LostFocus += LogSelector_LostFocus;
        }

        private void LogSelector_LostFocus(object sender, EventArgs e)
        {
        }

        private void LogSelector_Load(object sender, EventArgs e)
        {

        }

        internal void SetCategories(Dictionary<string, CategoryConfiguration> categoryDic)
        {
            _categoryDic = categoryDic;
            list_category.Items.Clear();
            foreach (var c in categoryDic.Values)
            {
                list_category.Items.Add(c.Name);
            }
        }

        private void Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = list_category.SelectedIndex;
            if (index != -1)
            {
                var category = _categoryDic.Values.ToArray()[index];
                _category = category;
                cb_level.SelectedItem = category.MinLevel.ToString();
                cb_show.Checked = category.Show;
            }
        }

        private void Show_CheckedChanged(object sender, EventArgs e)
        {
            if (_category != null)
            {
                _category.Show = cb_show.Checked;
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

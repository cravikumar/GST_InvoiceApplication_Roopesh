using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GST_InvoiceApplication
{
    public partial class AutoCompleteCombobox : ComboBox
    {
        public AutoCompleteCombobox()
        {
            //InitializeComponent();
        }
        public IList<object> m_collectionList = null;


        public AutoCompleteCombobox(IContainer container)
        {
            container.Add(this);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            if (m_collectionList == null)
            {
                m_collectionList = this.Items.OfType<object>().ToList();
            }

            IList<object> values = m_collectionList
                .Where(x => x.ToString().ToLower().Contains(Text.ToLower()))
                .ToList<object>();

            this.Items.Clear();
            this.Items.AddRange(this.Text != string.Empty ? values.ToArray() : m_collectionList.ToArray());

            this.SelectionStart = this.Text.Length;
            this.DroppedDown = true;
            this.Cursor = Cursors.Default;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Text != string.Empty) return;
            this.Items.Clear();
            this.Items.AddRange(m_collectionList.ToArray());
        }

    }
}
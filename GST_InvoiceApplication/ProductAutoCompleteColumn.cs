using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Windows.Forms;
using SergeUtils;

namespace GST_InvoiceApplication
{
    public class ProductAutoCompleteColumn : DataGridViewColumn
    {
        public static Func<String[]> ProductNamesProvider { get; set; }

        public ProductAutoCompleteColumn() : base(new ProductAutoCompleteCell())
        { }

        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if(value != null && !(value is ProductAutoCompleteCell))
                {
                    throw new InvalidCastException("Cell template must be a ProductAutoCompleteCell");

                }
                base.CellTemplate = value;

            }


        }
    }

    public class ProductAutoCompleteCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(ProductAutoCompleteEditingControl);

        public override Type ValueType => typeof(string);

        public override object DefaultNewRowValue => string.Empty;

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if(DataGridView.EditingControl is ProductAutoCompleteEditingControl control)
            {

                control.SuggestionSource = ProductAutoCompleteColumn.ProductNamesProvider?.Invoke();
                control.MatchingMethod = StringMatchingMethod.UseRegexs;
                control.Text = initialFormattedValue?.ToString() ?? string.Empty;
            }
        }

        public override object Clone()
        {
            return (ProductAutoCompleteCell)base.Clone();
        }

    }

    public class ProductAutoCompleteEditingControl :EasyCompletionComboBox, IDataGridViewEditingControl
    {
        private DataGridView _dataGridView;
        private bool _valueChanged;
        private int _rowIndex;

        public ProductAutoCompleteEditingControl()
        {
            MatchingMethod = StringMatchingMethod.UseRegexs;
        }

        public object EditingControlFormattedValue
        {
            get => Text;
            set => Text = value?.ToString() ?? string.Empty;
        }

        public DataGridView EditingControlDataGridView
        {
            get => _dataGridView;
            set => _dataGridView = value;
        }

        public int EditingControlRowIndex
        {
            get => _rowIndex;
            set => _rowIndex = value;
        }

        public bool EditingControlValueChanged
        {
            get => _valueChanged;
            set => _valueChanged = value;
        }

        public Cursor EditingPanelCursor => base.Cursor;
        public bool RepositionEditingControlOnValueChange => false;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            ForeColor = dataGridViewCellStyle.ForeColor;
            BackColor = dataGridViewCellStyle.BackColor;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if(selectAll)
            {
                SelectAll();
            }
        }


        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch(key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Home:
                case Keys.End:
                case Keys.PageUp:
                case Keys.PageDown:
                    return true;

                case Keys.Enter:
                case Keys.Escape:
                    return DroppedDown;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            
            if(keyData == Keys.Tab && (DroppedDown || Text.Length>0))
            {
                base.OnKeyDown(new KeyEventArgs(Keys.Enter));
                return true;
            }

            bool commitEnter = keyData == Keys.Enter && Text.Length > 0;
            if(commitEnter || (DroppedDown && (keyData == Keys.Enter || keyData == Keys.Escape)))
            {
                base.OnKeyDown(new KeyEventArgs(keyData));
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            _valueChanged = true;
            _dataGridView?.NotifyCurrentCellDirty(true);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            _valueChanged = true;
            _dataGridView?.NotifyCurrentCellDirty(true);
        }


    }
}

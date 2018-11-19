using System;
using System.Windows.Forms;
using System.Drawing;
using Model;

namespace Test_Table_View
{
    public class ComboBoxCustom : ComboBox
    {
        public ComboBoxCustom()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            if (e.Index < 0) { return; }
            e.DrawBackground();
            int date = Convert.ToInt16(Name[3]) - 48;
            int part = Convert.ToInt16(Name[4]) - 48;
            int index = Convert.ToInt16(Name[5]) - 48;
            Time time = new Time(date, part);

            ComboBoxItem item = (ComboBoxItem)Items[e.Index];
            
            Brush brush = new SolidBrush(item.ForeColor);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { brush = Brushes.Yellow; }
            e.Graphics.DrawString(item.Text, this.Font, brush, e.Bounds.X, e.Bounds.Y);
        }
    }
    public class ComboBoxItem
    {
        public ComboBoxItem() { }

        public ComboBoxItem(string pText, object pValue)
        {
            text = pText; val = pValue;
        }

        public ComboBoxItem(string pText, object pValue, Color pColor)
        {
            text = pText; val = pValue; foreColor = pColor;
        }

        string text = "";
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        object val;
        public object Value
        {
            get { return val; }
            set { val = value; }
        }

        Color foreColor = Color.Black;
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        public override string ToString()
        {
            return text;
        }
    }
}

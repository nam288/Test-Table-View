using System;
using System.Drawing;
using System.Windows.Forms;
using Model;

namespace Test_Table_View
{
    public partial class Form1 : Form
    {
        internal Data data;
        internal Schedule schedule;
        internal Random random;

        public Form1()
        {
            random = new Random();
            data = new Data(40, random);
            data.GenerateOperation(random);
            data.GenerateShift(random);

            InitializeComponent();
            GenerateTable(mainPnl);

            schedule = new Schedule(data, this);
            //schedule.Main();
        }

        public void GenerateCell(TableLayoutPanel pnl, int row, int col)
        {
            pnl.Controls.Clear();
            pnl.ColumnStyles.Clear();
            pnl.RowStyles.Clear();
            pnl.ColumnCount = 1;
            pnl.RowCount = row;
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            for (int index = 0; index < row; index++)
            {
                Time time = new Time(col, row == 5 ? 0 : 1);
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                ComboBox cmb = new ComboBox
                {
                    FlatStyle = FlatStyle.System,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
                    Size = new Size(30, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Name = string.Format("cmb{0}{1}{2}", time.date, time.part, index)
                };
                cmb.Click += new EventHandler((object o, EventArgs e) =>
                {
                    if (cmb.Items.Count == 0)
                        return;
                    var select = cmb.SelectedItem.ToString();
                    cmb.Items.Clear();
                    cmb.Items.Add(select);
                    var tmp = schedule.FreemanInTime(time);
                    tmp.Sort();
                    foreach (var item in tmp)
                    {
                        if (item.ToString() != select)
                            cmb.Items.Add(item);
                    }
                    cmb.SelectedIndex = 0;
                });

                cmb.SelectedIndexChanged += new EventHandler((object o, EventArgs e) =>
                {
                    int date = Convert.ToInt16(cmb.Name[3]) - 48;
                    int part = Convert.ToInt16(cmb.Name[4]) - 48;
                    int i = Convert.ToInt16(cmb.Name[5]) - 48;
                    Console.WriteLine(cmb.Name);
                    if (schedule.primary[date,part, i])
                    {
                        schedule.primary[date, part, i] = false;
                        schedule.Set(new Time(date, part), i, Convert.ToInt16(cmb.SelectedItem));
                    };
                });

                pnl.Controls.Add(cmb, 0, index);
            }
        }

        public void Set(Time t, int index, int value)
        {
            TableLayoutPanel dateCell = mainPnl.GetControlFromPosition(t.date, t.part + 1) as TableLayoutPanel;
            ComboBox cmb = dateCell.GetControlFromPosition(0, index) as ComboBox;
            cmb.Items.Clear();
            cmb.Items.Add(schedule._schedule[t.date][t.part][index]);
            schedule.primary[t.date, t.part, index] = false;
            cmb.SelectedIndex = 0;
            schedule.primary[t.date, t.part, index] = true;
        }

        public void GenerateTable(TableLayoutPanel pnl)
        {

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 3; row++)
                {

                    if (col >= 1 && col <= 5 && row >= 1 && row <= 2)
                    {
                        var pnlChild = new TableLayoutPanel();
                        GenerateCell(pnlChild, 6 - row, col);

                        pnl.Controls.Add(pnlChild, col, row);
                    }
                }
            }
        }

        private void runBtn_Click(object sender, EventArgs e)
        {
            schedule.Main();
        }
    }
}

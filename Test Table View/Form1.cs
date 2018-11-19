using System;
using System.Drawing;
using System.Windows.Forms;
using Model;

namespace Test_Table_View
{
    public partial class CustomForm : Form
    {
        internal Data _data;
        internal Schedule _schedule;
        internal Random random;
        public readonly int rowMainPnl = 3, colMainPnl = 6;

        public CustomForm()
        {
            random = new Random();
            _data = new Data(40, random);
            _data.GenerateOperation(random);
            _data.GenerateShift(random);

            _schedule = new Schedule(_data, this);

            InitializeComponent();
            GenerateTable(mainPnl);
            GenerateTreeView();
        }

        public void GenerateTreeView()
        {
            foreach (var dep in _data.depts)
            {
                TreeNode depNode = new TreeNode(dep.name);
                foreach (var doctor in dep.doctors)
                    depNode.Nodes.Add(new TreeNode(doctor.name));
                treeView.Nodes.Add(depNode);
            }
        }

        public void GenerateCell(TableLayoutPanel pnl, int dateCell, int partCell)
        {
            pnl.Controls.Clear();
            pnl.ColumnStyles.Clear();
            pnl.RowStyles.Clear();
            pnl.ColumnCount = 1;
            pnl.RowCount = _schedule.amount[dateCell][partCell];
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            for (int indexInCell = 0; indexInCell < pnl.RowCount; indexInCell++)
            {
                Time timeCell = new Time(dateCell, partCell);
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                ComboBoxCustom cmb = new ComboBoxCustom
                {
                    FlatStyle = FlatStyle.System,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
                    Size = new Size(30, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Name = string.Format("cmb{0}{1}{2}", timeCell.date, timeCell.part, indexInCell)
                };
                cmb.Click += new EventHandler((object o, EventArgs e) =>
                {
                    //if (cmb.Items.Count == 0)
                    //    return;
                    //var select = cmb.SelectedItem.ToString();
                    //cmb.Items.Clear();
                    //cmb.Items.Add(select);
                    //var tmp = _schedule.AvailableMen(timeCell);
                    //tmp.Sort();
                    //foreach (var item in tmp)
                    //{
                    //    if (item.ToString() != select)
                    //        cmb.Items.Add(item);
                    //}
                    //cmb.SelectedIndex = 0;

                });

                cmb.SelectedIndexChanged += new EventHandler((object o, EventArgs e) =>
                {
                    int date = Convert.ToInt16(cmb.Name[3]) - 48;
                    int part = Convert.ToInt16(cmb.Name[4]) - 48;
                    int index = Convert.ToInt16(cmb.Name[5]) - 48;
                    Console.WriteLine(_schedule.isPrimary[date][part][index]);
                    if (_schedule.isPrimary[date][part][index])
                    {
                        _schedule.isPrimary[date][part][index] = false;
                        _schedule.SetSchedule(new Time(date, part), index, _data.GetDoctor(cmb.SelectedItem.ToString()).index);
                        _schedule.Refresh();
                    };
                });

                pnl.Controls.Add(cmb, 0, indexInCell);
            }
        }

        public void Set(Time t, int index, int value)
        {
            var cmb = GetComboBox(t, index);
            cmb.Items.Clear();
            //cmb.Items.Add(_data.GetDoctor(value).name);
            //cmb.Items.Add(new ComboBoxItem(_data.GetDoctor(value).name,"0",Color.Green));
            cmb.Items.Add(_schedule.ColorComboBoxItem(t, value, 0));
            _schedule.isPrimary[t.date][t.part][index] = false;
            cmb.SelectedIndex = 0;
            _schedule.isPrimary[t.date][t.part][index] = true;
        }

        public ComboBoxCustom GetComboBox(Time t, int index)
        {
            TableLayoutPanel timeCell = mainPnl.GetControlFromPosition(t.date, t.part + 1) as TableLayoutPanel;
            return timeCell.GetControlFromPosition(0, index) as ComboBoxCustom;
        }

        public void GenerateTable(TableLayoutPanel pnl)
        {

            for (int colIndex = 0; colIndex < colMainPnl; colIndex++)
                for (int rowIndex = 0; rowIndex < rowMainPnl; rowIndex++)
                    if (colIndex > 0 && rowIndex > 0)
                    {
                        var pnlChild = new TableLayoutPanel();
                        GenerateCell(pnlChild, colIndex, rowIndex - 1);
                        pnl.Controls.Add(pnlChild, colIndex, rowIndex);
                    }

        }

        private void runBtn_Click(object sender, EventArgs e)
        {
            _schedule.Main();
        }
    }
}

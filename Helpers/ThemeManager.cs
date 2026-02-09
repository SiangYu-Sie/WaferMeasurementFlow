using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WaferMeasurementFlow.Helpers
{
    /// <summary>
    /// 統一的專業深色主題管理器 - Industrial Professional Dark Theme
    /// </summary>
    public static class ThemeManager
    {
        // === Primary Colors ===
        public static readonly Color BgPrimary = Color.FromArgb(18, 18, 22);
        public static readonly Color BgSecondary = Color.FromArgb(28, 28, 35);
        public static readonly Color BgCard = Color.FromArgb(38, 40, 48);
        public static readonly Color BgCardHover = Color.FromArgb(48, 50, 60);
        public static readonly Color BgInput = Color.FromArgb(25, 27, 32);

        // === Border Colors ===
        public static readonly Color BorderColor = Color.FromArgb(55, 60, 70);
        public static readonly Color BorderFocus = Color.FromArgb(59, 130, 246);

        // === Text Colors ===
        public static readonly Color TextPrimary = Color.FromArgb(240, 242, 245);
        public static readonly Color TextSecondary = Color.FromArgb(140, 145, 160);
        public static readonly Color TextMuted = Color.FromArgb(90, 95, 110);
        public static readonly Color TextDisabled = Color.FromArgb(70, 75, 85);

        // === Status Colors ===
        public static readonly Color StatusGreen = Color.FromArgb(34, 197, 94);
        public static readonly Color StatusYellow = Color.FromArgb(250, 204, 21);
        public static readonly Color StatusRed = Color.FromArgb(239, 68, 68);
        public static readonly Color StatusBlue = Color.FromArgb(59, 130, 246);
        public static readonly Color StatusGray = Color.FromArgb(100, 105, 120);

        // === Accent Color ===
        public static readonly Color Accent = StatusBlue;

        public static void ApplyTheme(Form form)
        {
            form.BackColor = BgPrimary;
            form.ForeColor = TextPrimary;
            form.Font = new Font("Segoe UI", 9.5F);

            ApplyToControls(form.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                ApplyStyle(c);
                if (c.HasChildren)
                {
                    ApplyToControls(c.Controls);
                }
            }
        }

        private static void ApplyStyle(Control c)
        {
            c.Font = new Font("Segoe UI", 9.5F);

            if (c is Button btn)
            {
                StyleButton(btn);
            }
            else if (c is GroupBox gb)
            {
                StyleGroupBox(gb);
            }
            else if (c is Label lbl)
            {
                StyleLabel(lbl);
            }
            else if (c is TextBox txt)
            {
                StyleTextBox(txt);
            }
            else if (c is ComboBox combo)
            {
                StyleComboBox(combo);
            }
            else if (c is ListBox lst)
            {
                StyleListBox(lst);
            }
            else if (c is CheckedListBox clb)
            {
                StyleCheckedListBox(clb);
            }
            else if (c is DataGridView grid)
            {
                StyleDataGridView(grid);
            }
            else if (c is Panel pnl)
            {
                pnl.BackColor = Color.Transparent;
            }
            else if (c is TableLayoutPanel tlp)
            {
                tlp.BackColor = Color.Transparent;
            }
            else if (c is RichTextBox rtb)
            {
                StyleRichTextBox(rtb);
            }
        }

        private static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = BorderColor;
            btn.BackColor = BgCard;
            btn.ForeColor = TextPrimary;
            btn.Font = new Font("Segoe UI Semibold", 9.5F);
            btn.Cursor = Cursors.Hand;
            btn.Height = 36;

            // Primary action buttons
            if (btn.Name.Contains("Create") || btn.Name.Contains("Execute") ||
                btn.Name.Contains("Save") || btn.Name.Contains("Start") ||
                btn.Name.Contains("Monitor") || btn.Name.Contains("OK"))
            {
                btn.BackColor = StatusBlue;
                btn.FlatAppearance.BorderColor = StatusBlue;
                btn.ForeColor = Color.White;
            }
            // Danger buttons
            else if (btn.Name.Contains("Delete") || btn.Name.Contains("Remove") || btn.Name.Contains("Stop"))
            {
                btn.BackColor = Color.FromArgb(60, StatusRed);
                btn.FlatAppearance.BorderColor = StatusRed;
                btn.ForeColor = StatusRed;
            }

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                if (btn.Enabled)
                    btn.BackColor = Color.FromArgb(Math.Min(btn.BackColor.R + 20, 255),
                                                    Math.Min(btn.BackColor.G + 20, 255),
                                                    Math.Min(btn.BackColor.B + 20, 255));
            };
            btn.MouseLeave += (s, e) =>
            {
                // Reset to original - simplified by re-applying
                if (btn.Name.Contains("Create") || btn.Name.Contains("Execute") ||
                    btn.Name.Contains("Save") || btn.Name.Contains("Start") ||
                    btn.Name.Contains("Monitor") || btn.Name.Contains("OK"))
                    btn.BackColor = StatusBlue;
                else if (btn.Name.Contains("Delete") || btn.Name.Contains("Remove") || btn.Name.Contains("Stop"))
                    btn.BackColor = Color.FromArgb(60, StatusRed);
                else
                    btn.BackColor = BgCard;
            };

            // Custom disabled text rendering
            btn.Paint += (s, e) =>
            {
                Button b = (Button)s!;
                if (!b.Enabled)
                {
                    e.Graphics.Clear(BgCard);
                    TextRenderer.DrawText(e.Graphics, b.Text, b.Font, b.ClientRectangle,
                        TextMuted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            };
        }

        private static void StyleGroupBox(GroupBox gb)
        {
            gb.ForeColor = TextPrimary;
            gb.BackColor = Color.Transparent;
            gb.Font = new Font("Segoe UI Semibold", 10F);
            gb.Padding = new Padding(10);

            // Custom paint for modern look
            gb.Paint += (s, e) =>
            {
                GroupBox g = (GroupBox)s!;
                e.Graphics.Clear(BgPrimary);

                var textSize = e.Graphics.MeasureString(g.Text, g.Font);
                var rect = new Rectangle(0, (int)(textSize.Height / 2), g.Width - 1, g.Height - (int)(textSize.Height / 2) - 1);

                using (var pen = new Pen(BorderColor, 1))
                    e.Graphics.DrawRectangle(pen, rect);

                using (var brush = new SolidBrush(BgPrimary))
                    e.Graphics.FillRectangle(brush, 8, 0, textSize.Width + 4, textSize.Height);

                using (var brush = new SolidBrush(TextPrimary))
                    e.Graphics.DrawString(g.Text, g.Font, brush, 10, 0);
            };
        }

        private static void StyleLabel(Label lbl)
        {
            lbl.ForeColor = TextPrimary;
            lbl.BackColor = Color.Transparent;

            if (lbl.Name.Contains("Title") || lbl.Font.Size > 10)
            {
                lbl.ForeColor = Color.White;
                lbl.Font = new Font("Segoe UI Semibold", lbl.Font.Size);
            }
            else if (lbl.Name.Contains("Desc") || lbl.Name.Contains("Sub"))
            {
                lbl.ForeColor = TextSecondary;
            }
        }

        private static void StyleTextBox(TextBox txt)
        {
            txt.BackColor = BgInput;
            txt.ForeColor = TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = new Font("Segoe UI", 10F);
        }

        private static void StyleComboBox(ComboBox combo)
        {
            combo.BackColor = BgInput;
            combo.ForeColor = TextPrimary;
            combo.FlatStyle = FlatStyle.Flat;
            combo.Font = new Font("Segoe UI", 10F);
        }

        private static void StyleListBox(ListBox lst)
        {
            lst.BackColor = BgCard;
            lst.ForeColor = TextPrimary;
            lst.BorderStyle = BorderStyle.None;
            lst.Font = new Font("Segoe UI", 10F);
        }

        private static void StyleCheckedListBox(CheckedListBox clb)
        {
            clb.BackColor = BgCard;
            clb.ForeColor = TextPrimary;
            clb.BorderStyle = BorderStyle.None;
            clb.Font = new Font("Segoe UI", 10F);
        }

        private static void StyleRichTextBox(RichTextBox rtb)
        {
            rtb.BackColor = Color.FromArgb(12, 12, 16);
            rtb.ForeColor = Color.FromArgb(180, 190, 200);
            rtb.BorderStyle = BorderStyle.None;
            rtb.Font = new Font("Consolas", 9.5F);
        }

        private static void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = BgCard;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = BorderColor;
            grid.EnableHeadersVisualStyles = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Header Style
            grid.ColumnHeadersDefaultCellStyle.BackColor = BgSecondary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = BgSecondary;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 40;

            // Row Style
            grid.DefaultCellStyle.BackColor = BgCard;
            grid.DefaultCellStyle.ForeColor = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(40, StatusBlue);
            grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.DefaultCellStyle.Padding = new Padding(10, 6, 10, 6);
            grid.RowTemplate.Height = 36;

            // Alternating Rows
            grid.AlternatingRowsDefaultCellStyle.BackColor = BgCardHover;
        }

        /// <summary>
        /// 建立帶有標題的區塊面板
        /// </summary>
        public static Panel CreateSectionPanel(string title, int height = 0)
        {
            var panel = new Panel
            {
                BackColor = BgCard,
                Margin = new Padding(0, 0, 0, 10),
                Dock = height == 0 ? DockStyle.Fill : DockStyle.Top,
                Height = height == 0 ? 100 : height,
                Padding = new Padding(15, 45, 15, 15)
            };
            panel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(BorderColor, 1))
                    g.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                using (var brush = new SolidBrush(TextPrimary))
                    g.DrawString(title, new Font("Segoe UI Semibold", 10F), brush, 15, 15);
            };
            return panel;
        }
    }
}

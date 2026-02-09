using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WaferMeasurementFlow.UI
{
    // 自定義狀態指示燈
    public class StatusIndicator : Control
    {
        private string _title;
        private string _value = "";
        private Color _color = IndTheme.StatusGray;

        public StatusIndicator(string title, string value, Color color)
        {
            _title = title;
            _value = value;
            _color = color;
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        public void SetStatus(string value, Color color)
        {
            _value = value;
            _color = color;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 背景
            using (var brush = new SolidBrush(IndTheme.BgCardHover))
            {
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.FillRectangle(brush, rect);
            }

            // 左側裝飾條
            using (var brush = new SolidBrush(_color))
                g.FillRectangle(brush, 0, 0, 4, Height);

            // 右側圓形燈號
            int lampSize = 16;
            int lampX = Width - lampSize - 15;
            int lampY = (Height - lampSize) / 2;
            using (var brush = new SolidBrush(Color.FromArgb(50, _color)))
                g.FillEllipse(brush, lampX - 4, lampY - 4, lampSize + 8, lampSize + 8);
            using (var brush = new SolidBrush(_color))
                g.FillEllipse(brush, lampX, lampY, lampSize, lampSize);

            // 標題 (左上)
            using (var brush = new SolidBrush(IndTheme.TextSecondary))
                g.DrawString(_title, new Font("Segoe UI", 9F), brush, 15, 12);

            // 數值 (左下)
            using (var brush = new SolidBrush(IndTheme.TextPrimary))
                g.DrawString(_value, new Font("Segoe UI Semibold", 13F), brush, 15, 35);
        }
    }

    // 自定義操作按鈕 (實心填滿風格)
    public class ActionButton : Control
    {
        private Color _accentColor;
        private bool _hovering = false;
        private bool _pressed = false;

        public ActionButton(string text, Color accent)
        {
            Text = text;
            _accentColor = accent;
            Size = new Size(130, 40);
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
        }

        protected override void OnMouseEnter(EventArgs e) { _hovering = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hovering = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _pressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _pressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color bg, fg;

            if (!Enabled)
            {
                bg = Color.FromArgb(60, 60, 70);
                fg = Color.FromArgb(120, 120, 130);
            }
            else if (_pressed)
            {
                // 按下時顏色變深
                bg = ControlPaint.Dark(_accentColor, 0.15f);
                fg = Color.White;
            }
            else if (_hovering)
            {
                // Hover 時顏色變亮
                bg = ControlPaint.Light(_accentColor, 0.1f);
                fg = Color.White;
            }
            else
            {
                // 正常狀態: 實心填滿 accent color
                bg = _accentColor;
                fg = Color.White;
            }

            // 繪製圓角背景
            using (var brush = new SolidBrush(bg))
            {
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.FillRectangle(brush, rect);
            }

            // 繪製文字 (置中)
            var textSize = TextRenderer.MeasureText(Text, Font);
            TextRenderer.DrawText(g, Text, Font,
                new Point((Width - textSize.Width) / 2, (Height - textSize.Height) / 2), fg);
        }
    }

    // 區塊面板 (帶標題)
    public class SectionPanel : Panel
    {
        public string Title { get; set; }

        public SectionPanel()
        {
            BackColor = IndTheme.BgCard;
            Padding = new Padding(1); // 避免內容壓到邊框
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 邊框
            using (var pen = new Pen(IndTheme.BorderColor, 1))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            // 標題
            if (!string.IsNullOrEmpty(Title))
            {
                using (var brush = new SolidBrush(IndTheme.TextPrimary))
                    g.DrawString(Title, IndTheme.SubHeaderFont, brush, 15, 15);
            }

            base.OnPaint(e);
        }
    }
}

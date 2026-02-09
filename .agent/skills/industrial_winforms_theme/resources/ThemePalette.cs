using System.Drawing;

namespace WaferMeasurementFlow.UI
{
    public static class IndTheme
    {
        // 背景色系
        public static readonly Color BgPrimary = Color.FromArgb(18, 18, 22);
        public static readonly Color BgSecondary = Color.FromArgb(28, 28, 35);
        public static readonly Color BgCard = Color.FromArgb(38, 40, 48);
        public static readonly Color BgCardHover = Color.FromArgb(48, 50, 60);
        public static readonly Color BorderColor = Color.FromArgb(55, 60, 70);

        // 文字色系
        public static readonly Color TextPrimary = Color.FromArgb(240, 242, 245);
        public static readonly Color TextSecondary = Color.FromArgb(140, 145, 160);
        public static readonly Color TextMuted = Color.FromArgb(90, 95, 110);

        // 狀態色系
        public static readonly Color StatusGreen = Color.FromArgb(34, 197, 94);
        public static readonly Color StatusYellow = Color.FromArgb(250, 204, 21);
        public static readonly Color StatusRed = Color.FromArgb(239, 68, 68);
        public static readonly Color StatusBlue = Color.FromArgb(59, 130, 246);
        public static readonly Color StatusGray = Color.FromArgb(100, 105, 120);

        // 字型設定
        public static Font HeaderFont => new Font("Segoe UI Semibold", 16F);
        public static Font SubHeaderFont => new Font("Segoe UI Semibold", 10F);
        public static Font BodyFont => new Font("Segoe UI", 9.5F);
        public static Font MonoFont => new Font("Consolas", 9F);
    }
}

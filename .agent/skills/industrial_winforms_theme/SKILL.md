---
description: Apply the professional industrial dark theme from SecsMonitorForm to other Forms.
---

# Industrial WinForms Theme

此 Skill 用於將 Windows Forms 應用程式統一為「工業風暗色主題」。
資源檔案位於 `resources/` 資料夾中。

## 包含資源

*   `ThemePalette.cs`: 定義主題顏色 (`IndTheme`)
*   `IndustrialControls.cs`: 包含自定義元件 (`StatusIndicator`, `ActionButton`, `SectionPanel`)

## 使用步驟

### 1. 複製資源

將 `resources/` 資料夾中的以下檔案複製到您的專案目錄 (建議放在 `UI` 或 `Helpers/UI` 資料夾)。

- `ThemePalette.cs`
- `IndustrialControls.cs`

### 2. 調整 Namespace

開啟複製後的檔案，確認 Namespace 與您的專案結構相符。
預設 Namespace 為 `WaferMeasurementFlow.UI`。若您放在不同資料夾，請調整為對應的 Namespace。

### 3. 套用 Form 基本樣式

在 Form 的 `InitializeComponent` 或建構子中，使用 `IndTheme` 設定基本顏色與字型：

```csharp
using WaferMeasurementFlow.UI; // 記得引用 Namespace

public MyForm()
{
    InitializeComponent();
    
    // 套用主題
    this.BackColor = IndTheme.BgPrimary;
    this.ForeColor = IndTheme.TextPrimary;
    this.Font = IndTheme.BodyFont;
}
```

### 4. 使用版面配置 (Layout)

建議使用 `TableLayoutPanel` 進行排版。
使用 `SectionPanel` 作為區塊容器：

```csharp
// 建立區塊
var mySection = new SectionPanel 
{ 
    Title = "區塊標題", 
    Dock = DockStyle.Top, 
    Height = 200 
};

// 內容容器 (Padding Top 45px 是為了避開標題文字)
var content = new Panel 
{ 
    Dock = DockStyle.Fill, 
    Padding = new Padding(10, 45, 10, 10),
    BackColor = Color.Transparent
};

mySection.Controls.Add(content);
this.Controls.Add(mySection);
```

### 5. 使用自定義元件

**操作按鈕 (ActionButton)**:

```csharp
var btnStart = new ActionButton("啟動", IndTheme.StatusBlue);
btnStart.Click += (s, e) => { /* 處理點擊事件 */ };
content.Controls.Add(btnStart);
```

**狀態指示燈 (StatusIndicator)**:

```csharp
var indStatus = new StatusIndicator("連線狀態", "未連線", IndTheme.StatusGray);
// 更新狀態
indStatus.SetStatus("已連線", IndTheme.StatusGreen);
content.Controls.Add(indStatus);
```

## 風格指南

*   **Header**: 使用 `IndTheme.BgSecondary` (高度約 70px).
*   **Content Background**: `IndTheme.BgPrimary`.
*   **Card Background**: `IndTheme.BgCard`.
*   **字體**: 介面優先使用 `Segoe UI`，Log 區域使用 `Consolas`.

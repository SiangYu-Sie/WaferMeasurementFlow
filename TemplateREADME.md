# Industrial Project Template Kit

這是一個針對半導體/工業自動化設備開發的 WinForms 模板工具集。

## 內容清單
1.  **`init-project.ps1`**: 一鍵重構腳本。自動更換 Namespace、專案名稱及檔案路徑。
2.  **`.template.config/`**: 支援 `dotnet new` 的標準範本配置。
3.  **`Managers/BaseSecsManager.cs`**: 提取自 DIASECS SDK 的核心邏輯，方便未來擴充不同設備。
4.  **`Managers/SecsTypes.cs`**: 統一的 SECS 狀態與配置定義，避免重複代碼。

## 如何使用 `init-project.ps1` 開新專案
如果你想基於此專案建立一個名為 `PanelMeasurementApp` 的新專案：

1.  將此目錄全部複製到新專案路徑。
2.  開啟 PowerShell 並執行：
    ```powershell
    .\init-project.ps1 -ProjectName "PanelMeasurementApp"
    ```
3.  腳本會自動完成：
    *   重新命名 `.sln` 與 `.csproj`。
    *   全域替換 `WaferMeasurementFlow` 關鍵字為 `PanelMeasurementApp`。
    *   清理無效的 Namespace 參考。

## 如何擴充新的通訊協定
*   **SECS/GEM**: 繼承 `BaseSecsManager` 並在子類別中實作具體的報點邏輯。
*   **PLC/Hardware**: 參考 `Managers/EtelManager.cs` 的結構进行模組化封裝。

## AI 開發加成
專案保留了 `.agent` 資料夾，包含：
*   **Skills**: 例如 `industrial_winforms_theme` 可用於一鍵美化 Form。
*   **Workflows**: 如果你有特定的開發流程（如 SECS 測試），建議記錄在 `.agent/workflows` 中。

---
*Created by Antigravity AI*

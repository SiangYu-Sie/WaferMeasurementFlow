# DLL 檔案整理說明

## 自動整理機制

專案已設定在每次建置後自動將第三方 DLL 整理到 `lib` 資料夾。

### 整理的檔案包括:
- ? DIASECSGEM.dll
- ? DIASECS.dll  
- ? GEMDataModel.dll
- ? ProductLicenseChecker.dll
- ? SoftLicenseAPI.dll
- ? UniAuto.UniBCS.Core.Cryptography.dll
- ? UniAuto.UniBCS.MISC.StateMachine.dll
- ? DNGuard*.dll (所有 DNGuard 相關檔案)

### 特殊處理:
- **DNGuard Runtime DLL** 會被複製回主目錄,因為它們需要在應用程式根目錄才能正常運作

### 執行時載入機制:
程式啟動時會自動註冊 `AssemblyResolve` 事件處理器,當找不到組件時會:
1. 先搜尋 `lib` 資料夾
2. 再搜尋主目錄 (備援)
3. 輸出載入資訊到 Debug 視窗

## 下一步操作

### 1. 重新建置專案
```
建置 → 重建方案 (Ctrl+Shift+B)
```

### 2. 檢查輸出
建置完成後,檢查:
- `bin\x64\Debug\net10.0-windows\lib\` 資料夾應該包含所有第三方 DLL
- 主目錄應該只有主程式 DLL 和 DNGuard Runtime DLL

### 3. 如果 DNGuard 錯誤仍然存在
需要從 DIASECSGEM 供應商獲取完整的 DNGuard Runtime 檔案:
- DNGuard.Runtime.dll
- DNGuard.HVM.dll

並將它們放到:
- `bin\x64\Debug\net10.0-windows\` (主目錄)
- `bin\x64\Debug\net10.0-windows\lib\` (lib 資料夾也備份一份)

### 4. 測試
執行應用程式並開啟 SECS Monitor 視窗,確認不再出現 DNGuard 錯誤。

## 疑難排解

如果仍有問題:
1. 檢視 Debug Output 視窗查看組件載入訊息
2. 確認 App.config 已包含在專案中
3. 確認 DNGuard Runtime 檔案未被防毒軟體封鎖
4. 右鍵檔案 → 內容 → 解除封鎖

## 變更的檔案
- ? `Program.cs` - 加入 AssemblyResolve 處理器
- ? `App.config` - 設定探查路徑
- ? `WaferMeasurementFlow.csproj` - 加入建置後自動整理任務
- ? `Equipment.cs` - 改善錯誤訊息 (已完成)

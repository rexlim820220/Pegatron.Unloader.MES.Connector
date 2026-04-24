## Kinsus VCP MES Connector (AULink Integration)

本專案為開發給景碩 (Kinsus) VCP 設備使用的 .NET 通訊組件 (DLL)，旨在簡化設備資料上報至 AULink (SFIS) 系統的流程。透過封裝 AAS 與 MVIX 兩種通訊格式，提供統一且高可靠性的調用接口。

## 🚀 核心功能 (Key Features)

* 雙格式支援：完整封裝 AAS 上拋格式與 MVIX 上拋格式。
* 高相容性：採用 .NET Standard 2.0 開發，支援舊版 .NET Framework 4.5.2
* 穩定傳輸機制：
* 內建 Auto-Retry (自動重試) 機制，應對不穩定的網路環境。
   * 可自定義 Timeout 設定 (預設 5-8 秒)。
* 防錯與紀錄：
* 詳盡的異常捕獲 (Exception Handling) 與業務錯誤碼解析。
   * 內建 Logging 日誌系統，便於產線端排查通訊問題。

## MVIX 資料上拋流程圖

特點： 需包含 apiKey 認證，且判斷邏輯較單純（只看 status）。

```
sequenceDiagram
    participant M as 冠穎程序 (Caller)
    participant DLL as .NET DLL (Connector)
    participant AUL as AULink Middleware (MVIX)

    M->>DLL: 呼叫 UploadMvixData(eq_id, raw_datas)
    
    rect rgb(240, 240, 240)
        Note right of DLL: DLL 內部封裝
        DLL->>DLL: 1. 產生 yyyyMMddHHmmss 時間格式
        DLL->>DLL: 2. 加入 Header: apiKey (433576dd...)
        DLL->>DLL: 3. 設定 Timeout (10秒)
    end

    DLL->>AUL: POST /api/v1/mvix/dispatch (JSON)
    
    alt 網路連線正常
        AUL-->>DLL: 回傳 {"status": "success", "data": null, ...}
        rect rgb(200, 230, 255)
            Note right of DLL: 解析回應
            alt status == "success"
                DLL-->>M: 回傳 Success 物件
            else status != "success"
                DLL-->>M: 回傳 Error 物件 (包含 message)
            end
        end
    else 網路超時或伺服器異常
        DLL-->>M: 拋出 WebException / 回傳 Timeout 狀態
    end

```

## AAS 資料上拋流程圖

特點： 單一入口 (dispatch)，且需進行「兩段式」成功判定（status 與 result）。

```
sequenceDiagram
    participant M as 冠穎程序 (Caller)
    participant DLL as .NET DLL (Connector)
    participant AUL as AULink Middleware (AAS)

    M->>DLL: 呼叫 AAS 方法 (如: CheckLoader)
    
    rect rgb(240, 240, 240)
        Note right of DLL: DLL 內部封裝
        DLL->>DLL: 1. 指定 service_name (如: "CheckLoader")
        DLL->>DLL: 2. 封裝服務專屬 Data (如: Lot, Task[])
        DLL->>DLL: 3. 確保不帶 apiKey Header
    end

    DLL->>AUL: POST /api/v1/aas/dispatch (JSON)
    
    alt API 回應成功
        AUL-->>DLL: 回傳 JSON (包含 status, result, code...)
        rect rgb(200, 230, 255)
            Note right of DLL: 核心判定邏輯
            alt status == "success" AND result == 1
                DLL-->>M: 回傳 成功 (包含 Part, Time 等擴充欄位)
            else 業務邏輯失敗 (result != 1)
                DLL-->>M: 回傳 失敗 (包含 code: 錯誤碼, unit: 權責單位)
            end
        end
    else HTTP 500 / 404 / Timeout
        DLL-->>M: 回傳 網路異常狀態
    end
```

## 🛠 技術棧 (Tech Stack)

* Language: C# 8.0+
* Framework: .NET Framework 4.5.2
* Protocol: HTTP/HTTPS (POST)
* Data Format: JSON (Newtonsoft.Json)

## 📅 開發進度 (Roadmap)

* Phase 1: 模型建立 (4/25 - 4/28) - 8 項服務之 JSON Models 定義。
* Phase 2: 核心開發 (4/29 - 5/2) - HTTP 傳輸邏輯與自動重試機制。
* Phase 3: 異常處理 (5/3 - 5/6) - 錯誤碼解析與日誌系統整合。
* Phase 4: 模擬測試 (5/7 - 5/10) - 使用 Postman 進行全 API 驗證。
* Phase 5: 交付驗收 (5/11) - 提供 Release DLL 與範例程式。

## 📦 使用範例 (Usage Example)

```
// 初始化服務 (範例)var client = new KinsusVcpClient(config);
// 上傳 AAS 格式資料var result = await client.UploadAasDataAsync(aasModel);
if (result.IsSuccess) {
    Console.WriteLine("上傳成功！");
} else {
    Console.WriteLine($"失敗代碼: {result.ErrorCode}, 原因: {result.Message}");
}
```

## 系統需求 (Requirements)

* Runtime: .NET Framework 4.4.5.2
* Network: 支援 5G 廠內專網或企業內部網路連線。

## 聯絡與支援
© 2026 Pegatron Corp. All Rights Reserved

namespace Pegatron.Unloader.MES.Connector
{
    /// <summary>
    /// 依照宇沛/友達規範定義之 AULink 通訊常數
    /// </summary>
    public static class AULinkConstants
    {
        /// <summary>
        /// 設備狀態定義 (EQ Status)
        /// 用於 AAS ChangeEQStatus 與 MVIX EQ_Status 點位
        /// </summary>
        public static class EQStatus
        {
            /// <summary> 故障 (1) </summary>
            public const string Down = "1";
            /// <summary> 閒置 (2) </summary>
            public const string Idle = "2";
            /// <summary> 運作 (3) </summary>
            public const string Running = "3";
            /// <summary> 逐步運行 (4) </summary>
            public const string Step = "4";
            /// <summary> 覆歸/初始過程中 (5) </summary>
            public const string Initial = "5";
            /// <summary> 保養模式 (6) </summary>
            public const string PM = "6";
        }

        /// <summary>
        /// 運行模式定義 (Mode)
        /// 用於 AAS ChangeEQMode 與 MVIX Mode_Status 點位
        /// </summary>
        public static class Mode
        {
            /// <summary> 未使用 AAS (0) </summary>
            public const string Off = "0";
            /// <summary> 有使用 AAS (1) </summary>
            public const string On = "1";
        }

        /// <summary>
        /// MVIX 點位名稱定義 (根據 CSV alias_name)
        /// </summary>
        public static class MvixParams
        {
            public const string ModeStatus = "Mode_Status";
            public const string EqStatus = "EQ_Status";
            public const string RecipeNo = "Recipe_NO";
            public const string PanelId = "Panel_ID";
            public const string HourMeter = "Hour_Meter";
            public const string TotalCount = "Total_Count_Nmeter";
        }
    }
}

# AudioPlayService 音樂播放服務

- 為了避免每個app自行播放音檔時，播放時音效疊合會造成一片混亂，因此建立此服務並運行於中控系統PC。
- 提供 API 給其他程式或腳本呼叫，達成在中控系統PC播放特定音檔的功能。

# API

### 控制

- 播放音樂_獨立播放。
- 播放音樂_加入佇列。
- 停止播放所有音樂。
- 停止播放指定的音樂。
- 將指定的音樂從播放佇列中移出。

### 資訊

- 一包資訊
    - IsAudioPlaying
    - playingAudioInfoList
        - audioFilePath：音檔路徑。
        - playingBy:由誰請求播放。
        - playingMode:standalone/ queue




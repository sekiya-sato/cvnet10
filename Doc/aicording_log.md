# AI Coding Log

---

## [2026-03-11] 商品メンテ画面 - 日付フィールドを DatePicker に変更

### Agent
- GitHub Copilot (Claude 3.7 Sonnet)
### Editor
- VS2026

### 目的
- ユーザーからの要望：`MasterShohinMenteView.xaml` の「出荷日」「納品日」「店舗投入日」の3項目を `TextBox` から `DatePicker` へ変更する

### 実施内容
- `Cvnet10Wpfclient/Views/01Master/MasterShohinMenteView.xaml`:
  - Row12「出荷日」: `TextBox (Text=DayShukka)` → `DatePicker (SelectedDate=DayShukka)`
  - Row13「納品日」: `TextBox (Text=DayNohin)` → `DatePicker (SelectedDate=DayNohin)`
  - Row14「店舗投入日」: `TextBox (Text=DayTento)` → `DatePicker (SelectedDate=DayTento)`

### 技術決定 Why
- `DateYmd8Converter` (`yyyyMMdd` 形式の string ↔ `DateTime?` 相互変換) が `App.xaml` でグローバル登録済みのため、View 側での追加宣言なしに `{StaticResource DateYmd8Converter}` を使用
- `Style="{StaticResource MaterialDesignFloatingHintDatePicker}"` を適用し、既存の Material Design デザイン統一性を維持
- `IsTodayHighlighted="False"` はサンプルコードの指示に準拠
- `Mode=TwoWay, UpdateSourceTrigger=PropertyChanged` により ViewModel との双方向バインディングを維持

### 確認
- Build 確認（WPF は Windows 専用のため Linux/WSL2 環境では省略可）

// ファイル概要:
// - 各種マスターで共有するコード/名称インターフェイス `IBaseCodeName` を宣言します。
// - ViewModel や DB モデル間で一貫したプロパティ構造を提供します。
// 依存関係:
// - CommunityToolkit.Mvvm の ObservableObject 実装クラスから参照されます。
// 変更ポリシー:
// - インターフェイスにプロパティを追加する際は全実装クラスがコンパイルエラーになるため、一括対応できるタイミングで行ってください。
// - 不要なプロパティを削除する場合はマイグレーションと API 契約の影響を確認します。
// COPILOT: 共通プロパティの型を変更する場合は DTO/エンティティ/クライアント UI の整合を確認し、必要に応じて新インターフェイスを追加検討してください。

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvnet10Base {
    public interface IBaseCodeName {
		public string Code { get; set; }
		public string? Name { get; set; }
		public string? Ryaku { get; set; }
		public string? Kana { get; set; }
	}
}

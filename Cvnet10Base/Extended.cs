// ファイル概要:
// - C# 12/14 の extension メンバー機能のメモやサンプルを集約するユーティリティです。
// - 既存の Common クラスなどに拡張メソッドを追加するためのテンプレートとして利用します。
// 依存関係:
// - System.* の基本名前空間のみ。
// 変更ポリシー:
// - 実際に使用する拡張メソッドを追加する場合はコメントではなく具象実装として公開し、不要なサンプルは削除します。
// - プロジェクト全体へ影響するユーティリティを追加する際は単体テストを同時に作成してください。
// COPILOT: 拡張メソッドを追加するときは対象型の責務を侵害しないか検討し、状態を持たない純粋関数として実装すること。

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Cvnet10Base;

public static class Extended {
	/*
	private static int _field = 1;
	// インスタンス拡張メソッドの例
	extension(Common c) {
		public void Method(string s) {
			Console.WriteLine(s);
		}
		public int Property { get => _field; }
	}
	// 静的拡張メソッドの例
	extension(Common) {
		public static void Method2(string s) {
			Console.WriteLine(s);
		}
		public static int Property2 { get => _field; }
	}
	// ジェネリック拡張メソッドの例
	extension<T>(List<T> t) {
		public T GenericMethod(T value) => value;
	}
null合体演算子（??）	a ?? b（aがnullならb、非nullならa）
null結合代入演算子（??=）	a ??= b（aがnullならbを代入、非nullならaのまま）
null条件メンバーアクセス演算子（?.）	a?.b（aがnullならnull、非nullならbにアクセス）
null条件要素アクセス演算子?[]	a?[b]（aがnullならnull、非nullならa[b]にアクセス）

	ReadOnlySpan Span の優先
   public static void Method(IEnumerable<int> values) { }
   public static void Method(ReadOnlySpan<int> values) { }

	*/
}

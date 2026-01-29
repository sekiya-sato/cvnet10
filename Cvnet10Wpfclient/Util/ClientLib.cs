/* ============================================================================
 * Cvnet8Common : ClientLib.cs
 * Created by Sekiya.Sato 2024/08/22
 * 説明: WPF用 Windowのオープン、クローズ共通処理
 * [Description: Common processing for opening and closing Windows in WPF]
 * 使用ライブラリ [Libraries Used]:
 *	開発メモ [Development Notes]:
 *		ViewModelからViewの操作を Messenger で実現する場合、コードをView,ViewModel両方に記述する必要がある
 *		添付ビヘイビアを使う方法も長いコードを書く必要がある。継続して調査
 *		将来的にもっと便利なライブラリがでてくれば処理を変更してもよい (2024/08/22)
 *		コードビハインドにClose()やShowDialog()を書けば1行で済むので、同程度の手軽さを切に希望
 *		ViewModelにI/FをつくりView側コードビハインドで vm.Close += () => { this.Close(); }; とする方法もある
 *		Viewから CommandParameter="{Binding RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=Window}}" 渡す方法
 *		(MVVMでのほぼ全てのやり方は結局はなんとかしてViewの参照を得てから処理をしているだけではないか....)
 *		MVVM原理主義に忖度し、ViewModelから呼ばれるがViewは直接参照してない。自分のViewModelを所持しているのが自分のView
 *		
 *	   [When using Messenger to achieve View manipulation from ViewModel, it is necessary to write code in both View and ViewModel
 *		Using attached behaviors also requires writing lengthy code. Further investigation is ongoing
 *		If more convenient libraries become available in the future, processing can be adjusted (2024/08/22)
 *		Writing Close() or ShowDialog() in the code-behind can achieve the same level of simplicity, so such ease of use is highly desired
 *		Another method is to create an interface in the ViewModel and handle it in the View's code-behind with vm.Close += () => { this.Close(); };
 *		Passing CommandParameter="{Binding RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=Window}}" from View
 *		(Almost all methods in MVVM eventually involve obtaining some form of reference to the View and then performing operations...)
 *		Respecting MVVM principles, the ViewModel is called from the View, but the View is not directly referenced. Each View owns its own ViewModel]
 * ============================================================================  */
using System.Windows;

namespace Cvnet10Wpfclient.Util {
    /// <summary>
    /// 主にViewModel側からViewを操作するためのクラス
    /// [Class mainly for manipulating the View from the ViewModel]
    /// </summary>
    public class ClientLib
    {
        /// <summary>
        /// アクティブなWindowを閉じる
        /// [Close the active Window]
        /// </summary>
        public static void Exit(object vm)
        {
			var win = GetActiveView(vm);
			if (win != null) {
				win.Close();
				if (win.Owner != null)
					win.Owner.Activate();
			}
		}
        /// <summary>
        /// 全てのWindowを閉じる
        /// [Close all Windows]
        /// </summary>
        public static void ExitAll()
        {
            foreach (var win in Application.Current.Windows.OfType<Window>())
            {
                win.Close();
            }
        }
        /// <summary>
        /// 自分と親以外全てのWindowを閉じる
        /// [Close all Windows except for the current and parent ones]
        /// </summary>
        public static void ExitAllWithoutMe(object vm) {
			var myview = GetActiveView(vm);
			var parent = myview?.Owner;
			foreach (var win in Application.Current.Windows.OfType<Window>()) {
				if (win != myview && win != parent)
					win.Close();
			}
		}
        /// <summary>
        /// アクティブなWindowを取得する(非推奨)
        /// [Get the active Window (not recommended)]
        /// </summary>
        /// <returns></returns>
        private static Window? GetActiveWin() {
			var activeWin = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            /*
			 1. クライアントアプリなので何かしら操作した直後はそのWindowがアクティブである
			[1. Since it's a client application, the Window just operated on will be active immediately afterward.]
			 2. アクティブなWindowがない場合は最後に開いたWindowを返す(VS2022デバッグを考慮 Microsoft.VisualStudio.DesignTools...)
			[2. If there is no active Window, return the last opened Window (considering VS2022 debugging Microsoft.VisualStudio.DesignTools...)]
			 */
            return activeWin ?? Application.Current.Windows.OfType<Window>().Where(c=>!c.ToString().StartsWith("Microsoft.")).LastOrDefault();
		}
        /// <summary>
        /// ViewModelが紐づけられてるViewを取得する
        /// [Retrieve the View associated with the ViewModel]
        /// </summary>
        /// <returns></returns>
        public static Window? GetActiveView(object vm) {
			Window? myWin = null;
			var activeWins = Application.Current.Windows.OfType<Window>().Reverse();
			foreach (var ac in activeWins) {
				var myVm = ac.DataContext;
				if (myVm == vm)
					myWin = ac;
			}
			return myWin;
		}
        /// <summary>
        /// ViewのDialogResultを設定して閉じる
        /// [Set the DialogResult of the View and close it]
        /// </summary>
        /// <param name="result"></param>
        public static void ExitDialogResult(object vm, bool result) {
			var win = GetActiveView(vm);
			if (win != null) {
				try {
					win.DialogResult = result; 
				}
				catch (Exception) {
                    /* ShowかShowDialogか自分でわかってない*/
                    //[Whether to use Show or ShowDialog is not determined by this code]
                }
                Exit(vm);
			}
		}

        /// <summary>
        /// Viewを親として子Windowをオープンする
        /// [Open a child Window with the View as its parent]
        /// </summary>
        /// <param name="childWin">子Window</param> [Child Window]
        /// <param name="loc">表示位置</param> [Display position]
        /// <param name="IsDialog">true=ダイアログとして表示 false=独立Windowsとして表示</param> 
		/// [true = Display as a dialog, false = Display as an independent Window]
        /// <param name="IsShowTaskbar">true=タスクバーに表示 false=表示しない</param>
		/// [true = Display in the taskbar, false = Do not display]
        /// IsShowTaskbar
		public static bool? ShowDialogView(Window childWin,object myVm, WindowStartupLocation loc = WindowStartupLocation.CenterOwner, bool IsDialog = true, bool IsShowTaskbar=false) {
            childWin.Owner = GetActiveView(myVm);
            childWin.WindowStartupLocation = loc;
			childWin.ShowInTaskbar = IsShowTaskbar;
			if (IsDialog) 
				return childWin.ShowDialog();
			else {
				childWin.Show();
				return null;
			}
		}
        /// <summary>
        /// DataGridに対しDictionary型を参照して列を作成する
        /// [Create columns in a DataGrid by referring to a Dictionary type]
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="obj"></param>
        public static void SetDataGridDic(System.Windows.Controls.DataGrid dg, Dictionary<string, object> obj) {
			dg.Columns.Clear();
			dg.AutoGenerateColumns = false;
			foreach (var item in obj.Keys) { // 列を追加する [Add columns]
				var textColumn = new System.Windows.Controls.DataGridTextColumn();
				textColumn.Header = item;
				textColumn.Binding = new System.Windows.Data.Binding($"Item[{item}]");
				dg.Columns.Add(textColumn);
			}
		}

        /// <summary>
        /// 使用可能なデータフォルダを取得
        /// [Retrieve the available data folder]
        /// </summary>
        /// <returns>データフォルダ</returns> [Data folder]
        public static string GetDataDir() {
			try {
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                //string folder = appData + "\\" + System.Windows.Forms.Application.CompanyName + "\\" + System.Windows.Forms.Application.ProductName;
                // 固定で返す(バージョンを大きく変える場合には変更する)
                // [Return a fixed value (change if a major version update is made)]
                string folder = appData + @"\cvnet10";
				if (!System.IO.Directory.Exists(folder)) {
					System.IO.Directory.CreateDirectory(folder);
				}
				return folder;
			}
			catch (Exception ex) {
				var log = NLog.LogManager.GetCurrentClassLogger();
				log.Error(ex, "GetDataDirエラー");
				return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			}
		}
        /// <summary>
        /// マウスカーソルをWaitにして処理を実行する
        /// [Change the mouse cursor to "Wait" and execute the process]
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static TimeSpan WaitCursor(Action method) {
			var start = DateTime.Now;
			System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
			method();
			System.Windows.Input.Mouse.OverrideCursor = null;
			var span = DateTime.Now - start;
			return span;
		}
	}
}

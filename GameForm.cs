using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using DxLibDLL;

namespace LunarCityGame {

	public class GameForm : Form {
		
        private DxGraph _DXGRAPH = null;
        private DxFont _DXFONT = null;
        private DxSound _DXSOUND = null;
        private GameTitle _GAME_TITLE = null;
        private GameMain _GAME_MAIN = null;

        private GameState _GAME_STATE = 0;

		/// <summary>
		/// GameFormクラスのコンストラクタ。
		/// </summary>
		public GameForm() {

			this.Text = Settings.WINDOW_TITLE;
			this.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
			this.ClientSize = new Size(Settings.FORM_WIDTH, Settings.FORM_HEIGHT);
			this.ShowInTaskbar = true;
			this.StartPosition = FormStartPosition.WindowsDefaultLocation;
			this.Opacity = 1.0;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.BackColor = Color.Black;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.AllowDrop = false;
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            // Log.txtの生成を設定
            DX.SetOutApplicationLogValidFlag(Settings.DX_LOG);

            // DxLibの親ウインドウをこのフォームウインドウにセット
            DX.SetUserWindow(this.Handle);
            DX.SetWindowSizeChangeEnableFlag(DX.TRUE);
            DX.SetFullScreenResolutionMode(DX.DX_FSRESOLUTIONMODE_DESKTOP);
            DX.SetFullScreenScalingMode(DX.DX_FSSCALINGMODE_BILINEAR);
            DX.ChangeWindowMode(Settings.WINDOW_MODE);
            DX.SetGraphMode(Settings.FORM_WIDTH, Settings.FORM_HEIGHT, Settings.COLOR_DEPTH);

            if (DX.DxLib_Init() == -1)
                ExitApplication(Settings.ERR_NO_INIT);
            
            //
            _GAME_STATE = GameState.Title;

            _DXGRAPH = DxGraph.GetInstance();
            _DXFONT = DxFont.GetInstance();
            _DXSOUND = DxSound.GetInstance();
            _GAME_TITLE = GameTitle.GetInstance();
            _GAME_MAIN = GameMain.GetInstance();
		}

        /// <summary>
        /// ゲームのメインループ。
        /// </summary>
        public void MainLoop() {

            // ESCキーが押下されるまで繰り返す
            if (DX.CheckHitKey(DX.KEY_INPUT_ESCAPE) == 1) {
                this.Close();
                return;
            }

            if (_GAME_STATE == GameState.Title)
                _GAME_STATE = _GAME_TITLE.DrawScreen(_GAME_STATE);
            else
                //
                _GAME_STATE = _GAME_MAIN.DrawScreen(_GAME_STATE);
        }

        /// <summary>
        /// フォームイベント：
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e) {

            DX.DxLib_End();
        }

        /// <summary>
        /// アプリケーションの強制終了。
        /// </summary>
        /// <param name="s">メッセージボックスに表示する文字列を指定する。</param>
        private void ExitApplication(string s) {

            MessageBox.Show(s);
            Environment.Exit(0x8020);
        }
	}
}

using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;
using DxLibDLL;

namespace LunarCityGame {

	public class GameTitle {

        // インスタンス
        private static GameTitle singleton = null;

        //
        private DxGraph _DXGRAPH = DxGraph.GetInstance();
        private DxFont _DXFONT = DxFont.GetInstance();
        private DxSound _DXSOUND = DxSound.GetInstance();
        private GameMain _GAME_MAIN = GameMain.GetInstance();

        // オブジェクト
        private Point[] _STARS = new Point[30];

        private int _TIMMING = 0;
        private int _PAT = 0;

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns>当クラスのインスタンスを返却する。シングルトン構成です。</returns>
        public static GameTitle GetInstance() {

            if (singleton == null) {
                singleton = new GameTitle();
            }
            return singleton;
        }

		/// <summary>
		/// GameTitleクラスのコンストラクタ。
		/// </summary>
        private GameTitle() {

            Random random = new Random();

			for (int i = 0; i < 30; i++) {
				int x = random.Next(79) + 1;
				int y = random.Next(23) + 1;
				_STARS[i] = new Point(x, y);
			}
        }

        public GameState DrawScreen(GameState state) {

            // おまじない
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);
            DX.ProcessMessage();
            DX.ClearDrawScreen();

            // ゲーム開始の条件
            if (DX.CheckHitKey(DX.KEY_INPUT_SPACE) == 1)
                state = GameState.PlayGame;
            if ((DX.GetJoypadInputState(DX.DX_INPUT_PAD1) & DX.PAD_INPUT_1) != 0)
                state = GameState.PlayGame;

            // ゲームを初期設定する
            if (state == GameState.PlayGame) {
                DX.PlaySoundMem(_DXSOUND.DXSOUND[6], DX.DX_PLAYTYPE_NORMAL, DX.TRUE);
                _GAME_MAIN.InitGame();
                state = GameState.Barrier;
            }
                

            DrawContent();

            // おまじない
            DX.ScreenFlip();

            return state;
        }

        /// <summary>
        /// 描画対象にレンダリングを行う。
        /// </summary>
        private void DrawContent() {

            if (--_TIMMING < 0) {
                _PAT = (_PAT == 0) ? 1 : 0;
                _TIMMING = 20;
            }

			foreach (Point star in _STARS)
				DrawImage(star.X, star.Y, _DXGRAPH.STAR[0]);

            string st1 = "####";
            string st2 = "L U N A R   C I T Y   S O S ! !";

            DrawString(18, 2, st1, Settings.Magenta);
            DrawString(25, 2, st2, Settings.Yellow);
            DrawString(59, 2, st1, Settings.Magenta);

            string sr1 = "TOPSIDER STATION    20  POINTS";
            string sr2 = "TOPSIDER UFO        40  POINTS";
            string sr3 = "TOPSIDER            60  POINTS";
            string sr4 = "TOPSIN              80  POINTS";
            string sr5 = "TOPON              100  POINTS";

            DrawString(35, 5, sr1, Settings.SkyBlue);
            DrawString(35, 7, sr2, Settings.SkyBlue);
            DrawString(35, 9, sr3, Settings.SkyBlue);
            DrawString(35, 11, sr4, Settings.SkyBlue);
            DrawString(35, 13, sr5, Settings.SkyBlue);

            string su1 = "***    KEY FUNCTION    ***";
            string su2 = "(1) BEAM SHIP          [LEFT] <=         => [RIGHT]";
            string su3 = "(2) BEAM SHOOT         [SPACE]";
            string su4 = "(3) BARRIER            [SHIFT]";
            string su5 = "BEAMSHIP IS ADDED AT 5000 POINTS.";
            string su6 = "HIT SPACE KEY TO START";
            string su7 = "HIT SPACE KEY OR PUSH A BUTTON TO START";

            DrawString(14, 16, su1, Settings.LimeGreen);
            DrawString(16, 18, su2, Settings.White);
            DrawString(16, 19, su3, Settings.White);
            DrawString(16, 20, su4, Settings.White);
            DrawString(16, 22, su5, Settings.Yellow);
            if (DX. GetJoypadNum() == 0)
                DrawString(16, 24, su6, Settings.White);
            else
                DrawString(16, 24, su7, Settings.White);
            
            DrawImage(17, 5, _DXGRAPH.TOPSIDER_STATION[_PAT]);
            DrawImage(21, 7, _DXGRAPH.TOPSIDER_UFO[0]);
            DrawImage(23, 9, _DXGRAPH.TOPSIDER[_PAT]);
            DrawImage(24, 11, _DXGRAPH.TOPSIN[_PAT]);
            DrawImage(25, 13, _DXGRAPH.TOPON[0]);
            DrawImage(50, 18, _DXGRAPH.BEAMSHIP[0]);
       }

        /// <summary>
        /// 文字列を描画する。
        /// </summary>
        /// <param name="x">X軸カラム位置を指定する。</param>
        /// <param name="y">Y軸カラム位置を指定する。</param>
        /// <param name="s">描画する文字列を指定する。</param>
        /// <param name="c">文字色を指定する。</param>
        private void DrawString(int x, int y, string s, uint c) {

            DX.DrawStringToHandle((x - 1) * 8, (y - 1) * 16, s, c, _DXFONT.DXFONT[0, 0]);
		}

        /// <summary>
        /// ビットマップを描画する。起点は左上。
        /// </summary>
        /// <param name="x">X軸カラム位置を指定する。</param>
        /// <param name="y">Y軸カラム位置を指定する。</param>
        /// <param name="b">DxLibへ登録したビットマップ番号を指定する。</param>
		private void DrawImage(int x, int y, int b) {

			DX.DrawGraph((x - 1) * 8, (y - 1) * 16, b, DX.TRUE);
		}
    }
}

/**
            1         2         3         4         5         6         7         8
   ----+----0----+----0----+----0----+----0----+----0----+----0----+----0----+----0
 1|                                                                                |
 2|                 ####   L U N A R   C I T Y   S O S ! !   ####                  |
 3|                                                                                |
 4|                                                                                |
 5|                                  TOPSIDER STATION    20  POINTS                |
 6|                                                                                |
 7|                                  TOPSIDER UFO        40  POINTS                |
 8|                                                                                |
 9|                                  TOPSIDER            60  POINTS                |
10|                                                                                |
11|                                  TOPSIN              80  POINTS                |
12|                                                                                |
13|                                  TOPON              100  POINTS                |
14|                                                                                |
15|                                                                                |
16|             ***    KEY FUNCTION    ***                                         |
17|                                                                                |
18|               (1) BEAM SHIP          [LEFT] <=  @@@@@  => [RIGHT]              |
19|               (2) BEAM SHOOT         [SPACE]                                   |
20|               (3) BARRIER            [SHIFT]                                   |
21|                                                                                |
22|               ADD THE SHIP EVERY 5000 POINTS                                   |
23|                                                                                |
24|               HIT SPACE KEY TO START                                           |
25|                                                                                |
   ----+----0----+----0----+----0----+----0----+----0----+----0----+----0----+----0
 */

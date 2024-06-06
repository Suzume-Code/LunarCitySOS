using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Xml;
using DxLibDLL;

namespace LunarCityGame {

	class Program {

		[STAThread]
		public static void Main(string[] args) {

			if (StartupProcessCheck())
				return;

			Settings settings = new Settings();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			GameForm f= new GameForm();
            f.Show();

            while (f.Created) {
				f.MainLoop();
                Application.DoEvents();
            }
		}

        /// <summary>
		/// アプリケーション開始時のチェック。
		/// </summary>
        /// <returns>自分自身を含んで同名のプロセスが２つ以上起動していればtrue、以外はfalse。</returns>
        private static bool StartupProcessCheck() {

			string processName = Process.GetCurrentProcess().ProcessName;
			return (Process.GetProcessesByName(processName).Length > 1);
        }
	}

	/// <summary>
	/// プログラム全体の設定値クラス。
	/// </summary>
	public class Settings {

        public static readonly string WINDOW_TITLE = "LUNAR CITY SOS!!";

		public static readonly int FORM_WIDTH  = 640;
		public static readonly int FORM_HEIGHT = 400;
		public static readonly int COLOR_DEPTH = 8;

		// DxLib設定値
		public static int DX_LOG      = DX.FALSE;
		public static int WINDOW_MODE = DX.TRUE;

		public static readonly int BEAM_MAX_COUNT = 4;

		public static int BEAMSHIP_INIT_COUNT = 4;
		public static int BARRIER_INIT_COUNT  = 1;

		// 1UP閾値
		public static int BEAMSHIP_THRESHOLD_SCORE = 5000;

		// TOPSIDER-STATIONとBARRIERの初期表示数
		public static int[] ENEMY_INIT_PATTERN = new int[] { 1, 4, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4 };

		// 敵機被弾数
		public static readonly int TOPSIDER_STATION_DESTROY_COUNT = 4;
		public static readonly int TOPSIDER_UFO_DESTROY_COUNT     = 3;
		public static readonly int TOPSIDER_DESTROY_COUNT         = 2;
		public static readonly int TOPSIN_DESTROY_COUNT           = 1;
		public static readonly int TOPON_DESTROY_COUNT            = 1;
		public static readonly int BEAMSHIP_DESTROY_COUNT         = 1;
		public static readonly int CITY_DESTROY_COUNT             = 1;

		// 敵機ミサイルの発生率（n/500）
		public static int PROBABILITY_OF_DROPBOMB = 10;

		// インターバルタイム（60fps x Wait sec.）
		public static readonly int STAGE_CLEAR_INTERVAL        = 300;
		public static readonly int GAME_OVER_INTERVAL          = 300;
		public static readonly int BEAMSHIP_DESTROYED_INTERVAL = 90;

		// DxLibカラー設定値
		public const uint Magenta   = 0xFF00FF;
        public const uint Yellow    = 0xFFFF00;
        public const uint SkyBlue   = 0x87CEEB;
        public const uint White     = 0xFFFFFF;
        public const uint LimeGreen = 0x32CD32;
		public const uint Red       = 0xFF0000;

		// エラーメッセージ
		public static readonly string ERR_NO_GRAPH      = "指定のグラフィックファイルが見つかりません。\n{0}";
		public static readonly string ERR_NO_READ_GRAPH = "読み込めないグラフィックファイルです。\n{0}";
		public static readonly string ERR_NO_FONT       = "指定のフォントファイルが見つかりません。\n{0}";
		public static readonly string ERR_NO_SOUND      = "指定のサウンドファイルが見つかりません。\n{0}";
		public static readonly string ERR_NO_INIT       = "DxLibを初期化できませんでした。";
		public static readonly string ERR_NO_XMLFILE    = "設定ファイルがありません。\n{0}";
		public static readonly string ERR_WRONG_XML     = "設定ファイルに記載誤りがあります。\n{0}";

		public Settings() {
		
			string xml_path = Path.Combine(Environment.CurrentDirectory, "Settings.xml");

            if (!File.Exists(xml_path))
				ExitApplication(string.Format(ERR_NO_XMLFILE, xml_path));

			XmlDocument xml_doc = new XmlDocument();
            try {
                xml_doc.PreserveWhitespace = false;
                xml_doc.Load(xml_path);
				//
				string path = "/SystemSettings/System/DxLog";
				bool dx_log = Convert.ToBoolean(xml_doc.SelectSingleNode(path).InnerText);
				DX_LOG = (dx_log) ? DX.TRUE : DX.FALSE;
				
				path = "/SystemSettings/System/FullScreenMode";
				bool full_screen_mode = Convert.ToBoolean(xml_doc.SelectSingleNode(path).InnerText);
				WINDOW_MODE = (full_screen_mode) ? DX.FALSE : DX.TRUE;
				
				//
				path = "/SystemSettings/MySettings/MaximumBeamShip";
				int maximum_beam_ship = Int32.Parse(xml_doc.SelectSingleNode(path).InnerText);
				if (maximum_beam_ship <= 0)
					maximum_beam_ship = 1;
				if (maximum_beam_ship >= 99)
					maximum_beam_ship = 99;
				if (maximum_beam_ship != 0)
					BEAMSHIP_INIT_COUNT = maximum_beam_ship;
				
				path = "/SystemSettings/MySettings/NumberOfBarrierRepair";
				int number_of_barrier_repair = Int32.Parse(xml_doc.SelectSingleNode(path).InnerText);
				if (number_of_barrier_repair <= 0)
					number_of_barrier_repair = 1;
				if (number_of_barrier_repair >= 99)
					number_of_barrier_repair = 99;
				if (number_of_barrier_repair != 0)
					BARRIER_INIT_COUNT = number_of_barrier_repair;
				
				//
				path = "/SystemSettings/EnemySettings/AttackRatio";
				int attack_raito = Int32.Parse(xml_doc.SelectSingleNode(path).InnerText);
				if (attack_raito <= 1)
					attack_raito = 1;
				if (attack_raito >= 100)
					attack_raito = 100;
				PROBABILITY_OF_DROPBOMB = attack_raito;
			}
			catch (Exception ex) {
				ExitApplication(string.Format(ERR_WRONG_XML, ex.Message));
			}
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

	/// <summary>
	/// ゲームオブジェクトの設定値保持クラス。
	/// </summary>
    public class PositionState {

		/**
			int	X,		-- X軸座標
			int	Y,		-- Y軸座標
			int GrHandle	-- Bitmap番号
			int GrHandle2   --
			int Count,
			int	AmountX,	-- x軸移動量
			int	AmountY,	-- Y軸移動量
			int Score,
			bool IsDead,	
		*/
        public PositionState(int x, int y, int gr1, int gr2, int c, int ax, int ay, int sc) {

            X = x;
            Y = y;
			GrHandle = gr1;
			GrHandle2 = gr2;
			Count = c;
			AmountX = ax;
			AmountY = ay;
			Score = sc;
			IsDead = false;
        }
        public int X { get; set; }
        public int Y { get; set; }
		public int GrHandle { get; set; }
		public int GrHandle2 { get; set; }
		public int Count { get; set; }
		public int AmountX { get; set; }
		public int AmountY { get; set; }
		public int Score { get; set; }
		public bool IsDead { get; set; }
		
    }

	public enum GameState {
		Title,
		PlayGame,
		Barrier,
		BeamShipDead,
		PlayContinue,
		StageClear,
		GameClear,
		GameOver,
	}
}

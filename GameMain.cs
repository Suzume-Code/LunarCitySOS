using System;
using System.Drawing;
using System.Drawing.Text;
using System.Collections.Generic;
using DxLibDLL;

namespace LunarCityGame {

	public class GameMain {

        // インスタンス
        private static GameMain singleton = null;

        //
        private DxGraph _DXGRAPH = DxGraph.GetInstance();
        private DxFont _GAME_FONT = DxFont.GetInstance();
        private DxSound _GAME_SOUND = DxSound.GetInstance();

        // オブジェクト
        private Point[] _STARS = new Point[30];

        // オブジェクト
        private List<PositionState> _ENEMIES;
        private List<PositionState> _BEAM;
        private List<PositionState> _CITY_DESTORY;
        private List<PositionState> _MISSILE;
        private List<PositionState> _BARRIER;
        private List<PositionState> _BEAMSHIP;
        private List<PositionState> _CITY;

        private Queue<PositionState> _QUEUE;

        //
        private int _STAGE = 0;
        private int _SCORE = 0;
        private int _HISCORE = 10000;
        private int _BARRIER_LEFT = Settings.BARRIER_INIT_COUNT;
        private int _SHIP_LEFT = Settings.BEAMSHIP_INIT_COUNT;
        private int _CITY_REMAIN = 0;
        private int _CITY_SCORE = 0;
        private int _BONUS_SCORE = 0;
        private bool _1UPPED = false;

        //
        private int _BEAM_WAIT = 0;

        private int _GOWAIT = 0;
        private int _TIMMING = 0;
        private int _PAT = 0;

        private Random random = new Random();

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns>当クラスのインスタンスを返却する。シングルトン構成です。</returns>
        public static GameMain GetInstance() {

            if (singleton == null) {
                singleton = new GameMain();
            }
            return singleton;
        }

		/// <summary>
		/// GameTitleクラスのコンストラクタ。
		/// </summary>
        private GameMain() {

            _ENEMIES = new List<PositionState>();
            _BARRIER = new List<PositionState>();
            _BEAMSHIP = new List<PositionState>();
            _CITY = new List<PositionState>();
            _BEAM = new List<PositionState>();
            _MISSILE = new List<PositionState>();
            _CITY_DESTORY = new List<PositionState>();

            _QUEUE = new Queue<PositionState>();

            InitGame();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitGame() {

            _SCORE = 0;

            // 星の設定
			for (int i = 0; i < 30; i++) {
				int x = random.Next(79) * 8;
				int y = random.Next(23) * 16;
				_STARS[i] = new Point(x, y);
			}

            _ENEMIES.Clear();
            _BARRIER.Clear();
            _BEAMSHIP.Clear();
            _CITY.Clear();
            _BEAM.Clear();
            _MISSILE.Clear();
            _CITY_DESTORY.Clear();

            _QUEUE.Clear();

            _STAGE = 1;
            _1UPPED = false;

            _BARRIER_LEFT = Settings.BARRIER_INIT_COUNT;

            // ２行目、４０桁目が開始位置、ドット数は８
            int gr0 = _DXGRAPH.TOPSIDER_STATION[0];
            int gr1 = _DXGRAPH.TOPSIDER_STATION[1];
            int destroy = Settings.TOPSIDER_STATION_DESTROY_COUNT;
            _ENEMIES.Add(new PositionState(39 * 8, 2 * 16, gr0, gr1, destroy, RandomAbsorute(2), 0, 20));

            // １６行目、４０桁目が開始位置、ドット数は８
            gr0 = _DXGRAPH.BEAMSHIP[0];
            gr1 = _DXGRAPH.BEAMSHIP[1];
            destroy = Settings.BEAMSHIP_DESTROY_COUNT;
            _BEAMSHIP.Add(new PositionState(40 * 8, 19 * 16, gr0, gr1, destroy, 0, 0, 0));

            // CITYの設定
            gr0 = _DXGRAPH.CITY[0];
            gr1 = _DXGRAPH.CITY[1];
            destroy = Settings.CITY_DESTROY_COUNT;
            _CITY.Add(new PositionState(8 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(13 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(24 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(34 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(40 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(52 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(60 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
            _CITY.Add(new PositionState(67 * 8, 22 * 16, gr0, gr1, destroy, 0, 0, 0));
        }

        /// <summary>
        /// ステージクリア後の次ステージ初期値設定
        /// </summary>
        public void ContinueGame() {

            // 星の設定
			for (int i = 0; i < 30; i++) {
				int x = random.Next(79) * 8;
				int y = random.Next(23) * 16;
				_STARS[i] = new Point(x, y);
			}

            _ENEMIES.Clear();
            _BARRIER.Clear();
            _BEAMSHIP.Clear();
            _BEAM.Clear();
            _MISSILE.Clear();
            _CITY_DESTORY.Clear();

            _QUEUE.Clear();

            _STAGE++;

            // TOPSIDER-STATIONとBARRIERの初期表示数
            int pat = _STAGE - 1;
            int max = Settings.ENEMY_INIT_PATTERN[pat];

            // BARRIERの設定
            if (Settings.BARRIER_INIT_COUNT == 0)
                _BARRIER_LEFT = max;
            else
                _BARRIER_LEFT = Settings.BARRIER_INIT_COUNT;

            // ２行目、４０桁目が開始位置、ドット数は８
            int gr0 = _DXGRAPH.TOPSIDER_STATION[0];
            int gr1 = _DXGRAPH.TOPSIDER_STATION[1];
            int destroy = Settings.TOPSIDER_STATION_DESTROY_COUNT;
            for (int i = 0; i < max; i++) {
                int col = random.Next(60) + 10;
                _ENEMIES.Add(new PositionState(col * 8, ((i * 2) + 2) * 16, gr0, gr1, destroy, RandomAbsorute(2), 0, 20));
            }

            // １６行目、４０桁目が開始位置、ドット数は８
            gr0 = _DXGRAPH.BEAMSHIP[0];
            gr1 = _DXGRAPH.BEAMSHIP[1];
            destroy = Settings.BEAMSHIP_DESTROY_COUNT;
            _BEAMSHIP.Add(new PositionState(40 * 8, 19 * 16, gr0, gr1, destroy, 0, 0, 0));
        }

        private int RandomAbsorute(int n) {

            return (random.Next(100) < 50) ? -(n) : n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GameState DrawScreen(GameState state) {

            // おまじない
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);
            DX.ProcessMessage();
            DX.ClearDrawScreen();

            switch (state) {
                case GameState.Barrier:
                    state = DrawBarrier(state);
                    break;
                case GameState.BeamShipDead:
                    state = DrawBeamShipDead(state);
                    break;
                case GameState.StageClear:
                    state = DrawSceneClear(state);
                    break;
                case GameState.GameClear:
                    state = DrawGameClear(state);
                    break;
                case GameState.GameOver:
                    state = DrawGameOver(state);
                    break;
                default:
                    state = Action(state);
                    break;
            }
            
            DrawContent(state);

            // おまじない
            DX.ScreenFlip();

            return state;
        }

        /// <summary>
        /// バリアを張る。位置は１６行目。
        /// </summary>
        private GameState DrawBarrier(GameState state) {

            int barriers = _BARRIER.Count;
            if (barriers == 0)
                DX.PlaySoundMem(_GAME_SOUND.DXSOUND[3], DX.DX_PLAYTYPE_BACK, DX.TRUE);
            if (barriers >= 80) {
                return GameState.PlayGame;
            }

            int position = barriers + 1;
            int gr = _DXGRAPH.BARRIER[0];
            _BARRIER.Add(new PositionState(barriers * 8, 16 * 16, gr, gr, 1, 0, 0, 0));

            return state;
        }

        private GameState DrawBeamShipDead(GameState state) {

            // ステージクリア判定でウェイト時間を設定
            if (--_GOWAIT <= 0) {
                foreach (PositionState o in _BEAMSHIP)
                    o.IsDead = false;
                _MISSILE.Clear();
                return GameState.PlayGame;
            }

            return state;
        }

        /// <summary>
        /// ステージクリア。約5秒で次ステージに進む。
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private GameState DrawSceneClear(GameState state) {

            // ステージクリア判定でウェイト時間を設定
            if (--_GOWAIT <= 0) {
                // 全ステージクリア判定
                if (_STAGE == Settings.ENEMY_INIT_PATTERN.Length) {
                    DX.PlaySoundMem(_GAME_SOUND.DXSOUND[9], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                    _GOWAIT = 900;
                    return GameState.GameClear;
                }
                ContinueGame();
                return GameState.Barrier;
            }
            
            return state;
        }

        /// <summary>
        /// ゲームクリア。約10秒でタイトルに戻る。
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private GameState DrawGameClear(GameState state) {

            // ステージクリア判定でウェイト時間を設定
            if (--_GOWAIT <= 0) {
                return GameState.Title;
            }
            
            return state;
        }

        /// <summary>
        /// ゲームオーバー。約5秒でタイトルに戻る。
        /// </summary>
        private GameState DrawGameOver(GameState state) {

            // ゲームオーバー判定でウェイト時間を設定
            if (--_GOWAIT <= 0)
                return GameState.Title;

            return state;
        }

        private GameState Action(GameState state) {

            // 自機ビームの移動
            MoveBeam();

            // 自機の移動
            MoveBeamShip();

            // 自機からビームを発射
            if (DX.CheckHitKey(DX.KEY_INPUT_SPACE) == 1 || 
               (DX.GetJoypadInputState(DX.DX_INPUT_PAD1) & DX.PAD_INPUT_2) != 0)
                BeamShoot();

            // バリアの張り直し
            if (DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1 ||
                DX.CheckHitKey(DX.KEY_INPUT_RSHIFT) == 1 ||
               (DX.GetJoypadInputState(DX.DX_INPUT_PAD1) & DX.PAD_INPUT_1) != 0) {
                if (_BARRIER_LEFT > 0) {
                    _BARRIER_LEFT--;
                    state = GameState.Barrier;
                    _BARRIER.Clear();
                }
            }
                
            // 敵機ミサイルの移動
            MoveMissile();

            // 敵機の移動
            MoveEnemies();

            // CITY爆発パターン
            if (_CITY_DESTORY.Count > 0) {
                Queue<PositionState> q = new Queue<PositionState>(); 
                foreach (PositionState cd in _CITY_DESTORY)
                    if (--cd.Count == 0)
                        q.Enqueue(cd);
                for (int i = 0; i < q.Count; i++)
                    _CITY_DESTORY.Remove(q.Dequeue());
            }

            // 自機の残機判定
            foreach (PositionState o in _BEAMSHIP)
                if (o.IsDead) {
                    if (--_SHIP_LEFT <= 0) {
                        DX.PlaySoundMem(_GAME_SOUND.DXSOUND[5], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                        _GOWAIT = 300;
                        return GameState.GameOver;
                    }
                    _GOWAIT = 90;
                    return GameState.BeamShipDead; 
                }

            // ゲームオーバー判定
            int city_remain = 0;
            foreach (PositionState o in _CITY)
                if (!o.IsDead)
                    city_remain++;
            if (city_remain == 0) {
                // ゲームオーバーBGM
                DX.PlaySoundMem(_GAME_SOUND.DXSOUND[5], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                _GOWAIT = 300;
                return GameState.GameOver;
            }

            // ステージクリア判定
            int enemy_count = 0;
            foreach (PositionState o in _ENEMIES)
                if (!o.IsDead)
                    enemy_count++;
            if (enemy_count == 0) {
                _CITY_SCORE += 50;
                _BONUS_SCORE = city_remain * _CITY_SCORE;
                _CITY_REMAIN = city_remain;
                // ステージクリアBGM
                DX.PlaySoundMem(_GAME_SOUND.DXSOUND[4], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                _GOWAIT = 300;
                return GameState.StageClear;
            }

            return state;
        }

        /// <summary>
        /// 自機ビームの移動
        /// </summary>
        private void MoveBeam() {

            for (int i = 0; i < _BEAM.Count; i++) {
                _BEAM[i].Y += _BEAM[i].AmountY;
                if (_BEAM[i].Y < 0)
                    _BEAM.RemoveAt(i);
            }
        }

        /// <summary>
        /// 自機の移動を制御する。
        /// </summary>
        private void MoveBeamShip() {

            const int MOVESPEED = 4;

            if (DX.CheckHitKey(DX.KEY_INPUT_LEFT) == 1)
                _BEAMSHIP[0].X -= MOVESPEED;
            if (DX.CheckHitKey(DX.KEY_INPUT_RIGHT) == 1)
                _BEAMSHIP[0].X += MOVESPEED;
            if ((DX.GetJoypadInputState(DX.DX_INPUT_PAD1) & DX.PAD_INPUT_LEFT) != 0)
                _BEAMSHIP[0].X -= MOVESPEED;
            if ((DX.GetJoypadInputState(DX.DX_INPUT_PAD1) & DX.PAD_INPUT_RIGHT) != 0)
                _BEAMSHIP[0].X += MOVESPEED;

            if (_BEAMSHIP[0].X < 0)
                _BEAMSHIP[0].X = 0;
            if (_BEAMSHIP[0].X > Settings.FORM_WIDTH)
                _BEAMSHIP[0].X = Settings.FORM_WIDTH;
        }

        /// <summary>
        /// 自機ビーム発射
        /// </summary>
        private void BeamShoot() {

            if (--_BEAM_WAIT > 0)
                return;

            if (_BEAM.Count >= Settings.BEAM_MAX_COUNT)
                return;

            // ビーム発射
            int gr = _DXGRAPH.BEAM[0];
            int bx = _BEAMSHIP[0].X;
            int by = _BEAMSHIP[0].Y - 8;
            _BEAM.Add(new PositionState(bx, by, gr, gr, 0, 0, -8, 0));

            // 自機ショット音
            DX.PlaySoundMem(_GAME_SOUND.DXSOUND[0], DX.DX_PLAYTYPE_BACK, DX.TRUE);

            _BEAM_WAIT = 6;
        }

        /// <summary>
        /// 敵機ミサイル
        /// </summary>
        private void MoveMissile() {

            if (_MISSILE.Count == 0)
                return;

            for (int i = 0; i < _MISSILE.Count; i++) {
                _MISSILE[i].Y += _MISSILE[i].AmountY;
                if (_MISSILE[i].Y >= (23 * 16)) {
                    _MISSILE.RemoveAt(i);
                    continue;
                }
                // ミサイルの矩形を取得
                Rectangle o_rect = GraphToRect(_MISSILE[i]);

                //
                foreach (PositionState b in _BEAMSHIP) {
                    if (b.IsDead)
                        continue;
                    Rectangle b_rect = new Rectangle(b.X - 20, b.Y - 8, 40, 16);
                    if (o_rect.Right < b_rect.Left)
                        continue;
                    if (o_rect.Bottom < b_rect.Top)
                        continue;
                    if (b_rect.Right < o_rect.Left)
                        continue;
                    if (b_rect.Bottom < o_rect.Top)
                        continue;
                    b.IsDead = true;
                    _MISSILE.RemoveAt(i);

                    DX.PlaySoundMem(_GAME_SOUND.DXSOUND[8], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                    break;
                }
                
                foreach (PositionState b in _BARRIER) {
                    if (b.IsDead)
                        continue;
                    Rectangle b_rect = new Rectangle(b.X - 4, b.Y - 8, 8, 16);
                    if (o_rect.Right < b_rect.Left)
                        continue;
                    if (o_rect.Bottom < b_rect.Top)
                        continue;
                    if (b_rect.Right < o_rect.Left)
                        continue;
                    if (b_rect.Bottom < o_rect.Top)
                        continue;
                    b.IsDead = true;
                    _MISSILE.RemoveAt(i);
                    break;
                }

                foreach (PositionState b in _CITY) {
                    if (b.IsDead)
                        continue;
                    Rectangle b_rect = new Rectangle(b.X, b.Y, 32, 16);
                    if (o_rect.Right < b_rect.Left)
                        continue;
                    if (o_rect.Bottom < b_rect.Top)
                        continue;
                    if (b_rect.Right < o_rect.Left)
                        continue;
                    if (b_rect.Bottom < o_rect.Top)
                        continue;
                    b.IsDead = true;

                    //
                    int gr = _DXGRAPH.CITY[2];
                    _CITY_DESTORY.Add(new PositionState(b.X, b.Y, gr, gr, 20, 0, 0, 0));
                    _MISSILE.RemoveAt(i);
                    
                    // CITY爆破音
                    DX.PlaySoundMem(_GAME_SOUND.DXSOUND[2], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                    break;
                }
            }
        }
        
        /// <summary>
        /// 敵期の移動
        /// </summary>
        private void MoveEnemies() {

            foreach (PositionState o in _ENEMIES) {
                if (o.IsDead)
                    continue;
                if (o.GrHandle == _DXGRAPH.TOPON[0])
                    MoveTopon(o);
                else
                    EnemyMove(o);

                if (IsHitBeam(o)) {
                    
                    if (--o.Count > 0) {
                        DX.PlaySoundMem(_GAME_SOUND.DXSOUND[1], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                        continue;
                    }

                    o.IsDead = true;
                    _SCORE += o.Score;
                    if (_SCORE > _HISCORE)
                        _HISCORE = _SCORE;
                    if (_SCORE >= Settings.BEAMSHIP_THRESHOLD_SCORE && _1UPPED == false) {
                        _1UPPED = true;
                        DX.PlaySoundMem(_GAME_SOUND.DXSOUND[10], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                    }
                    
                    // 次に変化する敵機
                    NextEnemy(o);
                    
                    if (o.GrHandle == _DXGRAPH.TOPSIN[0])
                        DX.PlaySoundMem(_GAME_SOUND.DXSOUND[7], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                    
                    continue;
                }

                // TOPSIDER-STATION、TOPSIN、TOPONNはミサイルを発射しない
                if (o.GrHandle == _DXGRAPH.TOPSIDER_STATION[0] || 
                    o.GrHandle == _DXGRAPH.TOPSIDER_STATION[1] || 
                    o.GrHandle == _DXGRAPH.TOPSIN[0] ||
                    o.GrHandle == _DXGRAPH.TOPSIN[1] ||
                    o.GrHandle == _DXGRAPH.TOPON[0])
                    continue;
                
                MissileDrop(o);
            }

            for (int i = 0; i < _QUEUE.Count; i++) {
                _ENEMIES.Add(_QUEUE.Dequeue());
            }
        }

        /// <summary>
        /// 敵機の移動
        /// </summary>
        /// <param name="o"></param>
        private void EnemyMove(PositionState o) {

            o.X += o.AmountX;
            o.Y += o.AmountY;
            if (o.X < 0) {
                o.X = 0;
                o.AmountX = -(o.AmountX);
            }
            if (o.X > Settings.FORM_WIDTH) {
                o.X = Settings.FORM_WIDTH;
                o.AmountX = -(o.AmountX);
            }
            if (o.Y < 0) {
                o.Y = 0;
                o.AmountY = -(o.AmountY);
            }
            if (o.Y > Settings.FORM_HEIGHT - (11 * 16)) {
                o.Y = Settings.FORM_HEIGHT - (11 * 16);
                o.AmountY = -(o.AmountY);
            }
        }

        /// <summary>
        /// TOPONの移動
        /// </summary>
        /// <param name="o"></param>
        private void MoveTopon(PositionState o) {

            o.Y += o.AmountY;
            if (o.Y > Settings.FORM_HEIGHT - (3 * 16)) {
                o.IsDead = true;
                return;
            }
            Rectangle o_rect = GraphToRect(o);
            foreach (PositionState b in _BARRIER) {
                if (b.IsDead)
                    continue;
                Rectangle b_rect = new Rectangle(b.X - 4, b.Y - 8, 8, 16);
                if (o_rect.Right < b_rect.Left)
                    continue;
                if (o_rect.Bottom < b_rect.Top)
                    continue;
                if (b_rect.Right < o_rect.Left)
                    continue;
                if (b_rect.Bottom < o_rect.Top)
                    continue;
                o.IsDead = true;
                b.IsDead = true;
                break;
            }
            foreach (PositionState b in _CITY) {
                if (b.IsDead)
                    continue;
                Rectangle b_rect = new Rectangle(b.X - 16, b.Y - 8, 32, 16);
                if (o_rect.Right < b_rect.Left)
                    continue;
                if (o_rect.Bottom < b_rect.Top)
                    continue;
                if (b_rect.Right < o_rect.Left)
                    continue;
                if (b_rect.Bottom < o_rect.Top)
                    continue;
                o.IsDead = true;
                b.IsDead = true;
                break;
            }
            foreach (PositionState b in _BEAMSHIP) {
                Rectangle b_rect = new Rectangle(b.X - 20, b.Y - 8, 40, 16);
                if (o_rect.Right < b_rect.Left)
                    continue;
                if (o_rect.Bottom < b_rect.Top)
                    continue;
                if (b_rect.Right < o_rect.Left)
                    continue;
                if (b_rect.Bottom < o_rect.Top)
                    continue;
                o.IsDead = true;
                b.IsDead = true;
                DX.PlaySoundMem(_GAME_SOUND.DXSOUND[8], DX.DX_PLAYTYPE_BACK, DX.TRUE);
                break;
            }
        }

        /// <summary>
        /// 次に出現する敵機を選択する。
        /// </summary>
        /// <param name="o"></param>
        private void NextEnemy(PositionState o) {

            // TOPSIDER-STATION -> TOPSIDER-UFO (SCORE=40)
            if (o.GrHandle == _DXGRAPH.TOPSIDER_STATION[0] ||
                o.GrHandle == _DXGRAPH.TOPSIDER_STATION[1]) {
                //
                int gr = _DXGRAPH.TOPSIDER_UFO[0];
                int destroy = Settings.TOPSIDER_UFO_DESTROY_COUNT;
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr, gr, destroy, -2, 0, 40));
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr, gr, destroy, +2, 0, 40));
                return;
            }
            // TOPSIDER-UFO -> TOPSIDER (SCORE=60)            
            if (o.GrHandle == _DXGRAPH.TOPSIDER_UFO[0]) {
                //
                int gr0 = _DXGRAPH.TOPSIDER[0];
                int gr1 = _DXGRAPH.TOPSIDER[1];
                int destroy = Settings.TOPSIDER_DESTROY_COUNT;
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr0, gr1, destroy, -4, 0, 60));
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr1, gr0, destroy, +4, 0, 60));
                return;
            }
            // TOPSIDER -> TOPSIN (SCORE=80)
            if (o.GrHandle == _DXGRAPH.TOPSIDER[0] || o.GrHandle == _DXGRAPH.TOPSIDER[1]) {
                //
                int gr0 = _DXGRAPH.TOPSIN[0];
                int gr1 = _DXGRAPH.TOPSIN[1];
                int destroy = Settings.TOPSIN_DESTROY_COUNT;
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr0, gr1, destroy, -2, +2, 80));
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr1, gr0, destroy, +2, +2, 80));
                return;
            }
            // TOPSIN -> TOPON (SCORE=100)
            if (o.GrHandle == _DXGRAPH.TOPSIN[0] || o.GrHandle == _DXGRAPH.TOPSIN[1]) {
                //
                int gr = _DXGRAPH.TOPON[0];
                int destroy = Settings.TOPSIN_DESTROY_COUNT;
                _QUEUE.Enqueue(new PositionState(o.X, o.Y, gr, gr, destroy, 0, +2, 100));
                return;
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o">敵機の情報</param>
        /// <returns>BEAMにヒットしていたらtrue。</returns>
        private bool IsHitBeam(PositionState o) {

            // 敵機のRectangleを取得
            Rectangle o_rect = GraphToRect(o);

            foreach (PositionState b in _BEAM) {
                if (b.IsDead)
                    continue;
                // BEAMのRectangleを取得
                Rectangle b_rect = new Rectangle(b.X - 1, b.Y - 6, 2, 12);
                if (o_rect.Right < b_rect.Left)
                    return false;
                if (o_rect.Bottom < b_rect.Top)
                    return false;
                if (b_rect.Right < o_rect.Left)
                    return false;
                if (b_rect.Bottom < o_rect.Top)
                    return false;
                _BEAM.Remove(b);
                return true;
            }
            return false;
        }

        private Rectangle GraphToRect(PositionState o) {

            int w = 0;
            int h = 0;
            DX.GetGraphSize(o.GrHandle, out w, out h);
            return new Rectangle(o.X - (w / 2), o.Y - (h / 2), w, h);
        }

        private void MissileDrop(PositionState o) {

            if (random.Next(500) < Settings.PROBABILITY_OF_DROPBOMB) {
                int gr = _DXGRAPH.MISSILE[0];
                _MISSILE.Add(new PositionState(o.X, o.Y, gr, gr, 1, 0, +4, 0));
            }
        }

        /// <summary>
        /// 描画対象にレンダリングを行う。
        /// </summary>
        /// <param name="state"></param>
        private void DrawContent(GameState state) {

            if (--_TIMMING < 0) {
                _PAT = (_PAT == 0) ? 1 : 0;
                _TIMMING = 12;
            }

			foreach (Point star in _STARS)
				DrawImage(star.X, star.Y, _DXGRAPH.STAR[0]);

            // スコア
            string sc1 = "SCORE";
            string sc2 = _SCORE.ToString().PadLeft(7, ' ');
            string sc3 = "HI-SCORE";
            string sc4 = _HISCORE.ToString().PadLeft(7, ' ');
            string sc5 = "BARRIER";
            string sc6 = _BARRIER_LEFT.ToString().PadLeft(2, ' ');
            string sc7 = "BEAMSHIP";
            string sc8 = _SHIP_LEFT.ToString().PadLeft(2, ' ');

            DrawString(5, 25, sc1, Settings.Yellow);
            DrawString(12, 25, sc2, Settings.Yellow);
            DrawString(25, 25, sc3, Settings.Yellow);
            DrawString(35, 25, sc4, Settings.Yellow);
            DrawString(50, 25, sc5, Settings.Yellow);
            DrawString(59, 25, sc6, Settings.Yellow);
            DrawString(65, 25, sc7, Settings.Yellow);
            DrawString(75, 25, sc8, Settings.Yellow);

            // BARRIERの描画
            foreach (PositionState o in _BARRIER)
                if (!o.IsDead)
                    DrawImage(o.X, o.Y, o.GrHandle);

            foreach (PositionState o in _CITY)
                if (o.IsDead)
                    DrawImage(o.X, o.Y, o.GrHandle2);
                else
                    DrawImage(o.X, o.Y, o.GrHandle);
            
            foreach (PositionState o in _CITY_DESTORY)
                DrawImage(o.X, o.Y, o.GrHandle);

            foreach (PositionState o in _MISSILE)
                DrawImageF(o.X, o.Y, o.GrHandle);

            foreach (PositionState o in _ENEMIES)
                if (!o.IsDead)
                    if (_PAT == 0)
                        DrawImageF(o.X, o.Y, o.GrHandle);
                    else
                        DrawImageF(o.X, o.Y, o.GrHandle2);

            foreach (PositionState o in _BEAM)
                DrawImageF(o.X, o.Y, o.GrHandle);

            foreach (PositionState o in _BEAMSHIP)
            if (o.IsDead)
                DrawImageF(o.X, o.Y, o.GrHandle2);
            else
                DrawImageF(o.X, o.Y, o.GrHandle);

            // バリア
            if (state == GameState.Barrier) {
                if (_BARRIER_LEFT == Settings.BARRIER_INIT_COUNT) {
                    string bs1 = "S T A G E   " + _STAGE.ToString();
                    DrawString(34, 8, bs1, Settings.Yellow);
                }
            }

            // ゲームオーバー
            if (state == GameState.GameOver) {
                string cc1 = "G A M E   O V E R";
                DrawString(26, 10, cc1, Settings.Red, _GAME_FONT.DXFONT[0, 1]);
            }

            // ステージクリア
            if (state == GameState.StageClear) {
                string ts1 = _CITY_SCORE.ToString().PadLeft(3, ' ');
                string ts2 = _CITY_REMAIN.ToString();
                string ts3 = _BONUS_SCORE.ToString().PadLeft(4, ' ');
                string cs1 = "S T A G E   C L E A R";
                string cs3 = string.Format("C I T Y  =  {0}", ts1);
                string cs4 = string.Format("B O N U S  =  {0} x {1} = {2}", ts1, ts2, ts3);
                DrawString(30, 8, cs1, Settings.Yellow);
                DrawString(31, 11, cs3, Settings.Yellow);
                DrawString(29, 13, cs4, Settings.Yellow);
            }

            // ゲームクリア
            if (state == GameState.GameClear) {
                string ce1 = "THE FIFTH SPACE FLEET ARRIVED";
                string ce2 = "AND DESTROYED THE ALIEN TOPSIDER.";
                string ce3 = "THE TWO-DAY BATTLE FOR LUNAR CITY WAS OVER.";
                DrawString(20, 8, ce1, Settings.LimeGreen);
                DrawString(20, 10, ce2, Settings.LimeGreen);
                DrawString(20, 12, ce3, Settings.LimeGreen);
            }

            // 地面を描く
            for (int i = 1; i <= 80; i++) {
                DrawImageC(i, 24, _DXGRAPH.GROUND[0]);
            }
            DrawImageC(1, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(2, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(3, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(4, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(5, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(6, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(7, 23, _DXGRAPH.GROUND[0]);
            
            DrawImageC(19, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(20, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(21, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(22, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(23, 23, _DXGRAPH.GROUND[0]);

            DrawImageC(30, 23, _DXGRAPH.GROUND[2]);
            DrawImageC(31, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(32, 23, _DXGRAPH.GROUND[3]);

            DrawImageC(46, 23, _DXGRAPH.GROUND[4]);
            DrawImageC(47, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(48, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(49, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(50, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(51, 23, _DXGRAPH.GROUND[5]);

            

            DrawImageC(73, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(74, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(75, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(76, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(77, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(78, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(79, 23, _DXGRAPH.GROUND[0]);
            DrawImageC(80, 23, _DXGRAPH.GROUND[0]);

        }

        /// <summary>
        /// 文字列を描画する。
        /// </summary>
        /// <param name="x">X軸カラム位置を指定する。</param>
        /// <param name="y">Y軸カラム位置を指定する。</param>
        /// <param name="s">描画する文字列を指定する。</param>
        /// <param name="c">文字色を指定する。</param>
        private void DrawString(int x, int y, string s, uint c) {

            DX.DrawStringToHandle((x - 1) * 8, (y - 1) * 16, s, c, _GAME_FONT.DXFONT[0, 0]);
		}

        private void DrawString(int x, int y, string s, uint c, int font) {

            DX.DrawStringToHandle((x - 1) * 8, (y - 1) * 16, s, c, font);
		}

        /// <summary>
        /// ビットマップを描画する。起点は左上。
        /// </summary>
        /// <param name="x">X軸ドット位置を指定する。</param>
        /// <param name="y">Y軸ドット位置を指定する。</param>
        /// <param name="b">DxLibへ登録したビットマップ番号を指定する。</param>
		private void DrawImage(int x, int y, int b) {

            DX.DrawGraph(x, y, b, DX.TRUE);
		}

        /// <summary>
        /// ビットマップを描画する。起点は左上。
        /// </summary>
        /// <param name="x">X軸カラム位置を指定する。</param>
        /// <param name="y">Y軸カラム位置を指定する。</param>
        /// <param name="b">DxLibへ登録したビットマップ番号を指定する。</param>
		private void DrawImageC(int x, int y, int b) {

            DX.DrawGraph((x - 1) * 8, (y - 1) * 16, b, DX.TRUE);
		}

        /// <summary>
        /// ビットマップを描画する。起点は中央。
        /// </summary>
        /// <param name="x">X軸ドット位置を指定する。</param>
        /// <param name="y">Y軸ドット位置を指定する。</param>
        /// <param name="b">DxLibへ登録したビットマップ番号を指定する。</param>
		private void DrawImageF(int x, int y, int b) {

			DX.DrawRotaGraphF(x, y, 1.0F, 0.0F, b, DX.TRUE);
		}
    }
}

/**
            1         2         3         4         5         6         7         8
   ----+----0----+----0----+----0----+----0----+----0----+----0----+----0----+----0
   ----+----0----+----0----+----0----+----0----+----0----+----0----+----0----+----0
            1         2         3         4         5         6         7         8
   ----+----0----+----0----+----0----+----0----+----0----+----0----+----0----+----0
 1|                                                                                |
 2|                 ####   L U N A R   C I T Y   S O S ! !   ####                  |
 3|                                                                                |
 4|                                                                                |
 5|                                  TOPSIDER STATION    20  POINTS                |
 6|                                   
 7|                                  TOPSIDER UFO        40  POINTS                |
 8|
 9|                                  TOPSIDER            60  POINTS                |
10|
11|                                  TOPSIN              80  POINTS                |
12|
13|                                  TOPON              100  POINTS                |
14|                                                                                |
15|
16|XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX|
17|
18|
19|        SHIP 
20|
21|@@@
22|@@@@@@ XXXX  XXXX @@@@@XXXX @@@@ XXXX  XXXX @@@@@@ XXXX    XXXX   XXXX @@@@@@@@@|
23|@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@|
24|    SCORE  0000000      HI-SCORE  0000000        BARRIER  0     BEAM-SHIP 03    |
   ----+----0----+----0----+----0----+----0----+----0----+----0----+----0----+----0
  |                          C O N G R A T U L A T I O N !                         |
  |                             S T A G E   C L E A R                              |
  |                              C I T Y  =  xxxx                                  |
  |                            B O N U S  =  xxxx x x = xxxx                       |
  |                                 S T A G E   1
  |           CC  OO  NN  GG  RR  AA  TT  UU  LL  AA  TT  II  OO  NN  !!
   ----------++++++++++///                                  ///++++++++++----------|
 */

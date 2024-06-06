# Name
LUNAR CITY SOS!! for C# with DxLib.

月間I/O 1981年4月に掲載された投稿プログラムを勝手にC#へ移植。
オリジナルは、芸夢狂人氏。

# DEMO
```2001年 1月 1日
LUNAR CITY は TOPSIDERの襲撃を受けつつあり、支給援軍を送られたし。
　　　　　　　　　　　　　　　　　　　　　　　　LUNAR CITY 防衛隊
```

# Features
当初は、C#のみで作成していましたが音が上手くだせないと悩んでいたところ
C#でDxLibを使用できることがわかったので、DxLib版へ進路変更しました。
DxLibを使用するため、コントローラーも使えます。

# Requirement
下記のものが必要になります。
* DxLib (VisualC# 用パッケージ)
* C# version 5 (Windows標準)

# Installation
DxLib (VisualC# 用パッケージ)のインストール
ＤＸライブラリ置き場　https://dxlib.xsrv.jp/ の「ＤＸライブラリのダウンロード」ページにある
「VisualC# 用パッケージ」をダウンロードしてください。
zip形式で圧縮されているので、解凍後以下のファイルをプロジェクト内にコピーする。
* DxDLL.cs       コンパイル時に必要
* DxLib.dll      実行時に必要
* DxLib_x64.dll  実行時に必要
* DxLibDotNet    実行時に必要

実行時に必要なdllは、PATHの通った場所に配置してもらっても問題ありません。
プロジェクト内の「コンパイル.bat」を実行して.exeを作成します。

C#のコンパイラは、「コンパイル.bat」にパスを記載していますので適宜変更をお願いします。
DotNET Frameworkのバージョンはv4.0.30319です。（Windows11のもの）

# Usage
「コンパイル.bat」で作成した.exeを実行します。

Setings.xmlについて
* DxLog                  true:ログ作成 false:ログ作成しない
* FullScreenMode         true:フルスクリーンモード false:ウィンドウモード
* MaximumBeamShip        ビームシップ最大数:0～99（0は、デフォルト4機）
* NumberOfBarrierRepair  バリアの最大数:0～99（0は、デフォルト）
* AttackRatio            敵ミサイルドロップ率:1～100

# Note
キャラクターは、月間I/Oに記載のドット絵を描き起こしたものです。オリジナルにできるだけ似るようにしました。
記載から40年以上経ちましたが、まだオリジナルで遊んだことはありません。動画配信サイトなどで観察して作成しました。

参考：
https://archive.org/search?query=%E3%82%A2%E3%82%A4%E3%83%BB%E3%82%AA%E3%83%BC+1981+4

効果音については、
ユウラボ8bitサウンド工房 http://www.skipmore.com/sound/ を使用しました。

# Author
 
# License

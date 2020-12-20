# SelfMemoPrototype

![rapture_20201213162204](https://user-images.githubusercontent.com/16277861/102005836-6627bd80-3d5f-11eb-90da-fc34409f1307.png)

たまに使うけど忘れがちな用語、言い回しなどを記録しておき、忘れたときに素早く調べるための個人メモツールです。

「この言葉、毎回検索してるけど全然覚えられない…」みたいな用語をメモっておき、検索の時短につなげる、といった使用方法を想定していますが、
単語帳や備忘録、セルフ辞書ツールなどとしても使えるかと思います。

### 特徴

- いつでもショートカットキーで一発起動
- キーワードや説明文全部を対象にリアルタイム検索
- リストにない用語はシンプルなフォームで追加

## 使い方

### 起動

実行すると常駐します。
ショートカット（デフォルトではAlt+Shift+F2）で起動します。

### 用語登録

メニューの「登録ダイアログを開く (Ctrl+R) 」から用語を追加できます。

![rapture_20201213162205_1](https://user-images.githubusercontent.com/16277861/102005839-69bb4480-3d5f-11eb-9f46-b94887bbbcf0.png)

### 検索

![rapture_20201213162205](https://user-images.githubusercontent.com/16277861/102005838-67f18100-3d5f-11eb-901b-7b3d83e63e13.png)

検索フォームに入力することで、キーワードリストの内容がフィルタされます。スペース区切りでAND検索可能。

カテゴリ にチェックを入れると、カテゴリで表示をフィルタすることができます。

## 利用例

- 「キーワード」に日本語、「説明」に英文や英単語を入力し、英作文でよく使う言い回しリストにする
- 「キーワード」に英単語、「キーワード2」に日本語訳、「説明」に用例を入力し、マイ英単語帳にする
- 「キーワード」に略語、「キーワード2」に正式名称、「説明」に詳細を入力し、略語解説リストにする

など？

## Thanks

タスクトレイ常駐、グローバルホットキーの実装は下記記事を参考に（丸パクリ）させていただきました。

- [WPF用にNotifyIconクラスをラップしてみた](http://sourcechord.hatenablog.com/entry/2017/02/11/125649)
- [WPFでホットキーの登録](http://sourcechord.hatenablog.com/entry/2017/02/13/005456)
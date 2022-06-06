using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum RotState
    {
        Up=0,
        Right=1,
        Down=2,
        Left=3,
        Invalid=-1,
    }

    //ぷよのスクリプトを配列の形で持つ
    //インスペクターで設定できるように「[SerializeField]」をつける
    //設定もれに気づけるように「default!」を初期値として設定
    [SerializeField] PuyoController[] _puyoControllers = new PuyoController[2] { default!, default! };
    //「BoardController」はあらかじめ設定しておくように「[SerializeField]」をつけてメンバー変数として用意しておく
    [SerializeField] BoardController boardController = default!;

    Vector2Int _position; //軸ぷよの位置
    //角度は0:上 1:右 2:下 3:左 で持つ（子ぷよの位置）
    //プレイヤーの開店の状態として位置「_position」に対応するメンバー変数「_rotate」を導入します。
    RotState _rotate = RotState.Up; 

    // Start is called before the first frame update
    void Start()
    {
        //ひとまず決め打ちで色を決定
        //ぷよぷよの色を設定。ひとまず適当に設定。
        _puyoControllers[0].SetPuyoType(PuyoType.Green);
        _puyoControllers[1].SetPuyoType(PuyoType.Red);

        //軸ぷよの位置を（2,12）に設定
        _position = new Vector2Int(2, 12);
        _rotate = RotState.Up;

        //表示するゲームオブジェクトの位置を設定
        _puyoControllers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        //「Start」メソッドで子ゲームオブジェクトの位置を設定
        Vector2Int posChild = CalcChildPuyoPos(_position, _rotate);
        _puyoControllers[1].SetPos(new Vector3((float)_position.x, (float)_position.y + 1.0f, 0.0f));
    }

    //「CalcChildPuyoPos」の計算の高速化のために定数の配列「rotate_tbl」を用意
    static readonly Vector2Int[] rotate_tbl = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    //回転した際の位置の計算
    //「RotState」の値に応じて、子ぷよがどの位置にいるか計算する「CalcChildPuyoPos」を導入する
    private static Vector2Int CalcChildPuyoPos(Vector2Int pos,RotState rot)
    {
        //「_rotate」の値を整数に変換して、テーブル引き
        return pos + rotate_tbl[(int)rot];
    }

    //キーが押された際に処理するメソッド「Translateを用意する」
    private bool CanMove(Vector2Int pos,RotState rot)
    {
        //「BoardController」スクリプトに移動した先の軸ぷよと子ぷよの状態が秋になっているか問い合わせる
        //移動できていないなら処理を終了
        if (!boardController.CanSettle(pos)) return false;
        //if (!boardController.CanSettle(pos + Vector2Int.up)) return false; //修正したのでコメント化
        //「CanMove」メソッドでの子ゲームオブジェクトの位置の検証の修正
        if (!boardController.CanSettle(CalcChildPuyoPos(pos, rot))) return false;

        return true;
    }

    //回転させる処理
    bool Rotate(bool is_right)
    {
        //ローカル変数「rot」を導入
        //「rot」は「_rotate」の値を右回転、左開店に応じて+1、-1する
        //※-1したときに負になるのは避けたいので４の剰余を取った際に同じ値になるように-1+4の+3を-1の代わりに取る
        //「rot」を進めた後、0-3の範囲の値を取るように4の剰余の計算を行う
        //※コンピュータに高速に処理することを明示的に表すように「%4」ではなく「&3」で（「&(4-1)」で）計算する
        RotState rot = (RotState)(((int)_rotate + (is_right ? +1 : +3)) & 3);

        //仮想的に移動できるか検証する（上下左右にずらしたときも確認）
        Vector2Int pos = _position;

        switch(rot)
        {
            case RotState.Down:
                //右（左）から下に回転：自分の下か右（左）にブロックがあれば引きあがる
                if(!boardController.CanSettle(pos+Vector2Int.down)||!boardController.CanSettle(pos+new Vector2Int(is_right?1:-1,-1)))
                {
                    pos += Vector2Int.up;
                }
                break;
            case RotState.Right:
                //上（下）から右に回転：右がうまッていれば、左に移動
                if (!boardController.CanSettle(pos + Vector2Int.right)) pos += Vector2Int.left;
                break;
            case RotState.Left:
                //上（下）から左に回転：左が埋まっていれば、右に移動
                if (!boardController.CanSettle(pos + Vector2Int.left)) pos += Vector2Int.right;
                break;
            case RotState.Up:
                break;
            default:
                Debug.Assert(false);
                break;
        }

        //仮想的に動かしてみて、そこに行けないようであれば処理を終える
        if (!CanMove(pos, rot)) return false;

        //実際に移動
        //動けるようであれば。メンバー変数を更新
        _rotate = rot;
        _position = pos;

        //表示されるゲームオブジェクトの位置を更新
        _puyoControllers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        Vector2Int posChild = CalcChildPuyoPos(_position, _rotate);
        _puyoControllers[1].SetPos(new Vector3((float)posChild.x, (float)posChild.y, 0.0f));

        return true;
    }

    void QuickDrop()
    {
        //落ちれる一番下まで落ちる
        //一段ずつ落とせるか確認して、落とせなくなる直前の場所を取得する
        Vector2Int pos = _position;
        do
        {
            pos += Vector2Int.down;
        } while (CanMove(pos, _rotate));
        pos -= Vector2Int.down; //一つ上の場所（最後に置けた場所に戻す）

        _position = pos;

        //直接接地
        //接地する場所が判明したら、ボードの情報を交信する
        bool is_set0= boardController.Settle(_position, (int)_puyoControllers[0].GetPuyoType());
        Debug.Assert(is_set0); //本当に置けたかどうかのアサーションを使ってすぐわかるようにした

        bool is_set1 = boardController.Settle(CalcChildPuyoPos(_position, _rotate), (int)_puyoControllers[1].GetPuyoType());
        Debug.Assert(is_set1); //本当に置けたかどうかのアサーションを使ってすぐわかるようにした

        //ボードにぷよを置いたら（プレイヤーの方で表示しているぷよを消すため）自分のゲームオブジェクトを非アクティブにする
        gameObject.SetActive(false);
    }

    private bool Translate(bool is_right)
    {
        //仮想的に移動できるか検証する
        //動いた先の場所を計算
        //右移動ならｘ成分を＋１、そうでなければ（左移動なら）ｘ成分をー１する
        Vector2Int pos = _position + (is_right ? Vector2Int.right : Vector2Int.left);
        //移動できるか検証する
        //if (!CanMove(pos)) return false; //修正したのでコメント化
        if (!CanMove(pos, _rotate)) return false;

        //実際に移動
        //「_position」プロパティを更新
        _position = pos;

        //ぷよのゲームオブジェクトの位置を更新
        _puyoControllers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        //「Translate」メソッドでの子ゲームオブジェクトの位置の検証の修正
        Vector2Int posChiild = CalcChildPuyoPos(_position, _rotate);
        _puyoControllers[1].SetPos(new Vector3((float)_position.x, (float)_position.y + 1.0f, 0.0f));

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        //平行移動のキー入力取得
        //「Update」内で「Input.GetKeyDown」で「→」と「←」が押されたのを毎フレーム調べる
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Translate(true);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Translate(false);
        }

        //回転のキー入力取得
        //回転キーが押されたら「Rotate」メソッドで処理することにする
        //「x」で右回転を受付け
        if(Input.GetKeyDown(KeyCode.X))//
        {
            Rotate(true);
        }
        //「z」で左開店を受付け
        if(Input.GetKeyDown(KeyCode.Z))//
        {
            Rotate(false);
        }

        //クイックドロップのキー入力取得
        //「↑」が押された際に処理
        //「QuickDrop」を呼び出す
        if(Input.GetKey(KeyCode.UpArrow))
        {
            QuickDrop();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //const定義で幅と高さを定義
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    //「Instantiate」呼び出しで型情報が必要なのでひな形となるぷよのPrefabを設定するようにする
    //「[SerializedField]」でエディタから設定できるようにする
    //「=default!」で必ず設定しなくてはならないようにする
    [SerializeField] GameObject prefabPuyo = default!;

    //ゲーム世界を２次元配列としてメンバーに用意
    //配列の要素はｘ成分を後ろにする
    //牛をの次元の方がメモリ的に近くに配置される
    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    //ゲームオブジェクトの配列を保持
    GameObject[,] _Puyos = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    private void ClearAll()
    {
        for(int y=0;y<BOARD_HEIGHT;y++)
        {
            //ループを回す際はｘ成分を内側にする方がメモリキャッシュに乗りやすくなる
            for(int x=0;x<BOARD_WIDTH;x++)
            {
                _board[y, x] = 0;

                //クリア処理にオブジェクトの破棄を追加
                //BoardControllerが消えるときにクリア処理を呼ぶと安心
                if (_Puyos[y, x] != null) Destroy(_Puyos[y, x]);
                _Puyos[y, x] = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Startで２時限配列を初期化
        ClearAll();

        ////２重のforループでボード全体をなめる
        //for(int y=0;y<BOARD_HEIGHT;y++)
        //{
        //    for(int x=0;x<BOARD_WIDTH;x++)
        //    {
        //        //[1,6]の範囲の乱数を生成して、それを各ボードの弁目に設定する
        //        Settle(new Vector2Int(x,y), Random.Range(1, 7));
        //    }
        //}
    }

    public static bool IsValidated(Vector2Int pos)
    {
        //置こうとしている場所は盤面をはみ出していないか
        return 0 <= pos.x && pos.x < BOARD_WIDTH && 0 <= pos.y && pos.y < BOARD_HEIGHT;
    }

    public bool CanSettle(Vector2Int pos)
    {
        if (!IsValidated(pos)) return false;
        
        //配列の値が埋まっていないか（０になっていないか）
        return 0 == _board[pos.y, pos.x];
    }

    //配列「_board」に値を設定するメソッド「Settle」を用意する
    public bool Settle(Vector2Int pos, int val)
    {
        //値を設定する前に置くことができるのかチェックする
        if (!CanSettle(pos)) return false;
        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyos[pos.y, pos.x] == null); //

        //親の位置をより組むために、その前に設定されている親の位置であるtransform.positionの値を足し込む
        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);

        //「Instantiateを呼び出し」
        _Puyos[pos.y, pos.x] = Instantiate(prefabPuyo, world_position, Quaternion.identity, transform);
        //ゲームオブジェクトの色を設定する
        //GetComponentを使うが、「Settle」メソッドは毎フレーム呼ばれるわけではないと考え、今回はPuyoControllerを変数として保持する処理はやめてみた
        _Puyos[pos.y,pos.x].GetComponent<PuyoController>().SetPuyoType((PuyoType)val);

        return true;
    }
}

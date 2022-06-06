using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//列挙型メンバーでしか色を指定できないようにする
public enum PuyoType
{
    Blank=0,　//未定義に対応する「０」

    Green =1,
    Red=2,
    Yellow=3,
    Blue=4,
    Purple=5,
    Cyan=6,

    Invalid=7,　//不正に対応する最大の値「７」を定義する
};

[RequireComponent(typeof(Renderer))] //「RequireComponet」をつけて、Rendererコンポーネントをつけ忘れも防ぐ
public class PuyoController : MonoBehaviour
{
    //列挙型に対応する色を定義する
    static readonly Color[] color_table = new Color[]
    {
        Color.black,

        Color.green,
        Color.red,
        Color.yellow,
        Color.blue,
        Color.magenta,
        Color.cyan,

        Color.gray,
    };

    [SerializeField] Renderer my_renderer = default!; 

    //自分自身の色を変えられるように、Rendererを保持する
    //[SerializeField]でエディターで指定する形にする
    //設定が忘れが内容に「=defeult!」で非null指定しておく
    PuyoType _type = PuyoType.Invalid;
    
    //set/getを導入
    public void SetPuyoType(PuyoType type)
    {
        _type = type;

        my_renderer.material.color = color_table[(int)_type]; //typeが設定されたときに引数のテーブル引きで色を設定
    }

    public PuyoType GetPuyoType()
    {
        return _type;
    }

    //外部から位置を変えるためのSetPosを用意する
    public void SetPos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
}

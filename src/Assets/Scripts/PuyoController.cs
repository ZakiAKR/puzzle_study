using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�񋓌^�����o�[�ł����F���w��ł��Ȃ��悤�ɂ���
public enum PuyoType
{
    Blank=0,�@//����`�ɑΉ�����u�O�v

    Green =1,
    Red=2,
    Yellow=3,
    Blue=4,
    Purple=5,
    Cyan=6,

    Invalid=7,�@//�s���ɑΉ�����ő�̒l�u�V�v���`����
};

[RequireComponent(typeof(Renderer))] //�uRequireComponet�v�����āARenderer�R���|�[�l���g�����Y����h��
public class PuyoController : MonoBehaviour
{
    //�񋓌^�ɑΉ�����F���`����
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

    //�������g�̐F��ς�����悤�ɁARenderer��ێ�����
    //[SerializeField]�ŃG�f�B�^�[�Ŏw�肷��`�ɂ���
    //�ݒ肪�Y�ꂪ���e�Ɂu=defeult!�v�Ŕ�null�w�肵�Ă���
    PuyoType _type = PuyoType.Invalid;
    
    //set/get�𓱓�
    public void SetPuyoType(PuyoType type)
    {
        _type = type;

        my_renderer.material.color = color_table[(int)_type]; //type���ݒ肳�ꂽ�Ƃ��Ɉ����̃e�[�u�������ŐF��ݒ�
    }

    public PuyoType GetPuyoType()
    {
        return _type;
    }

    //�O������ʒu��ς��邽�߂�SetPos��p�ӂ���
    public void SetPos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
}

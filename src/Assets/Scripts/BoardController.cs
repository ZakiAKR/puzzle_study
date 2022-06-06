using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //const��`�ŕ��ƍ������`
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    //�uInstantiate�v�Ăяo���Ō^��񂪕K�v�Ȃ̂łЂȌ`�ƂȂ�Ղ��Prefab��ݒ肷��悤�ɂ���
    //�u[SerializedField]�v�ŃG�f�B�^����ݒ�ł���悤�ɂ���
    //�u=default!�v�ŕK���ݒ肵�Ȃ��Ă͂Ȃ�Ȃ��悤�ɂ���
    [SerializeField] GameObject prefabPuyo = default!;

    //�Q�[�����E���Q�����z��Ƃ��ă����o�[�ɗp��
    //�z��̗v�f�͂����������ɂ���
    //�����̎����̕����������I�ɋ߂��ɔz�u�����
    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    //�Q�[���I�u�W�F�N�g�̔z���ێ�
    GameObject[,] _Puyos = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    private void ClearAll()
    {
        for(int y=0;y<BOARD_HEIGHT;y++)
        {
            //���[�v���񂷍ۂ͂�����������ɂ�������������L���b�V���ɏ��₷���Ȃ�
            for(int x=0;x<BOARD_WIDTH;x++)
            {
                _board[y, x] = 0;

                //�N���A�����ɃI�u�W�F�N�g�̔j����ǉ�
                //BoardController��������Ƃ��ɃN���A�������ĂԂƈ��S
                if (_Puyos[y, x] != null) Destroy(_Puyos[y, x]);
                _Puyos[y, x] = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Start�łQ�����z���������
        ClearAll();

        ////�Q�d��for���[�v�Ń{�[�h�S�̂��Ȃ߂�
        //for(int y=0;y<BOARD_HEIGHT;y++)
        //{
        //    for(int x=0;x<BOARD_WIDTH;x++)
        //    {
        //        //[1,6]�͈̗̔͂����𐶐����āA������e�{�[�h�ٖ̕ڂɐݒ肷��
        //        Settle(new Vector2Int(x,y), Random.Range(1, 7));
        //    }
        //}
    }

    public static bool IsValidated(Vector2Int pos)
    {
        //�u�����Ƃ��Ă���ꏊ�͔Ֆʂ��͂ݏo���Ă��Ȃ���
        return 0 <= pos.x && pos.x < BOARD_WIDTH && 0 <= pos.y && pos.y < BOARD_HEIGHT;
    }

    public bool CanSettle(Vector2Int pos)
    {
        if (!IsValidated(pos)) return false;
        
        //�z��̒l�����܂��Ă��Ȃ����i�O�ɂȂ��Ă��Ȃ����j
        return 0 == _board[pos.y, pos.x];
    }

    //�z��u_board�v�ɒl��ݒ肷�郁�\�b�h�uSettle�v��p�ӂ���
    public bool Settle(Vector2Int pos, int val)
    {
        //�l��ݒ肷��O�ɒu�����Ƃ��ł���̂��`�F�b�N����
        if (!CanSettle(pos)) return false;
        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyos[pos.y, pos.x] == null); //

        //�e�̈ʒu�����g�ނ��߂ɁA���̑O�ɐݒ肳��Ă���e�̈ʒu�ł���transform.position�̒l�𑫂�����
        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);

        //�uInstantiate���Ăяo���v
        _Puyos[pos.y, pos.x] = Instantiate(prefabPuyo, world_position, Quaternion.identity, transform);
        //�Q�[���I�u�W�F�N�g�̐F��ݒ肷��
        //GetComponent���g�����A�uSettle�v���\�b�h�͖��t���[���Ă΂��킯�ł͂Ȃ��ƍl���A�����PuyoController��ϐ��Ƃ��ĕێ����鏈���͂�߂Ă݂�
        _Puyos[pos.y,pos.x].GetComponent<PuyoController>().SetPuyoType((PuyoType)val);

        return true;
    }
}

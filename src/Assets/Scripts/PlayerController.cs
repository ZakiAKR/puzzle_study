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

    //�Ղ�̃X�N���v�g��z��̌`�Ŏ���
    //�C���X�y�N�^�[�Őݒ�ł���悤�Ɂu[SerializeField]�v������
    //�ݒ����ɋC�Â���悤�Ɂudefault!�v�������l�Ƃ��Đݒ�
    [SerializeField] PuyoController[] _puyoControllers = new PuyoController[2] { default!, default! };
    //�uBoardController�v�͂��炩���ߐݒ肵�Ă����悤�Ɂu[SerializeField]�v�����ă����o�[�ϐ��Ƃ��ėp�ӂ��Ă���
    [SerializeField] BoardController boardController = default!;

    Vector2Int _position; //���Ղ�̈ʒu
    //�p�x��0:�� 1:�E 2:�� 3:�� �Ŏ��i�q�Ղ�̈ʒu�j
    //�v���C���[�̊J�X�̏�ԂƂ��Ĉʒu�u_position�v�ɑΉ����郁���o�[�ϐ��u_rotate�v�𓱓����܂��B
    RotState _rotate = RotState.Up; 

    // Start is called before the first frame update
    void Start()
    {
        //�ЂƂ܂����ߑł��ŐF������
        //�Ղ�Ղ�̐F��ݒ�B�ЂƂ܂��K���ɐݒ�B
        _puyoControllers[0].SetPuyoType(PuyoType.Green);
        _puyoControllers[1].SetPuyoType(PuyoType.Red);

        //���Ղ�̈ʒu���i2,12�j�ɐݒ�
        _position = new Vector2Int(2, 12);
        _rotate = RotState.Up;

        //�\������Q�[���I�u�W�F�N�g�̈ʒu��ݒ�
        _puyoControllers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        //�uStart�v���\�b�h�Ŏq�Q�[���I�u�W�F�N�g�̈ʒu��ݒ�
        Vector2Int posChild = CalcChildPuyoPos(_position, _rotate);
        _puyoControllers[1].SetPos(new Vector3((float)_position.x, (float)_position.y + 1.0f, 0.0f));
    }

    //�uCalcChildPuyoPos�v�̌v�Z�̍������̂��߂ɒ萔�̔z��urotate_tbl�v��p��
    static readonly Vector2Int[] rotate_tbl = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    //��]�����ۂ̈ʒu�̌v�Z
    //�uRotState�v�̒l�ɉ����āA�q�Ղ悪�ǂ̈ʒu�ɂ��邩�v�Z����uCalcChildPuyoPos�v�𓱓�����
    private static Vector2Int CalcChildPuyoPos(Vector2Int pos,RotState rot)
    {
        //�u_rotate�v�̒l�𐮐��ɕϊ����āA�e�[�u������
        return pos + rotate_tbl[(int)rot];
    }

    //�L�[�������ꂽ�ۂɏ������郁�\�b�h�uTranslate��p�ӂ���v
    private bool CanMove(Vector2Int pos,RotState rot)
    {
        //�uBoardController�v�X�N���v�g�Ɉړ�������̎��Ղ�Ǝq�Ղ�̏�Ԃ��H�ɂȂ��Ă��邩�₢���킹��
        //�ړ��ł��Ă��Ȃ��Ȃ珈�����I��
        if (!boardController.CanSettle(pos)) return false;
        //if (!boardController.CanSettle(pos + Vector2Int.up)) return false; //�C�������̂ŃR�����g��
        //�uCanMove�v���\�b�h�ł̎q�Q�[���I�u�W�F�N�g�̈ʒu�̌��؂̏C��
        if (!boardController.CanSettle(CalcChildPuyoPos(pos, rot))) return false;

        return true;
    }

    //��]�����鏈��
    bool Rotate(bool is_right)
    {
        //���[�J���ϐ��urot�v�𓱓�
        //�urot�v�́u_rotate�v�̒l���E��]�A���J�X�ɉ�����+1�A-1����
        //��-1�����Ƃ��ɕ��ɂȂ�͔̂��������̂łS�̏�]��������ۂɓ����l�ɂȂ�悤��-1+4��+3��-1�̑���Ɏ��
        //�urot�v��i�߂���A0-3�͈̔͂̒l�����悤��4�̏�]�̌v�Z���s��
        //���R���s���[�^�ɍ����ɏ������邱�Ƃ𖾎��I�ɕ\���悤�Ɂu%4�v�ł͂Ȃ��u&3�v�Łi�u&(4-1)�v�Łj�v�Z����
        RotState rot = (RotState)(((int)_rotate + (is_right ? +1 : +3)) & 3);

        //���z�I�Ɉړ��ł��邩���؂���i�㉺���E�ɂ��炵���Ƃ����m�F�j
        Vector2Int pos = _position;

        switch(rot)
        {
            case RotState.Down:
                //�E�i���j���牺�ɉ�]�F�����̉����E�i���j�Ƀu���b�N������Έ���������
                if(!boardController.CanSettle(pos+Vector2Int.down)||!boardController.CanSettle(pos+new Vector2Int(is_right?1:-1,-1)))
                {
                    pos += Vector2Int.up;
                }
                break;
            case RotState.Right:
                //��i���j����E�ɉ�]�F�E�����܃b�Ă���΁A���Ɉړ�
                if (!boardController.CanSettle(pos + Vector2Int.right)) pos += Vector2Int.left;
                break;
            case RotState.Left:
                //��i���j���獶�ɉ�]�F�������܂��Ă���΁A�E�Ɉړ�
                if (!boardController.CanSettle(pos + Vector2Int.left)) pos += Vector2Int.right;
                break;
            case RotState.Up:
                break;
            default:
                Debug.Assert(false);
                break;
        }

        //���z�I�ɓ������Ă݂āA�����ɍs���Ȃ��悤�ł���Ώ������I����
        if (!CanMove(pos, rot)) return false;

        //���ۂɈړ�
        //������悤�ł���΁B�����o�[�ϐ����X�V
        _rotate = rot;
        _position = pos;

        //�\�������Q�[���I�u�W�F�N�g�̈ʒu���X�V
        _puyoControllers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        Vector2Int posChild = CalcChildPuyoPos(_position, _rotate);
        _puyoControllers[1].SetPos(new Vector3((float)posChild.x, (float)posChild.y, 0.0f));

        return true;
    }

    void QuickDrop()
    {
        //��������ԉ��܂ŗ�����
        //��i�����Ƃ��邩�m�F���āA���Ƃ��Ȃ��Ȃ钼�O�̏ꏊ���擾����
        Vector2Int pos = _position;
        do
        {
            pos += Vector2Int.down;
        } while (CanMove(pos, _rotate));
        pos -= Vector2Int.down; //���̏ꏊ�i�Ō�ɒu�����ꏊ�ɖ߂��j

        _position = pos;

        //���ڐڒn
        //�ڒn����ꏊ������������A�{�[�h�̏�����M����
        bool is_set0= boardController.Settle(_position, (int)_puyoControllers[0].GetPuyoType());
        Debug.Assert(is_set0); //�{���ɒu�������ǂ����̃A�T�[�V�������g���Ă����킩��悤�ɂ���

        bool is_set1 = boardController.Settle(CalcChildPuyoPos(_position, _rotate), (int)_puyoControllers[1].GetPuyoType());
        Debug.Assert(is_set1); //�{���ɒu�������ǂ����̃A�T�[�V�������g���Ă����킩��悤�ɂ���

        //�{�[�h�ɂՂ��u������i�v���C���[�̕��ŕ\�����Ă���Ղ���������߁j�����̃Q�[���I�u�W�F�N�g���A�N�e�B�u�ɂ���
        gameObject.SetActive(false);
    }

    private bool Translate(bool is_right)
    {
        //���z�I�Ɉړ��ł��邩���؂���
        //��������̏ꏊ���v�Z
        //�E�ړ��Ȃ炘�������{�P�A�����łȂ���΁i���ړ��Ȃ�j���������[�P����
        Vector2Int pos = _position + (is_right ? Vector2Int.right : Vector2Int.left);
        //�ړ��ł��邩���؂���
        //if (!CanMove(pos)) return false; //�C�������̂ŃR�����g��
        if (!CanMove(pos, _rotate)) return false;

        //���ۂɈړ�
        //�u_position�v�v���p�e�B���X�V
        _position = pos;

        //�Ղ�̃Q�[���I�u�W�F�N�g�̈ʒu���X�V
        _puyoControllers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        //�uTranslate�v���\�b�h�ł̎q�Q�[���I�u�W�F�N�g�̈ʒu�̌��؂̏C��
        Vector2Int posChiild = CalcChildPuyoPos(_position, _rotate);
        _puyoControllers[1].SetPos(new Vector3((float)_position.x, (float)_position.y + 1.0f, 0.0f));

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        //���s�ړ��̃L�[���͎擾
        //�uUpdate�v���ŁuInput.GetKeyDown�v�Łu���v�Ɓu���v�������ꂽ�̂𖈃t���[�����ׂ�
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Translate(true);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Translate(false);
        }

        //��]�̃L�[���͎擾
        //��]�L�[�������ꂽ��uRotate�v���\�b�h�ŏ������邱�Ƃɂ���
        //�ux�v�ŉE��]����t��
        if(Input.GetKeyDown(KeyCode.X))//
        {
            Rotate(true);
        }
        //�uz�v�ō��J�X����t��
        if(Input.GetKeyDown(KeyCode.Z))//
        {
            Rotate(false);
        }

        //�N�C�b�N�h���b�v�̃L�[���͎擾
        //�u���v�������ꂽ�ۂɏ���
        //�uQuickDrop�v���Ăяo��
        if(Input.GetKey(KeyCode.UpArrow))
        {
            QuickDrop();
        }
    }
}

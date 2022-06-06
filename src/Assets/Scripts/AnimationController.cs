using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//06/06　https://github.com/puzzle-study/06_animation
//次はPlayerControllerの修正から
public class AnimationController : MonoBehaviour
{
    const float DELTA_TIME_MAX = 1.0f;
    float _inv_time_max = 1.0f;
    float _time = 0.0f;
    
    public void Set(float max_time)
    {
        Debug.Assert(0.0f < max_time); //負の遍移動時間は不正

        _time = max_time;
    }

    //アニメーション中ならtrueを返す
    public bool Update(float delta_time)
    {
        _time -= delta_time;

        //０になったら終了
        if(_time<=0.0f)
        {
            _time = 0.0f; //負の数にしない
            return false;
        }

        return true;
    }

    public float GetNomalized()
    {
        return _time * _inv_time_max;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int maxHP_ = 3;
    public int minHP_ = 0;
    public int nowHP_ = 3;

    // Start is called before the first frame update
    void Start()
    {
        nowHP_ = maxHP_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Dead()
    {

    }

    public int GetNowHp()
    {
        return nowHP_;
    }
}

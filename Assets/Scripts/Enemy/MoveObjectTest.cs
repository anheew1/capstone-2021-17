using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectTest : MonoBehaviour
{
    
    //check enemy has Path
    [SerializeField]
    private EnemyChase checkAnim;
    Animator ani;
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // ��ΰ� ������ �ִϸ��̼� ����.
    void Update()
    {       
        if (checkAnim.hasP)
        {
            ani.SetBool("Walk", true);
        }
        else
        {
            ani.SetBool("Walk", false);
        }       
    }

    public void PlayAttAnim()
    {
        ani.SetTrigger("Attack");
    }
}

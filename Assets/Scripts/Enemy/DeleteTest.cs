using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾�� �浹 ���� �� �÷��̾ ���ְ� �ٸ� Ÿ���� ã�� ���� bool�� ���¸� �Ѱ��ش�
public class DeleteTest : MonoBehaviour
{
    [SerializeField]
    private EnemyControl checkCatch;

    public void OnTriggerEnter(Collider other)   
    {
        //�±װ� �÷��̾��
        if(other.CompareTag("Enemy"))
        {            
            Destroy(gameObject);
        }
    }
}

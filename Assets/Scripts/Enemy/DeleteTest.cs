using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾�� �浹 ���� �� �÷��̾ ���ְ� �ٸ� Ÿ���� ã�� ���� bool�� ���¸� �Ѱ��ش�
public class DeleteTest : MonoBehaviour
{
    
    public EnemyChase checkCatch;

    public void OnTriggerEnter(Collider other)   
    {
        //�±װ� �÷��̾��
        if(other.CompareTag("Player"))
        {
            //��Ҵٴ� �� �Ѱ��ش�.
            checkCatch.isCatched = true;
            Destroy(gameObject);
        }
    }
}

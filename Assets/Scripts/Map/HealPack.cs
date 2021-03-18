using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPack : MonoBehaviour
{
    GameObject testPlayer;
    GameObject healPack;
    HealTest playerHealth;

    public void UseHealPack()
    {
        playerHealth = testPlayer.GetComponent<HealTest>();
        HealPlayer();
    }

    void HealPlayer()
    {
        if (playerHealth.playerHP < playerHealth.maxHP)
        {
            playerHealth.Heal();
            DestroyItem();
            Debug.Log("������ ���");
        }
        else
        {
            Debug.Log("������ ��� �Ұ�");
        }
    }

    void DestroyItem()
    {
        healPack.SetActive(false);
    }

    public void GetHealObject(GameObject gameObj)
    {
        healPack = gameObj;
    }

    public void GetPlayerObject(GameObject gameObj)
    {
        testPlayer = gameObj;
    }
}

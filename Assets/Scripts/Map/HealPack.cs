using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPack : Item
{
    private PlayerHealth playerHealth;

    public override void Use()
    {
        playerHealth = OwnedPlayer.PlayerHealth;
        HealPlayer();
    }

    void HealPlayer()
    {
        if (playerHealth.Health < PlayerHealth.MAXHP)
        {
            playerHealth.Heal();
            Debug.Log("������ ���");
        }
        else
        {
            Debug.Log("������ ��� �Ұ� - ü���� ������ ����");
        }
    }
}

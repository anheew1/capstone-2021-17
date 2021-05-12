using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPack : Item
{
    private PlayerHealth playerHealth;

    public override bool CanUse()
    {
        if (playerHealth.health >= PlayerHealth.MAXHP)
        {
            Debug.Log("������ ��� �Ұ� - ü���� ������ ����");
            return false;
        }
        return true;
    }

    public override void Use()
    {
        playerHealth.Heal();
    }

    public override void OnPlayerOwnItem()
    {
        playerHealth = OwnedPlayer.GetComponent<PlayerHealth>();
    }


}

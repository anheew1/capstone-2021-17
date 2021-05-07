using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEnemy : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;   //����� �ҽ�
    [SerializeField] private AudioClip[] clips;         //������ Ŭ����
    [SerializeField] private BoxCollider[] colliders;   //�ȿ� �޸� �ݶ��̴���
    private int collidersLength;
    void Start()
    {
        collidersLength = colliders.Length;
        SetCollider();  //�ݶ��̴� ����
    }

    //�ȴ� �Ҹ� ���
    public void WalkSound()
    {
        audioSource.PlayOneShot(clips[0], 1f);
    }

    //���� �ÿ� �Ҹ� ���
    public void AttackSound()
    {
        audioSource.PlayOneShot(clips[1], 1f);
    }

    //���� ����, ���� �ݶ��̴� �Ѱ� ����
    public void SetCollider()
    {
        for(int i=0; i<collidersLength; i++)
        {
            colliders[i].enabled = colliders[i].enabled ? false : true;
        }
    }

    //���������ϴ� ���� ���
    public void DizzySound()
    {
        audioSource.PlayOneShot(clips[2], 1f);
    }
}

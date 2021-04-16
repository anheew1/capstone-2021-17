using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvent : MonoBehaviour
{
    [SerializeField]
    AudioSource walkAudio;
    [SerializeField]
    EnemyControl enemy;

    private Transform enemyPos;
    //���� �ȿ� �ִ��� Ȯ���ϴ� ����
    public bool isInArea = false;

    public void Awake()
    {
        enemyPos = enemy.transform;
    }
    public void SoundWhenAnim()
    {
        if (Vector3.Distance(transform.position, enemyPos.position) <= 5f)
        {
            enemy.findTargetSound = true;
            isInArea = true;
        }
        else
        {
            isInArea = false;
        }        
        walkAudio.Play();
    }

    public void WalkSound()
    {
        walkAudio.Play();
    }
}

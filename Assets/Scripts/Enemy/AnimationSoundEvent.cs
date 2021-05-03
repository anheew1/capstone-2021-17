using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvent : MonoBehaviour
{
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip[] playerAudioClip;
    [SerializeField] private Enemy enemy;

    private Transform enemyPos;
    //���� �ȿ� �ִ��� Ȯ���ϴ� ����
    private bool isInArea = false;

    public void Awake()
    {
        enemyPos = enemy.transform;
    }
    public void SoundWhenAnim()
    {
        ActiveSoundSensor();
        playerAudioSource.Play();
    }

    public void WalkSound()
    {
        playerAudioSource.Play();
    }

    public bool CheckInArea()
    {
        return isInArea;
    }

    public void ActiveSoundSensor()
    {
        if (Vector3.Distance(transform.position, enemyPos.position) <= 5f)
        {
            enemy.SoundSensorDetect();
            isInArea = true;
        }
        else
        {
            isInArea = false;
        }
    }
}

﻿using UnityEditor;
using UnityEngine;
using Mirror;

public class GunControlNetBehaviour : NetworkBehaviour
{
    [SerializeField]
    GunControl gunControl;

    public GameObject BulletPrefab;

    public void Start()
    {
        if (isLocalPlayer)
        {
            NetworkClient.UnregisterPrefab(BulletPrefab);
            NetworkClient.RegisterPrefab(BulletPrefab, SpawnBulletHandler, UnSpawnBulletHandler);
        }


    }

    public void SpawnBullet()
    {
        CmdSpawnBullet();
    }
    [Command]
    public void CmdSpawnBullet()
    {
        GameObject Bullet = Instantiate(BulletPrefab, gunControl.BulletPos.position, gunControl.BulletPos.rotation);
        NetworkServer.Spawn(Bullet);
        Rigidbody BulletRigid = Bullet.GetComponent<Rigidbody>(); //Rigidbody creation
        Vector3 forward = gunControl.BulletPos.forward;
        BulletRigid.velocity = new Vector3(forward.x,0,forward.z) * 15;

    }
    
    GameObject SpawnBulletHandler(SpawnMessage msg)
    {
        GameObject Bullet = Instantiate(BulletPrefab, msg.position, msg.rotation);
        Rigidbody BulletRigid = Bullet.GetComponent<Rigidbody>(); //Rigidbody creation
        Vector3 forward = gunControl.BulletPos.forward;
        BulletRigid.velocity = new Vector3(forward.x, 0, forward.z) * 15;
        return Bullet;
    }

    void UnSpawnBulletHandler(GameObject spawned)
    {
        Destroy(spawned);
    }
}
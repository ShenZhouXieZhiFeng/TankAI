using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹池
/// </summary>
public class BulletPool : SingletonMono<BulletPool> {

    [Header("子弹预制")]
    public GameObject BulletPrefab;

    private Stack<Bullet> poolStack = new Stack<Bullet>();

    public Bullet GetBullet(Transform _initPos, Vector3 _shootDir) {
        Bullet returnBullet = null;
        if (poolStack.Count == 0)
        {
            returnBullet = Instantiate(BulletPrefab).GetComponent<Bullet>();
            returnBullet.transform.parent = transform;
        }
        else {
            returnBullet = poolStack.Pop();
            returnBullet.gameObject.SetActive(true);
        }
        returnBullet.transform.position = _initPos.position;
        returnBullet.transform.rotation = _initPos.rotation;

        returnBullet.Shoot(_shootDir);
        return returnBullet;
    }

    public void PushBullet(Bullet _bullet) {
        poolStack.Push(_bullet);
        _bullet.gameObject.SetActive(false);
    }
	
}

using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("对象池设置")]
    public GameObject bulletPrefab;
    public int initBulletCount = 15;

    private Queue<GameObject> bulletQueue = new Queue<GameObject>();

    void Awake()
    {
        // 单例模式
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 预先创建子弹存入池子
        for (int i = 0; i < initBulletCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            bulletQueue.Enqueue(bullet);
        }
    }

    // 取出子弹用于发射
    public GameObject GetBullet()
    {
        GameObject bullet;
        if (bulletQueue.Count > 0)
        {
            bullet = bulletQueue.Dequeue();
        }
        else
        {
            // 池子空了临时新建
            bullet = Instantiate(bulletPrefab, transform);
        }
        bullet.SetActive(true);
        return bullet;
    }

    // 回收子弹回池子
    public void RecycleBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}

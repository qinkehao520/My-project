using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float bulletSpeed = 6f;
    public Vector2 flyDir;

    void OnEnable()
    {
        // 激活子弹，开启2秒自动回收计时
        Invoke(nameof(RecycleSelf), 2f);
    }

    void Update()
    {
        // 按照传入方向移动
        transform.Translate(flyDir * bulletSpeed * Time.deltaTime, Space.World);
    }

    // 回收子弹回对象池
    void RecycleSelf()
    {
        BulletPool.Instance.RecycleBullet(gameObject);
    }

    // 子弹禁用时清空计时器，防止重复调用
    void OnDisable()
    {
        CancelInvoke(nameof(RecycleSelf));
    }

    // 碰撞任意物体立刻回收
    private void OnTriggerEnter2D(Collider2D other)
    {
        BulletPool.Instance.RecycleBullet(gameObject);
    }
}

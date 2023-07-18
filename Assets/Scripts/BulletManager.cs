using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public Bullet bulletPrefab;
    [SerializeField] private Queue<Bullet> bulletQueue = new Queue<Bullet>();

    public static BulletManager Instance { get; private set; }

    int number = 0;

    private void Awake()
    {
        Instance = this;
    }


    public Bullet Get()
    {
        if (bulletQueue.Count == 0)
        {
            AddBulletToQueue(1);
        }

        return bulletQueue.Dequeue();
    }

    void AddBulletToQueue(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Bullet unitInstantce = Instantiate(bulletPrefab, transform);
            unitInstantce.gameObject.SetActive(false);
            unitInstantce.id = number++;
            bulletQueue.Enqueue(unitInstantce);
        }
    }

    public void ReurnBulletToPool(Bullet bullet)
    {
        if (bullet.bulletLifeCorutine != null)
            StopCoroutine(bullet.bulletLifeCorutine);
        bullet.gameObject.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}

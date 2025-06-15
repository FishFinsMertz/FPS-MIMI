using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu(menuName = "FPSMimi/BulletSettings")]
public class BulletSettings : ScriptableObject
{
    public BulletType type;
    public GameObject prefab;
    public bool isAffectedByGravity;

    public Bullet Create() 
    {
        GameObject gameObject = Instantiate(prefab);
        Bullet bullet = gameObject.AddComponent<Bullet>();
        bullet.SetBulletSettings(this);
        bullet.GetComponent<NetworkObject>().Spawn();
        return bullet;
    }

    public void OnGet(Bullet bullet) 
    {
        bullet.gameObject.SetActive(true);
    }
    public void OnRelease(Bullet bullet) 
    {
        bullet.gameObject.SetActive(false);
    }
    public void OnDestroyPoolObject(Bullet bullet) => Destroy(bullet.gameObject);
}
public enum BulletType { Default }

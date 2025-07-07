using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu(menuName = "FPSMimi/BulletSettings")]
public class BulletSettings : ScriptableObject
{
    public BulletType type;
    public GameObject prefab;
    public bool isAffectedByGravity;
    public float damage = 50f; // Change later

    public Bullet Create()
    {
        GameObject gameObject = Instantiate(prefab);
        Bullet bullet = gameObject.AddComponent<Bullet>();
        bullet.SetBulletSettings(this);
        return bullet;
    }

    public void OnGet(Bullet bullet) 
    {
        bullet.gameObject.SetActive(true);
    }
    public void OnRelease(Bullet bullet)
    {
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        if (netObj.IsSpawned) netObj.Despawn();
        bullet.gameObject.SetActive(false);
    }
    public void OnDestroyPoolObject(Bullet bullet) => Destroy(bullet.gameObject);
}
public enum BulletType { Default }

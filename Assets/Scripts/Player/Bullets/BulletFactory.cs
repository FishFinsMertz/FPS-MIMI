using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class BulletFactory : NetworkBehaviour
{
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxPoolSize = 100;
    static BulletFactory instance;
    readonly Dictionary<BulletType, IObjectPool<Bullet>> pools = new();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public static Bullet Spawn(BulletSettings bulletSettings) => instance.GetPoolFor(bulletSettings).Get();
    public static void ReturnToPool(Bullet bullet) 
    {
        if (bullet.gameObject.activeInHierarchy)
        {
            bullet.gameObject.SetActive(false);
            instance.GetPoolFor(bullet.GetBulletSettings())?.Release(bullet);
        }
    }

    IObjectPool<Bullet> GetPoolFor(BulletSettings bullet) 
    {
        IObjectPool<Bullet> pool;
        if (pools.TryGetValue(bullet.type, out pool)) return pool;

        pool = new ObjectPool<Bullet>(
            bullet.Create,
            bullet.OnGet,
            bullet.OnRelease,
            bullet.OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
        );
        pools.Add(bullet.type, pool);

        NetworkManager.Singleton.PrefabHandler.AddHandler(bullet.prefab, new PooledBulletPrefabInstanceHandler(bullet));
        return pool;
    }
}

public class PooledBulletPrefabInstanceHandler : INetworkPrefabInstanceHandler 
{
    BulletSettings settings;

    public PooledBulletPrefabInstanceHandler(BulletSettings settings) 
    {
        this.settings = settings;
    }

    NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) 
    {
        return BulletFactory.Spawn(settings).gameObject.GetComponent<NetworkObject>();
    }
    void INetworkPrefabInstanceHandler.Destroy(NetworkObject bullet) 
    {
        BulletFactory.ReturnToPool(bullet.GetComponent<Bullet>());
    }
}

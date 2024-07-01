namespace TwitchParachuteSpawns.Mono;

using System;
using System.Collections;
using UnityEngine;

internal class ZChute : MonoBehaviour
{
    private Entity entity;
    private Rigidbody rb;
    private GameObject parachuteObject;

    private static GameObject chutePrefab;
    public static GameObject ChutePrefab
    {
        get
        {

            if (chutePrefab == null)
            {
                chutePrefab = Resources.Load<GameObject>("prefabs/prefabEntitySupplyCrate");
            }

            return chutePrefab;
        }
    }

    public void Awake()
    {
        entity = gameObject.GetComponent<Entity>();
        if (entity == null)
        {
            Log.Error("[TwitchParachuteSpawns] Failed to get Entity while adding ZChute");
            DestroyImmediate(this);
            return;
        }

        rb = entity.gameObject.GetComponentInChildren<Rigidbody>(true);

        if (rb == null)
        {
            Log.Error("[TwitchParachuteSpawns] Failed to get Rigidbody while adding ZChute");
            DestroyImmediate(this);
        }
    }

    public void Start()
    {
        parachuteObject = GameObject.Instantiate(ChutePrefab, entity.gameObject.transform.position, entity.gameObject.transform.rotation);
        GameObject.DestroyImmediate(parachuteObject.GetComponentInChildren<BoxCollider>());
        parachuteObject.SetActive(true);
        parachuteObject.transform.SetParent(entity.gameObject.transform);
    }

    public void Update()
    {
        rb.velocity = new Vector3(0, -0.0125f, 0);
        entity.fallDistance = 0f;
        if (entity.onGround)
        {
            Destroy(parachuteObject);
        }
    }
}

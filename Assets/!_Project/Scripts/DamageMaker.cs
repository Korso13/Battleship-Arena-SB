using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMaker : MonoBehaviour
{
    [SerializeField] private int _damage = 10;
    public int Damage { get => _damage; set => _damage = value; }

    public void SafeDestroy()
    {
        StartCoroutine(ScheduleDestruction(0f));
    }

    private IEnumerator ScheduleDestruction(float seconds)
    {
        //Debug.Log($"[DamageMaker][{gameObject.name}] ScheduleDestruction");
        if (seconds > 0)
            yield return new WaitForSeconds(seconds);
        // Hack to prompt TriggerExits in EnemySpawners
        gameObject.transform.position += new Vector3(-100, -100, -100);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (gameObject.TryGetComponent<Rigidbody>(out var rb))
            rb.detectCollisions = false;
        yield return new WaitForEndOfFrame();
        //Debug.Log($"[DamageMaker][{gameObject.name}] Destroying...");
        Destroy(gameObject);
    }
}

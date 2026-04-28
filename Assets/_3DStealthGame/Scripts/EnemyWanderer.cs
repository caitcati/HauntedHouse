using System.Collections;
using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 10f;
    public float waitTime = 2f;

    private Vector3 targetPosition;
    private bool isWaiting = false;

    void Start()
    {
        SetNewTarget();
    }

    void Update()
    {
        if (isWaiting) return;

        MoveToTarget();
    }

    void MoveToTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            StartCoroutine(WaitAndPickNewPoint());
        }
    }

    IEnumerator WaitAndPickNewPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        SetNewTarget();
        isWaiting = false;
    }

    void SetNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );
    }
}
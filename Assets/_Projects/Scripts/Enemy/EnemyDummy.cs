using UnityEngine;

public class EnemyDummy : Enemy
{
    [SerializeField] float moveSpeed;

    void Update()
    {
        if (moveSpeed != 0)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }
}

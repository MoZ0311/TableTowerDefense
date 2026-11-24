using UnityEngine;

public class EnemyDummy : Enemy
{
    [SerializeField] float rotateSpeed;
    protected override void UpdateAttack()
    {
        base.UpdateAttack();
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}

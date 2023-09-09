using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{

    public float damage = 20f; // ���ݷ�
    public float timeBetAttack = 0.5f; // ���� ����
    private float lastAttackTime; // ������ ���� ����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        Debug.Log("���� ��Ҵ�.");

        // �ֱ� ���� �������� timeBetAttack �̻� �ð��� �����ٸ� ���� ����
        if (Time.time >= lastAttackTime + timeBetAttack)
        {
            // �������κ��� LivingEntity Ÿ���� �������� �õ�
            LivingEntity attackTarget
                = other.GetComponent<LivingEntity>();

            // ������ LivingEntity�� �ڽ��� ���� ����̶�� ���� ����
            if (attackTarget != null)
            {
                Debug.Log(attackTarget.name + "���� ��Ҵ� ���� ����");

                // �ֱ� ���� �ð��� ����
                lastAttackTime = Time.time;

                // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                // ���� ����
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}

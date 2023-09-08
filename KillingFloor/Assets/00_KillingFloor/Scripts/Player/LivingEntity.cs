using System;
using UnityEngine;

// ����ü�μ� ������ ���� ������Ʈ���� ���� ���븦 ����
// ü��, ������ �޾Ƶ��̱�, ��� ���, ��� �̺�Ʈ�� ����
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f; // ���� ü��
    public float health { get; protected set; } // ���� ü��
    public bool dead { get; protected set; } // ��� ����
    public event Action onDeath; // ����� �ߵ��� �̺�Ʈ


    //// ȣ��Ʈ->��� Ŭ���̾�Ʈ �������� ü�°� ��� ���¸� ����ȭ �ϴ� �޼���
    //[PunRPC]
    //public void ApplyUpdatedHealth(float newHealth, bool newDead)
    //{
    //    health = newHealth;
    //    dead = newDead;
    //}

    // ����ü�� Ȱ��ȭ�ɶ� ���¸� ����
    protected virtual void OnEnable()
    {
        // ������� ���� ���·� ����
        dead = false;
        // ü���� ���� ü������ �ʱ�ȭ
        health = startingHealth;
    }

    // ������ ó��
    // ȣ��Ʈ���� ���� �ܵ� ����ǰ�, ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ�鿡�� �ϰ� �����
    //[PunRPC]
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    // ��������ŭ ü�� ����
        //    health -= damage;

        //    // ȣ��Ʈ���� Ŭ���̾�Ʈ�� ����ȭ
        //    photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead);

        //    // �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� �����ϵ��� ��
        //    photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        //}


        health -= damage;
        // ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
        //if (health <= 0 && !dead)
        if (health <= 0)
        {
            health = 0;
            //Die();
        }
    }


    // ü���� ȸ���ϴ� ���
    //[PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            // �̹� ����� ��� ü���� ȸ���� �� ����
            return;
        }
        health += newHealth;
        // ȣ��Ʈ�� ü���� ���� ���� ����
        //if (PhotonNetwork.IsMasterClient)
        //{
        // ü�� �߰�
        //health += newHealth;
        //// �������� Ŭ���̾�Ʈ�� ����ȭ
        //photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead);

        //// �ٸ� Ŭ���̾�Ʈ�鵵 RestoreHealth�� �����ϵ��� ��
        //photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        //}
    }

    public virtual void Die()
    {
        // onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� ����
        if (onDeath != null)
        {
            onDeath();
        }

        // ��� ���¸� ������ ����
        dead = true;
    }
}
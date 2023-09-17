using Photon.Pun;
using System;
using UnityEngine;

// ����ü�μ� ������ ���� ������Ʈ���� ���� ���븦 ����
// ü��, ������ �޾Ƶ��̱�, ��� ���, ��� �̺�Ʈ�� ����
public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100f; // ���� ü��
    public float health { get; protected set; } // ���� ü��
    public float armor { get; protected set; }
    public int coin { get; protected set; }
    public bool dead { get; protected set; } // ��� ����
    public event Action OnDeath; // ����� �ߵ��� �̺�Ʈ


    //// ȣ��Ʈ->��� Ŭ���̾�Ʈ �������� ü�°� ��� ���¸� ����ȭ �ϴ� �޼���
    [PunRPC]
    public void ApplyUpdatedHealth(float newHealth,float newArmor, bool newDead)
    {
        health = newHealth;
        armor = newArmor;
        dead = newDead;
    }

    // ����ü�� Ȱ��ȭ�ɶ� ���¸� ����
    protected virtual void OnEnable()
    {
        // ������� ���� ���·� ����
        dead = false;
        // ü���� ���� ü������ �ʱ�ȭ
        health = startingHealth;
        armor = 0;
    }

    // ������ ó��
    // ȣ��Ʈ���� ���� �ܵ� ����ǰ�, ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ�鿡�� �ϰ� �����
    [PunRPC]
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ��������ŭ ü�� ����
            Damage(damage);

            // ȣ��Ʈ���� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, armor, dead);

            // �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� �����ϵ��� ��
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }

        // ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
        if (health <= 0 && !dead)
            if (health <= 0)
        {
            health = 0;
                Die();
            }
    }
    private void Damage(float _damage)
    {
        // �ƸӰ� 75�̻��̸� 75% ������ ���
        if (armor >= 75)
        {
            int armorDamage = Mathf.RoundToInt(_damage * 0.75f);
            Debug.Log(armorDamage);

            armor -= Mathf.RoundToInt(_damage * 0.75f);
            health -= Mathf.RoundToInt(_damage * 0.25f);
            if (0 >= armor) armor = 0;
        }
        else if (75 >= armor && armor > 50)
        {
            armor -= Mathf.RoundToInt(_damage * 0.65f);
            health -= Mathf.RoundToInt(_damage * 0.35f);
            if (0 >= armor) armor = 0;
        }
        else if (50 >= armor && armor > 0)
        {
            armor -= Mathf.RoundToInt(_damage * 0.55f);
            health -= Mathf.RoundToInt(_damage * 0.45f);
            if (0 >= armor) armor = 0;
        }
        else
        {
            health -= _damage;
        }

    }

    // ü���� ȸ���ϴ� ���
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            // �̹� ����� ��� ü���� ȸ���� �� ����
            return;
        }

        //ȣ��Ʈ�� ü���� ���� ���� ����
        if (PhotonNetwork.IsMasterClient)
        {
            //ü�� �߰�
            health += newHealth;

            // �������� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, armor, dead);

            // �ٸ� Ŭ���̾�Ʈ�鵵 RestoreHealth�� �����ϵ��� ��
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
    }

    // �ƸӸ� �����ϴ� ���
    [PunRPC]
    public virtual void RestoreArmor(float newArmor)
    {
        if(dead)
        {
            // �̹� ����� ��� �Ƹ� ȸ�� �Ұ��� : ������ �׾ ���ŵ� ��������...
            return;
        }

        //ȣ��Ʈ�� �ǵ带 ���� ���� ����
        if (PhotonNetwork.IsMasterClient)
        {
            armor += newArmor;
            // �������� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, armor, dead);

            // �ٸ� Ŭ���̾�Ʈ�鵵 RestoreHealth�� �����ϵ��� ��
            photonView.RPC("RestoreArmor", RpcTarget.Others, newArmor);

        }

    }
    [PunRPC]

    public virtual void GetCoin(int newCoin)
    {
        if(dead)
        {
            return;
        }
        coin += newCoin;
    }
    public virtual void SpendCoin(int newCoin)
    {
        if(dead)
        {
            return;
        }
        coin -= newCoin;
    }


    public virtual void Die()
    {
        // onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� ����
        if (OnDeath != null)
        {
            OnDeath();
        }

        // ��� ���¸� ������ ����
        dead = true;
    }
}
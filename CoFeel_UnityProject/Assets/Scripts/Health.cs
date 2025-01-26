using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class Health : NetworkBehaviour
{
    public NetworkVariable<int> hp = new NetworkVariable<int>(100);

    //public int hp = 100;
    public GameFlowManager gameFlowManager;
    public Image healthBar;
    void Start()
    {
        //gameFlowManager = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();
    }

    void OnTriggerEnter(Collider collision){
        if (collision.transform.tag == "attack"){
            Attack attack = collision.transform.GetComponent<Attack>();
            if (attack == null)
            {
                return;
            }

            if (!IsServer)
            {
                TakeDamageServerRpc(attack.damage);
            }
            else
            {
                TakeDamageClientRpc(attack.damage);
            }
            // TakeDamage(attack.damage);
            StartCoroutine(ContinuousDamage(1f, attack.continuousDamage));
        }
    }
    void OnCollisionEnter(Collision collision){
        if (collision.transform.tag == "attack"){
            Attack attack = collision.transform.GetComponent<Attack>();
            if (attack == null)
            {
                return;
            }
            if (!IsServer)
            {
                TakeDamageServerRpc(attack.damage);
            }
            else
            {
                TakeDamageClientRpc(attack.damage);
            }
            // TakeDamage(attack.damage);
            StartCoroutine(ContinuousDamage(1f, attack.continuousDamage));
        }
    }
    IEnumerator ContinuousDamage(float delayTime, int continuousDamage)
    {
        int i = 3;
        while (i > 0)
        {
            yield return new WaitForSeconds(delayTime);
            if (!IsServer)
            {
                TakeDamageServerRpc(continuousDamage);
            }
            else
            {
                TakeDamageClientRpc(continuousDamage);
            }
            // TakeDamage(continuousDamage);
            i -= 1;
        }
    }
    // void TakeDamage(int damage){
    //     hp -= damage;
    //     UpdateHealth(hp);
    // }
    [ServerRpc(RequireOwnership = false)]
    void TakeDamageServerRpc(int damage)
    {
        hp.Value -= damage;
        // UpdateHealth(hp.Value);
    }
    [ClientRpc(RequireOwnership = false)]
    void TakeDamageClientRpc(int damage)
    {
        hp.Value -= damage;
        // UpdateHealth(hp.Value);
    }
    
    [ServerRpc(RequireOwnership = false)]
    void UpdateHealthServerRpc(int health){
        healthBar.fillAmount = (float) health / (float)100;
    }
    [ClientRpc(RequireOwnership = false)]
    void UpdateHealthClientRpc(int health){
        healthBar.fillAmount = (float) health / (float)100;
    }
    void UpdateHealth(int health)
    {
        healthBar.fillAmount = (float)health / (float)100;
    }
    // void UpdateHealth(int health){
    //     healthBar.fillAmount = (float) health / (float)100;
    // }
    void Update(){
        if (hp.Value >= 0) {
            healthBar.fillAmount = (float)hp.Value / (float)100;
            //gameFlowManager.gameOver();
        }
    }
    
}

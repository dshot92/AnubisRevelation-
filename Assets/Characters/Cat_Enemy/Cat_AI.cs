﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cat_AI : MonoBehaviour
{
    [Range(0f, 500f)]
    public float awareness_radius = 10f;
    public float meele_radius = 2f;
    [Range(0f, 500f)]
    public float walk_radius = 100f;
    public float singleStep = 1f;
    public float speed_multiplier = 2f;
    float original_speed;
    public int meele_power = 2;
    public TextMeshProUGUI life_text;
    Animator anim;
    public int life = 2;
    GameObject player;
    UnityEngine.AI.NavMeshAgent agent;
    PlayerController play_contr;
    public AudioSource footsteps;
    public AudioSource meow_sound;
    float sound_cooldown = 1f;
    public float attack_cooldown = 1f;
    float elapsed_time = 0f;

    void Start()
    {

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        original_speed = agent.speed;
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        play_contr = player.GetComponentInChildren<PlayerController>();
        //Overlap step sound for 1/3 of the duration time
        sound_cooldown = footsteps.clip.length;
        anim.SetBool("isWalking", true);
        anim.SetBool("isRunning", false);
    }

    void FixedUpdate()
    {
        //sound timer
        elapsed_time += Time.deltaTime;

        //calculate distance e direction to player.
        float distancePlayer = Vector3.Distance(agent.transform.position, player.transform.position);

        if (distancePlayer < meele_radius * 2) life_text.gameObject.SetActive(true);


        if (distancePlayer > awareness_radius)
        {
            //Random walk
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);

            agent.speed = original_speed;
            agent.speed /= speed_multiplier;

            life_text.gameObject.SetActive(false);

            /// https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html

            // If 1/5 of destination left rework another random one
            ///TODO
            // life value could act as a swiftness multiplier, creating a more chaotically pattern based on remaining lifes points
            if (agent.remainingDistance < walk_radius / 5)
            {
                Vector3 randomDirection = Random.insideUnitSphere * walk_radius;
                randomDirection += transform.position;
                UnityEngine.AI.NavMeshHit hit;
                Vector3 finalPosition = Vector3.zero;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walk_radius, 1))
                {
                    finalPosition = hit.position;
                }
                agent.SetDestination(finalPosition);
            }
        }
        else if (distancePlayer < awareness_radius && distancePlayer > meele_radius)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", true);

            agent.speed = original_speed;
            agent.speed *= speed_multiplier;

            life_text.gameObject.SetActive(false);
            //walk torwards player
            agent.SetDestination(player.transform.position);
        }
        else if (distancePlayer < meele_radius )
        {
            agent.SetDestination(player.transform.position);
            if (elapsed_time > attack_cooldown)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, meele_radius))
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        anim.SetBool("isWalking", false);
                        anim.SetBool("isRunning", false);
                        InstantlyTurnAttack();
                        Debug.Log("Player hitted");
                        meow_sound.Play();
                        anim.Play("Cat_Attack");
                        play_contr.life -= meele_power;
                        elapsed_time = 0f;
                    }
                }
            }
        }

        InstantlyTurn(agent.destination);

        //Debug.Log(sound_cooldown.ToString());
        if (elapsed_time > sound_cooldown && distancePlayer > meele_radius)
        {
            elapsed_time = 0f;
            footsteps.volume = (1 / distancePlayer );  // Inverse square law
            footsteps.Play();
        }

        // uPDATE lIFE TEXT
        switch (life)
        {
            case 1:
                life_text.color = Color.red;
                break;
            case 2:
                life_text.color = Color.yellow;
                break;
        }
        life_text.text = (life.ToString());

        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    // https://answers.unity.com/questions/1170087/instantly-turn-with-nav-mesh-agent.html
    private void InstantlyTurn(Vector3 destination)
    {
        //When on target -> dont rotate!
        if ((destination - transform.position).magnitude < 0.1f) return;

        Vector3 direction = (destination - transform.position).normalized;
        Quaternion qDir = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * singleStep);
    }

    private void InstantlyTurnAttack()
    {
        //When on target -> dont rotate!
        if ((player.transform.position - transform.position).magnitude < 0.1f) return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion qDir = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * singleStep);
    }
}

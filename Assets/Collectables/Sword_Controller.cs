﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Controller : MonoBehaviour
{
    public float item_rotating_speed = 2f;
    public AudioSource audio;

    private void Setup()
    {
        audio = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (GameManager.has_sword)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        transform.RotateAround(Vector3.up, item_rotating_speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponentInChildren<PlayerController>();

        if (other.gameObject.CompareTag("Player"))
        {
            //StartCoroutine(PlaySound());
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            other.gameObject.GetComponentInChildren<PlayerController>().meele_power++;
            StartCoroutine(PlaySound());
            player.has_sword = true;
        }
    }

    public IEnumerator PlaySound()
    {
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}
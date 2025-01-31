﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch_Controller : MonoBehaviour
{
    public float item_rotating_speed = 2f;
    public AudioSource audio;

    private void Setup()
    {
        audio = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (GameManager.has_torch)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.transform.position -= new Vector3(0, -50, 0);
        }
        transform.RotateAround(Vector3.up, item_rotating_speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponentInChildren<PlayerController>();

        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(PlaySound());
            // really bad, but this particles system remaining alive is annoying me
            gameObject.transform.position -= new Vector3(0, -50, 0);
            player.has_torch = true;
        }
    }

    public IEnumerator PlaySound()
    {
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}

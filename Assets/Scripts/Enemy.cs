/**
 * BSD 3-Clause License
 *
 * Copyright(c) 2018, John Pennycook
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 *
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *
 * * Neither the name of the copyright holder nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public BoxCollider trigger;
    public GameObject explosionPrefab;

    const int MAX_HEALTH = 3;
    const float MAX_SPEED = 1f;

    protected AudioSource audio;
    protected Rigidbody body;
    protected MeshRenderer renderer;
    protected int health;
    protected bool spawned = false;
    protected GameObject explosion;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
        health = MAX_HEALTH;
        spawned = false;
        transform.localScale = new Vector3(0, 0, 0);
        trigger.enabled = false;
    }

    IEnumerator spawn()
    {
        spawned = true;
        trigger.enabled = true;
        float scale = 0;
        while (scale < 0.5f)
        {
            scale += 0.05f;
            scale = Mathf.Clamp(scale, 0, 0.5f);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    void Update()
    {
        if (GameManager.EnemyReleased() && !spawned)
        {
            StartCoroutine(spawn());
        }

        if (spawned)
        {
            // Move randomly (while alive)
            if (health > 0)
            {
                body.velocity += Random.onUnitSphere * MAX_SPEED;
            }
        }
    }

    public void Die()
    {
        if (health > 0) // TODO: Work out why this is necessary
        {
            health = 0;
            body.velocity = new Vector3(0, 0, 0);
            body.useGravity = true;
            trigger.enabled = false;
            audio.Play();
            explosion = Instantiate(explosionPrefab, this.transform);
            renderer.enabled = false;
        }
    }

}

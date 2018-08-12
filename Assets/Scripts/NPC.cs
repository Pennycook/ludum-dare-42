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

public class NPC : MonoBehaviour
{
    protected CharacterController controller;
    protected Vector3 direction;
    protected int counter = 0;
    protected bool dead;

    public GameObject explosionPrefab;

    protected AudioSource audio;
    protected MeshRenderer renderer;
    protected GameObject explosion;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        renderer = GetComponent<MeshRenderer>();
        dead = false;
    }

    void Update()
    {
        // Run away from the player
        if (GameManager.SmashedGlass())
        {
            // If too close to the player, change direction
            Vector3 fromPlayer = transform.position - GameManager.player.transform.position;
            fromPlayer.y = 0;
            if (fromPlayer.magnitude < 5)
            {
                direction = fromPlayer;
                controller.SimpleMove(fromPlayer);
            }

            // Choose a new direction and a time to run in it
            else if (counter == 0 || controller.velocity.magnitude < 5)
            {
                direction = Random.onUnitSphere;
                counter = Random.Range(60, 300);
            }

            // Otherwise continue in the current direction
            else if (counter > 0)
            {
                counter--;
            }
            direction.y = 0;
            controller.SimpleMove(direction);
        }
    }

    public void Die()
    {
        if (!dead)
        {
            audio.Play();
            explosion = Instantiate(explosionPrefab, this.transform);
            renderer.enabled = false;
            dead = true;
        }
    }

}

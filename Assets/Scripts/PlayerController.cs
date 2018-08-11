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

public class PlayerController : MonoBehaviour
{
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip pewpew;

    private Camera camera;
    private AudioSource audio;
    protected CharacterController controller;

    protected AudioClip[] footsteps;
    private int step;
    private float lastStepTime;

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        audio = GetComponent<AudioSource>();
        controller = GetComponentInChildren<CharacterController>();
        Cursor.visible = false;

        footsteps = new AudioClip[2];
        footsteps[0] = footstep1;
        footsteps[1] = footstep2;
        step = 0;
        lastStepTime = Time.time;
    }

    void Update()
    {
        // Move player on WASD, Arrow Keys, or Joystick
        float strafe = Input.GetAxis("Horizontal");
        float walk = Input.GetAxis("Vertical");
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        Vector3 speed = forward * walk + right * strafe;
        controller.SimpleMove(speed);

        // Change look direction based on mouse location
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");
        transform.localRotation *= Quaternion.Euler(0, horizontal, 0);
        camera.transform.localRotation *= Quaternion.Euler(-vertical, 0, 0);

        // Play a footstep sound when walking
        if (controller.isGrounded && (strafe != 0 || walk != 0)
            && !audio.isPlaying && Time.time - lastStepTime >= 0.5f)
        {
            audio.clip = footsteps[step];
            audio.Play();
            step = (step + 1) % footsteps.Length;
            lastStepTime = Time.time;
        }

        // Shoot on mouse
        // TODO: Use "Fire1" instead of mouse?
        // TODO: Spawn a laser or something when firing
        if (Input.GetMouseButtonDown(0))
        {
            audio.clip = pewpew;
            audio.Play();

            int mask = ~(1 << 8); // Player layer is first layer that isn't reserved (8)
            RaycastHit target;
            bool hit = Physics.Raycast(camera.transform.position, camera.transform.forward, out target, Mathf.Infinity, mask); // TODO: Tweak firing distance
            if (hit)
            {
                if (target.transform.gameObject.CompareTag("Glass"))
                {
                    GameManager.HitGlass();
                }
                else if (target.transform.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemy = target.transform.gameObject.GetComponent<Enemy>(); // TODO: Replace this with messages?
                    enemy.Die();
                }
            }
        }
    }

}

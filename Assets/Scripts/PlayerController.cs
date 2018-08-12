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
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip pewpew;
    public AudioClip ouch;
    public LineRenderer leftLaser;
    public LineRenderer rightLaser;
    public Image healthUI;

    private Camera camera;
    private AudioSource audio;
    protected CharacterController controller;

    private const float MAX_HEALTH = 3;
    private float health;

    protected AudioClip[] footsteps;
    private int step;
    private float lastStepTime;

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        audio = GetComponent<AudioSource>();
        controller = GetComponentInChildren<CharacterController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        footsteps = new AudioClip[2];
        footsteps[0] = footstep1;
        footsteps[1] = footstep2;
        step = 0;
        lastStepTime = Time.time;

        health = MAX_HEALTH;
        healthUI.color = new Color32(200, 0, 0, 0);

        leftLaser.enabled = false;
        rightLaser.enabled = false;
    }

    void Update()
    {
        // Disallow movement if game is paused.
        if (GameManager.IsPaused())
        {
            return;
        }

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
        // TODO: Spawn a laser or something when firing
        if (GameManager.HavePowers() && Input.GetButtonDown("Fire1"))
        {
            StopCoroutine(FireLaser());
            StartCoroutine(FireLaser());
        }
    }

    IEnumerator FireLaser()
    {
        leftLaser.enabled = true;
        rightLaser.enabled = true;

        audio.clip = pewpew;
        audio.Play();

        Vector3 leftEye = camera.transform.position - 0.5f * camera.transform.right + 0.1f * camera.transform.forward;
        Vector3 rightEye = camera.transform.position + 0.5f * camera.transform.right + 0.1f * camera.transform.forward;
        leftLaser.SetPosition(0, leftEye);
        rightLaser.SetPosition(0, rightEye);

        RaycastHit target;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);        
        bool hit = Physics.Raycast(ray, out target, Mathf.Infinity, ~(1 << 8));
        if (hit)
        {
            leftLaser.SetPosition(1, target.point);
            rightLaser.SetPosition(1, target.point);

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
        else
        {
            Vector3 point = ray.GetPoint(10);
            leftLaser.SetPosition(1, point);
            rightLaser.SetPosition(1, point);
        }

        yield return new WaitForSeconds(0.1f);
        leftLaser.enabled = false;
        rightLaser.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.IsPaused())
        {
            return;
        }

        if (other.gameObject.CompareTag("Glass"))
        {
            GameManager.BumpGlass();
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            audio.clip = ouch;
            audio.Play();
            StartCoroutine(FlashHealth());
            
            health--;
            if (health == 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    IEnumerator FlashHealth()
    {
        byte alpha = 64;
        while (alpha > 0)
        {
            healthUI.color = new Color32(200, 0, 0, alpha);
            alpha -= 4;
            yield return null;
        }
    }

    IEnumerator Die()
    {
        for (int a = 0; a < 30; ++a)
        {
            transform.localEulerAngles += new Vector3(0, 0, -3);
            yield return null;
        }
        yield return StartCoroutine(GameManager.instance.OutOfHealth());
    }
}

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

public class Cell : MonoBehaviour
{

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public AudioSource audio;
    public AudioClip squash;
    public AudioClip smash;

    const float HEIGHT = 2.5f;
    const float MIN_WIDTH = 1f;
    const float MAX_WIDTH = 10f;
    const float DELTA = 0.0025f;

    protected float width;
    protected GameObject[] walls;
    protected GameObject floor;
    protected GameObject ceiling;
    protected bool smashed;

    void Start()
    {
        // Create cell geometry
        width = MAX_WIDTH;

        floor = GameObject.Instantiate(floorPrefab, this.transform);
        floor.transform.localScale = new Vector3(width, width, 1);
        floor.transform.localPosition = new Vector3(0, -0.5f, 0);

        ceiling = GameObject.Instantiate(ceilingPrefab, this.transform);
        ceiling.transform.localScale = new Vector3(width, width, 1);
        ceiling.transform.localPosition += new Vector3(0, HEIGHT, 0);

        walls = new GameObject[4];
        for (int d = 0; d < 4; ++d)
        {
            walls[d] = GameObject.Instantiate(wallPrefab, this.transform);
            walls[d].transform.localScale = new Vector3(0.02f, HEIGHT, width);
            walls[d].transform.localPosition += new Vector3(0, HEIGHT / 2, 0);
            switch (d)
            {
                case 0:
                    walls[d].transform.localEulerAngles = new Vector3(0, 0, 0);
                    walls[d].transform.localPosition += new Vector3(width / 2, 0, 0);
                    break;
                case 1:
                    walls[d].transform.localEulerAngles = new Vector3(0, -90, 0);
                    walls[d].transform.localPosition += new Vector3(0, 0, width / 2);
                    break;
                case 2:
                    walls[d].transform.localEulerAngles = new Vector3(0, 90, 0);
                    walls[d].transform.localPosition += new Vector3(0, 0, -width / 2);
                    break;
                case 3:
                    walls[d].transform.localEulerAngles = new Vector3(0, 180, 0);
                    walls[d].transform.localPosition += new Vector3(-width / 2, 0, 0);
                    break;
            }
        }

        smashed = false;
    }

    void Shrink()
    {
        // Reduce the size of the cell geometry by a fixed amount
        // (But stop it from shrinking too much)
        width -= DELTA;
        width = Mathf.Clamp(width, MIN_WIDTH, MAX_WIDTH);

        // Trigger various events based on the size of the cell
        if (!GameManager.EnemyReleased() && width <= 0.9 * MAX_WIDTH)
        {
            StartCoroutine(GameManager.instance.ReleaseTheEnemy());
        }
        if (!GameManager.HavePowers() && width <= 0.5f * MAX_WIDTH)
        {
            GameManager.ImbuePowers();
        }
        if (width == MIN_WIDTH)
        {
            audio.clip = squash;
            StartCoroutine(GameManager.instance.OutOfSpace(audio));
            return;
        }
        
        // Update the geometry to match the new size
        if (GameManager.SmashedGlass())
        {
            return;
        }
        floor.transform.localScale = new Vector3(width, width, 1);
        ceiling.transform.localScale = new Vector3(width, width, 1);
        for (int d = 0; d < 4; ++d)
        {
            walls[d].transform.localScale = new Vector3(0.02f, HEIGHT, width);

            // TODO: Replace this switch statement with something more elegant
            switch (d)
            {
                case 0:
                    walls[d].transform.localPosition -= new Vector3(DELTA / 2, 0, 0);
                    break;
                case 1:
                    walls[d].transform.localPosition -= new Vector3(0, 0, DELTA / 2);
                    break;
                case 2:
                    walls[d].transform.localPosition -= new Vector3(0, 0, -DELTA / 2);
                    break;
                case 3:
                    walls[d].transform.localPosition -= new Vector3(-DELTA / 2, 0, 0);
                    break;
            }
        }
    }

    void Update()
    {
        if (!GameManager.IsPaused())
        {
            Shrink();
        }
        if (GameManager.SmashedGlass() && smashed == false)
        {
            smashed = true;
            audio.clip = smash;
            audio.Play();
            foreach (GameObject wall in walls)
            {
                Destroy(wall);
            }
            Destroy(ceiling);
        }
    }

}

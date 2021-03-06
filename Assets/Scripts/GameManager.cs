﻿/**
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
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Singleton instance
    public static GameManager instance = null;

    // Shorthands to access other managers
    public static DialogueManager dialogueManager;

    // Magic constants for win/lose conditions
    private const int MAX_HITS = 30;

    // Game state
    public static GameObject player;
    public static bool firstGame;
    protected static int subjectNo;
    protected static int hits;
    protected static int bumps;
    protected static bool powers;
    protected static bool released;
    protected static bool smashed;
    protected static bool locked;
    protected static bool paused;

    void Awake()
    {
        // Ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
            subjectNo = (int)Random.Range(1, 8192);
            firstGame = true;
        }
        else if (instance != this)
        {
            firstGame = false;
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        // Initialize game state
        player = GameObject.FindGameObjectWithTag("Player");
        hits = 0;
        bumps = 0;
        powers = false;
        released = false;
        smashed = false;
        locked = true;
        paused = false;

        // Find other managers
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public IEnumerator Restart()
    {
        subjectNo += 1;
        hits = 0;
        bumps = 0;
        powers = false;
        released = false;
        smashed = false;
        locked = true;

        SceneManager.LoadScene(0, LoadSceneMode.Single);
        yield return null;
    }

    public IEnumerator Begin()
    {
        float success = Random.Range(51, 100);
        float failure = (100 - success);

        Dialogue exposition = new Dialogue();
        exposition.color = new Color32(255, 150, 255, 255);
        exposition.name = "Professor";
        exposition.sentences = new string[] {
            string.Format("Hello, Subject #{0}.", subjectNo),
            string.Format("I have good news: based on your results so far, there is a {0:#.##}% chance that this next test will be your last.", success),
            string.Format("Unfortunately, there is only a {0:#.##}% chance that you will not die horribly.", failure),
            "Please, try not to panic."
        };
        yield return dialogueManager.OpenDialogue(exposition);
        locked = false;
    }

    public static bool IsLocked()
    {
        return locked;
    }

    public static bool IsPaused()
    {
        return paused;
    }

    public static void Pause()
    {
        Time.timeScale = 0.0f;
        paused = true;
    }

    public static void Resume()
    {
        Time.timeScale = 1.0f;
        paused = false;
    }

    public static void HitGlass()
    {
        hits++;
        if (hits == 5)
        {
            Dialogue dialogue = new Dialogue();
            dialogue.color = new Color32(150, 150, 255, 255);
            dialogue.name = "Assistant";
            dialogue.sentences = new string[] {
                "Subject shows clear signs of distress, and has begun trying to escape from the test."
            };
            dialogueManager.OpenDialogue(dialogue);
        }
        if (hits == MAX_HITS / 2)
        {
            Dialogue dialogue = new Dialogue();
            dialogue.color = new Color32(150, 150, 255, 255);
            dialogue.name = "Professor";
            dialogue.sentences = new string[] {
                string.Format("Give up, Subject #{0} -- it's no use.  You think we can manufacture shrinking glass, but can't stop it from breaking?", subjectNo),
            };
            dialogueManager.OpenDialogue(dialogue);
        }
        if (hits >= MAX_HITS)
        {
            smashed = true;
            Dialogue dialogue = new Dialogue();            
            dialogue.color = new Color32(255, 150, 255, 255);
            dialogue.name = "Professor";
            dialogue.sentences = new string[] {
                "...The subject seems to have broken free of the glass.",
                "This has never happened before."
            };
            dialogueManager.OpenDialogue(dialogue);
        }
    }

    public static void BumpGlass()
    {
        const int MAX_BUMPS = 5;
        if (bumps < MAX_BUMPS)
        {
            bumps++;
            if (bumps == MAX_BUMPS)
            {
                Dialogue dialogue = new Dialogue();
                dialogue.color = new Color32(150, 150, 255, 255);
                dialogue.name = "Assistant";
                dialogue.sentences = new string[] {
                    "Subject appears to have forgotten that they cannot walk through glass."
                };
                dialogueManager.OpenDialogue(dialogue);
            }
        }
    }

    public static bool SmashedGlass()
    {
        return smashed;
    }

    public IEnumerator ReleaseTheEnemy()
    {
        released = true;

        Dialogue dialogue = new Dialogue();
        dialogue.color = new Color32(150, 150, 255, 255);
        dialogue.name = "Assistant";
        dialogue.sentences = new string[] {
            "Releasing the stimulus."
        };
        yield return dialogueManager.OpenDialogue(dialogue);

        dialogue.color = new Color32(255, 200, 255, 255);
        dialogue.name = "Professor";
        dialogue.sentences = new string[] {
            "Try not to let it touch you."
        };
        yield return dialogueManager.OpenDialogue(dialogue);
    }

    public static bool EnemyReleased()
    {
        return released;
    }

    public static void ImbuePowers()
    {
        powers = true;
        Dialogue dialogue = new Dialogue();
        dialogue.color = new Color32(255, 150, 255, 255);
        dialogue.name = "Professor";
        dialogue.sentences = new string[] {
            "It's working!",
            "Subject's latent abilities have awakened -- as expected -- in response to stress."
        };
        dialogueManager.OpenDialogue(dialogue);
    }

    public static bool HavePowers()
    {
        return powers;
    }

    public IEnumerator OutOfSpace(AudioSource audio)
    {
        audio.Play();
        locked = true;
        Camera.main.cullingMask = (1 << 5); // only show the UI
        Dialogue dialogue = new Dialogue();
        dialogue.color = new Color32(255, 150, 255, 255);
        dialogue.name = "Professor";
        dialogue.sentences = new string[] {
            "Sigh...  How disappointing.",
            string.Format("I really expected better of you, Subject #{0}.", subjectNo),
            "We're going to have to try again.  Prepare the next subject."
        };
        yield return dialogueManager.OpenDialogue(dialogue);
        yield return GameManager.instance.Restart();
    }

    public IEnumerator OutOfHealth()
    {
        locked = true;

        Dialogue dialogue = new Dialogue();
        dialogue.color = new Color32(255, 150, 255, 255);
        dialogue.name = "Professor";
        dialogue.sentences = new string[] {
            "Another dead subject.  Why do they all find it so hard to avoid the stimulus?",
            "Oh well...  Onward and upward!"
        };
        yield return dialogueManager.OpenDialogue(dialogue);
        yield return GameManager.instance.Restart();
    }

    public IEnumerator Win()
    {
        locked = true;

        Dialogue dialogue = new Dialogue();
        dialogue.color = new Color32(255, 255, 255, 255);
        dialogue.name = "Developer";
        dialogue.sentences = new string[] {
            "You did it!  I hope you didn't encounter too many bugs along the way.",
            "Thanks for taking the time to play 'Shrinking Panes'.",
            "This game will self-destruct in 3, 2, 1..."
        };
        yield return dialogueManager.OpenDialogue(dialogue);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

}
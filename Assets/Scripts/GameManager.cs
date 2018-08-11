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

public class GameManager : MonoBehaviour
{

    // Singleton instance
    public static GameManager instance = null;

    // Shorthands to access other managers
    public static DialogueManager dialogueManager;

    // Magic constants for win/lose conditions
    private const int MAX_HITS = 5;

    // Game state
    protected static int hits;

    void Awake()
    {
        // Ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        // Initialize game state
        hits = 0;

        // Find other managers
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Start()
    {
        Begin();
    }

    public static void Begin()
    {
        Dialogue exposition = new Dialogue();
        exposition.color = new Color32(255, 150, 255, 255);
        exposition.name = "Professor";
        exposition.sentences = new string[] {
            string.Format("Hello, Subject #{0}.", (int) Random.Range(1, 8192)),
            "I have good news: based on your results so far, there is a 75% chance that this next test will be your last.",
            "Unfortunately, there is also a 25% chance that you will die -- horribly.",
            "Please, try not to panic."
        };
        dialogueManager.OpenDialogue(exposition);
    }

    public static void HitGlass()
    {
        hits++;
        if (hits >= MAX_HITS)
        {
            Dialogue secretEnding = new Dialogue();            
            secretEnding.color = new Color32(255, 150, 255, 255);
            secretEnding.name = "Professor";
            secretEnding.sentences = new string[] {
                "...The subject seems to have broken free of the glass.",
                "This has never happened before."
            };
            dialogueManager.OpenDialogue(secretEnding);
        }
    }

}
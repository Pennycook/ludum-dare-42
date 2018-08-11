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
/**
 * Based on Brackey's Dialogue-System tutorial:
 * https://github.com/Brackeys/Dialogue-System
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public Image box;
    public Text nameUI;
    public Text dialogueUI;

    private Queue<string> sentenceQueue;

    void Awake()
    {
        sentenceQueue = new Queue<string>();
    }

    public void OpenDialogue(Dialogue dialogue)
    {
        nameUI.color = dialogue.color;
        nameUI.text = dialogue.name;

        sentenceQueue.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentenceQueue.Enqueue(string.Format("\"{0}\"", sentence));
        }

        box.GetComponent<CanvasGroup>().alpha = 1;
        StopAllCoroutines();
        StartCoroutine(TypeDialogue());
    }

    IEnumerator TypeDialogue()
    {
        while (sentenceQueue.Count > 0)
        {
            string sentence = sentenceQueue.Dequeue();

            dialogueUI.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueUI.text += letter;
                yield return null;
            }
            yield return new WaitForSeconds(2.0f);
        }
        CloseDialogue();
    }
    
    public void CloseDialogue()
    {
        box.GetComponent<CanvasGroup>().alpha = 0;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }

    public static bool dialogueDisplay = false;

    public GameObject dialogueUI;
    public Text dialogueText;

    public float sentenceDelay = 3f;

    private Queue<string> sentences = new Queue<string>();

    public void StartDialogue(string[] sentences)
    {
        dialogueUI.SetActive(true);
        dialogueDisplay = true;

        this.sentences.Clear();

        foreach(string sentence in sentences)
        {
            this.sentences.Enqueue(sentence);
        }
        StartCoroutine(DisplaySentences());        
    }

    IEnumerator DisplaySentences()
    {
        string sentence;

        while (sentences.Count > 0)
        {
            sentence = sentences.Dequeue();
            dialogueText.text = sentence;

            yield return new WaitForSeconds(sentenceDelay);
        }

        EndDialogue();
    }

    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
        dialogueDisplay = false;
    }
}

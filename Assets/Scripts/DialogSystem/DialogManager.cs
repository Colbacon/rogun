using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogManager : MonoBehaviour
{

    #region Singleton

    public static DialogManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public static bool dialogueDisplay = false;

    public GameObject dialogueUI;
    public Text dialogueText;

    public float sentenceDelay = 2;

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
        //DisplayNextSentence();
        
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

    /*
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }*/

    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
        dialogueDisplay = false;
    }
}

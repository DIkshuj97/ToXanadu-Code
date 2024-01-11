using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroSequenceManager : MonoBehaviour
{
    [SerializeField] TMP_Text introtext;
    [SerializeField] GameObject startButton;

    [TextArea(3,5)][SerializeField] string sentence;

    private void Awake()
    {
        startButton.SetActive(false);
        introtext.text = "";
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        yield return new WaitForSeconds(1f);
        foreach (char letter in sentence.ToCharArray())
        {
            //SoundManager.ins.PlayExtraAudio("DialogueType", aS);
            introtext.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.05f);
        startButton.SetActive(true);
    }

    public void StartGame()
    {
        SoundManager.ins.ClickSFX();
        LevelLoader.MapScreen();
    }
}

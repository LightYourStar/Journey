using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasicTypewriter : MonoBehaviour
{
    public float charsPerSecond = 0.1f; 
    private string fullText;
    private Text textComponent;
    private bool isAnimating = false; 

    void Awake()
    {
        textComponent = GetComponent<Text>();
    }



    IEnumerator TypeText()
    {
        isAnimating = true;
        int currentCharIndex = 0;

        while (currentCharIndex < fullText.Length)
        {
            textComponent.text = fullText.Substring(0, currentCharIndex + 1);
            currentCharIndex++;
            yield return new WaitForSeconds(charsPerSecond);
        }
        isAnimating = false;
    }

    public void SetText(string newText)
    {
        StopAllCoroutines(); 
        fullText = newText;
        textComponent.text = ""; 
        StartCoroutine(TypeText());
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public void CompleteImmediately()
    {
        StopAllCoroutines(); 
        textComponent.text = fullText; 
        isAnimating = false;
    }
}
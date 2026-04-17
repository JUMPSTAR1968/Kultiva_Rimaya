using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[System.Serializable]
public struct FaceBinding {
    public string faceName;
    public Sprite faceImage;
}

[System.Serializable]
public class DialogueLine {
    public string speakerName;
    public string initialFaceKey;
    [TextArea(3, 10)]
    public string rawText;
    public AudioClip voiceLine;
}

public class MidLineTag {
    public int triggerIndex;
    public string action;
    public string value;

    public MidLineTag(int index, string act, string val) {
        triggerIndex = index;
        action = act;
        value = val;
    }
}

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogueArea;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI lineText;
    [SerializeField] private UnityEngine.UI.Image characterSprite;
    [SerializeField] private AudioSource dialogueAudioSource;

    [Header("Configuration")]
    [SerializeField] private float defaultTypingDelay = 0.05f;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private FaceBinding[] inspectorFaces;

    private Dictionary<string, Sprite> faceDictionary = new Dictionary<string, Sprite>();
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    
    private bool isPlaying = false;
    private bool isAwaitingAdvance = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        foreach (var binding in inspectorFaces) {
            if (!faceDictionary.ContainsKey(binding.faceName)) {
                faceDictionary.Add(binding.faceName, binding.faceImage);
            }
        }
        dialogueArea.SetActive(false);
    }

    private void Update() {
        if (isAwaitingAdvance) { //todo: adapt next button logic
            isAwaitingAdvance = false;
            //ProcessNextLine();
        }
    }

    public void StartDialogue(DialogueLine[] lines) {
        if (isPlaying) return;
        
        isPlaying = true;
        dialogueArea.SetActive(true);
        dialogueQueue.Clear();
        
        foreach (var line in lines) {
            dialogueQueue.Enqueue(line);
        }
        
        ProcessNextLine();
    }

    private void ProcessNextLine() {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLineData = dialogueQueue.Dequeue();
        
        string cleanText;
        List<MidLineTag> tagQueue;
        ScanTags(currentLineData.rawText, out cleanText, out tagQueue);

        nameText.text = currentLineData.speakerName;
        UpdateFace(currentLineData.initialFaceKey);
        lineText.text = "";

        float baseTypingDelay = defaultTypingDelay;
        bool hasVA = currentLineData.voiceLine != null;
        
        if (hasVA) {
            float totalPauseTime = 0f;
            foreach (var tag in tagQueue) {
                // bool isValidPauseTag = tag.action == "pause" && float.TryParse(tag.value, out float pauseVal)
                if (tag.action == "pause" && float.TryParse(tag.value, out float pauseVal)) {
                    totalPauseTime += pauseVal;
                }
            }

            // Typing delay excludes VA silence/pauses, only considers speaking time
            if (cleanText.Length > 0) { // bool isCleanTextNotEmpty = cleanText.Length > 0
                // float onlySpeakingTime = currentLineData.voiceLine.length - totalPauseTime
                float safeAudioTime = Mathf.Max(0.01f, currentLineData.voiceLine.length - totalPauseTime);
                   // avoids negative time as a result of total duration of all pause tags being greater than VA audio's duration
                baseTypingDelay = safeAudioTime / cleanText.Length;
            }
            
            dialogueAudioSource.clip = currentLineData.voiceLine;
            dialogueAudioSource.Play();
        }

        StartCoroutine(TypewriterCoroutine(cleanText, tagQueue, baseTypingDelay, hasVA));
    }

    private void ScanTags(string rawText, out string cleanText, out List<MidLineTag> tagQueue) {
        cleanText = rawText;
        tagQueue = new List<MidLineTag>();
        int accumulatedShift = 0;

        /*
         regex looks for tag chunks that are enclosed within "[]", and additionally looks for a space
            character

                                                                 |- space character
                                                                 V
                rawText = "Noong unang panahon, [sprite=OpenEyes] may ..."
                                                ^               ^ ^- immediately proceeding character/letter
                                                |----- tag -----|
                                                |--regex match---|
         */

        Regex regex = new Regex(@"\[.*?\]\s?");
        MatchCollection matches = regex.Matches(rawText);

        foreach (Match match in matches) {
            /* 
             * -triggerIndex pinpoints the character/letter immediately after the tag AND a space character
             *
             *                                                  |- space character
             *                                                  V
             *  rawText = "Noong unang panahon, [sprite=OpenEyes] may ..."
             *                                  ^               ^ ^- immediately proceeding character/letter
             *                                  |----- tag -----|    & future triggerIndex location in "cleanText"
             *                                  |--regex match---|
             *                                  |accumulatedShift|
             *                                  |     count      |
             *
             * -in the example: triggerIndex will point to the letter 'm', which is immediately after the sprite
             *      tag plus a space character
             *
             * recap:
             *    regex looks for tag chunks that are enclosed within "[]", and additionally includes a space
             *        character
             *
             * -whatever tag chunks the regex found are inserted to variable "matches"
             * -this foreach loop iterates every matches item, with each item individually referenced as "match" 
             * -accumulatedShift tracks how many tag chunk characters have since been filtered
             * -without accumulatedShift, triggerIndex cannot catch where the immediately proceeding character is in
             *      the filtered text ("cleanText")
             * -after filtering out the tag chunks in the "cleanText", all characters after the tag chunk must shift
             *      to the left, to ensure accurate triggerIndex
             * -triggerIndex excludes accumulatedShift count in executing tag commands at the exact location of
             *      immediately proceeding character in cleanText
             *                                   
             *      cleanText = "Noong unang panahon, may ..."
             *                                        ^- triggerIndex location & immediately proceeding character/letter
             *
             * -cleanText will be the reference for triggerIndex, not anymore the rawText with unfiltered tag chunks
             */

            int triggerIndex = match.Index - accumulatedShift;
            
            string tagData = match.Value.Trim().Trim('[', ']');
            /*
             * first Trim() in the chain removes the space character
             * 
             *      tagData = "[sprite=OpenEyes] "
             *                                  ^- space character
             * 
             * second Trim() in the chain removes the brackets
             * 
             */
            string[] tagParts = tagData.Split('=');

            if (tagParts.Length == 2) {
                tagQueue.Add(new MidLineTag(triggerIndex, tagParts[0].Trim(), tagParts[1].Trim()));
            }

            cleanText = cleanText.Remove(triggerIndex, match.Length); // check function
            accumulatedShift += match.Length;
        }
    }

    private void UpdateFace(string key) {
        if (string.IsNullOrEmpty(key)) return;

        if (faceDictionary.TryGetValue(key, out Sprite sprite)) {
            characterSprite.sprite = sprite;
            Debug.Log($"Current sprite key: '{key}', {sprite.name}");
        }
        else {
            Debug.LogWarning($"DialogueManager: Sprite key '{key}' not found.");
            if (defaultSprite != null) characterSprite.sprite = defaultSprite;
        }
    }

    private IEnumerator TypewriterCoroutine(string cleanText, List<MidLineTag> tagQueue, float baseTypingDelay, bool hasVA) {
        int currentIndex = 0;
        float currentDelay = baseTypingDelay;

        while (currentIndex <= cleanText.Length) {
            for (int i = tagQueue.Count - 1; i >= 0; i--) {
                if (tagQueue[i].triggerIndex == currentIndex) {
                    MidLineTag tag = tagQueue[i];
                    tagQueue.RemoveAt(i);

                    switch (tag.action) {
                        case "sprite":
                            UpdateFace(tag.value);
                            break;
                        case "pause":
                            if (float.TryParse(tag.value, out float pauseDuration)) {
                                yield return new WaitForSeconds(pauseDuration);
                            }
                            break;
                        case "speed":
                            if (!hasVA && float.TryParse(tag.value, out float newSpeed)) {
                                currentDelay = newSpeed;
                            }
                            break;
                        case "name":
                            nameText.text = tag.value;
                            break;
                    }
                }
            }

            // FIX: If we have reached the end of the text, break the loop NOW 
            // before we try to print text[currentIndex] and cause an out-of-bounds error.

            // bool atFinalIndexForLastCommand = currentIndex == cleanText.Length
            if (currentIndex == cleanText.Length)
            {
                break;
            }

            lineText.text += cleanText[currentIndex];
            yield return new WaitForSeconds(currentDelay);
            currentIndex++;
        }

        isAwaitingAdvance = true;
    }

    private void EndDialogue() {
        isPlaying = false;
        dialogueArea.SetActive(false);
        lineText.text = "";
        nameText.text = "";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[System.Serializable]
public class Dialogue 
{
    [Header("DIALOGUE")]
    public string npcName;
    public Texture background;
    public DialogueType dialogueType;
    public int dialogueSegmentNum;
    public DialogueSegment[] dialogueSegment;
    public bool isDeactivated;

    [Header("STORE")]
    public GameObject[] storeItemPrefab;

    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the active quest.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number of active quest")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;
    public bool globalUpdatePhase;
    public string itemName;
    public int removeQuantity;

    [HideInInspector] public QuestData questData;
    [HideInInspector] public AIController controller;
    [HideInInspector] public DialogueState dialogueState;

    [HideInInspector] public int defaultSegment;
    [HideInInspector] public int completeSegment;
    [HideInInspector] public int completedSegment;
    [HideInInspector] public int optionASegment;
    [HideInInspector] public int optionBSegment;
    [HideInInspector] public bool dialogueAltered;

    public enum DialogueType
    {
        Default,
        Store,
        ItemGiver,
        QuestGiver,
        QuestInProgress,
        FollowAsCompanion
    }

    public enum AIState
    {
        Idle,
        Follow,
        RoamArea,
        PatrolPoints,
        Dead
    }

    public enum DialogueState
    {
        Default,
        OptionA,
        OptionB
    }

    public enum TalkAnim
    {
        None,
        Talk1,
        Talk2, 
        Talk3, 
        Talk4,
        Talk5,
        Talk6, 
        Talk7,
        Talk8
    }

    public enum CameraState
    {
        LookAt, Tween, orbit
    }

    [System.Serializable]
    public struct DialogueSegment
    {
        public string segmentName;

        [TextArea(3, 10)]
        public string[] defaultSentence;
        public AIState newAIState;

        [Header("TALK SOUND TRIGGER")]
        public AudioClip talkAudio;

        [Header("TALK ANIMATION TRIGGER")]
        public TalkAnim talkAnim;

        [Header("CINEMATIC CAMERA")]
        public Transform[] newCamPos;
        public Transform[] newCamLookAt;
        [Tooltip("Any value higher than zero will trigger a count down")]
        public float cameraStayTime;
        public CameraState cameraState;
        [HideInInspector] public Transform originCamPos;

        [Header("QUEST")]
        [Tooltip("The name of the active quest.")]
        public string questSequenceNameReceipt;
        [Tooltip("The required phase number of active quest")]
        public int questPhaseReceipt;
        public AIState questNewState;
        [TextArea(3, 10)]
        public string[] completeRequired;
        [TextArea(3, 10)]
        public string[] completedRequired;

        [Header("QUESTION")]
        public bool triggerOption;
        public bool timedQuestion;
        public string optionAAnswer;
        public string optionBAnswer;
        [TextArea(3, 10)]
        public string[] optionASentence;
        [TextArea(3, 10)]
        public string[] optionBSentence;

        [Header("ITEMS")]
        [HideInInspector] public bool hasReceived;
        public DialogueState dialogueItemTrigger;
        public GameObject[] givenItemPrefab;
    }
}

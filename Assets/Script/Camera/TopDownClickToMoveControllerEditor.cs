﻿using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

namespace BLINK.Controller
{
    [CustomEditor(typeof(TopDownClickToMoveController))]
    public class TopDownClickToMoveControllerEditor : Editor
    {
        private TopDownClickToMoveController REF;
        
        private void OnEnable()
        {
            REF = (TopDownClickToMoveController) target;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((TopDownClickToMoveController) target),
                typeof(TopDownClickToMoveController),
                false);
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();

            GUIStyle titleStyle = GetStyle("title");
            
            GUILayout.Space(5);
            GUILayout.Label("REFERENCES", titleStyle);
            GUILayout.Space(5);
            
            //added layer
            //REF.enemyLayer = EditorGUILayout.LayerField(GetGUIContent("Enemy Layer:", "The layer that defines the enemy"),
                //REF.enemyLayer);

            REF.cameraAudio = (AudioSource) EditorGUILayout.ObjectField(GetGUIContent("Audio Source:", "The audio source used by the character controller"),
                REF.cameraAudio, typeof(AudioSource), true);
            REF.anim = (Animator) EditorGUILayout.ObjectField(GetGUIContent("Animator:", "The animator used by the character"),
                REF.anim, typeof(Animator), true);
            REF.agent = (NavMeshAgent) EditorGUILayout.ObjectField(
                GetGUIContent("NavMesh Agent:", "The NavMesh agent component used by this character"),
                REF.agent, typeof(NavMeshAgent), true);
            
            
            GUILayout.Space(5);
            GUILayout.Label("CAMERA", titleStyle);
            GUILayout.Space(5);
            REF.cameraEnabled = EditorGUILayout.Toggle(GetGUIContent("Camera Enabled?", "Should the camera logic and input be executed?"),
                REF.cameraEnabled);
            if (REF.cameraEnabled)
            {
                REF.initCameraOnSpawn = EditorGUILayout.Toggle(
                    GetGUIContent("Init Camera?", "Should the camera be initialized after spawning the player?"),
                    REF.initCameraOnSpawn);
                if (REF.initCameraOnSpawn)
                {
                    REF.cameraName = EditorGUILayout.TextField(
                        GetGUIContent("Camera Name:", "The GameObject name of the camera to find on start"),
                        REF.cameraName);
                }
                else
                {
                    REF.playerCamera = (Camera) EditorGUILayout.ObjectField(
                        GetGUIContent("Camera:", "The camera used to follow the player"),
                        REF.playerCamera, typeof(Camera), true);
                }

                REF.cameraPositionOffset = EditorGUILayout.Vector3Field(
                    GetGUIContent("Position:", "The position offset that the camera will have relative to the player"),
                    REF.cameraPositionOffset);
                REF.cameraRotationOffset = EditorGUILayout.Vector3Field(
                    GetGUIContent("Rotation:", "The rotation at which the camera will start"),
                    REF.cameraRotationOffset);
                REF.minCameraHeight = EditorGUILayout.FloatField(
                    GetGUIContent("Min. Height:", "The minimum height at which the camera can be"),
                    REF.minCameraHeight);
                REF.maxCameraHeight = EditorGUILayout.FloatField(
                    GetGUIContent("Max. Height:", "The maximum height at which the camera can be"),
                    REF.maxCameraHeight);
                REF.minCameraVertical = EditorGUILayout.FloatField(
                    GetGUIContent("Min. Vertical:",
                        "The minimum distance on the Z axis that the camera can be relative to the character"),
                    REF.minCameraVertical);
                REF.maxCameraVertical = EditorGUILayout.FloatField(
                    GetGUIContent("Max. Vertical:",
                        "The maximum distance on the Z axis that the camera can be relative to the character"),
                    REF.maxCameraVertical);
                REF.cameraZoomSpeed = EditorGUILayout.FloatField(
                    GetGUIContent("Zoom Speed:", "The speed at which the camera zoom is applied each fixed frame"),
                    REF.cameraZoomSpeed);
                REF.cameraZoomPower = EditorGUILayout.FloatField(
                    GetGUIContent("Zoom Power:", "How strong the zoom is for each input"),
                    REF.cameraZoomPower);
                REF.cameraRotateSpeed = EditorGUILayout.FloatField(
                    GetGUIContent("Rotation Speed:", "How fast should the camera rotate"),
                    REF.cameraRotateSpeed);
            }

            
            GUILayout.Space(5);
            GUILayout.Label("NAVIGATION SETTINGS", titleStyle);
            GUILayout.Space(5);
            REF.movementEnabled = EditorGUILayout.Toggle(GetGUIContent("Movement Enabled?", "Should the movement logic and input be executed?"),
                REF.movementEnabled);
            int layersSelection = REF.groundLayers;
            if (REF.movementEnabled)
            {
                REF.stunned = EditorGUILayout.Toggle(GetGUIContent("Stunned?", "If true, the character cannot move"),
                    REF.stunned);
                REF.destinationTreshold = EditorGUILayout.FloatField(
                    GetGUIContent("Dest. Treshold:", "The margin of error accepted for a destination to be reached"),
                    REF.destinationTreshold);
                
                 layersSelection = EditorGUILayout.MaskField("Layers", LayerMaskToField(REF.groundLayers), InternalEditorUtility.layers);
                
                REF.maxGroundRaycastDistance = EditorGUILayout.FloatField(
                    GetGUIContent("Max. Raycast Distance:",
                        "How far away from the camera should the ground hit be registered?"),
                    REF.maxGroundRaycastDistance);
                REF.minimumPathDistance = EditorGUILayout.FloatField(
                    GetGUIContent("Min. Path Distance:",
                        "The minimum distance between the click and the player required for the path to be accepted"),
                    REF.minimumPathDistance);
                REF.samplePositionDistanceMax = EditorGUILayout.FloatField(
                    GetGUIContent("Invalid Path Treshold:",
                        "How far in meters an invalid path should be for the controller to attempt finding a new valid path close by, the highest the value, the more heavy on performance it is"),
                    REF.samplePositionDistanceMax);
            }

            
            GUILayout.Space(5);
            GUILayout.Label("INPUT SETTINGS", titleStyle);
            GUILayout.Space(5);
            REF.moveKey = (KeyCode) EditorGUILayout.EnumPopup(GetGUIContent("Move Key:", "The key to trigger a move to the cursor"), REF.moveKey);
            REF.allowHoldKey = EditorGUILayout.Toggle(GetGUIContent("Can Hold Move Key?", "Should the character follow the mouse position if the move key is held down?"),
                REF.allowHoldKey);
            REF.holdMoveCd = EditorGUILayout.FloatField(
                GetGUIContent("Move Hold Interval:", "The duration between each path update when the move key is held down"),
                REF.holdMoveCd);
            
            REF.standKey = (KeyCode) EditorGUILayout.EnumPopup(GetGUIContent("Stand Key:", "If pressed, the current path will be cancelled. If held down, the character will stand still"), REF.standKey);
            REF.charLookAtCursorWhileStanding = EditorGUILayout.Toggle(
                GetGUIContent("Look At Cursor?", "Should the character follow the cursor position while standing?"),
                REF.charLookAtCursorWhileStanding);
            
            REF.canRotateCamera = EditorGUILayout.Toggle(
                GetGUIContent("Can Rotate Camera?", "Can the camera be rotated?"),
                REF.canRotateCamera);
            if (REF.canRotateCamera)
            {
                REF.camRotateKeyLeft = (KeyCode) EditorGUILayout.EnumPopup(
                    GetGUIContent("Rotate Left Key:", "The key to rotate the camera to the left"),
                    REF.camRotateKeyLeft);
                REF.camRotateKeyRight = (KeyCode) EditorGUILayout.EnumPopup(
                    GetGUIContent("Rotate Right Key:", "The key to rotate the camera to the right"),
                    REF.camRotateKeyRight);
            }

            GUILayout.Space(5);
            GUILayout.Label("INPUT FEEDBACK", titleStyle);
            GUILayout.Space(5);
            REF.alwaysTriggerGroundPathFeedback = EditorGUILayout.Toggle(
                GetGUIContent("Always Show?", "Should the ground marker always show even when holding the move key?"),
                REF.alwaysTriggerGroundPathFeedback);
            REF.validGroundPathPrefab = (GameObject) EditorGUILayout.ObjectField(
                GetGUIContent("Valid Path Prefab:", "The prefab that will be spawned on the cursor position when a valid path was clicked"),
                REF.validGroundPathPrefab, typeof(GameObject), false);
            REF.rectifiedGroundPathPrefab = (GameObject) EditorGUILayout.ObjectField(
                GetGUIContent("Rectified Path Prefab:", "The prefab that will be spawned on the new path position when a invalid path was clicked, but a valid one was found"),
                REF.rectifiedGroundPathPrefab, typeof(GameObject), false);
            REF.markerPositionOffset = EditorGUILayout.Vector3Field(
                GetGUIContent("Prefab Position:", "The position at which the prefab will be spawned, relative to its original position"),
                REF.markerPositionOffset);
            REF.groundMarkerDuration = EditorGUILayout.FloatField(
                GetGUIContent("Prefab Duration:", "The prefab will be destroyed after this duration"),
                REF.groundMarkerDuration);
            
            REF.validGroundPathAudio = (AudioClip) EditorGUILayout.ObjectField(
                GetGUIContent("Valid Path Audio:", "The sound that will be played when a valid path was clicked"),
                REF.validGroundPathAudio, typeof(AudioClip), false);
            REF.rectifiedGroundPathAudio = (AudioClip) EditorGUILayout.ObjectField(
                GetGUIContent("Rectified Path Audio:", "The sound that will be played when a invalid path was clicked, but a valid one was found"),
                REF.rectifiedGroundPathAudio, typeof(AudioClip), false);
                

            if (!EditorGUI.EndChangeCheck()) return;
            REF.groundLayers = FieldToLayerMask(layersSelection);
            EditorUtility.SetDirty(REF);
            serializedObject.ApplyModifiedProperties();
            
        }

        private GUIStyle GetStyle(string styleName)
        {
            var style = new GUIStyle();
            switch (styleName)
            {
                case "title":
                    style.alignment = TextAnchor.UpperLeft;
                    style.fontSize = 17;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = Color.white;
                    break;
            }

            return style;
        }

        private GUIContent GetGUIContent (string name, string tooltip)
        {
            return new GUIContent(name, tooltip);
        }
        
        private LayerMask FieldToLayerMask(int field)
        {
            LayerMask mask = 0;
            var layers = InternalEditorUtility.layers;
            for (int c = 0; c < layers.Length; c++)
            {
                if ((field & (1 << c)) != 0)
                {
                    mask |= 1 << LayerMask.NameToLayer(layers[c]);
                }
            }
            return mask;
        }
        private int LayerMaskToField(LayerMask mask)
        {
            int field = 0;
            var layers = InternalEditorUtility.layers;
            for (int c = 0; c < layers.Length; c++)
            {
                if ((mask & (1 << LayerMask.NameToLayer(layers[c]))) != 0)
                {
                    field |= 1 << c;
                }
            }
            return field;
        }
    }
}

using PlayerStates;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

// [CanEditMultipleObjects]
// [CustomEditor(typeof(PlayerController))]
public class PlayerInspector : Editor {
    PlayerController pc;



    bool showStats;
    bool showMovement;
    bool showWASD;
    bool showJumping;
    bool showCamera;
    bool showSliding;
    bool showDash;
    bool showState;
    bool showSlam;
    bool showStamina;
    bool showElements;


    SerializedProperty slideReductionIncrement;
    SerializedProperty slideReductionIncrementTime;

    SerializedProperty dashReductionIncrement;
    SerializedProperty dashReductionIncrementTime;
    SerializedProperty dashDirectionChangeSpeed;

    SerializedProperty currentElements;

    public void DisplayList(SerializedProperty item, ref bool Foldout, string name, string itemName = "item")
    {
        GUILayout.Label(name);

        item.arraySize = EditorGUILayout.IntField("Size", item.arraySize);
        Foldout = EditorGUILayout.Foldout(Foldout, "items");

        if (Foldout)
        {

            for (int i = 0; i < item.arraySize; i++)
            {
                EditorGUI.indentLevel++;

                var dialogue = item.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(dialogue, new GUIContent($"{itemName}  {i}"));

                EditorGUI.indentLevel--;
            }
        }
    }

    void OnEnable()
    {
        pc = target as PlayerController;

        slideReductionIncrement = serializedObject.FindProperty("slideReductionIncrement");
        slideReductionIncrementTime = serializedObject.FindProperty("slideReductionIncrementTime");
        dashReductionIncrement = serializedObject.FindProperty("dashReductionIncrement");
        dashReductionIncrementTime = serializedObject.FindProperty("dashReductionIncrementTime");
        dashDirectionChangeSpeed = serializedObject.FindProperty("dashDirectionChangeSpeed");
        currentElements = serializedObject.FindProperty("currentElements");


        showStats = PlayerPrefs.GetInt(nameof(showStats)) == 1;
        showMovement = PlayerPrefs.GetInt(nameof(showMovement)) == 1;
        showWASD = PlayerPrefs.GetInt(nameof(showWASD)) == 1;
        showJumping = PlayerPrefs.GetInt(nameof(showJumping)) == 1;
        showCamera = PlayerPrefs.GetInt(nameof(showCamera)) == 1;
        showSliding = PlayerPrefs.GetInt(nameof(showSliding)) == 1;
        showDash = PlayerPrefs.GetInt(nameof(showDash)) == 1;
        showState = PlayerPrefs.GetInt(nameof(showState)) == 1;
        showSlam = PlayerPrefs.GetInt(nameof(showSlam)) == 1;
        showStamina = PlayerPrefs.GetInt(nameof(showStamina)) == 1;
    }




    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        showElements = EditorGUILayout.Foldout(showElements, "Elements", true);
        PlayerPrefs.SetInt(nameof(showElements), Convert.ToInt32(showElements));

        if (showElements)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(currentElements);
            EditorGUI.indentLevel--;
            
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showState = EditorGUILayout.Foldout(showState, "State", true);

        PlayerPrefs.SetInt(nameof(showState), Convert.ToInt32(showState));

        if (showState)
        {
            pc.s = (state)EditorGUILayout.EnumPopup("State", pc.s);
            pc.adminState = (AdminState)EditorGUILayout.EnumPopup("Admin State", pc.adminState);

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showStats = EditorGUILayout.Foldout(showStats, "Stats", true);
        PlayerPrefs.SetInt(nameof(showStats), Convert.ToInt32(showStats));
        
        if (showStats)
        {
            pc.health = EditorGUILayout.FloatField("Health", pc.health);
            pc.defence = EditorGUILayout.FloatField("Defence", pc.defence);

            showStamina = EditorGUILayout.Foldout(showStamina, "Stamina", true);
            PlayerPrefs.SetInt(nameof(showStamina), Convert.ToInt32(showStamina));

            if (showStamina)
            {
                pc.stamina = EditorGUILayout.FloatField("Stamina", pc.stamina);
                pc.minStamina = EditorGUILayout.FloatField("Min Stamina", pc.minStamina);
                pc.maxStamina = EditorGUILayout.FloatField("Max Stamina", pc.maxStamina);
                pc.staminaPerTick = EditorGUILayout.FloatField("Stamina Per Tick", pc.staminaPerTick);
                pc.deltaTick = EditorGUILayout.FloatField("Δ Tick", pc.deltaTick);
                pc.dashStamina = EditorGUILayout.FloatField("Dash Stamina", pc.dashStamina);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showCamera = EditorGUILayout.Foldout(showCamera, "Camera", true);
        PlayerPrefs.SetInt(nameof(showCamera), Convert.ToInt32(showCamera));
        
        if (showCamera)
        {
            pc.mouseSensitivityX = EditorGUILayout.FloatField("Mouse Sensitivity X", pc.mouseSensitivityX);
            pc.mouseSensitivityY = EditorGUILayout.FloatField("Mouse Sensitivity Y", pc.mouseSensitivityY);

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showMovement = EditorGUILayout.Foldout(showMovement, "Movement", true);
        PlayerPrefs.SetInt(nameof(showMovement), Convert.ToInt32(showMovement));
            EditorGUI.indentLevel++;
        
        if (showMovement)
        {
            showWASD = EditorGUILayout.Foldout(showWASD, "WASD", true);
            PlayerPrefs.SetInt(nameof(showWASD), Convert.ToInt32(showWASD));
            if (showWASD)
            {
                pc.canMove = EditorGUILayout.ToggleLeft("Can Move", pc.canMove);
                pc.forwardSpeed = EditorGUILayout.FloatField("Forward Speed", pc.forwardSpeed);
                pc.sidewaysSpeed = EditorGUILayout.FloatField("Sideways Speed", pc.sidewaysSpeed);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showJumping = EditorGUILayout.Foldout(showJumping, "Jumping", true);
            PlayerPrefs.SetInt(nameof(showJumping), Convert.ToInt32(showJumping));

            if (showJumping)
            {
                pc.canJump = EditorGUILayout.ToggleLeft("Can Jump", pc.canJump);
                pc.jumpForce = EditorGUILayout.FloatField("Jump Force", pc.jumpForce);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showSliding = EditorGUILayout.Foldout(showSliding, "Sliding", true);
            PlayerPrefs.SetInt(nameof(showSliding), Convert.ToInt32(showSliding));

            if (showSliding)
            {
                pc.canSlide = EditorGUILayout.ToggleLeft("Can Slide", pc.canSlide);
                pc.slideForce = EditorGUILayout.FloatField("Slide Force", pc.slideForce);
                slideReductionIncrement.floatValue = EditorGUILayout.FloatField("Slide Reduction Increment", slideReductionIncrement.floatValue);
                slideReductionIncrementTime.floatValue = EditorGUILayout.FloatField("Slide Reduction Increment Time", slideReductionIncrementTime.floatValue);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showDash = EditorGUILayout.Foldout(showDash, "Dashing", true);
            PlayerPrefs.SetInt(nameof(showDash), Convert.ToInt32(showDash));

            if (showDash)
            {
                pc.canDash = EditorGUILayout.ToggleLeft("Can Dash", pc.canDash);
                pc.dashForce = EditorGUILayout.FloatField("Dash Force", pc.dashForce);
                dashReductionIncrement.floatValue = EditorGUILayout.FloatField("Dash Reduction Increment", dashReductionIncrement.floatValue);
                dashReductionIncrementTime.floatValue = EditorGUILayout.FloatField("Dash Reduction Increment Time", dashReductionIncrementTime.floatValue);
                dashDirectionChangeSpeed.floatValue = EditorGUILayout.FloatField("Dash Direction Change Speed", dashDirectionChangeSpeed.floatValue);
                pc.dashDirection = EditorGUILayout.Vector3Field("Dash Direction", pc.dashDirection);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showSlam = EditorGUILayout.Foldout(showSlam, "Slamming");
            PlayerPrefs.SetInt(nameof(showSlam), Convert.ToInt32(showSlam));
            
            if (showSlam)
            {
                pc.canSlam = EditorGUILayout.ToggleLeft("Can Slam", pc.canSlam);
                pc.slamJumpForce = EditorGUILayout.FloatField("Slam Jump Force", pc.slamJumpForce);
                pc.slamForce = EditorGUILayout.Vector3Field("Slam Force", pc.slamForce);

            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();



        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // base.OnInspectorGUI();

        EditorGUI.EndChangeCheck();
        serializedObject.ApplyModifiedProperties();
    }

}
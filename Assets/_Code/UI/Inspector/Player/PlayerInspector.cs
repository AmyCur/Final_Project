using PlayerStates;
using UnityEditor;
using UnityEngine.UIElements;

[CanEditMultipleObjects]
[CustomEditor(typeof(PlayerController))]
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


    SerializedProperty slideReductionIncrement;
    SerializedProperty slideReductionIncrementTime;

    SerializedProperty dashReductionIncrement;
    SerializedProperty dashReductionIncrementTime;
    SerializedProperty dashDirectionChangeSpeed;

    void OnEnable() {
        pc = target as PlayerController;

        slideReductionIncrement = serializedObject.FindProperty("slideReductionIncrement");
        slideReductionIncrementTime = serializedObject.FindProperty("slideReductionIncrementTime");
        dashReductionIncrement = serializedObject.FindProperty("dashReductionIncrement");
        dashReductionIncrementTime = serializedObject.FindProperty("dashReductionIncrementTime");
        dashDirectionChangeSpeed = serializedObject.FindProperty("dashDirectionChangeSpeed");
    }




    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();


        showState = EditorGUILayout.Foldout(showState, "State", true);
        if (showState) {
            pc.s = (state)EditorGUILayout.EnumPopup("State", pc.s);
            pc.adminState = (AdminState)EditorGUILayout.EnumPopup("Admin State", pc.adminState);
         
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showStats = EditorGUILayout.Foldout(showStats, "Stats", true);
        if (showStats) {
            pc.health = EditorGUILayout.FloatField("Health", pc.health);
            pc.defence = EditorGUILayout.FloatField("Defence", pc.defence);

            showStamina = EditorGUILayout.Foldout(showStamina, "Stamina", true);
            if (showStamina) {
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
        if (showCamera) {
            pc.mouseSensitivityX = EditorGUILayout.FloatField("Mouse Sensitivity X", pc.mouseSensitivityX);
            pc.mouseSensitivityY = EditorGUILayout.FloatField("Mouse Sensitivity Y", pc.mouseSensitivityY);
            
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showMovement = EditorGUILayout.Foldout(showMovement, "Movement", true);
        if (showMovement) {
            showWASD = EditorGUILayout.Foldout(showWASD, "WASD", true);
            if (showWASD) {
                pc.canMove = EditorGUILayout.ToggleLeft("Can Move", pc.canMove);
                pc.forwardSpeed = EditorGUILayout.FloatField("Forward Speed", pc.forwardSpeed);
                pc.sidewaysSpeed = EditorGUILayout.FloatField("Sideways Speed", pc.sidewaysSpeed);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showJumping = EditorGUILayout.Foldout(showJumping, "Jumping", true);
            if (showJumping) {
                pc.canJump = EditorGUILayout.ToggleLeft("Can Jump", pc.canJump);
                pc.jumpForce = EditorGUILayout.FloatField("Jump Force", pc.jumpForce);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showSliding = EditorGUILayout.Foldout(showSliding, "Sliding", true);
            if (showSliding) {
                pc.canSlide = EditorGUILayout.ToggleLeft("Can Slide", pc.canSlide);
                pc.slideForce = EditorGUILayout.FloatField("Slide Force", pc.slideForce);
                slideReductionIncrement.floatValue = EditorGUILayout.FloatField("Slide Reduction Increment", slideReductionIncrement.floatValue);
                slideReductionIncrementTime.floatValue = EditorGUILayout.FloatField("Slide Reduction Increment Time", slideReductionIncrementTime.floatValue);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showDash = EditorGUILayout.Foldout(showDash, "Dashing", true);
            if (showDash) {
                pc.canDash = EditorGUILayout.ToggleLeft("Can Dash", pc.canDash);
                pc.dashForce = EditorGUILayout.FloatField("Dash Force", pc.dashForce);
                dashReductionIncrement.floatValue = EditorGUILayout.FloatField("Dash Reduction Increment", dashReductionIncrement.floatValue);
                dashReductionIncrementTime.floatValue = EditorGUILayout.FloatField("Dash Reduction Increment Time", dashReductionIncrementTime.floatValue);
                dashDirectionChangeSpeed.floatValue = EditorGUILayout.FloatField("Dash Direction Change Speed", dashDirectionChangeSpeed.floatValue);
                pc.dashDirection = EditorGUILayout.Vector3Field("Dash Direction", pc.dashDirection);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showSlam = EditorGUILayout.Foldout(showSlam, "Slamming");
            if (showSlam) {
                pc.canSlam = EditorGUILayout.ToggleLeft("Can Slam", pc.canSlam);
                pc.slamJumpForce = EditorGUILayout.FloatField("Slam Jump Force", pc.slamJumpForce);
                pc.slamForce = EditorGUILayout.Vector3Field("Slam Force", pc.slamForce);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();



        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // base.OnInspectorGUI();

        EditorGUI.EndChangeCheck();
        serializedObject.ApplyModifiedProperties();
    }

}
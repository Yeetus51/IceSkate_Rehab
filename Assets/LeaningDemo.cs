using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using com.rfilkov.kinect;
using TMPro;

public class LeaningDemo : MonoBehaviour, GestureListenerInterface
{
    public bool isTrackingPlayer = false;
    public UnityEvent OnUserLost;
    public UnityEvent OnUserDetected;

    [SerializeField] private int playerIndex = 0;
    [SerializeField] private TextMeshProUGUI _progressText = null;

    private KinectGestureManager _gestureManager;

    private float _leanFactor = 0; // negative is leaning to left, positive to right
    private float _horizontalMoveFactor = 0; // negative is moving to left, positive to right
    private float _headPosition = 0;
    private float _leftShoulder = 0;
    private float _rightShoulder = 0;

    private bool _playerInView = false;

    private KinectManager _kinectManager;


    public float GetLeanFactor() => _leanFactor;
    public float GetBodyHorizontalPosition() => _horizontalMoveFactor;
    public float GetHeadPos() => _headPosition;
    public bool playerIsDetected() => _playerInView;
    public float GetLeftShoulder() => _leftShoulder;
    public float GetRightShoulder() => _rightShoulder;


    private void Start()
    {
        _kinectManager = FindObjectOfType<KinectManager>();
    }


    private void Update()
    {
        updatePositions();
    }


    public void UserDetected(ulong userId, int userIndex)
    {
        if (userIndex == playerIndex)
        {
            _playerInView = true;
            _gestureManager = KinectManager.Instance.gestureManager;

            _gestureManager.DetectGesture(userId, GestureType.LeanLeft);
            _gestureManager.DetectGesture(userId, GestureType.LeanRight);
            _gestureManager.DetectGesture(userId, GestureType.RaiseLeftHand);
            _gestureManager.DetectGesture(userId, GestureType.RaiseRightHand);
            OnUserDetected?.Invoke();
        }
    }


    public void UserLost(ulong userId, int userIndex)
    {
        if (userIndex == playerIndex)
        {
            OnUserLost?.Invoke();
            _playerInView = false;
        }
    }


    public void GestureInProgress(ulong userId, int userIndex, GestureType gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (userIndex != playerIndex) return;

        // Lean
        if (gesture == GestureType.LeanLeft || gesture == GestureType.LeanRight)
        {
            if (gesture == GestureType.LeanLeft) _leanFactor = screenPos.z;
            else _leanFactor = -screenPos.z;
        }
        else
        {
            _leanFactor = 0;
        }
    }


    public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (userIndex != playerIndex) return false;
        return true;
    }


    public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint)
    {
        if (userIndex != playerIndex) return false;
        return true;
    }


    private void updatePositions()
    {
        ulong userID = _kinectManager.GetUserIdByIndex(playerIndex);

        isTrackingPlayer = _kinectManager.IsUserTracked(userID);
        Vector3 pelvisPos = _kinectManager.GetJointPosition(userID, KinectInterop.JointType.Pelvis);
        _horizontalMoveFactor = pelvisPos.x;

        Vector3 headPos = _kinectManager.GetJointPosition(userID, KinectInterop.JointType.Head);
        _headPosition = headPos.y;

        Vector3 leftShoulder = _kinectManager.GetJointOrientation(userID, KinectInterop.JointType.ShoulderLeft, false).eulerAngles;
        Vector3 rightShoulder = _kinectManager.GetJointOrientation(userID, KinectInterop.JointType.ShoulderRight, false).eulerAngles;

        _rightShoulder = rightShoulder.z;
        _leftShoulder = leftShoulder.z;


        _progressText.text =
            "pelvis:" + _horizontalMoveFactor.ToString() + "\n" +
            "head: " + _headPosition.ToString() + "\n" +
            "L Shoulder: " + leftShoulder + "\n" +
            "R Shoulder: " + rightShoulder + "\n" +
            "Lean: " + _leanFactor + "\n"; // negative is leaning to left, positive to right
    }
}


using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TripodController : UdonSharpBehaviour
{
    [Header("References")]
    [Tooltip("The pickup component from this object")]
    [SerializeField] private VRC_Pickup pickupHandTarget; 
    
    [Tooltip("The transform that will be rotated to point at the pickup. This is usually the camera pivot on the tripod")]
    [SerializeField] private Transform cameraPivot;   

    [Tooltip("The transform that will be rotated")]
    [SerializeField] private Transform gripResetPoint;

    [SerializeField] private CameraComponent cameraComponent;

    [Header("Settings")]
    [Tooltip("Smoothing factor for syncing to prevent jitter")]
    [SerializeField] private float lerpSpeed = 15f;       

    [Tooltip("Rotation offset if needed")]
    [SerializeField] private Vector3 rotationOffsetEuler = new Vector3(0, -90, 0); //90 by default because it's sideways otherwise


    //Networked Rotation
    [UdonSynced] private Quaternion syncedRotation;

    private bool isHeld = false;
    private Rigidbody pickupRigidbody;
    private Quaternion rotationCorrection;

    private void SetOwnerIfNotOwnerYet(GameObject gameObject)
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }

    void Start()
    {
        if (cameraPivot == null) cameraPivot = transform;
        syncedRotation = cameraPivot.rotation;

        if (pickupHandTarget != null)
        {
            pickupRigidbody = pickupHandTarget.GetComponent<Rigidbody>();
        }

        rotationCorrection = Quaternion.Euler(rotationOffsetEuler);
    }

    void LateUpdate()
    {
        if (Networking.IsOwner(gameObject))
        {
            if (isHeld && pickupHandTarget != null)
            {
                Vector3 targetDirection = pickupHandTarget.transform.position - cameraPivot.position;

                if (targetDirection != Vector3.zero)
                {
                    Quaternion rawLookRotation = Quaternion.LookRotation(targetDirection);
                    
                    Quaternion targetRotation = rawLookRotation * rotationCorrection;
                    
                    cameraPivot.rotation = targetRotation;

                    syncedRotation = targetRotation;
                    RequestSerialization();
                }
            }
            else if (!isHeld && gripResetPoint != null && pickupHandTarget != null)
            {
                pickupHandTarget.transform.position = gripResetPoint.position;
                pickupHandTarget.transform.rotation = gripResetPoint.rotation;
            }
        }
        else
        {
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, syncedRotation, Time.deltaTime * lerpSpeed);
            
            if (!isHeld && gripResetPoint != null && pickupHandTarget != null)
            {
                pickupHandTarget.transform.position = gripResetPoint.position;
                pickupHandTarget.transform.rotation = gripResetPoint.rotation;
            }
        }
    }

    public override void OnPickup()
    {
        SetOwnerIfNotOwnerYet(gameObject);
        cameraComponent.OnPickup();
        isHeld = true;
    }

    public override void OnDrop()
    {
        isHeld = false;
        cameraComponent.OnDrop();

        if (pickupHandTarget != null && gripResetPoint != null)
        {
            SetOwnerIfNotOwnerYet(pickupHandTarget.gameObject);
            pickupHandTarget.transform.position = gripResetPoint.position;
            pickupHandTarget.transform.rotation = gripResetPoint.rotation;

            if (pickupRigidbody != null)
            {
                pickupRigidbody.velocity = Vector3.zero;
                pickupRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using PDollarGestureRecognizer;
using System.IO;
using Unity.Netcode;
using UnityEngine.Events;

public class MovementRecognizer : NetworkBehaviour
{
    public AudioSource drawingAudio;
    public AudioSource recognizedAudio;
    public Transform movementSource;

    public Transform chestSource;
    // public HandPoseDetector detector;
    public HandType hand;

    public enum HandType
    {
        Left,
        Right
    }

    public float newPositionThresholdDistance = 0.001f;
    public GameObject debugCubePrefab;
    public bool creationMode = false;
    public string newGestureName;

    public float recognitionThreshold = 0.9f;

    public float recognitionDelay = 1.5f;
    private float timer = 0;
    
    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    public List<GameObject> spells;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();
    private int strokeID = 0;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));


        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var item in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
        bool isPressed = false;

        // if (detectedPose != null && detectedPose.name == "Point")
        // {
        //     isPressed = true;
        //     Debug.Log(detectedPose.name);
        // }
        
        if (hand == HandType.Left)
        {
            float leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            if (leftTriggerValue > 0.5f)
            {
                isPressed = true;
            }
            if (isPressed)
            {
                if (!drawingAudio.isPlaying)
                {
                    drawingAudio.Play();
                }
            }
            else
            {
                if (drawingAudio.isPlaying)
                {
                    drawingAudio.Stop();
                }
            }
        }
        else if (hand == HandType.Right)
        {
            float rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            if (rightTriggerValue > 0.5f)
            {
                isPressed = true;
            }
            if (isPressed)
            {
                if (!drawingAudio.isPlaying)
                {
                    drawingAudio.Play();
                }
            }
            else
            {
                if (drawingAudio.isPlaying)
                {
                    drawingAudio.Stop();
                }
            }
        }

        

        //Start The Movement
        if (!isMoving && isPressed)
        {
            strokeID = 0;
            StartMovement();
        }
        //Ending The Movement
        else if(isMoving && !isPressed)
        {
            timer += Time.deltaTime;
            if (timer > recognitionDelay)
                EndMovement();
        }
        //Updating The Movement
        else if(isMoving && isPressed)
        {
            if(timer > 0)
            {
                strokeID++;
            }

            timer = 0;
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        Debug.Log("Start Movement");
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);


        if (debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity),3);
    }

    void EndMovement()
    {
        Debug.Log("End Movement");
        isMoving = false;

        //Create The Gesture FRom The Position List
        Point[] pointArray = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        //Add A new gesture to training set
        if(creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        //recognize
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log("RECOGNIZED GESTURE: " + result.GestureClass + " " + result.Score);
            if(result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    void UpdateMovement()
    {
        Debug.Log("Update Movement");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];

        if(Vector3.Distance(movementSource.position,lastPosition) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);
            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void InstantiateSpellServerRpc(int spelli, Vector3 pos, Quaternion rot)
    {
        
        // Instantiate and spawn logic
        GameObject spellInstance = Instantiate(spells[spelli], pos, rot);
        NetworkObject networkObject = spellInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    
    }
    private void InstantiateSpell(GameObject spell, Vector3 pos, Quaternion rot)
    {
        
        // Instantiate and spawn logic
        GameObject spellInstance = Instantiate(spell, pos, rot);
        NetworkObject networkObject = spellInstance.GetComponent<NetworkObject>();
        Debug.Log(spellInstance.name + " " + spellInstance.transform.position + " " + (networkObject == null) + " ");
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("NetworkObject component is missing on the instantiated spell instance.");
        }
    
    }

    public void FireBallSpawn()
    {
        Vector3 spawnPosition = chestSource.forward * 0.3f + chestSource.transform.position;
        if (!IsServer)
        {
            Debug.Log("Client is sending RequestSpellSpawnServerRpc.");
            InstantiateSpellServerRpc(0, spawnPosition, chestSource.rotation);
        }
        else
        {
            Debug.Log("Server is executing FireBallSpawnClientRpc.");
            InstantiateSpell(spells[0], spawnPosition, chestSource.rotation);
        }
        
    }
  
    
    public void LightSpawn()
    {
        Vector3 spawnPosition = chestSource.forward * 0.3f + chestSource.transform.position;
        if (!IsServer)
        {
            Debug.Log("Client is sending RequestSpellSpawnServerRpc.");
            InstantiateSpellServerRpc(1, spawnPosition, chestSource.rotation);
        }
        else
        {
            Debug.Log("Server is executing ZapSpawnClientRpc.");
            InstantiateSpell(spells[1], spawnPosition, chestSource.rotation);
        }
    }
    public void WallSpawn()
    {
        Vector3 spawnPosition = chestSource.forward * 0.3f + chestSource.transform.position;
        spawnPosition.y = spawnPosition.y / 2.4f;
        
        if (!IsServer)
        {
            Debug.Log("Client is sending RequestSpellSpawnServerRpc.");
            InstantiateSpellServerRpc(2, spawnPosition, chestSource.rotation);
        }
        else
        {
            Debug.Log("Server is executing WallBallSpawnClientRpc.");
            InstantiateSpell(spells[2], spawnPosition, chestSource.rotation);
        }
    }
    public void SpawnSpell(string gestureName)
    {
        Vector3 spawnPosition = chestSource.forward * 0.3f + chestSource.transform.position;
        if (gestureName == "F")
        {
            recognizedAudio.Play();
            FireBallSpawn();
        } else if (gestureName == "L")
        {
            recognizedAudio.Play();
            LightSpawn();
        } else if (gestureName == "E")
        {
            recognizedAudio.Play();
            WallSpawn();
        } else if (gestureName == "N")
        {
            
        }
    }
}

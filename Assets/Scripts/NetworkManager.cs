using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public Canvas canvas;
    public SocketIOComponent socket;
    public InputField playerNameInput;
    public GameObject player;
    public GameObject startMenuCamera;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("other player head", OnOtherPlayerHead);
        socket.On("other player right hand", OnOtherPlayerRightHand);
        socket.On("other player left hand", OnOtherPlayerLeftHand);
        socket.On("play", OnPlay);
        socket.On("head move", OnHeadMove);
        socket.On("head turn", OnHeadTurn);
        socket.On("player move", OnPlayerMove);
        socket.On("player turn", OnPlayerTurn);
        socket.On("right hand move", OnRightHandMove);
        socket.On("right hand turn", OnRightHandTurn);
        socket.On("left hand move", OnLeftHandMove);
        socket.On("left hand turn", OnLeftHandTurn);
        socket.On("health", OnHealth);
        socket.On("other player disconnected", OnOtherPlayerDisconnected);

        // 名前入力を省いて入室
        StartCoroutine(ConnectToServer());
    }

    public void JoinGame()
    {
        StartCoroutine(ConnectToServer());
    }

    #region Commands

    // プレイヤー接続
    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("player connect");

        yield return new WaitForSeconds(1f);

        string playerName = "desktop";
        Debug.Log("Input name : " + playerName);
        PlayerSpawner ps = GetComponent<PlayerSpawner>();
        SpawnPoint playerSpawnPoint = ps.playerSpawnPoint;
        Vector3 playerSpawnPosition = playerSpawnPoint.spawnPosition;
        Quaternion playerSpawnRotation = playerSpawnPoint.spawnRotation;
        GameObject startMenuCamera = GetComponent<GameObject>();

        PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPosition, playerSpawnRotation);
        string data = JsonUtility.ToJson(playerJSON);

        Debug.Log("playerJSON : " + data);
        socket.Emit("play", new JSONObject(data));
        Debug.Log("Emit : play");

        canvas.gameObject.SetActive(false);

    }

    // "Command ~~" : プレイヤーやオブジェクトの操作情報をサーバー側に送る
    public void CommandMove(Vector3 vec3)
    {
        string data = JsonUtility.ToJson(new PositionJSON(vec3));
        socket.Emit("player move", new JSONObject(data));
    }

    public void CommandTurn(Quaternion quat)
    {
        string data = JsonUtility.ToJson(new RotationJSON(quat));
        socket.Emit("player turn", new JSONObject(data));
    }

    public void CommandHeadMove(Vector3 vec3)
    {
        string data = JsonUtility.ToJson(new HeadPositionJSON(vec3));
        socket.Emit("head move", new JSONObject(data));
    }

    public void CommandHeadTurn(Quaternion quat)
    {
        string data = JsonUtility.ToJson(new HeadRotationJSON(quat));
        socket.Emit("head turn", new JSONObject(data));
    }

    public void CommandRightHandMove(Vector3 vec3)
    {
        string data = JsonUtility.ToJson(new RightHandPositionJSON(vec3));
        socket.Emit("right hand move", new JSONObject(data));
    }

    public void CommandRightHandTurn(Quaternion quat)
    {
        string data = JsonUtility.ToJson(new RightHandRotationJSON(quat));
        socket.Emit("right hand turn", new JSONObject(data));
    }

    public void CommandLeftHandMove(Vector3 vec3)
    {
        string data = JsonUtility.ToJson(new LeftHandPositionJSON(vec3));
        socket.Emit("left hand move", new JSONObject(data));
    }

    public void CommandLeftHandTurn(Quaternion quat)
    {
        string data = JsonUtility.ToJson(new LeftHandRotationJSON(quat));
        socket.Emit("left hand turn", new JSONObject(data));
    }
    
    #endregion

    #region Listening

    void OnOtherPlayerConnected(SocketIOEvent socketIOEvent)
    {
        print("Someone else Joined ");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Debug.Log("userJSON is ganerated : " + data);
        Vector3 position = new Vector3(userJSON.playerPosition[0], userJSON.playerPosition[1], userJSON.playerPosition[2]);
        Quaternion rotation = Quaternion.Euler(userJSON.playerRotation[0], userJSON.playerRotation[1], userJSON.playerRotation[2]);
        GameObject o = GameObject.Find(userJSON.name) as GameObject;
        Debug.Log("O is : " + o);
        if (o != null)
        {
            return;
        }
        Debug.Log("Again O is : " + o);
        GameObject p = Instantiate(player, position, rotation) as GameObject;
        Debug.Log(userJSON.name + " :  body is generated ");
        GameObject EyeCamera = p.transform.Find("Other Head Avator").gameObject;
        EyeCamera.gameObject.SetActive(true);
        GameObject OtherRightHand = p.transform.Find("Other R Hand").gameObject;
        OtherRightHand.gameObject.SetActive(true);
        GameObject OtherLeftHand = p.transform.Find("Other L Hand").gameObject;
        OtherLeftHand.gameObject.SetActive(true);
        MultiPlayerController pc = p.GetComponent<MultiPlayerController>();
        Transform t = p.transform.Find("Healthbar Canvas");
        Transform t1 = t.transform.Find("Player Name");
        Text playerName = t1.GetComponent<Text>();
        playerName.text = userJSON.name;
        pc.isLocalPlayer = false;
        p.name = userJSON.name;
        Debug.Log("Joining player name is : " + p.name);
        Health h = p.GetComponent<Health>();
        h.currentHealth = userJSON.health;
        h.OnChangeHealth();

    }

    void OnOtherPlayerHead(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        HeadJSON headJSON = HeadJSON.CreateFromJSON(data);
        Vector3 headPosition = new Vector3(headJSON.headPosition[0], headJSON.headPosition[1], headJSON.headPosition[2]);
        Quaternion headRotation = Quaternion.Euler(headJSON.headRotation[0], headJSON.headRotation[1], headJSON.headRotation[2]);
        GameObject o = GameObject.Find(headJSON.name) as GameObject;
        if (o != null)
        {
            return;
        }
        GameObject p = GameObject.Find(headJSON.name) as GameObject;
        GameObject EyeCamera = p.transform.Find("Other Head Avator").gameObject;
        GameObject ec = Instantiate(EyeCamera, headPosition, headRotation) as GameObject;
        ec.transform.parent = p.transform;
        p.name = headJSON.name;
        EyeCamera.gameObject.SetActive(false);
    }

    void OnOtherPlayerRightHand(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        RightHandJSON rightHandJSON = RightHandJSON.CreateFromJSON(data);
        Vector3 rightHandPosition = new Vector3(rightHandJSON.rightHandPosition[0], rightHandJSON.rightHandPosition[1], rightHandJSON.rightHandPosition[2]);
        Quaternion rightHandRotation = Quaternion.Euler(rightHandJSON.rightHandRotation[0], rightHandJSON.rightHandRotation[1], rightHandJSON.rightHandRotation[2]);
        GameObject o = GameObject.Find(rightHandJSON.name) as GameObject;
        if (o != null)
        {
            return;
        }
        GameObject p = GameObject.Find(rightHandJSON.name) as GameObject;
        GameObject OtherRightHand = p.transform.Find("Other R Hand").gameObject;
        GameObject orh = Instantiate(OtherRightHand, rightHandPosition, rightHandRotation) as GameObject;
        orh.transform.parent = p.transform;
        p.name = rightHandJSON.name;
        OtherRightHand.gameObject.SetActive(false);

    }

    void OnOtherPlayerLeftHand(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        LeftHandJSON leftHandJSON = LeftHandJSON.CreateFromJSON(data);
        Vector3 leftHandPosition = new Vector3(leftHandJSON.leftHandPosition[0], leftHandJSON.leftHandPosition[1], leftHandJSON.leftHandPosition[2]);
        Quaternion leftHandRotation = Quaternion.Euler(leftHandJSON.leftHandRotation[0], leftHandJSON.leftHandRotation[1], leftHandJSON.leftHandRotation[2]);
        GameObject o = GameObject.Find(leftHandJSON.name) as GameObject;
        if (o != null)
        {
            return;
        }
        GameObject p = GameObject.Find(leftHandJSON.name) as GameObject;
        GameObject OtherLeftHand = p.transform.Find("Other L Hand").gameObject;
        GameObject olh = Instantiate(OtherLeftHand, leftHandPosition, leftHandRotation) as GameObject;
        olh.transform.parent = p.transform;
        p.name = leftHandJSON.name;
        OtherLeftHand.gameObject.SetActive(false);

    }
    
    void OnPlay(SocketIOEvent socketIOEvent)
    {
        print("you joined");
        startMenuCamera.gameObject.SetActive(false);
        string data = socketIOEvent.data.ToString();
        print("your JSON data was recieved");
        PlayerJSON currentUserJSON = PlayerJSON.CreateFromJSON(data);

        print("your JSON was created : " + data);
        Vector3 position = new Vector3(currentUserJSON.playerPosition[0], currentUserJSON.playerPosition[1], currentUserJSON.playerPosition[2]);
        print("your name isssssssssssssssss ");
        Quaternion rotation = Quaternion.Euler(currentUserJSON.playerRotation[0], currentUserJSON.playerRotation[1], currentUserJSON.playerRotation[2]);
        print("your name isssssssssssssssss ");
        GameObject p = Instantiate(player, position, rotation) as GameObject;
        MultiPlayerController pc = p.GetComponent<MultiPlayerController>();
        Transform t = p.transform.Find("Healthbar Canvas");
        Transform t1 = t.transform.Find("Player Name");
        Text playerName = t1.GetComponent<Text>();
        playerName.text = currentUserJSON.name;
        pc.isLocalPlayer = true;
        p.name = currentUserJSON.name;
        GameObject Eye = p.transform.Find("OVRCameraRig").gameObject;
        Eye.gameObject.SetActive(true);
        print("your name is " + currentUserJSON.name);
    }

    void OnHeadMove(SocketIOEvent socketIOEvent)
    {

        string data = socketIOEvent.data.ToString();
        HeadJSON headJSON = HeadJSON.CreateFromJSON(data);

        Vector3 headPosition = new Vector3(headJSON.headPosition[0], headJSON.headPosition[1], headJSON.headPosition[2]);
        if (headJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(headJSON.name) as GameObject;
        GameObject eye = p.transform.Find("Other Head Avator").gameObject;
        if (p != null)
        {
            eye.transform.position = headPosition;
        }

    }

    void OnHeadTurn(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        HeadJSON headJSON = HeadJSON.CreateFromJSON(data);
        Quaternion headRotation = Quaternion.Euler(headJSON.headRotation[0], headJSON.headRotation[1], headJSON.headRotation[2]);
        if (headJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(headJSON.name) as GameObject;
        GameObject eye = p.transform.Find("Other Head Avator").gameObject;
        if (p != null)
        {
            eye.transform.rotation = headRotation;
        }
    }

    void OnPlayerMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector3 playerPosition = new Vector3(userJSON.playerPosition[0], userJSON.playerPosition[1], userJSON.playerPosition[2]);
        if (userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.position = playerPosition;
        }
        Debug.Log(userJSON.name + " moved! ! !");

    }

    void OnPlayerTurn(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Quaternion playerRotation = Quaternion.Euler(userJSON.playerRotation[0], userJSON.playerRotation[1], userJSON.playerRotation[2]);
        if (userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.rotation = playerRotation;
        }
        Debug.Log(userJSON.name + " turned! ! !");
    }

    void OnRightHandMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        RightHandJSON rightHandJSON = RightHandJSON.CreateFromJSON(data);
        Vector3 rightHandPosition = new Vector3(rightHandJSON.rightHandPosition[0], rightHandJSON.rightHandPosition[1], rightHandJSON.rightHandPosition[2]);
        if (rightHandJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(rightHandJSON.name) as GameObject;
        GameObject orh = p.transform.Find("Other R Hand").gameObject;
        if (p != null)
        {
            orh.transform.position = rightHandPosition;
        }

    }

    void OnRightHandTurn(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        RightHandJSON rightHandJSON = RightHandJSON.CreateFromJSON(data);
        Quaternion rightHandRotation = Quaternion.Euler(rightHandJSON.rightHandRotation[0], rightHandJSON.rightHandRotation[1], rightHandJSON.rightHandRotation[2]);
        if (rightHandJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(rightHandJSON.name) as GameObject;
        GameObject orh = p.transform.Find("Other R Hand").gameObject;
        if (p != null)
        {
            orh.transform.rotation = rightHandRotation;
        }
    }

    void OnLeftHandMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        LeftHandJSON leftHandJSON = LeftHandJSON.CreateFromJSON(data);
        Vector3 leftHandPosition = new Vector3(leftHandJSON.leftHandPosition[0], leftHandJSON.leftHandPosition[1], leftHandJSON.leftHandPosition[2]);
        if (leftHandJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(leftHandJSON.name) as GameObject;
        GameObject olh = p.transform.Find("Other L Hand").gameObject;
        if (p != null)
        {
            olh.transform.position = leftHandPosition;
        }

    }

    void OnLeftHandTurn(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        LeftHandJSON leftHandJSON = LeftHandJSON.CreateFromJSON(data);
        Quaternion leftHandRotation = Quaternion.Euler(leftHandJSON.leftHandRotation[0], leftHandJSON.leftHandRotation[1], leftHandJSON.leftHandRotation[2]);
        if (leftHandJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(leftHandJSON.name) as GameObject;
        GameObject olh = p.transform.Find("Other L Hand").gameObject;
        if (p != null)
        {
            olh.transform.rotation = leftHandRotation;
        }
    }

    void OnHealth(SocketIOEvent socketIOEvent)
    {
        print("changing the health");
        string data = socketIOEvent.data.ToString();
        UserHealthJSON userHealthJSON = UserHealthJSON.CreateFromJSON(data);
        GameObject p = GameObject.Find(userHealthJSON.name);
        Health h = p.GetComponent<Health>();
        h.currentHealth = userHealthJSON.health;
        h.OnChangeHealth();
    }

    void OnOtherPlayerDisconnected(SocketIOEvent socketIOEvent)
    {
        print("user disconnected");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Destroy(GameObject.Find(userJSON.name));
    }

    #endregion

    #region JSONMessageClasses

    [Serializable]
    public class PlayerJSON
    {
        public string name;
        public float[] playerPosition;
        public float[] playerRotation;

        public static PlayerJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<PlayerJSON>(data);
        }

        public PlayerJSON(string _name, Vector3 _playerSpawnPosition, Quaternion _playerSpawnRotation)
        {
            name = _name;
            playerPosition = new float[] { _playerSpawnPosition.x, _playerSpawnPosition.y, _playerSpawnPosition.z };
            playerRotation = new float[] { _playerSpawnRotation.eulerAngles.x, _playerSpawnRotation.eulerAngles.y, _playerSpawnRotation.eulerAngles.z };
        }
    }

    [Serializable]
    public class PointJSON
    {
        public float[] position;
        public float[] rotation;
        public PointJSON(SpawnPoint spawnPoint)
        {
            position = new float[]
            {
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y,
                spawnPoint.transform.position.z
            };

            rotation = new float[]
            {
                spawnPoint.transform.eulerAngles.x,
                spawnPoint.transform.eulerAngles.y,
                spawnPoint.transform.eulerAngles.z
            };
        }
    }
    


    [Serializable]
    public class HeadJSON
    {
        public string name;
        public float[] headPosition;
        public float[] headRotation;

        public static HeadJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<HeadJSON>(data);
        }
    }

    [Serializable]
    public class HeadPositionJSON
    {
        public float[] headPosition;

        public HeadPositionJSON(Vector3 _headPosition)
        {
            headPosition = new float[] { _headPosition.x, _headPosition.y, _headPosition.z };
        }
    }

    [Serializable]
    public class HeadRotationJSON
    {
        public float[] headRotation;

        public HeadRotationJSON(Quaternion _headRotation)
        {
            headRotation = new float[] { _headRotation.eulerAngles.x, _headRotation.eulerAngles.y, _headRotation.eulerAngles.z };
        }
    }

    [Serializable]
    public class PositionJSON
    {
        public float[] playerPosition;

        public PositionJSON(Vector3 _position)
        {
            playerPosition = new float[] { _position.x, _position.y, _position.z };
        }
    }

    [Serializable]
    public class RotationJSON
    {
        public float[] playerRotation;

        public RotationJSON(Quaternion _rotation)
        {
            playerRotation = new float[] { _rotation.eulerAngles.x, _rotation.eulerAngles.y, _rotation.eulerAngles.z };
        }
    }

    [Serializable]
    public class RightHandJSON
    {
        public string name;
        public float[] rightHandPosition;
        public float[] rightHandRotation;

        public static RightHandJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<RightHandJSON>(data);
        }
    }

    [Serializable]
    public class RightHandPositionJSON
    {
        public float[] rightHandPosition;

        public RightHandPositionJSON(Vector3 _rightHandPosition)
        {
            rightHandPosition = new float[] { _rightHandPosition.x, _rightHandPosition.y, _rightHandPosition.z };
        }
    }

    [Serializable]
    public class RightHandRotationJSON
    {
        public float[] rightHandRotation;

        public RightHandRotationJSON(Quaternion _rightHandRotation)
        {
            rightHandRotation = new float[] { _rightHandRotation.eulerAngles.x, _rightHandRotation.eulerAngles.y, _rightHandRotation.eulerAngles.z };
        }
    }

    [Serializable]
    public class LeftHandJSON
    {
        public string name;
        public float[] leftHandPosition;
        public float[] leftHandRotation;

        public static LeftHandJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<LeftHandJSON>(data);
        }
    }

    [Serializable]
    public class LeftHandPositionJSON
    {
        public float[] leftHandPosition;

        public LeftHandPositionJSON(Vector3 _leftHandPosition)
        {
            leftHandPosition = new float[] { _leftHandPosition.x, _leftHandPosition.y, _leftHandPosition.z };
        }
    }

    [Serializable]
    public class LeftHandRotationJSON
    {
        public float[] leftHandRotation;

        public LeftHandRotationJSON(Quaternion _leftHandRotation)
        {
            leftHandRotation = new float[] { _leftHandRotation.eulerAngles.x, _leftHandRotation.eulerAngles.y, _leftHandRotation.eulerAngles.z };
        }
    }

    [Serializable]
    public class UserJSON
    {
        public string name;
        public float[] playerPosition;
        public float[] playerRotation;
        public int health;

        public static UserJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
        }
    }

    [Serializable]
    public class HealthChangeJSON
    {
        public string name;
        public int healthChange;
        public string from;
        public bool isEnemy;

        public HealthChangeJSON(string _name, int _healthChange, string _from, bool _isEnemy)
        {
            name = _name;
            healthChange = _healthChange;
            from = _from;
            isEnemy = _isEnemy;
        }
    }

    [Serializable]
    public class UserHealthJSON
    {
        public string name;
        public int health;

        public static UserHealthJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserHealthJSON>(data);
        }
    }

    #endregion
}

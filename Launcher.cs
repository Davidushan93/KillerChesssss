using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.Davidushan.KillerChess
{
    [System.Serializable]
    public class ProfileData
    {
        public string username;
        //CIAONE public int level;
        //CIAONE public int xp;

        public ProfileData()
        {
            this.username = "";
            //CIAONE this.level = 1;
            //CIAONE this.xp = 0;
        }

        public ProfileData(string u)//CIAONE,int l, int x)
        {
            this.username = u;
            //CIAONE this.level = l;
            //CIAONE this.xp = x;
        }
    }

    public class Launcher : MonoBehaviourPunCallbacks
    {
        public InputField usernameField;
        public InputField roomnameField; 
        public Slider maxPlayersSlider; 
        public Text maxPlayersValue; 
        public static ProfileData myProfile = new ProfileData();

        public GameObject tabMain;
        public GameObject tabRooms;
        public GameObject tabCreate; 

        public GameObject buttonRoom;

        private List<RoomInfo> roomList;

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            myProfile = Data.LoadProfile();
            if (!string.IsNullOrEmpty(myProfile.username))
            {
                usernameField.text = myProfile.username;
            }

            Connect();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("CONNESSO!");

            PhotonNetwork.JoinLobby();
            base.OnConnectedToMaster();
        }

        public override void OnJoinedRoom()
        {
            StartGame();

            base.OnJoinedRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Create();

            base.OnJoinRandomFailed(returnCode, message);
        }

        public void Connect ()
        {
            Debug.Log("Tentativo di connessione...");
            PhotonNetwork.GameVersion = "Alfa";
            PhotonNetwork.ConnectUsingSettings();
        } 

        public void Join ()
        {
            PhotonNetwork.JoinRandomRoom();
        } 

        public void Create ()
        {
            RoomOptions options = new RoomOptions(); 
            options.MaxPlayers = (byte) maxPlayersSlider.value; 

            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable(); 
            properties.Add("map", 0); 
            options.CustomRoomProperties = properties; 

            PhotonNetwork.CreateRoom(roomnameField.text, options); //SE NON METTO NESSUN NOME ALLA PARTITA, IL SERVER MI METTE QUALCOSA IN AUTOMATICO
        }

        public void ChangeMap () 
        {

        }

        public void ChangeMaxPlayersSlider (float t_value) 
        {
            maxPlayersValue.text = Mathf.RoundToInt(t_value).ToString();
        }

        public void TabCloseAll()
        {
            tabMain.SetActive(false);
            tabRooms.SetActive(false);
            tabCreate.SetActive(false); 
        }

        public void TabOpenMain ()
        {
            TabCloseAll();
            tabMain.SetActive(true);
        }

        public void TabOpenRooms ()
        {
            TabCloseAll();
            tabRooms.SetActive(true);
        }

        public void TabOpenCreate () 
        {
            TabCloseAll();
            tabCreate.SetActive(true);
        }

        private void ClearRoomList ()
        {
            Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");
            foreach (Transform a in content) Destroy(a.gameObject);
        }

        private void VerifyUsername ()
        {
            if (string.IsNullOrEmpty(usernameField.text))
            {
                myProfile.username = "SONO_PIRLA_" + Random.Range(100, 1000);
            }
            else
            {
                myProfile.username = usernameField.text;
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> p_list)
        {
            roomList = p_list;

            ClearRoomList();

            Debug.Log("Caricate le stanze disponibili");

            Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");

            foreach (RoomInfo a in roomList)
            {
                GameObject newRoomButton = Instantiate(buttonRoom, content) as GameObject;

                newRoomButton.transform.Find("Name").GetComponent<Text>().text = a.Name;
                newRoomButton.transform.Find("Players").GetComponent<Text>().text = a.PlayerCount + " / " + a.MaxPlayers;

                newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
            }

            base.OnRoomListUpdate(roomList);
        }

        public void JoinRoom (Transform p_button)
        {
            Debug.Log("Entro nella stanza");

            string t_roomName = p_button.transform.Find("Name").GetComponent<Text>().text;

            VerifyUsername();

            PhotonNetwork.JoinRoom(t_roomName);
        }

        public void StartGame ()
        {
            VerifyUsername();

            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Data.SaveProfile(myProfile);
                PhotonNetwork.LoadLevel(1);
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Globals : MonoBehaviour {
	
	
	//Timers
	public static int WARNING_BEFORE_EVENT_SECONDS = 5;
	public static int WARNING_MESSAGE_TIMER = 3;
	public static int SATISFACTION_VEHICLES_DECREMENTER = 90;
	public static int EMPTY_STREET_BEFORE_THIEF_TIMER = 10;
	public static int EMPTY_STREET_AFTER_THIEF_TIMER = 7;
	
	//booleans
	public static bool vibrationEnabled;
	
	//dimentions
	public static float STREET_WIDTH = 23;
	public static int ANGER_TIMER = 10;
	
	//speeds
	public static float starting_normal_avg_speed = 25;
	
	public static float AMBULANCE_SPEED = 36;
	public static float BUS_SPEED = 12;
	public static float CARAVAN_SPEED = 10;
	public static float SERVICE_CAR_SPEED = 10;
	public static float TAXI_SPEED = 25;
	public static float THIEF_SPEED = 35;
	public static float POLICE_SPEED = 37;
	public static float ACCIDENT_SPEED = 36;
	
	public static int speed_increase_rate = 3;
	
	//ranges
	public static float RAY_CAST_RANGE = 10;
	public static float ACCELERATE_FORWARD_RANGE = 10;
	
	//anger data
	public static int	angerMinTime = 10;
	public static float angerMinAmount = .5f;
	public static int nextAngerIncreaseRate = 2;
	
	//audios
	public static List<AudioClip> Horns;
	public List<AudioClip> HornsGos;
	
	public AudioClip humanAnger;
	public static AudioClip humanAngerCalled;
	
	
	//waypoints
	public List<GameObject> busStoppersGos;
	public static List<GameObject> busStoppers;
	
	
	
	// Messages Bar Texts
	public static string AMBULANCE_WARNING_MSG = "Ambulance is Coming from the east";
	public static string BUS_WARNING_MSG = "Bus is Coming from the west";
	public static string SERVICE_CAR_WARNING_MSG = "Service Car is Coming from the west";
	public static string THIEF_WARNING_MSG = "A Thief is coming from the west";
	public static string POLICE_WARNING_MSG = "help the police to catch him";
	public static string CARAVAN_WARNING_MSG = "Caravan is Coming";
	public static string TAXI_WARNING_MSG = "Taxi is coming from the north";

	public static string ANGERY_CAR_MESSAGE = "Some car is going to be angery";
	public static string ANGERY_HUMAN_MESSAGE = "Some pedestrian is going to be angery";
	public static string SATISTFY_BAR_MSG_1 = "The Mayor is proud of you!";
	public static string SATISTFY_BAR_MSG_2 = "Attention! the mayor is going to be angery!!";
	public static string SATISTFY_BAR_MSG_3 = "The Mayor is angery!!";
	public static string SATISTFY_BAR_MSG_4 = "What the hell are you doing??!!";
	public static string SATISTFY_BAR_MSG_5 = "You are about to lose !!";
	
	void Awake(){
		
	}
	
	
	// Use this for initialization
	void Start () {
		Horns = HornsGos;
		busStoppers = busStoppersGos;
		//vibrationEnabled = false;
		
		humanAngerCalled = humanAnger;
		
		
	}
	
	
	// Update is called once per frame
	void Update () {
	//	Debug.Log(vibrationEnabled);
	}
}

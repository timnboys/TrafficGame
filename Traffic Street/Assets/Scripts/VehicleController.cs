using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class VehicleController : MonoBehaviour {
	
	private int curScore;
 
	public Vehicle myVehicle;
	//for the street
	private Street _street;
	private GamePath _path;
	private TrafficLight _light;
	private float _stopPosition;
	private Vector3 _endPosition;
	private Queue _myQueue;
	private int _queueSize;
	//for the vehicle
	
	private StreetDirection lastDirection;
	private StreetDirection _direction;
	private StreetDirection _nextDirection;
	public float speed;
	private float _size;
	public VehicleType vehType;
	
	//private CharacterController _charController ;
	private BoxCollider boxColl;	
	
	
	
	private bool insideOnTriggerEnter; 
	
	private bool passed;
	private bool dequeued;
	private bool enqueued;
	private bool gameOver;
	
	public bool thereIsAbus;
	public bool haveToReduceMySpeed;
	
	private bool satisfyAdjustedOnTime;
	
	private float Offset;
	private int carsStillInsideNumber;
	
	private GameMaster gameMasterScript;
	private float stoppingTimerforAnger;
	private bool stoppingTimerforAngerSet;
	
	private List<int> serviceCarStops; 
	
	public float busStopTimer;
	private float serviceCarStopTimer;
	
	private bool transfered;
	private bool onIncrease;
	
	private Vector3 mainColliderSize;
	
	//public bool enableRayCast;
		
	
	// Use this for initialization
	void Awake(){
		 
		initInstancesAtFirst();
		
		
	}
	
	public void initInstancesAtFirst(){
		gameMasterScript = GameObject.FindGameObjectWithTag("master").GetComponent<GameMaster>();
		boxColl = GetComponent<BoxCollider>();
		
		mainColliderSize = boxColl.size;
		
		Offset = 0;
		dequeued = false;
		enqueued = false;
		insideOnTriggerEnter = false;
		passed = false;
		satisfyAdjustedOnTime = false;
		stoppingTimerforAnger = 0;
		stoppingTimerforAngerSet = false;
		thereIsAbus = false;
		haveToReduceMySpeed = false;
		busStopTimer = 0;
		serviceCarStopTimer = 0;
		transfered = false;
		onIncrease = false;
		
	}
	
	void Start () {
		InitStreetAndVehicleAttributes();
		
	}
	
	public void InitStreetAndVehicleAttributes(){
		_path			= myVehicle.MyPath;		
		_street 		= myVehicle.CurrentStreet;
		_direction 		= myVehicle.CurrentDirection;
		speed 			= myVehicle.Speed;
		_size 			= myVehicle.Size;
		vehType			= myVehicle.Type;
		
		_light 			= _street.StreetLight;
		_stopPosition 	= _street.StopPosition;
		_endPosition 	= _street.EndPoint;
		_myQueue 		= _street.StrQueue;
		_queueSize 		= _myQueue.Count;
		
		_nextDirection = myVehicle.NextStreet.StreetLight.Type;
		
		if(vehType == VehicleType.ServiceCar){
			serviceCarStops = ServiceCar.SetGetServiceCarRandomStops(gameMasterScript.gameTime -6, gameMasterScript.gameTime-9);
		}
		
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
		//SetupColliderSize();
		
		PerformEnqueue();
		SetStopOffset();
		CheckPosition_DeqIfPassed();
		
		Move();
		
		if(speed == 0){
			if(CheckIfColliderIsOriginal()){
				//ChangeColliderSize();
			}
		}
		
		Debug.LogWarning("myVehicle.NextStreet == " + myVehicle.NextStreet);
		if(myVehicle.NextStreet!=null && MathsCalculatios.IsLeavingTheStreet(transform, _direction, _endPosition, _street)){
			TransfereToNextStreet();
		}
		CheckAndDeactivateAtEnd();
		if(!passed){
			//if(vehType != VehicleType.Thief)
				StopMovingOnRed();
		}
		CheckServiceCarStops();
		CheckMyAnger();
		
	//	ChangeColliderSize();
		
		if(busStopTimer >= gameMasterScript.gameTime){
			speed = myVehicle.Speed;
			//Debug.Log("haaaaaaaaaaaaaa");
		}
		
		if(!(_light.Stopped) && !haveToReduceMySpeed){ /////////////////////////////////////////////////
			speed = myVehicle.Speed;
			
		}
		
	}
	
	
	private void PerformEnqueue(){
		if(!enqueued ){
			
			_myQueue.Enqueue(gameObject);
			
			enqueued = true;
			//Debug.Log(gameObject.name +" is enqueued"  +"  and The queue Size ------------> " + _queueSize  );
		}
	}
	
	private void SetStopOffset(){
		Offset =  (GetMyOrderInQueue()) * (_size + 9);
	}
	
	private void CheckPosition_DeqIfPassed(){
		//Debug.Log(gameObject.name +" The queue Size ------------> " + _queueSize );

		if( _direction == StreetDirection.Right && transform.position.x >= _stopPosition    ||
			_direction == StreetDirection.Left && transform.position.x <= _stopPosition  ||
			_direction == StreetDirection.Down  && transform.position.z <= _stopPosition  ||
			_direction == StreetDirection.Up && transform.position.z >= _stopPosition ){
			
			
			passed = true ;
			if(!dequeued && (!(_light.Stopped)  || StillInStopRange()) ){
								
				if(_myQueue.Count > 0){
					
					_myQueue.Dequeue();
					dequeued = true;
					
					_street.VehiclesNumber --;
					if(!satisfyAdjustedOnTime && vehType == VehicleType.Ambulance){
						GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SatisfyBar>().AddjustSatisfaction(-1);
						gameMasterScript.satisfyBar --;
						satisfyAdjustedOnTime = true;
					}					
				}
			}
			boxColl.isTrigger = true;
		}
	}
	
	private bool StillInStopRange(){
		if(_light.Stopped){
			if( _direction == StreetDirection.Right && transform.position.x >= _stopPosition  +3  ||
			_direction == StreetDirection.Left && transform.position.x <= _stopPosition -5 ||
			_direction == StreetDirection.Down  && transform.position.z <= _stopPosition -5 ||
			_direction == StreetDirection.Up && transform.position.z >= _stopPosition +3 ){
			
				return true;
				
			}
			
			
		}
		return false;
	}
	
	private void TransfereToNextStreet(){
		ResetVehicleAttributes();
		/*
		if(_direction == StreetDirection.Left && transform.position.x <= _street.EndPoint.x){
			ResetVehicleAttributes();
			//Debug.Log("resetting attributes ... " + gameObject.name);
		}
		else if(_direction == StreetDirection.Right && transform.position.x >= _street.EndPoint.x){
			ResetVehicleAttributes();
			//Debug.Log("resetting attributes");
		}
		else if(_direction == StreetDirection.Down && transform.position.z <= _street.EndPoint.z){
			//Debug.Log("resetting attributes");
			ResetVehicleAttributes();
		}
		else if(_direction == StreetDirection.Up && transform.position.z >=_street.EndPoint.z){
			
			ResetVehicleAttributes();
			//Debug.Log("resetting attributes ... " + gameObject.name);
		}
		*/
	}
	
	private void ResetVehicleAttributes(){
			if(myVehicle.NextStreet != null){
			myVehicle.CurrentStreet = myVehicle.NextStreet;
			Debug.Log("transfering to the street ... " + myVehicle.CurrentStreet.ID);
			myVehicle.CurrentDirection = myVehicle.CurrentStreet.StreetLight.Type;
			myVehicle.CurrentStreetNumber ++;
			
			if(myVehicle.CurrentStreetNumber != _path.PathStreets.Count ){
				myVehicle.NextStreet =_path.PathStreets[myVehicle.CurrentStreetNumber];
				_nextDirection = myVehicle.NextStreet.StreetLight.Type;
			}
			else{
				myVehicle.NextStreet = null;
				Debug.Log("next street is null " );
			}
			
			_street = myVehicle.CurrentStreet; 
			_direction 		= myVehicle.CurrentDirection;			
			
			_street.VehiclesNumber++;
			/*
			if(_street.StreetLight.tLight == null){
				if(_street.VehiclesNumber == _street.StreetCapacity){
					
					_path.PathStreets[myVehicle.CurrentStreetNumber-1].StreetLight.Stopped = true;
				}
				else{
					Debug.Log("StreetLight.Stopped = false");
					_path.PathStreets[myVehicle.CurrentStreetNumber-1].StreetLight.Stopped = false;
				}
			}
			*/
			_light 			= _street.StreetLight;
			_stopPosition 	= _street.StopPosition;
			_endPosition 	= _street.EndPoint;
			_myQueue 		= _street.StrQueue;
			_queueSize 		= _street.StrQueue.Count;
			
			Offset = 0;
			dequeued = false;
			enqueued = false;
			insideOnTriggerEnter = false;
			passed = false;
			haveToReduceMySpeed = false;
		}
	}
	
	private void CheckAndDeactivateAtEnd(){
		if(myVehicle.NextStreet == null){
			
		if(MathsCalculatios.CheckMyEndPosition(transform, _direction, _endPosition)){
			gameObject.SetActive(false);
			if(vehType == VehicleType.Normal){
				gameMasterScript.existedVehicles.Enqueue(gameObject);
			}
			gameMasterScript.score += 1;
			
		}
		}
		
	}
	
	private void StopMovingOnRed(){
		SetStopOffset();
	/*	if(vehType == VehicleType.Bus){
			
		}
		*/
		
		//if(_light.tLight != null){
			if(_light.Stopped){
				if( (_direction == StreetDirection.Right && transform.position.x > _stopPosition - Offset ) ||
					(_direction == StreetDirection.Left && transform.position.x < _stopPosition + Offset) ||
					(_direction == StreetDirection.Down && transform.position.z < _stopPosition + Offset) ||
					(_direction == StreetDirection.Up && transform.position.z > _stopPosition - Offset)  ){
					
					speed = 0.0f;
					
					
					if(!satisfyAdjustedOnTime && vehType == VehicleType.Ambulance){
						GameObject.FindGameObjectWithTag("satisfyBar").GetComponent<SatisfyBar>().AddjustSatisfaction(2);
						gameMasterScript.satisfyBar += 2;
						Debug.Log("Ambulance stopped ... not good");
						satisfyAdjustedOnTime = true;
					}
				}
		
			}
				
		//}
		
	}
	
	private bool CheckIfColliderIsOriginal(){
		if(boxColl.size.Equals(mainColliderSize)){
			return true;
		}
		return false;
	}
	
	
	
	private void CheckServiceCarStops(){
		if(vehType == VehicleType.ServiceCar){
			if(ServiceCar.InsideServiceCarStops(serviceCarStops , gameMasterScript.gameTime)){
				if(serviceCarStopTimer ==0){
					serviceCarStopTimer = gameMasterScript.gameTime-3;
					//stop
					//Debug.Log("Stopping the service car" + gameMasterScript.gameTime );
					speed = 0;
					haveToReduceMySpeed = true;
				}
				
			}
			
			else if(gameMasterScript.gameTime <= serviceCarStopTimer){
				//move
				//Debug.Log("Moving the service car againnn" + gameMasterScript.gameTime );
				haveToReduceMySpeed = false;
				serviceCarStopTimer = 0;
			}
			if(serviceCarStopTimer != 0) {
				speed = 0;
				haveToReduceMySpeed = true;
			}
		}
		 
			
	}
	
	private void CheckMyAnger(){
		if(speed == 0 && GetMyOrderInQueue()== 0){
				if(! stoppingTimerforAngerSet){
					stoppingTimerforAnger = gameMasterScript.gameTime - 13 ;
					stoppingTimerforAngerSet = true;
				}
		}
		if(GetMyOrderInQueue()== 0){
			if(vehType != VehicleType.Ambulance && gameMasterScript.gameTime <= stoppingTimerforAnger){
				stoppingTimerforAngerSet = false;
				GameObject.FindGameObjectWithTag("satisfyBar").GetComponent<SatisfyBar>().AddjustSatisfaction(0.5f);
				gameMasterScript.satisfyBar += 0.5f;
			//	satisfyAdjustedOnTime = true;
				stoppingTimerforAnger =0;
				
			}
		}
		//Debug.Log(stoppingTimerforAngerSet);
	}
	
	
	private int GetMyOrderInQueue(){
		object [] array  = _myQueue.ToArray();
		return Array.IndexOf(array, gameObject);
	}
	
	
	public void SetStopTimeForBus(){
		busStopTimer = gameMasterScript.gameTime - 3;
	}
	
	
	
	private void Move(){
	
		if(!CheckIfColliderIsOriginal()){
			boxColl.size = mainColliderSize;
		}
		
		if(_direction == StreetDirection.Left){
    		transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
			transform.Translate(transform.TransformDirection(Vector3.left) * speed * Time.deltaTime, Space.Self);
			
			Ray ray = new Ray(transform.position, Vector3.left);
			ReduceMeIfHit(ray);
		}
		else if(_direction == StreetDirection.Right){
    		transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
			transform.Translate(transform.TransformDirection(Vector3.right) * speed * Time.deltaTime, Space.Self);
			
			Ray ray = new Ray(transform.position, Vector3.right);
			ReduceMeIfHit(ray);
		}
		else if(_direction == StreetDirection.Down){
    		transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
			transform.Translate(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime, Space.Self);
			
			Ray ray = new Ray(transform.position, Vector3.back);
			ReduceMeIfHit(ray);
		}
		else if(_direction == StreetDirection.Up){
    		transform.localRotation = Quaternion.AngleAxis(270, Vector3.up);
			transform.Translate(transform.TransformDirection(Vector3.back) * speed * Time.deltaTime, Space.Self);
			
			Ray ray = new Ray(transform.position, Vector3.forward);
			ReduceMeIfHit(ray);
		}
	}
	
	private void ReduceMeIfHit(Ray ray){
		RaycastHit hit ;
		if(Physics.Raycast(ray, out hit, 6)){
			//Debug.Log("I hit something");
			Debug.DrawLine (ray.origin, hit.point);
			VehicleController hitVehicleController = hit.collider.gameObject.GetComponent<VehicleController>();
		
			
			if((hitVehicleController.speed < speed || haveToReduceMySpeed)  ){
			//	Debug.Log(gameObject.name +  " ... I have to reduce my speed");
				speed = hitVehicleController.speed;
				haveToReduceMySpeed = true;
			}
			else{
				
				haveToReduceMySpeed = false;
				if(vehType == VehicleType.ServiceCar || vehType == VehicleType.Bus)
					speed = myVehicle.Speed;
				
			}
		}
		else{ 
			if((vehType != VehicleType.Bus || vehType != VehicleType.ServiceCar) && (vehType != VehicleType.ServiceCar) && (vehType != VehicleType.Bus))
				haveToReduceMySpeed = false;
		}
		
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log("on trigger enterrrrr");
	//	if(vehType == VehicleType.Thief || other.gameObject.GetComponent<VehicleController>().vehType == VehicleType.Thief){
			if(vehType == VehicleType.Thief){
				GameObject.Destroy(gameObject);
				GameObject.FindGameObjectWithTag("satisfyBar").GetComponent<SatisfyBar>().AddjustSatisfaction(1);
				gameMasterScript.satisfyBar += 1;
			}
			
			
			
	//	}
		
		else {
			other.gameObject.GetComponent<VehicleController>().haveToReduceMySpeed = true;
			other.gameObject.GetComponent<VehicleController>().speed = 0.0f;
			gameMasterScript.gameOver = true;
		}
		
		
		
   	}	
	
	
	
	void OnTriggerExit(Collider other) {
		if(other.transform.tag == "vehicle"){
			Debug.Log("on trigger exit");
			other.gameObject.GetComponent<VehicleController>().haveToReduceMySpeed = false;
			other.gameObject.GetComponent<VehicleController>().speed = myVehicle.Speed;			//speed = myVehicle.Speed;
		}
		//Debug.Log ("speed " + speed);
	
   	}
	
	private string getVehicleLargerAxis(GameObject v){
		
		if(v.transform.localScale.x > v.transform.localScale.z)
			return "x";
		else
			return "z";
	}
	
	
	
	private void ChangeColliderSize(){
		
	//	if(vehType == VehicleType.Normal){
		if(speed == 0){
			onIncrease = true;
			if(getVehicleLargerAxis(gameObject) == "x"){
				boxColl.size = new Vector3(mainColliderSize.x+4  , mainColliderSize.y , mainColliderSize.z );
			}
			else{
				boxColl.size = new Vector3(mainColliderSize.x   , mainColliderSize.y , mainColliderSize.z+4 );
			}
		}
		else{
			onIncrease = false;
		}
	//	}
		
	}
	
	
	
	/*
	private void CheckNextStreetEmptiness(){
		if( _direction == StreetDirection.Right && transform.position.x >= _endPosition.x - 10   ||
			_direction == StreetDirection.Left && transform.position.x <= _endPosition.x + 10  ||
			_direction == StreetDirection.Down && transform.position.z <= _endPosition.z +10 ||
			_direction == StreetDirection.Up && transform.position.z >= _endPosition.z - 10){
		
			Debug.Log("and the next street direction " + _nextDirection + "of vehicle .." + gameObject.name);
			
			if(myVehicle.NextStreet != null){
				Debug.Log("inside check emptinesssssssss .." + (_endPosition.z - 10));			
				if(myVehicle.NextStreet.VehiclesNumber == myVehicle.NextStreet.StreetCapacity){
					Debug.Log("it is full capacityyyyy " + gameObject.name ); 
		//			nextStreetIsEmpty = false;
					speed = 0;
				}
				else{
//					nextStreetIsEmpty = true;
					
				}
			}
		}
			
	}
	*/
	
}

using UnityEngine;
using System.Collections;

public class Driving : MonoBehaviour
{
    #region Public Members
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;

    public Transform flWheelModel;
    public Transform frWheelModel;
    public Transform bWheelModel;
    public Transform flDiscBreak;
    public Transform frDiscBreak;

    public float motorTorque = 50;//车子的动力
    public float steerAngle = 10;//轮子的转向
    public Transform centerOfMass;

    public float maxSpeed = 300;
    public float minSpeed = 30;
    private Rigidbody rigMinSpeed;
    public float FirRigMinForce = 500; //11 KM/H
    public float SecRigMinForce = 1200;
    public float ThiRigMinForce = 2500;
    public float FouRigMinForce = 5000;
    public float FifRigMinForce = 7500;

    public float breakTorque = 500;//刹车的力量
    private  float currentSpeed;
    public float block;//挡数
    public GameObject lights_headlights;
    public GameObject lights_stop;
    private bool headlightActive = false;

    //TODO
    //粒子系统及特效
    public ParticleSystem smoke;

    public bool isBigBreaking = false;
    public bool isSmallBreaking = false;
    private AudioSource engineAudio;
    private AudioSource draftAudio;
    public AudioSource[] CarAudioSource;
    
   
    #endregion
    void Awake()
    {
        engineAudio = CarAudioSource[0];
        draftAudio = CarAudioSource[1];
        rigidbody.centerOfMass = centerOfMass.localPosition;
        rigMinSpeed = this.GetComponent<Rigidbody>();
       
    }
	
	// Update is called once per frame
	void FixedUpdate() {

        //灯光
        if (Input .GetKeyDown("f")){
            if (!headlightActive)
            {
                smoke.Play ();
                lights_headlights.SetActive(true);
                headlightActive = true;
            }else
                if (headlightActive)
            {
                lights_headlights.SetActive(false);
                headlightActive = false;
            }
        }



        //取得当前的速度大小
        currentSpeed = SpeedDisplay.Instance.currentSpeed;
        //currentSpeed = flWheelCollider.rpm * (flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;
        //currentSpeed = (int)Mathf.Round(currentSpeed);
      
        //TODO
        //上下方向键控制加速踏板的松紧，shift是刹车踏板，空格键为离合器踏板（motorTorque设置大一点这样？）
        #region  各个档位
        if ((currentSpeed > maxSpeed && Input.GetAxis("Vertical") > 0)  //限制条件,此时禁止速度大小再增加了
          ||(SpeedDisplay .Instance.isShouSha ==true))//手刹没拉下

          //TODO 手刹还没做
      {
          flWheelCollider.motorTorque = 0;
          frWheelCollider.motorTorque = 0;
      }
      else
      {
          //不同挡位限制不同的速度  不同挡位对应不同的MaxSpeed , MinSpeed , 怠速；
          //一档25 二挡50 三挡80 四挡110 五档140

          if (SpeedDisplay.Instance.dangShu == -1)            //倒挡时
          {
              #region   Complian
              //根据value值0-100控制车速怠速(-15km/h)到最快速度(-40km/h)的上升和下降
              //沿着当前物体的Z轴给克隆的物体一个初速度。
              //clone.velocity = transform.TransformDirection (Vector3.forward * 10);
              //每次更新都是：v1=f/m*t+v0   所以Addforce 和 Motoque的方式都涉及了t
              //rigMinSpeed.velocity.magnitude += SpeedDisplay.Instance.YouMenSlider.value * 25;   可惜啊！！
              //真是日了狗了 rigMinSpeed.velocity.magnitude是只读的草！意味着不能直接操作物体的速度，真是坑死了！！！！

              #endregion


              //Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

              lights_stop.SetActive(true);
              //控制加速UI百分比的显示（包括前进后退都是这一句）  
              SpeedDisplay.Instance.YouMenSlider.value = -((float)(currentSpeed * 2.5 / 100));//  100/(Max speed -Min speed) = 2.5
              //(currentSpeed - MinSpeed)*(100/(MaxSpeed - MinSpeed)) /100

              if (currentSpeed < -40)          //到达最大的速度，给一个向前的推力
              {
                  flWheelCollider.motorTorque = 0;
                  frWheelCollider.motorTorque = 0;
                  rigMinSpeed.AddRelativeForce(0, 0, 2 * FifRigMinForce);
              }
              else
                  if (currentSpeed >= -15 && currentSpeed <= 0)          //怠速  -15km/h
                  {
                      rigMinSpeed.AddRelativeForce(0, 0, -2 * FirRigMinForce);
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * -motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * -motorTorque;
                  }
                  else
                      if (currentSpeed < -15 && currentSpeed >= -40)
                      {
                          flWheelCollider.motorTorque = Input.GetAxis("Vertical") * -motorTorque;
                          frWheelCollider.motorTorque = Input.GetAxis("Vertical") * -motorTorque;
                      }
                      else
                      {
                          flWheelCollider.motorTorque = -60;
                          frWheelCollider.motorTorque = -60;
                          rigMinSpeed.AddRelativeForce(0, 0, -2 * FirRigMinForce);
                      }
          }
        

          if (SpeedDisplay.Instance.dangShu == 0)         //空挡时 
          {
             
              engineAudio.pitch = 1f;

              flWheelCollider.motorTorque = 0;
              frWheelCollider.motorTorque = 0;
              if (currentSpeed >0)                     //想方设法使速度变为0
              {
                
                  rigMinSpeed.AddRelativeForce(0, 0, -FifRigMinForce);
              }
              else if (currentSpeed<0)
              {
                
                  rigMinSpeed.AddRelativeForce(0, 0, FifRigMinForce);
              }
          }

          if (SpeedDisplay.Instance.dangShu == 1)              //一挡时
          {
              SpeedDisplay.Instance.YouMenSlider.value = (float)(currentSpeed * 4 / 100);
              //(currentSpeed - MinSpeed)*(100/(MaxSpeed - MinSpeed)) /100
            
              if (currentSpeed > 25)                 //到达最大速度时         
              {
                  flWheelCollider.motorTorque = -60;
                  frWheelCollider.motorTorque = -60;
              }
              else 
                  if (currentSpeed <=10&&currentSpeed >=0){              //怠速 10
                      rigMinSpeed.AddRelativeForce(0, 0, FirRigMinForce);
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else if (currentSpeed > 10 && currentSpeed <= 25)
                  {
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else {
                      flWheelCollider.motorTorque = 0;
                      frWheelCollider.motorTorque = 0;
                      rigMinSpeed.AddRelativeForce(0, 0, 2 * FifRigMinForce);
                  }
                  
              
          }
          if (SpeedDisplay.Instance.dangShu == 2)              //二挡时
          {
              SpeedDisplay.Instance.YouMenSlider.value = (float)((currentSpeed -25) * (100/(50-25))/100);//100/(Max speed -Min speed)
              //(currentSpeed - MinSpeed)*(100/(MaxSpeed - MinSpeed)) /100
              if (currentSpeed > 50)                 //到达最大速度时         
              {
                  flWheelCollider.motorTorque = -50;
                  frWheelCollider.motorTorque = -50;
              }
              else
                  if (currentSpeed <= 35 && currentSpeed >= 25)
                  {              //怠速
                      rigMinSpeed.AddRelativeForce(0, 0, FirRigMinForce);
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else if (currentSpeed > 35 && currentSpeed <= 55)
                  {
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else
                  {
                      if (Input.GetAxis("Vertical") > 0)
                      {
                          flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                          frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      }
                      else
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, FifRigMinForce / 2);
                      }
                      
                  }
                  
          }
          if (SpeedDisplay.Instance.dangShu == 3)              //三挡时
          {
             
              SpeedDisplay.Instance.YouMenSlider.value = (float)((currentSpeed - 50) * (100 / (80 - 50)) / 100);//100/(Max speed -Min speed)
              //(currentSpeed - MinSpeed)*(100/(MaxSpeed - MinSpeed)) /100
              if (currentSpeed > 80)                 //到达最大速度时         
              {
                  flWheelCollider.motorTorque = -50;
                  frWheelCollider.motorTorque = -50;
                  //rigMinSpeed.AddRelativeForce(0, 0, -2 * FifRigMinForce);
              }
              else
                  if (currentSpeed <=60  && currentSpeed >= 50)
                  {              //怠速
                      rigMinSpeed.AddRelativeForce(0, 0, FirRigMinForce);
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else if (currentSpeed > 60 && currentSpeed <= 80)
                  {
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else
                  {
                      if (Input.GetAxis("Vertical") > 0)
                      {
                          flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                          frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      }
                      else {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0,FifRigMinForce/2);
                      }
                      
                  }
          }
          if (SpeedDisplay.Instance.dangShu == 4)              //四挡时
          {
              SpeedDisplay.Instance.YouMenSlider.value = (float)((currentSpeed - 80) * (100 / (110 - 80)) / 100);//100/(Max speed -Min speed)
              //(currentSpeed - MinSpeed)*(100/(MaxSpeed - MinSpeed)) /100
              if (currentSpeed > 110)                 //到达最大速度时         
              {
                  flWheelCollider.motorTorque = -50;
                  frWheelCollider.motorTorque = -50;
                 
              }
              else
                  if (currentSpeed <= 90 && currentSpeed >= 80)
                  {              //怠速
                      rigMinSpeed.AddRelativeForce(0, 0, FirRigMinForce);
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else if (currentSpeed > 90 && currentSpeed <= 110)
                  {
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else
                  {
                      if (Input.GetAxis("Vertical") > 0)
                      {
                          flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                          frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      }
                      else
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, FifRigMinForce / 2);
                      }
                      
                  }
          }
          if (SpeedDisplay.Instance.dangShu == 5)              //五挡时
          {
              SpeedDisplay.Instance.YouMenSlider.value = (float)((currentSpeed - 110) * (100 / (140 - 110)) / 100);//100/(Max speed -Min speed)
              //(currentSpeed - MinSpeed)*(100/(MaxSpeed - MinSpeed)) /100
              if (currentSpeed > 140)                 //到达最大速度时         
              {
                  flWheelCollider.motorTorque = -50;
                  frWheelCollider.motorTorque = -50;
              }
              else
                  if (currentSpeed <= 120 && currentSpeed >= 110)
                  {              //怠速
                      rigMinSpeed.AddRelativeForce(0, 0, FirRigMinForce);
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else if (currentSpeed > 120 && currentSpeed <= 140)
                  {
                      flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                  }
                  else
                  {
                      if (Input.GetAxis("Vertical") > 0)
                      {
                          flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                          frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
                      }
                      else
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, FifRigMinForce / 2);
                      }
                      
                  }
          }
      }
        #endregion
        #region 刹车
        //刹车
      //if ((currentSpeed > 0 && Input.GetAxis("Vertical") < 0) ||
      //    (currentSpeed < 0 && Input.GetAxis("Vertical") > 0))

      //if (Input.GetKeyDown("space"))//只返回一帧true
        //大刹车
      if (Input.GetKey(KeyCode.LeftShift)&&Input .GetKey ("space"))//Shift+Space控制刹车
      {
          isBigBreaking = true;
          lights_stop.SetActive(true);
          if (draftAudio.isPlaying == false)
          {
              draftAudio.Play();
          }
         
      }
      else
      {
          if (Input.GetKey(KeyCode.LeftShift))
          {
              isSmallBreaking = true;
              lights_stop.SetActive(true);
              if (draftAudio.isPlaying == false)
              {
                  draftAudio.Play();
              }

          }
          else
          {
              isSmallBreaking = false;
              isBigBreaking = false;
              if (SpeedDisplay.Instance.dangShu != -1)
                  lights_stop.SetActive(false);
              draftAudio.Stop();

          }
        
         
      }

     
        //刹车  刹车降速自动换挡

        //小刹车
      //各个档位最快速度 ：一档10-25 二挡25-55 三挡45-90 四挡80-140 五档100-200
    
          if (isSmallBreaking)
          {
              if (SpeedDisplay.Instance.dangShu == 5)     //五档的刹车
              {
                  if (currentSpeed >= 110)
                  {
                      flWheelCollider.motorTorque = 0;
                      frWheelCollider.motorTorque = 0;

                      rigMinSpeed.AddRelativeForce(0, 0, -FifRigMinForce);
                  }
              }
              else
                  if (SpeedDisplay.Instance.dangShu == 4)     //四档的刹车
                  {
                      if (currentSpeed >= 80)
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, -FifRigMinForce);
                      }
                  }
                  else if (SpeedDisplay.Instance.dangShu == 3)     //三档的刹车
                  {
                      if (currentSpeed >= 50)
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, -FifRigMinForce); ;
                      }
                  }
                  else if (SpeedDisplay.Instance.dangShu == 2)     //二档的刹车
                  {
                      if (currentSpeed >= 25)
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, -FifRigMinForce);
                      }
                  }
                  else if (SpeedDisplay.Instance.dangShu == 1)     //一档的刹车
                  {
                      if (currentSpeed >= 0)
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, -FifRigMinForce);

                      }
                  }
                  else if (SpeedDisplay.Instance.dangShu == -1)      //倒档的刹车
                  {
                       if (currentSpeed < 0 )
                      {
                          flWheelCollider.motorTorque = 0;
                          frWheelCollider.motorTorque = 0;
                          rigMinSpeed.AddRelativeForce(0, 0, FifRigMinForce);

                      }
                  }
              
          }







          //大刹车
          if (isBigBreaking)
          {
              flWheelCollider.motorTorque = 0;
              frWheelCollider.motorTorque = 0;

              flWheelCollider.brakeTorque = breakTorque;
              frWheelCollider.brakeTorque = breakTorque;


              //刹车完以后根据速度更改挡位


              if (currentSpeed == 0)//空挡
              {
                  SpeedDisplay.Instance.dangShu = 0;
                  SpeedDisplay.Instance.blockLabel.text = "挡位: 空挡";
              }
              if (currentSpeed > 0 && currentSpeed < 25)//1挡
              {
                  SpeedDisplay.Instance.dangShu = 1;
                  SpeedDisplay.Instance.blockLabel.text = "挡位: 1挡";
              }
              if (currentSpeed > 25 && currentSpeed < 55)//2挡
              {
                  SpeedDisplay.Instance.dangShu = 2;
                  SpeedDisplay.Instance.blockLabel.text = "挡位: 2挡";
              }
              if (currentSpeed > 55 && currentSpeed < 90)//3挡
              {
                  SpeedDisplay.Instance.dangShu = 3;
                  SpeedDisplay.Instance.blockLabel.text = "挡位: 3挡";
              }
              if (currentSpeed > 90 && currentSpeed < 140)//4挡
              {
                  SpeedDisplay.Instance.dangShu = 4;
                  SpeedDisplay.Instance.blockLabel.text = "挡位: 4挡";
              }
              if (currentSpeed > 140 && currentSpeed < 200)//5挡
              {
                  SpeedDisplay.Instance.dangShu = 5;
                  SpeedDisplay.Instance.blockLabel.text = "挡位: 5挡";
              }
          }
       else
      {
          flWheelCollider.brakeTorque = 0;
          frWheelCollider.brakeTorque = 0;
      }
        #endregion

          //音效
        if (SpeedDisplay .Instance .dangShu !=0){
                 engineAudio.pitch = 1f + SpeedDisplay.Instance.YouMenSlider.value;
        
        }
      


        //轮子的转向
        flWheelCollider.steerAngle = Input.GetAxis("Horizontal") * steerAngle;
        frWheelCollider.steerAngle = Input.GetAxis("Horizontal") * steerAngle;

        RotateWheel();//用来转动车轮
        SteerWheel();//用来给车轮转向
	}
    //FixedUpdate结束







   void  RotateWheel(){
       flDiscBreak.Rotate(flWheelCollider.rpm * 360 / 60 * Time.deltaTime * Vector3.right);
       frDiscBreak.Rotate(frWheelCollider.rpm * 360 / 60 * Time.deltaTime * Vector3.right);
       bWheelModel.Rotate(rlWheelCollider.rpm * 360 / 60 * Time.deltaTime * Vector3.right);
    }
   void SteerWheel() {
       Vector3 localEulerAngles = flWheelModel.localEulerAngles;
       localEulerAngles.y = flWheelCollider.steerAngle;

       flWheelModel.localEulerAngles = localEulerAngles;
       frWheelModel.localEulerAngles = localEulerAngles;
   }

    //TODO
    //转向灯

    ////碰撞检测 暂时先这样
    //void OnCollisionEnter(Collision collision) 
    //{
    //        SpeedDisplay.Instance.dangShu = 0;
    //        SpeedDisplay.Instance.blockLabel.text = "挡位: 空挡";
    //}


}

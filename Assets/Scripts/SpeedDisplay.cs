using UnityEngine;
using System.Collections;

public class SpeedDisplay : MonoBehaviour
{
    #region Public Members
    public WheelCollider flWheelCollider;
    public float currentSpeed;
    public int currentRpm;

    public Transform pointContainer;
    private float zRotation;
    public int dangShu = 0;//挡数
    private float panSpeed;

    private UILabel speedLabel;
    public UILabel blockLabel;
    public UILabel shouShaLabel;
    public UILabel rpmLabel;
    public UILabel liHeQiLabel;
    public UISlider YouMenSlider;//油门进度条

    public bool isShouSha = true;//是否拉下了手刹
    public bool isDownLiHeQi = false;//是否踩下了离合器 
    public AudioClip [] audioArray;
    private AudioSource musicManager;
    //TODO
    //音效有点难啊

    //单例
    private static SpeedDisplay _instance; 
    public static SpeedDisplay Instance
    {
        get{return _instance ;}
    }
    #endregion
    void Awake() {
        _instance = this;
        musicManager = this.GetComponent<AudioSource>();
    }
	// Use this for initialization
	void Start () {
        speedLabel = this.GetComponent<UILabel>();
        zRotation = pointContainer.eulerAngles.z;
	}
	
	// Update is called once per frame
	void Update () {




        currentSpeed = flWheelCollider.rpm * (flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);
        speedLabel.text ="速度: "+ currentSpeed.ToString()+"千米/小时";
        currentRpm = (int)flWheelCollider.rpm;
        rpmLabel.text = "当前转数： " + currentRpm.ToString() + "转/分";

         #region                      离合器的踩下和松开,isDownLiHeQi完全由空格键的按下来取值
        if (isDownLiHeQi)
        {
            liHeQiLabel.text = "离合器状态: 踩下";
            
        }
        else {
            liHeQiLabel.text = "离合器状态: 松开";
        }
        if (Input.GetKey("space"))
        {
            isDownLiHeQi = true;
        }
        if (Input .GetKeyUp("space")){
            isDownLiHeQi = false;
            musicManager.clip = audioArray[2];
            musicManager.Play();

        }
        if (Input .GetKeyDown("space")){
            musicManager.clip = audioArray[2];
            musicManager.Play();
        }
#endregion
       
        #region       挡数的UI显示和控制

        //TODO
        //手动升挡，自动降挡
        //音效用AudioSouce.pitch控制

        if (dangShu ==0)
        {
            blockLabel.text = "挡位: 空挡";
        }
        //TODO
        //倒挡的设计（倒挡以后currrentspeed只能小于0）
        //if (dangShu <0){
        //    blockLabel.text = "挡位: 倒挡";
        //}
      


   
        //进挡与退档(不允许越级挂挡)
        //进挡
        //一档0-25 二挡25-50 三挡50-80 四挡80-110 五档110-140
        //怠速：一档0-10 二挡25-35 三挡50-60 四挡80-90 五档110-120
        //各个档位进挡需要速度：一进二：15 二进三： 40 三进四：70 四进五：100
        if (Input.GetKeyDown("e") &&Input.GetKey("space"))
        {
            

            if (dangShu ==-1)//要从倒挡挂到空挡
            {
                if (currentSpeed<=0)
                {
                    musicManager.clip = audioArray[3];
                    musicManager.Play();
                    dangShu++;
                    blockLabel.text = "挡位: 空挡";
                }
            }

            if (dangShu == 0)//要从空挡挂一挡
            {
                if (currentSpeed>=0)
                {
                    musicManager.clip = audioArray[3];
                    musicManager.Play();
                dangShu++;
                blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
                }
                else
                {
                    //TODO
                    //弹窗警告
                }
            }
            if (dangShu == 1)  //要从一挡挂二挡
            {
                if (currentSpeed >= 15)
                {
                    musicManager.clip = audioArray[3];
                    musicManager.Play();
                    dangShu++;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
                }
                else
                {
                    //TODO
                    //弹窗警告
                }
            }
            if (dangShu == 2)  //要从二挡挂三挡
            {
                if (currentSpeed >= 40)
                {
                    musicManager.clip = audioArray[3];
                    musicManager.Play();
                    dangShu++;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
                }
                else
                {
                    //TODO
                    //弹窗警告
                }
            }
            if (dangShu == 3)  //要从三挡挂四挡
            {
                if (currentSpeed >= 70)
                {
                    musicManager.clip = audioArray[3];
                    musicManager.Play();
                    dangShu++;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
                }
                else
                {
                    //TODO
                    //弹窗警告
                }
            }
            if (dangShu == 4)  //要从四挡挂五挡
            {
                if (currentSpeed >= 100)
                {
                    musicManager.clip = audioArray[3];
                    musicManager.Play();
                    dangShu++;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
                }
                else
                {
                    //TODO
                    //弹窗警告
                }
            }
        }
        #region  手动退挡.貌似可以实现，退挡是motoTorque归零，用刚体Addforce控制速度  TODO
        //退挡
        //一档0-25 二挡25-50 三挡50-80 四挡80-110 五档110-140
        //怠速：一档0-10 二挡25-35 三挡50-60 四挡80-90 五档110-120
        //随时可以退挡
        if (Input.GetKeyDown("q") && Input.GetKey("space"))
        {

            if (dangShu == 5)  //要从五挡退四挡
            {
                musicManager.clip = audioArray[3];
                musicManager.Play();
                    dangShu--;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
            
            }else if (dangShu == 4)  //要从四挡退三挡
            {
                musicManager.clip = audioArray[3];
                musicManager.Play();
                    dangShu--;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
              
            }
            else  if (dangShu == 3)  //要从三挡退二挡
            {
                musicManager.clip = audioArray[3];
                musicManager.Play();
                    dangShu--;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
               
            }
            else  if (dangShu == 2)  //要从二挡退一挡
            {
                musicManager.clip = audioArray[3];
                musicManager.Play();
                    dangShu--;
                    blockLabel.text = "挡位: " + dangShu.ToString() + "挡";
               
            }
            else  if (dangShu == 1)  //要从一挡退空挡
            {
                musicManager.clip = audioArray[3];
                musicManager.Play();
                    dangShu--;
                    blockLabel.text = "挡位: 空挡";
             
            }
            else if (dangShu == 0)  //要从空挡退到倒挡
            {
                musicManager.clip = audioArray[3];
                musicManager.Play();
                dangShu--;
                blockLabel.text = "挡位: 倒挡";

            }
        }
        #endregion
        #endregion


        //手刹的开关
     
        if (Input.GetKeyDown("r"))
        {
            if (isShouSha)
            {
            shouShaLabel.text = "手刹: 关";
            isShouSha = false;
            }
            //TODO      
            //Bug待解释
            //老子真是气死了，这里else换成if（！isShouSha）UI就只显示手刹： 开，换一个else就可以切换了？？为毛？？？
           // if (!isShouSha) 检验了一下，这里如果用了两个if循环的话，unity直接调用了这两个if语句里的代码，
            //所以一直是后面的if语句中的手刹： 开，为什么？
            else
            {
                shouShaLabel.text = "手刹: 开";
                isShouSha = true;
            }
            musicManager.clip = audioArray[1];
            musicManager.Play();
          
            //TODO
            //起步以后再拉手刹弹窗警告
        }

        //仪表盘的UI显示限制
        panSpeed = currentSpeed;
        if (currentSpeed <=0){
            panSpeed = 0;
        }
        if (currentSpeed >=140){
            panSpeed = 143;
        }

        float newZRotation = zRotation - panSpeed * (270 / 140f);
        pointContainer.eulerAngles = new Vector3(0,0,newZRotation);
	}
}

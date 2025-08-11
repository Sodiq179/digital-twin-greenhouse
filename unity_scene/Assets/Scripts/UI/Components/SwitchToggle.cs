using UnityEngine ;
using UnityEngine.UI ;

public class SwitchToggle : MonoBehaviour {
   [SerializeField] RectTransform uiHandleRectTransform ;
   [SerializeField] Color backgroundActiveColor ;
   [SerializeField] Color handleActiveColor ;
   [SerializeField] mqttReceiver mqttReceiverScript ;

   Image backgroundImage, handleImage ;

   Color backgroundDefaultColor, handleDefaultColor ;

   Toggle toggle ;

   Vector2 handlePosition ;

   void Awake ( ) {
      toggle = GetComponent <Toggle> ( ) ;

      handlePosition = uiHandleRectTransform.anchoredPosition ;

      backgroundImage = uiHandleRectTransform.parent.GetComponent <Image> ( ) ;
      handleImage = uiHandleRectTransform.GetComponent <Image> ( ) ;

      backgroundDefaultColor = backgroundImage.color ;
      handleDefaultColor = handleImage.color ;

      toggle.onValueChanged.AddListener (OnSwitch) ;

      if (toggle.isOn)
         OnSwitch (true) ;

      Debug.Log("Awake executed, Toggle is On: " + toggle.isOn);

   }

   void OnSwitch (bool on) {
      Debug.Log("Toggle Value Changed: " + on);

      //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
      uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;

      //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor ; // no anim
      backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;

      //handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
      handleImage.color =  on ? handleActiveColor : handleDefaultColor;

      if (mqttReceiverScript != null)
        {
            if (on)
            {
                mqttReceiverScript.Connect(); // Connect to MQTT broker
            }
            else
            {
                mqttReceiverScript.Disconnect(); // Disconnect from MQTT broker
            }
        }
   }

   void OnDestroy ( ) {
      Debug.Log("OnDestroy executed");

      toggle.onValueChanged.RemoveListener (OnSwitch) ;
   }
}

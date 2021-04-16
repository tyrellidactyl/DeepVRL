using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class TOFPublisher : UnityPublisher<MessageTypes.Std.Int16MultiArray>
    {
        //public Transform PublishedTransform;
        //public List<float> TOFreadings;
        //public ArrayList TOFreadings;
        public short[] TOFreadings;
        //public short[] data { get; set; }
        //public string FrameId = "Unity";
        public GameObject Lilbot;
        //GameObject g = GameObject.Find("Lilbot");
        private LilbotLPArrayV2 ArrayScript;
        //LilbotLPArrayV2 arrayScript = Lilbot.GetComponent<LilbotLPArrayV2>();
        //public short[] TOFreadings;
        //private TOFreadings = new ArrayList { ArrayScript.CenterDist, ArrayScript.LeftDist, ArrayScript.RightDist};

        //private ArrayList TOFreadings = new ArrayList { ArrayScript.CenterDist, ArrayScript.LeftDist, ArrayScript.RightDist };

        private MessageTypes.Std.Int16MultiArray message;

        protected override void Start()
        {
            LilbotLPArrayV2 arrayScript = Lilbot.GetComponent<LilbotLPArrayV2>();
            TOFreadings[0] = arrayScript.CenterDist;
            TOFreadings[1] = arrayScript.LeftDist;
            TOFreadings[2] = arrayScript.RightDist;
            base.Start();
            InitializeMessage();
        }


        private void InitializeMessage()
        {
            
            message = new MessageTypes.Std.Int16MultiArray
            {
                data = TOFreadings
            };
        }

        private void Update()
        {
            GetTOFValues();
            message.data = TOFreadings;
            //Debug.Log("TOF: " + TOFreadings[1]);
            Publish(message);
        }

        private void GetTOFValues()
        {
            LilbotLPArrayV2 arrayScript = Lilbot.GetComponent<LilbotLPArrayV2>();
            TOFreadings[0] = arrayScript.CenterDist;
            TOFreadings[1] = arrayScript.LeftDist;
            TOFreadings[2] = arrayScript.RightDist;
        }

    }

}


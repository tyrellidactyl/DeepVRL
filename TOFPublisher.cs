using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class TOFPublisher : UnityPublisher<MessageTypes.Std.Int16MultiArray>
    {
        public short[] TOFreadings;
        public GameObject Lilbot;

        private LilbotLPArrayV2 ArrayScript;
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

